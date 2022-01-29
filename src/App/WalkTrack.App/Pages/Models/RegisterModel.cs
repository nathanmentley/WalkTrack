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
using WalkTrack.UserService.Common;

namespace WalkTrack.App.Pages.Models;

public sealed class RegisterModel
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool Loading { get; set; } = false;

    public CreateUserRequest ToCreateRequest() =>
        new CreateUserRequest()
        {
            Username = Username,
            Email = Email,
            IsPublic = false,
            Password = Password
        };

    public AuthenticateRequest ToLoginRequest() =>
        new AuthenticateRequest()
        {
            Username = Username,
            Password = Password
        };
}