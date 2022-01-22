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
[Route("v1/password/forgot")]
public sealed class RequestForgottenPasswordV1Controller
{
    private readonly IAuthenticationService _service;

    public RequestForgottenPasswordV1Controller(IAuthenticationService service)
    {
        _service = service ??
            throw new ArgumentNullException(nameof(service));
    }

    [HttpPost]
    public async Task<IActionResult> ForgottenPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        if (request is null)
        {
            throw new MissingBodyException($"{nameof(request)} is required for {nameof(ForgottenPassword)}.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        await _service.RequestForgottenPassword(request, cancellationToken);

        return new NoContentResult();
    }
}