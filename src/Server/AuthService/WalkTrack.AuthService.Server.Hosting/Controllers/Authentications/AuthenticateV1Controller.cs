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
using WalkTrack.AuthService.Common;

namespace WalkTrack.AuthService.Server.Hosting.Controllers.Authentications;

[ApiController]
[Route("v1/authenticate")]
public sealed class AuthenticateV1Controller
{
    private readonly IAuthenticationService _service;

    public AuthenticateV1Controller(IAuthenticationService service)
    {
        _service = service ??
            throw new ArgumentNullException(nameof(service));
    }

    [HttpPost]
    public async Task<IActionResult> Authenticate(
        [FromBody] AuthenticateRequest request,
        CancellationToken cancellationToken
    )
    {
        if (request is null)
        {
            throw new MissingBodyException($"{nameof(request)} is required for {nameof(Authenticate)}.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        AuthenticateResponse response = await _service.Authenticate(request, cancellationToken);

        return new ObjectResult(response)
        {
            StatusCode = StatusCodes.Status201Created
        };
    }
}