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

using Flurl;
using WalkTrack.Common.Authentications;
using WalkTrack.Common.Resources;

namespace WalkTrack.Client.Authentications;

internal sealed class AuthenticationClient: BaseClient, IAuthenticationClient
{
    private readonly ITranscoderProcessor _transcoder;
    private readonly HttpClient _httpClient;

    public AuthenticationClient(ITranscoderProcessor transcoder)
    {
        _transcoder = transcoder ??
            throw new ArgumentNullException(nameof(transcoder));

        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:8000")
        };
    }

    public async Task<AuthenticateResponse> Login(AuthenticateRequest request, CancellationToken cancellationToken = default)
    {
        AuthenticateResponse response =
            await new RequestBuilder(_transcoder)
                .WithBody(request)
                .WithContentTypes(MediaTypes.AuthenticateRequest)
                .WithMethod(HttpMethod.Post)
                .WithUrl(
                    new Url()
                        .AppendPathSegment("v1")
                        .AppendPathSegment("authenticate")
                )
                .WithAcceptType(MediaTypes.AuthenticateResponse)
                .WithAcceptType(MediaTypes.ApiError)
                .WithErrorHandler(new ResourceNotFoundErrorHandler())
                .Fetch<AuthenticateRequest, AuthenticateResponse>(_httpClient, cancellationToken);

        AuthenticationContext.Token = response.Token;

        return response;
    }

    public Task Login(string token, CancellationToken cancellationToken = default)
    {
        AuthenticationContext.Token = token;

        return Task.CompletedTask;
    }

    public Task Logout(CancellationToken cancellationToken = default)
    {
        AuthenticationContext.Token = string.Empty;

        return Task.CompletedTask;
    }
}