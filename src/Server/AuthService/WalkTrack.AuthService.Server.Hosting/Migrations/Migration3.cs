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

[Migration(3)]
public class Migration3: Migration
{
    public override void Up()
    {
        Create
            .Table("RolePermissions")
            .WithColumn("Id").AsGuid().NotNullable().PrimaryKey().Unique()
            .WithColumn("RoleId").AsGuid().NotNullable()
            .WithColumn("PermissionId").AsGuid().NotNullable();

        Create.ForeignKey()
            .FromTable("RolePermissions").ForeignColumn("RoleId")
            .ToTable("Roles").PrimaryColumn("Id");

        Create.ForeignKey()
            .FromTable("RolePermissions").ForeignColumn("PermissionId")
            .ToTable("Permissions").PrimaryColumn("Id");
    }

    public override void Down()
    {
        Delete
            .Table("RolePermissions");
    }
}