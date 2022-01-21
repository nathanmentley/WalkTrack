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

namespace WalkTrack.App.State;

/// <summary>
/// </summary>
public static class AppState
{
    private static readonly string TokenKey = "token";
    private static readonly string UserIdKey = "userId";

    public static string UserId { get; private set; } = string.Empty;

    public static async Task<bool> IsLoggedIn(
        IAuthenticationClient authenticationClient,
        ILocalStorageService localStorage
    )
    {
        string token = await localStorage.GetItemAsync<string>(TokenKey);
        UserId = await localStorage.GetItemAsync<string>(UserIdKey);

        if (
            string.IsNullOrWhiteSpace(token) ||
            string.IsNullOrWhiteSpace(UserId)
        )
        {
            return false;
        }

        Token refreshedToken = await authenticationClient.RefreshToken(new Token() { Id = token });

        await authenticationClient.Login(refreshedToken);

        return true;
    }

    public static async Task PersistToken(ILocalStorageService localStorage, string token, string userId)
    {
        await localStorage.SetItemAsync(TokenKey, token);
        await localStorage.SetItemAsync(UserIdKey, userId);
    }

    public static async Task Logout(ILocalStorageService localStorage)
    {
        await localStorage.SetItemAsync(TokenKey, string.Empty);
        await localStorage.SetItemAsync(UserIdKey, string.Empty);

        UserId = string.Empty;
    }
}
