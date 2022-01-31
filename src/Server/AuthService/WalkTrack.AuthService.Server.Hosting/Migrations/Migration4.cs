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

[Migration(4)]
public class Migration4: Migration
{
    public override void Up()
    {
        Create
            .Table("Authentications")
            .WithColumn("Id").AsGuid().NotNullable().PrimaryKey().Unique()
            .WithColumn("Username").AsString(255).NotNullable().Unique()
            .WithColumn("Password").AsString(255).NotNullable()
            .WithColumn("RoleId").AsGuid().NotNullable()
            .WithColumn("Salt").AsString(255).NotNullable()
            .WithColumn("ResetToken").AsString(255).WithDefaultValue(string.Empty).NotNullable()
            .WithColumn("ResetTokenExpiresAt").AsDateTime2().WithDefaultValue(DateTime.MinValue).NotNullable();

        Create
            .Index("index_authentications_username")
            .OnTable("Authentications")
            .OnColumn("Username")
            .Unique();

        Create.ForeignKey()
            .FromTable("Authentications").ForeignColumn("RoleId")
            .ToTable("Roles").PrimaryColumn("Id");
    }

    public override void Down()
    {
        Delete
            .Table("Authentications");

        Delete
            .Index("index_authentications_username");
    }
}