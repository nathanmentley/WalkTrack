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
using WalkTrack.Framework.Server.Hosting.Attributes;
using WalkTrack.Framework.Server.Hosting.Exceptions;

namespace WalkTrack.AuthService.Server.Hosting.Controllers.Authentications;

[ApiController]
//[Authorize("TODO")]
[Route("v1/authentication")]
public sealed class CreateAuthenticationV1Controller
{
    private readonly IAuthenticationService _service;

    public CreateAuthenticationV1Controller(IAuthenticationService service)
    {
        _service = service ??
            throw new ArgumentNullException(nameof(service));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateAuthRequest request,
        CancellationToken cancellationToken
    )
    {
        if (request is null)
        {
            throw new MissingBodyException($"{nameof(request)} is required for {nameof(Create)}.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        await _service.Create(request, cancellationToken);

        return new NoContentResult();
    }
}