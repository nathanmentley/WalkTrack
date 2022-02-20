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

using Microsoft.AspNetCore.Mvc;
using WalkTrack.AuthService.Common;
using WalkTrack.Framework.Server.Authentications;
using WalkTrack.Framework.Server.Hosting.Attributes;
using WalkTrack.Framework.Server.Hosting.Exceptions;

namespace WalkTrack.AuthService.Server.Hosting.Controllers.Roles;

[ApiController]
[Authorize("TODO")]
[Route("v1/role")]
public sealed class CreateRoleV1Controller: ControllerBase
{
    private readonly IRoleService _service;

    public CreateRoleV1Controller(IRoleService service)
    {
        if (service is null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Role role, CancellationToken cancellationToken)
    {
        if (role is null)
        {
            throw new MissingBodyException($"{nameof(role)} is required for {nameof(Create)}.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        Role createdRole = await _service.Create(GetAuthenticationContext(), role, cancellationToken);

        return new ObjectResult(createdRole) { StatusCode = StatusCodes.Status201Created };
    }

    private AuthenticationContext GetAuthenticationContext() =>
        HttpContext.Items["AuthenticationContext"] as AuthenticationContext ??
            throw new UnauthorizedException();
}