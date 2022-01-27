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

using WalkTrack.Framework.Client.Authentications;
using WalkTrack.Framework.Client.Exceptions;
using WalkTrack.UserService.Common;

namespace WalkTrack.UserService.Client;

public abstract class AppAuthenticator : IAuthenticator
{
    private readonly IAuthenticationClient _authenticationClient;

    private string? _token = null;

    public AppAuthenticator(IAuthenticationClient authenticationClient)
    {
        _authenticationClient = authenticationClient ??
            throw new ArgumentNullException(nameof(authenticationClient));
    }

    public virtual Task<string> GetToken(CancellationToken cancellationToken = default)
    {
        if (_token is not null)
        {
            return Task.FromResult(_token);
        }

        throw new UnauthorizedClientException();
    }

    public virtual async Task<AuthenticateResponse> Login(
        AuthenticateRequest request,
        CancellationToken cancellationToken = default
    )
    {
        AuthenticateResponse response = await _authenticationClient.Login(request, cancellationToken);

        _token = response.Token;

        return response;
    }

    public virtual Task Login(string token, CancellationToken cancellationToken = default)
    {
        _token = token;

        return Task.CompletedTask;
    }

    public virtual Task Logout(CancellationToken cancellationToken = default)
    {
        _token = null;

        return Task.CompletedTask;
    }

    public virtual Task<bool> IsLoggedIn(CancellationToken cancellationToken = default) =>
        Task.FromResult(_token is not null);
}