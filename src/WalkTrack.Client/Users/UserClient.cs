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
using WalkTrack.Common.Users;
using WalkTrack.Common.Resources;
using WalkTrack.Client.Authentications;

namespace WalkTrack.Client.Users;

internal sealed class UserClient: BaseClient, IUserClient
{
    private readonly ITranscoderProcessor _transcoder;
    private readonly HttpClient _httpClient;

    private static readonly WalkTrackMediaType _userMediaType =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.User")
            .WithVersion(1)
            .Build();

    private static readonly WalkTrackMediaType _usersMediaType =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Users")
            .WithVersion(1)
            .Build();

    private static readonly WalkTrackMediaType _createUserMediaType =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.CreateUserRequest")
            .WithVersion(1)
            .Build();

    private static readonly WalkTrackMediaType _updatePasswordMediaType =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.UpdatePasswordRequest")
            .WithVersion(1)
            .Build();

    public UserClient(ITranscoderProcessor transcoder)
    {
        _transcoder = transcoder ??
            throw new ArgumentNullException(nameof(transcoder));

        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:8000")
        };
    }

    public async Task<User> Fetch(string id, CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithMethod(HttpMethod.Get)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("user")
                    .AppendPathSegment(id)
            )
            .WithAcceptType(_userMediaType)
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthToken(AuthenticationContext.Token)
            .Fetch<User>(_httpClient, cancellationToken);

    public async Task<IEnumerable<User>> Search(CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithMethod(HttpMethod.Get)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("user")
            )
            .WithAcceptType(_usersMediaType)
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthToken(AuthenticationContext.Token)
            .Fetch<IEnumerable<User>>(_httpClient, cancellationToken);

    public async Task<User> Create(CreateUserRequest request, CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithBody(request)
            .WithContentTypes(_createUserMediaType)
            .WithMethod(HttpMethod.Post)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("user")
            )
            .WithAcceptType(_userMediaType)
            .WithAcceptType(MediaTypes.ApiError)
            .Fetch<CreateUserRequest, User>(_httpClient, cancellationToken);

    public async Task Update(User user, CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithBody(user)
            .WithContentTypes(_userMediaType)
            .WithMethod(HttpMethod.Put)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("user")
            )
            .WithAuthToken(AuthenticationContext.Token)
            .WithAcceptType(MediaTypes.ApiError)
            .Send<User>(_httpClient, cancellationToken);

    public async Task UpdatePassword(UpdatePasswordRequest request, CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithBody(request)
            .WithContentTypes(_updatePasswordMediaType)
            .WithMethod(HttpMethod.Put)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("user")
            )
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthToken(AuthenticationContext.Token)
            .Send<UpdatePasswordRequest>(_httpClient, cancellationToken);

    public async Task Delete(User user, CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithMethod(HttpMethod.Delete)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("user")
                    .AppendPathSegment(user.Id)
            )
            .WithAcceptType(_userMediaType)
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthToken(AuthenticationContext.Token)
            .Send(_httpClient, cancellationToken);
}