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
using WalkTrack.Framework.Common.Criteria;
using WalkTrack.Framework.Server.Authentications;
using WalkTrack.Framework.Server.Exceptions;

namespace WalkTrack.AuthService.Server.Services.Roles;

internal sealed class RoleService: IRoleService
{
    private readonly IRoleRepository _repository;

    public RoleService(IRoleRepository repository)
    {
        if (repository is null)
        {
            throw new ArgumentNullException(nameof(repository));
        }

        _repository = repository;
    }
 
    public async Task<Role> Fetch(
        AuthenticationContext authenticationContext,
        string id,
        CancellationToken cancellationToken = default
    )
    {
        IEnumerable<Role> roles = await _repository.Search(
            SetupCriteriaForAuthenticationContext(
                authenticationContext,
                new ICriterion[] {
                    new IdCriterion(id)
                }
            ),
            cancellationToken
        );

        return roles.Count() switch {
            0 =>
                throw new ResourceNotFoundException(),
            1 =>
                roles.Single(),
            _ =>
                throw new ResourceNotFoundException()
        };
    }

    public Task<IEnumerable<Role>> Search(
        AuthenticationContext authenticationContext,
        IEnumerable<ICriterion> criteria,
        CancellationToken cancellationToken = default
    ) =>
        _repository.Search(
            SetupCriteriaForAuthenticationContext(authenticationContext, criteria),
            cancellationToken
        );

    public Task<Role> Create(
        AuthenticationContext authenticationContext,
        Role resource,
        CancellationToken cancellationToken = default
    ) =>
        _repository.Create(
            resource with { Id = Guid.NewGuid().ToString() },
            cancellationToken
        );

    public Task<Role> Update(
        AuthenticationContext authenticationContext,
        Role resource,
        CancellationToken cancellationToken = default
    ) =>
        _repository.Update(
            resource,
            cancellationToken
        );

    public async Task Delete(
        AuthenticationContext authenticationContext,
        string id,
        CancellationToken cancellationToken = default
    )
    {
        IEnumerable<Role> roles = await _repository.Search(
            SetupCriteriaForAuthenticationContext(
                authenticationContext,
                new ICriterion[] {
                    new IdCriterion(id)
                }
            ),
            cancellationToken
        );

        if (roles.Any())
        {
            await _repository.Delete(id, cancellationToken);
        }
    }

    private static IEnumerable<ICriterion> SetupCriteriaForAuthenticationContext(
        AuthenticationContext authenticationContext,
        IEnumerable<ICriterion> criteria
    )
    {
        if (authenticationContext is UserAuthenticationContext userAuthenticationContext)
        {
            List<ICriterion> newCriteria = new List<ICriterion>();

            newCriteria.AddRange(criteria);

            newCriteria.Add(new UserIdCriterion(userAuthenticationContext.UserId));

            return newCriteria;
        }

        return criteria;
    }
}