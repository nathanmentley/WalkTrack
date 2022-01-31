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

using WalkTrack.AuthService.Common;
using WalkTrack.Framework.Server.Authorizations;

namespace WalkTrack.AuthService.Client;

internal sealed class ServiceAuthorizer : IAuthorizer
{
    private readonly IAuthorizationClient _authenticationClient;


    public ServiceAuthorizer(
        IAuthorizationClient authenticationClient
    )
    {
        _authenticationClient = authenticationClient ??
            throw new ArgumentNullException(nameof(authenticationClient));
    }

    public async Task<bool> Authorize(
        string token,
        string permission,
        CancellationToken cancellationToken = default
    )
    {
        AuthorizeResponse response = await _authenticationClient.Authorize(
            new AuthorizeRequest()
            {
                Token = token,
                Permission = permission
            },
            cancellationToken
        );

        return response.IsAuthorized;
    }
}