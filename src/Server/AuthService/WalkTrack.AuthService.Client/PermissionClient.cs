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

internal sealed class PermissionClient: BaseClient, IPermissionClient, IDisposable
{
    private readonly IAuthenticator _authenicator;
    private readonly ITranscoderProcessor _transcoder;
    private readonly HttpClient _httpClient;

    public PermissionClient(string url, IAuthenticator authenicator, ITranscoderProcessor transcoder)
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

    public async Task<Permission> Fetch(string id, CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithMethod(HttpMethod.Get)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("permission")
                    .AppendPathSegment(id)
            )
            .WithAcceptType(MediaTypes.Permission)
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthenticator(_authenicator)
            .WithErrorHandler(new ResourceNotFoundErrorHandler())
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Fetch<Permission>(_httpClient, cancellationToken);

    public async Task<IEnumerable<Permission>> Search(CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithMethod(HttpMethod.Get)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("permission")
            )
            .WithAcceptType(MediaTypes.Permissions)
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthenticator(_authenicator)
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Fetch<IEnumerable<Permission>>(_httpClient, cancellationToken);

    public async Task<Permission> Create(Permission permission, CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithBody(permission)
            .WithContentTypes(MediaTypes.Permission)
            .WithMethod(HttpMethod.Post)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("permission")
            )
            .WithAcceptType(MediaTypes.Permission)
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthenticator(_authenicator)
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Fetch<Permission, Permission>(_httpClient, cancellationToken);

    public async Task Delete(string id, CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithMethod(HttpMethod.Delete)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("permission")
                    .AppendPathSegment(id)
            )
            .WithAcceptType(MediaTypes.Permission)
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthenticator(_authenicator)
            .WithErrorHandler(new ResourceNotFoundErrorHandler())
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Send(_httpClient, cancellationToken);

    public void Dispose() =>
        _httpClient.Dispose();
}