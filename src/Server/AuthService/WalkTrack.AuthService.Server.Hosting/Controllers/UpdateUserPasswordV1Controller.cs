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
/*
using Microsoft.AspNetCore.Mvc;
using WalkTrack.Framework.Server.Authentications;
using WalkTrack.Framework.Server.Hosting.Attributes;
using WalkTrack.Framework.Server.Hosting.Exceptions;
using WalkTrack.AuthService.Common;

namespace WalkTrack.AuthService.Server.Hosting.Controllers;

[ApiController]
[JwtAuthorize]
[Route("v1/user/{userId}/_updatePassword")]
public sealed class UpdateUserPasswordV1Controller: ControllerBase
{
    private readonly IAuthenticationService _service;

    public UpdateUserPasswordV1Controller(IAuthenticationService service)
    {
        if (service is null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        _service = service;
    }

    [HttpPut]
    public async Task<IActionResult> Update(
        [FromRoute] string userId,
        [FromBody] UpdatePasswordRequest updatePasswordRequest,
        CancellationToken cancellationToken
    )
    {
        if (updatePasswordRequest is null)
        {
            throw new MissingBodyException($"{nameof(updatePasswordRequest)} is required for {nameof(Update)}.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        await _service.UpdatePassword(
            GetAuthenticationContext(),
            userId,
            updatePasswordRequest.Password,
            cancellationToken
        );

        return new NoContentResult();
    }

    private AuthenticationContext GetAuthenticationContext() =>
        HttpContext.Items["AuthenticationContext"] as AuthenticationContext ??
            throw new UnauthorizedException();
}
*/