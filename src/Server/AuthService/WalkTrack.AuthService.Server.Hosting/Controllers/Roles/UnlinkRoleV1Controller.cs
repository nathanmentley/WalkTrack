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
[Authorize("unlink-role")]
[Route("v1/role/_unlink")]
public sealed class UnlinkRoleV1Controller: ControllerBase
{
    private readonly IRoleService _service;

    public UnlinkRoleV1Controller(IRoleService service)
    {
        _service = service ??
            throw new ArgumentNullException(nameof(service));
    }

    [HttpPost]
    public async Task<IActionResult> Unlink(
        [FromBody] RoleLinkRequest request,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (request is null)
        {
            throw new MissingBodyException();
        }

        await _service.Unlink(GetAuthenticationContext(), request, cancellationToken);

        return NoContent();
    }

    private AuthenticationContext GetAuthenticationContext() =>
        HttpContext.Items["AuthenticationContext"] as AuthenticationContext ??
            throw new UnauthorizedException();
}