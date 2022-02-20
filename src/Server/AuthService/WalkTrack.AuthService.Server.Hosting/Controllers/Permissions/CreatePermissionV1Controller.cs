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

namespace WalkTrack.AuthService.Server.Hosting.Controllers.Permissions;

[ApiController]
[Authorize("TODO")]
[Route("v1/permission")]
public sealed class CreatePermissionV1Controller: ControllerBase
{
    private readonly IPermissionService _service;

    public CreatePermissionV1Controller(IPermissionService service)
    {
        if (service is null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Permission permission, CancellationToken cancellationToken)
    {
        if (permission is null)
        {
            throw new MissingBodyException($"{nameof(permission)} is required for {nameof(Create)}.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        Permission createdPermission = await _service.Create(GetAuthenticationContext(), permission, cancellationToken);

        return new ObjectResult(createdPermission) { StatusCode = StatusCodes.Status201Created };
    }

    private AuthenticationContext GetAuthenticationContext() =>
        HttpContext.Items["AuthenticationContext"] as AuthenticationContext ??
            throw new UnauthorizedException();
}