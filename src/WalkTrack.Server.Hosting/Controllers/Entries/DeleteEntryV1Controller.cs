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
using WalkTrack.Server.Authentications;
using WalkTrack.Server.Entries;
using WalkTrack.Server.Hosting.Attributes;
using WalkTrack.Server.Hosting.Exceptions;

namespace WalkTrack.Server.Hosting.Controllers.Entries;

[ApiController]
[JwtAuthorize]
[Route("v1/entry/{id}")]
public sealed class DeleteEntryV1Controller: ControllerBase
{
    private readonly IEntryService _service;

    public DeleteEntryV1Controller(IEntryService service)
    {
        if (service is null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        _service = service;
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(id))
        {
            throw new MissingRouteParameterException();
        }

        await _service.Delete(GetAuthenticationContext(), id, cancellationToken);

        return new NoContentResult();
    }

    private AuthenticationContext GetAuthenticationContext() =>
        HttpContext.Items["AuthenticationContext"] as AuthenticationContext ??
            throw new UnauthorizedException();
}