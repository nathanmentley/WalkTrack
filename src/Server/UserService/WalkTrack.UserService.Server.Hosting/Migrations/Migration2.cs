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

namespace WalkTrack.UserService.Server.Hosting.Migrations;

[Migration(2)]
public class Migration2: Migration
{
    public override void Up()
    {
        Alter
            .Table("Users")
            .AddColumn("ResetToken")
                .AsString(255)
                .WithDefaultValue(string.Empty)
                .NotNullable()
            .AddColumn("ResetTokenExpiresAt")
                .AsDateTime2()
                .WithDefaultValue(DateTime.MinValue)
                .NotNullable();
    }

    public override void Down()
    {
        Delete
            .Column("ResetToken")
            .FromTable("Users");

        Delete
            .Column("ResetTokenExpiresAt")
            .FromTable("Users");
    }
}