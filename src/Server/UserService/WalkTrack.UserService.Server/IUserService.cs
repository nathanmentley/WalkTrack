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

using WalkTrack.Framework.Common.Criteria;
using WalkTrack.Framework.Server.Authentications;
using WalkTrack.UserService.Common;

namespace WalkTrack.UserService.Server;

public interface IUserService
{
    Task<User> Fetch(
        AuthenticationContext authenticationContext,
        string id,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<User>> Search(
        AuthenticationContext authenticationContext,
        IEnumerable<ICriterion> criteria,
        CancellationToken cancellationToken = default
    );

    Task<User> Create(
        User resource,
        CancellationToken cancellationToken = default
    );

    Task<User> Update(
        AuthenticationContext authenticationContext,
        User user,
        CancellationToken cancellationToken = default
    );

    Task<User> UpdatePassword(
        AuthenticationContext authenticationContext,
        string userId,
        string password,
        CancellationToken cancellationToken = default
    );

    Task Delete(
        AuthenticationContext authenticationContext,
        string id,
        CancellationToken cancellationToken = default
    );
}