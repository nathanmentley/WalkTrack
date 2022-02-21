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
using WalkTrack.Framework.Server.Hosting.Exceptions;
using WalkTrack.UserService.Common;

namespace WalkTrack.UserService.Server.Hosting.Controllers;

[ApiController]
[Route("v1/user/_forgotPassword")]
public sealed class ForgotPasswordV1Controller: ControllerBase
{
    private readonly IUserService _service;

    public ForgotPasswordV1Controller(IUserService service)
    {
        if (service is null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPassword request,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (request is null)
        {
            throw new MissingBodyException();
        }

        await _service.ForgotPassword(request, cancellationToken);

        return NoContent();
    }
}
