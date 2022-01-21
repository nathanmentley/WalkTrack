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
using WalkTrack.EntryService.Common;
using WalkTrack.Framework.Client;
using WalkTrack.Framework.Client.Authentications;
using WalkTrack.Framework.Common.Resources;

namespace WalkTrack.EntryService.Client;

internal sealed class EntryClient: BaseClient, IEntryClient
{
    private readonly ITranscoderProcessor _transcoder;
    private readonly HttpClient _httpClient;

    private static readonly WalkTrackMediaType _entryMediaType =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Entry")
            .WithVersion(1)
            .Build();

    private static readonly WalkTrackMediaType _entriesMediaType =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Entries")
            .WithVersion(1)
            .Build();

    public EntryClient(string url, ITranscoderProcessor transcoder)
    {
        _transcoder = transcoder ??
            throw new ArgumentNullException(nameof(transcoder));

        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(url)
        };
    }

    public async Task<Entry> Fetch(string id, CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithMethod(HttpMethod.Get)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("entry")
                    .AppendPathSegment(id)
            )
            .WithAcceptType(_entryMediaType)
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthToken(AuthenticationContext.Token)
            .WithErrorHandler(new ResourceNotFoundErrorHandler())
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Fetch<Entry>(_httpClient, cancellationToken);

    public async Task<IEnumerable<Entry>> Search(CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithMethod(HttpMethod.Get)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("entry")
            )
            .WithAcceptType(_entriesMediaType)
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthToken(AuthenticationContext.Token)
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Fetch<IEnumerable<Entry>>(_httpClient, cancellationToken);

    public async Task<Entry> Create(Entry entry, CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithBody(entry)
            .WithContentTypes(_entryMediaType)
            .WithMethod(HttpMethod.Post)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("entry")
            )
            .WithAcceptType(_entryMediaType)
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthToken(AuthenticationContext.Token)
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Fetch<Entry, Entry>(_httpClient, cancellationToken);

    public async Task Update(Entry entry, CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithBody(entry)
            .WithContentTypes(_entryMediaType)
            .WithMethod(HttpMethod.Put)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("entry")
            )
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthToken(AuthenticationContext.Token)
            .WithErrorHandler(new ResourceNotFoundErrorHandler())
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Send<Entry>(_httpClient, cancellationToken);

    public async Task Delete(Entry entry, CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithMethod(HttpMethod.Delete)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("entry")
                    .AppendPathSegment(entry.Id)
            )
            .WithAcceptType(_entryMediaType)
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthToken(AuthenticationContext.Token)
            .WithErrorHandler(new ResourceNotFoundErrorHandler())
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Send(_httpClient, cancellationToken);
}