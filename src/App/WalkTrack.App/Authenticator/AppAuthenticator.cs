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
using WalkTrack.Framework.Client.Authentications;
using WalkTrack.AuthService.Client;
using WalkTrack.AuthService.Common;

namespace WalkTrack.App.Authenticator;

public sealed class AppAuthenticator : IAuthenticator
{
    private static readonly string TokenKey = "token";

    private readonly IAuthenticationClient _authenticationClient;

    private string _token = string.Empty;

    public AppAuthenticator(
        IAuthenticationClient authenticationClient
    )
    {
        _authenticationClient = authenticationClient ??
            throw new ArgumentNullException(nameof(authenticationClient));
    }

    public Task<string> GetToken(CancellationToken cancellationToken = default) =>
        Task.FromResult(_token);

    public async Task Logout(
        ILocalStorageService localStorage,
        CancellationToken cancellationToken = default
    )
    {
        _token = string.Empty;

        await ClearStorage(localStorage, cancellationToken);
    }

    public async Task Login(
        ILocalStorageService localStorage,
        string token,
        CancellationToken cancellationToken = default
    )
    {
        _token = token;

        await Persist(localStorage, token, cancellationToken);
    }

    public async Task Login(
        ILocalStorageService localStorage,
        AuthenticateRequest request,
        CancellationToken cancellationToken = default
    )
    {
        AuthenticateResponse response = await _authenticationClient.Login(request, cancellationToken);

        _token = response.Token;

        await Persist(localStorage, response.Token, cancellationToken);
    }

    public async Task<bool> IsLoggedIn(
        ILocalStorageService localStorage,
        CancellationToken cancellationToken = default
    )
    {
        if (IsTokenSet())
        {
            return true;
        }

        await LoadFromStorage(localStorage, cancellationToken);

        return IsTokenSet();
    }

    private async Task LoadFromStorage(
        ILocalStorageService localStorage,
        CancellationToken cancellationToken = default
    )
    {
        string token = await localStorage.GetItemAsync<string>(TokenKey);

        if (string.IsNullOrWhiteSpace(token))
        {
            await Login(localStorage, token, cancellationToken);
        }
    }

    private async Task Persist(
        ILocalStorageService localStorage,
        string token,
        CancellationToken cancellationToken = default
    ) =>
        await localStorage.SetItemAsync(TokenKey, token, cancellationToken);

    private async Task ClearStorage(
        ILocalStorageService localStorage,
        CancellationToken cancellationToken = default
    ) =>
        await localStorage.SetItemAsync(TokenKey, string.Empty, cancellationToken);

    private bool IsTokenSet() =>
        !string.IsNullOrWhiteSpace(_token);
}