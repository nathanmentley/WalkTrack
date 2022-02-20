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
using WalkTrack.EntryService.Common;
using WalkTrack.Framework.Server.Authentications;
using WalkTrack.Framework.Server.Hosting.Attributes;
using WalkTrack.Framework.Server.Hosting.Exceptions;

namespace WalkTrack.EntryService.Server.Hosting.Controllers;

[ApiController]
[Authorize("create-entry")]
[Route("v1/entry")]
public sealed class CreateEntryV1Controller: ControllerBase
{
    private readonly IEntryService _service;

    public CreateEntryV1Controller(IEntryService service)
    {
        if (service is null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Entry entry, CancellationToken cancellationToken)
    {
        if (entry is null)
        {
            throw new MissingBodyException($"{nameof(entry)} is required for {nameof(Create)}.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        Entry createdEntry = await _service.Create(GetAuthenticationContext(), entry, cancellationToken);

        return new ObjectResult(createdEntry) { StatusCode = StatusCodes.Status201Created };
    }

    private AuthenticationContext GetAuthenticationContext() =>
        HttpContext.Items["AuthenticationContext"] as AuthenticationContext ??
            throw new UnauthorizedException();
}