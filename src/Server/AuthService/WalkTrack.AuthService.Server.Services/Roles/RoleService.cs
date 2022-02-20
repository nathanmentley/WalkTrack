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
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;

    public RoleService(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository
    )
    {
        _roleRepository = roleRepository ??
            throw new ArgumentNullException(nameof(roleRepository));

        _permissionRepository = permissionRepository ??
            throw new ArgumentNullException(nameof(permissionRepository));
    }
 
    public async Task<Role> Fetch(
        AuthenticationContext authenticationContext,
        string id,
        CancellationToken cancellationToken = default
    )
    {
        IEnumerable<Role> roles = await _roleRepository.Search(
            new ICriterion[] {
                new IdCriterion(id)
            },
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
        _roleRepository.Search(
            criteria,
            cancellationToken
        );

    public Task<Role> Create(
        AuthenticationContext authenticationContext,
        Role resource,
        CancellationToken cancellationToken = default
    ) =>
        _roleRepository.Create(
            resource with { Id = Guid.NewGuid().ToString() },
            cancellationToken
        );

    public Task<Role> Update(
        AuthenticationContext authenticationContext,
        Role resource,
        CancellationToken cancellationToken = default
    ) =>
        _roleRepository.Update(
            resource,
            cancellationToken
        );

    public async Task Delete(
        AuthenticationContext authenticationContext,
        string id,
        CancellationToken cancellationToken = default
    )
    {
        IEnumerable<Role> roles = await _roleRepository.Search(
            new ICriterion[] {
                new IdCriterion(id)
            },
            cancellationToken
        );

        if (roles.Any())
        {
            await _roleRepository.Delete(id, cancellationToken);
        }
    }

    public async Task Link(
        AuthenticationContext authenticationContext,
        RoleLinkRequest roleLinkRequest,
        CancellationToken cancellationToken = default
    ) =>
        await _roleRepository.Link(
            await GetRoleId(
                roleLinkRequest.RoleName,
                cancellationToken
            ),
            await GetPermissionId(
                roleLinkRequest.PermissionName,
                cancellationToken
            ),
            cancellationToken
        );
 
    public async Task Unlink(
        AuthenticationContext authenticationContext,
        RoleLinkRequest roleLinkRequest,
        CancellationToken cancellationToken = default
    ) =>
        await _roleRepository.Unlink(
            await GetRoleId(
                roleLinkRequest.RoleName,
                cancellationToken
            ),
            await GetPermissionId(
                roleLinkRequest.PermissionName,
                cancellationToken
            ),
            cancellationToken
        );

    private async Task<string> GetRoleId(
        string roleName,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<Role> roles =
            await _roleRepository.Search(
                Enumerable.Empty<ICriterion>(),
                cancellationToken
            );
        
        Role? role = roles.FirstOrDefault(
            role => string.Equals(roleName, role.Name)
        );

        if (role is not null)
        {
            return role.Id;
        }

        throw new InvalidRequestException($"Role {roleName} does not exist.");
    }

    private async Task<string> GetPermissionId(
        string permissionName,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<Permission> permissions =
            await _permissionRepository.Search(
                Enumerable.Empty<ICriterion>(),
                cancellationToken
            );
        
        Permission? permission = permissions.FirstOrDefault(
            permission => string.Equals(permissionName, permission.Name)
        );

        if (permission is not null)
        {
            return permission.Id;
        }

        throw new InvalidRequestException($"Permission {permissionName} does not exist.");
    }
}