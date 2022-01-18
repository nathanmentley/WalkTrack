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
using WalkTrack.Framework.Server.DAL.Mssql;
using WalkTrack.Framework.Server.DAL.Mssql.Criteria;
using WalkTrack.UserService.Common;
using WalkTrack.UserService.Server.Configuration;

namespace WalkTrack.UserService.Server.DAL;

internal sealed class UserRepository: BaseRepository<User, UserPresistedResource>, IUserRepository
{
    private static readonly IEnumerable<ICriterionHandler> Handlers =
        new ICriterionHandler[] {
            new IdCriterionHandler(),
            new UsernameCriterionHandler()
        };

    private static readonly string TableName = "Users";

    public UserRepository(IOptions<DalSettings> dalSettings):
        base(
            TableName,
            dalSettings.Value.ConnectionString,
            Handlers
        )
    {
    }

    protected override UserPresistedResource ConvertToRecord(User resource) =>
        new UserPresistedResource()
        {
            Id = Guid.Parse(resource.Id),
            Username = resource.Username,
            Email = resource.Email,
            IsPublic = resource.IsPublic,
            Password = resource.Password,
            Salt = resource.Salt
        };

    protected override User FromRecord(UserPresistedResource record) =>
        new User()
        {
            Id = record.Id.ToString(),
            Username = record.Username,
            Email = record.Email,
            IsPublic = record.IsPublic,
            Password = record.Password,
            Salt = record.Salt
        };
}
