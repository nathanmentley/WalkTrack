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

namespace WalkTrack.App.State;

/// <summary>
/// </summary>
public static class AppState
{
    public static async Task<bool> IsLoggedIn(IAuthenticationClient authenticationClient, ILocalStorageService localStorage)
    {

        string token = await localStorage.GetItemAsync<string>("token");

        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        await authenticationClient.Login(token);

        return true;
    }

    public static async Task PersistToken(ILocalStorageService localStorage, string token)
    {
        await localStorage.SetItemAsync("token", token);
    }

    public static async Task Logout(ILocalStorageService localStorage)
    {
        await localStorage.SetItemAsync("token", string.Empty);
    }
}
