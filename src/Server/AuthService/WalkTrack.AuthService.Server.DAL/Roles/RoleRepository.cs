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
}
