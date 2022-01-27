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

using Blazored.LocalStorage;
using WalkTrack.UserService.Client;
using WalkTrack.UserService.Common;

namespace WalkTrack.App.Authenticator;

public sealed class CachedAppAuthenticator: AppAuthenticator
{
    private static readonly string TokenKey = "token";

    private readonly ILocalStorageService _localStorage;

    public CachedAppAuthenticator(
        IAuthenticationClient authenticationClient,
        ILocalStorageService localStorage
    ):
        base(authenticationClient)
    {
        _localStorage = localStorage ??
            throw new ArgumentNullException(nameof(localStorage));
    }

    public override async Task Logout(CancellationToken cancellationToken = default)
    {
        await ClearStorage(cancellationToken);

        await base.Logout(cancellationToken);
    }

    public override async Task<AuthenticateResponse> Login(
        AuthenticateRequest request,
        CancellationToken cancellationToken = default
    )
    {
        AuthenticateResponse response = await base.Login(request, cancellationToken);

        await Persist(response, cancellationToken);

        return response;
    }

    public override async Task<bool> IsLoggedIn(CancellationToken cancellationToken = default)
    {
        if (await base.IsLoggedIn())
        {
            return true;
        }

        await LoadFromStorage(cancellationToken);

        return await base.IsLoggedIn();
    }

    private async Task LoadFromStorage(CancellationToken cancellationToken = default)
    {
        string token = await _localStorage.GetItemAsync<string>(TokenKey);

        if (string.IsNullOrWhiteSpace(token))
        {
            await Login(token, cancellationToken);
        }
    }

    private async Task Persist(AuthenticateResponse response, CancellationToken cancellationToken = default)
    {
        await _localStorage.SetItemAsync(TokenKey, response.Token);
    }

    private async Task ClearStorage(CancellationToken cancellationToken = default)
    {
        await _localStorage.SetItemAsync(TokenKey, string.Empty);
    }
}