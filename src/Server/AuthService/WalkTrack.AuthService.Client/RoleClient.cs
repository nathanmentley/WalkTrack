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
using WalkTrack.AuthService.Common;
using WalkTrack.Framework.Client;
using WalkTrack.Framework.Client.Authentications;
using WalkTrack.Framework.Common.Resources;

namespace WalkTrack.AuthService.Client;

internal sealed class RoleClient: BaseClient, IRoleClient, IDisposable
{
    private readonly IAuthenticator _authenicator;
    private readonly ITranscoderProcessor _transcoder;
    private readonly HttpClient _httpClient;

    public RoleClient(string url, IAuthenticator authenicator, ITranscoderProcessor transcoder)
    {
        _transcoder = transcoder ??
            throw new ArgumentNullException(nameof(transcoder));

        _authenicator = authenicator ??
            throw new ArgumentNullException(nameof(authenicator));

        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(url)
        };
    }

    public async Task<Role> Fetch(string id, CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithMethod(HttpMethod.Get)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("role")
                    .AppendPathSegment(id)
            )
            .WithAcceptType(MediaTypes.Role)
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthenticator(_authenicator)
            .WithErrorHandler(new ResourceNotFoundErrorHandler())
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Fetch<Role>(_httpClient, cancellationToken);

    public async Task<IEnumerable<Role>> Search(CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithMethod(HttpMethod.Get)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("role")
            )
            .WithAcceptType(MediaTypes.Roles)
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthenticator(_authenicator)
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Fetch<IEnumerable<Role>>(_httpClient, cancellationToken);

    public async Task<Role> Create(Role role, CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithBody(role)
            .WithContentTypes(MediaTypes.Role)
            .WithMethod(HttpMethod.Post)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("role")
            )
            .WithAcceptType(MediaTypes.Role)
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthenticator(_authenicator)
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Fetch<Role, Role>(_httpClient, cancellationToken);

    public async Task Delete(string id, CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithMethod(HttpMethod.Delete)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("role")
                    .AppendPathSegment(id)
            )
            .WithAcceptType(MediaTypes.Role)
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthenticator(_authenicator)
            .WithErrorHandler(new ResourceNotFoundErrorHandler())
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Send(_httpClient, cancellationToken);

    public async Task Link(
        RoleLinkRequest request,
        CancellationToken cancellationToken = default
    ) =>
        await new RequestBuilder(_transcoder)
            .WithBody(request)
            .WithContentTypes(MediaTypes.RoleLinkRequest)
            .WithMethod(HttpMethod.Post)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("role")
                    .AppendPathSegment("_link")
            )
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthenticator(_authenicator)
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Send<RoleLinkRequest>(_httpClient, cancellationToken);

    public async Task Unlink(
        RoleLinkRequest request,
        CancellationToken cancellationToken = default
    ) =>
        await new RequestBuilder(_transcoder)
            .WithBody(request)
            .WithContentTypes(MediaTypes.RoleLinkRequest)
            .WithMethod(HttpMethod.Post)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("role")
                    .AppendPathSegment("_unlink")
            )
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthenticator(_authenicator)
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Send<RoleLinkRequest>(_httpClient, cancellationToken);

    public void Dispose() =>
        _httpClient.Dispose();
}