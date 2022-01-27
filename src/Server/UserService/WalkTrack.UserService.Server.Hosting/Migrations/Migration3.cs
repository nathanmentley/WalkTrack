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

using FluentMigrator;
using WalkTrack.UserService.Common;

namespace WalkTrack.UserService.Server.Hosting.Migrations;

[Migration(3)]
public class Migration3: Migration
{
    private readonly IUserService _userService;

    public Migration3(IUserService userService)
    {
        _userService = userService ??
            throw new ArgumentNullException(nameof(userService));
    }

    public override void Up()
    {
        var a = _userService.Create(
            new User()
            {
                Username = "EmailService",
                Email = "EmailService@example.com",
                Password = "password"
            }
        ).Result;

        var b = _userService.Create(
            new User()
            {
                Username = "EntryService",
                Email = "EntryService@example.com",
                Password = "password"
            }
        ).Result;

        var v = _userService.Create(
            new User()
            {
                Username = "GoalService",
                Email = "GoalService@example.com",
                Password = "password"
            }
        ).Result;

        var d = _userService.Create(
            new User()
            {
                Username = "UserService",
                Email = "UserService@example.com",
                Password = "password"
            }
        ).Result;
    }

    public override void Down()
    {
    }
}