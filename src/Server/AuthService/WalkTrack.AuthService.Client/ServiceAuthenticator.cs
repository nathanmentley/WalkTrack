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

using Microsoft.Extensions.Options;
using WalkTrack.Framework.Client.Authentications;
using WalkTrack.Framework.Client.Exceptions;
using WalkTrack.AuthService.Common;

namespace WalkTrack.AuthService.Client;

internal sealed class ServiceAuthenticator : IAuthenticator
{
    private readonly ServiceAuthenticatorSettings _configuration;
    private readonly IAuthenticationClient _authenticationClient;

    private AuthenticateResponse? _cachedResponse = null;

    public ServiceAuthenticator(
        IOptions<ServiceAuthenticatorSettings> configuration,
        IAuthenticationClient authenticationClient
    )
    {
        _configuration = configuration.Value ??
            throw new ArgumentNullException(nameof(configuration));

        _authenticationClient = authenticationClient ??
            throw new ArgumentNullException(nameof(authenticationClient));
    }

    public async Task<string> GetToken(CancellationToken cancellationToken = default)
    {
        if (IsCacheStale())
        {
            await Login(cancellationToken);
        }

        return _cachedResponse is null ?
            throw new UnauthorizedClientException():
            _cachedResponse.Token;
    }

    private async Task Login(CancellationToken cancellationToken = default)
    {
        _cachedResponse = await _authenticationClient.Login(
            new AuthenticateRequest()
            {
                Username = _configuration.Username,
                Password = _configuration.Password
            },
            cancellationToken
        );
    }

    private bool IsCacheStale() =>
        true;
}