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

using Microsoft.Extensions.Options;
using SqlKata;
using SqlKata.Execution;
using WalkTrack.AuthService.Common;
using WalkTrack.AuthService.Server.Configuration;
using WalkTrack.Framework.Server.DAL.Mssql;
using WalkTrack.Framework.Server.DAL.Mssql.Criteria;

namespace WalkTrack.AuthService.Server.DAL.Roles;

internal sealed class RoleRepository: BaseRepository<Role, RolePresistedResource>, IRoleRepository
{
    private static readonly IEnumerable<ICriterionHandler> Handlers =
        new ICriterionHandler[] {
            new IdCriterionHandler()
        };

    private static readonly string TableName = "Roles";

    public RoleRepository(IOptions<DalSettings> dalSettings):
        base(
            TableName,
            dalSettings.Value.ConnectionString,
            Handlers
        )
    {
    }

    public async Task<bool> DoesRoleHavePermission(
        string roleId,
        string permissionId,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        IEnumerable<dynamic> records = await GetRolePermissionQuery()
            .Where("RoleId", roleId)
            .Where("PermissionId", permissionId)
            .GetAsync();

        return records.Any();
    }

    public async Task Link(string roleId, string permissionId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!await DoesRoleHavePermission(roleId, permissionId, cancellationToken))
        {
            await GetRolePermissionQuery()
                .InsertAsync(
                    new Dictionary<string, object>()
                    {
                        { "Id", Guid.NewGuid().ToString() },
                        { "RoleId", roleId },
                        { "PermissionId", permissionId }
                    }
                );
        }
    }

    public async Task Unlink(string roleId, string permissionId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await GetRolePermissionQuery()
            .Where("RoleId", roleId)
            .Where("PermissionId", permissionId)
            .DeleteAsync();
    }

    protected override RolePresistedResource ConvertToRecord(Role resource) =>
        new RolePresistedResource()
        {
            Id = Guid.Parse(resource.Id),
            Name = resource.Name
        };

    protected override Role FromRecord(RolePresistedResource record) =>
        new Role()
        {
            Id = record.Id.ToString(),
            Name = record.Name
        };

    private Query GetRolePermissionQuery() =>
        GetQueryFactory().Query("RolePermissions");
}
