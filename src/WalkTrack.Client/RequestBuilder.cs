/*
    WalkTrack - A simple walk tracker that lets people define their own milestones

    Copyright (C) 2022  Nathan Mentley
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.
    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System.Net.Http.Headers;
using Flurl;
using WalkTrack.Client.Exceptions;
using WalkTrack.Common.ApiErrorResponses;
using WalkTrack.Common.Resources;

namespace WalkTrack.Client;

internal class RequestBuilder
{
    private readonly ITranscoderProcessor _transcoder;
    private HttpMethod _httpMethod = HttpMethod.Get;
    private object? _body = null;
    private WalkTrackMediaType? _contentType = null;
    private readonly IList<WalkTrackMediaType> _acceptTypes = new List<WalkTrackMediaType>();
    private readonly IList<IErrorHandler> _errorHandlers = new List<IErrorHandler>();
    private Url? _url = null;
    private string? _authToken = null;

    public RequestBuilder(ITranscoderProcessor transcoder)
    {
        _transcoder = transcoder ??
            throw new ArgumentNullException(nameof(transcoder));
    }

    public RequestBuilder WithMethod(HttpMethod httpMethod)
    {
        _httpMethod = httpMethod;

        return this;
    }

    public RequestBuilder WithBody(object body)
    {
        _body = body;

        return this;
    }

    public RequestBuilder WithUrl(Url url)
    {
        _url = url;

        return this;
    }

    public RequestBuilder WithContentTypes(WalkTrackMediaType contentType)
    {
        _contentType = contentType;

        return this;
    }

    public RequestBuilder WithAcceptType(WalkTrackMediaType acceptType)
    {
        _acceptTypes.Add(acceptType);

        return this;
    }

    public RequestBuilder WithAuthToken(string authToken)
    {
        _authToken = authToken;

        return this;
    }

    public RequestBuilder WithErrorHandler(IErrorHandler errorHandler)
    {
        _errorHandlers.Add(errorHandler);

        return this;
    }

    public async Task Send(HttpClient httpClient, CancellationToken cancellationToken = default)
    {
        await ProcessRequest<object>(httpClient, cancellationToken);
    }

    public async Task Send<TBody>(HttpClient httpClient, CancellationToken cancellationToken = default)
    {
        await ProcessRequest<TBody>(httpClient, cancellationToken);
    }

    public async Task<TResponse> Fetch<TBody, TResponse>(HttpClient httpClient, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await ProcessRequest<TBody>(httpClient, cancellationToken);

        return await DecodeResponse<TResponse>(response, cancellationToken);
    }

    public async Task<TResponse> Fetch<TResponse>(HttpClient httpClient, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await ProcessRequest<object>(httpClient, cancellationToken);

        return await DecodeResponse<TResponse>(response, cancellationToken);
    }

    private async Task<HttpResponseMessage> ProcessRequest<TBody>(
        HttpClient httpClient,
        CancellationToken cancellationToken = default
    )
    {
        if (_url is null)
        {
            throw new InvalidOperationException("TODO");
        }

        HttpRequestMessage request = new HttpRequestMessage(_httpMethod, _url);

        foreach(WalkTrackMediaType accept in _acceptTypes)
        {
            request.Headers.Accept.Add(MedaiTypeToQualityHeader(accept));
        }

        using MemoryStream memoryStream = new MemoryStream();

        if (
            _body is TBody body &&
            _contentType is not null
        )
        {
            await _transcoder.Encode(_contentType, body, memoryStream, cancellationToken);

            memoryStream.Position = 0;
            request.Content = new StreamContent(memoryStream);

            request.Content.Headers.ContentType = MedaiTypeToHeader(_contentType);
        }

        if (!string.IsNullOrWhiteSpace(_authToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
        }

        HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);

        await CheckResponseForError(response);

        return response;
    }

    private async Task CheckResponseForError(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            ApiErrorResponse apiError = await DecodeResponse<ApiErrorResponse>(response);

            IErrorHandler? errorHandler = _errorHandlers.FirstOrDefault(errorHandler => errorHandler.CanHandle(apiError));

            if (errorHandler is not null)
            {
                throw errorHandler.Handle(apiError);
            }

            throw new UnhandledResponseErrorException($"An unhandled client error occured with the message: {apiError.Message}");
        }
    }

    private MediaTypeWithQualityHeaderValue MedaiTypeToQualityHeader(WalkTrackMediaType mediaType)
    {
        var ret = new MediaTypeWithQualityHeaderValue($"{mediaType.Type}/{mediaType.SubType}");
        ret.Parameters.Add(new NameValueHeaderValue("structure", mediaType.Structure));
        ret.Parameters.Add(new NameValueHeaderValue("version", $"{mediaType.Version}"));

        return ret;
    }

    private MediaTypeHeaderValue MedaiTypeToHeader(WalkTrackMediaType mediaType)
    {
        var ret = new MediaTypeHeaderValue($"{mediaType.Type}/{mediaType.SubType}");
        ret.Parameters.Add(new NameValueHeaderValue("structure", mediaType.Structure));
        ret.Parameters.Add(new NameValueHeaderValue("version", $"{mediaType.Version}"));

        return ret;
    }

    private async Task<T> DecodeResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (response.Content.Headers.ContentType is not null)
        {
            Stream stream = await response.Content.ReadAsStreamAsync();

            WalkTrackMediaType mediaType = HeaderToMediaType(response.Content.Headers.ContentType);

            return await _transcoder.Decode<T>(mediaType, stream, cancellationToken);
        }

        throw new InvalidOperationException("TODO");
    }

    private WalkTrackMediaType HeaderToMediaType(MediaTypeHeaderValue header)
    {
        string structure = header.Parameters.FirstOrDefault(x => string.Equals(x.Name, "structure", StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty;
        string version = header.Parameters.FirstOrDefault(x => string.Equals(x.Name, "version", StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty;

        return new WalkTrackMediaTypeBuilder()
            .WithType(header?.MediaType?.Split("/").First() ?? string.Empty)
            .WithSubType(header?.MediaType?.Split("/").Last() ?? string.Empty)
            .WithStructure(structure)
            .WithVersion(int.Parse(version))
            .Build();
    }
}