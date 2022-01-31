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
using WalkTrack.EmailService.Common;
using WalkTrack.Framework.Server.Hosting.Attributes;
using WalkTrack.Framework.Server.Hosting.Exceptions;

namespace WalkTrack.EmailService.Server.Hosting.Controllers;

[ApiController]
[Authorize("")]
[Route("v1/email/send")]
public class SendEmailV1Controller : ControllerBase
{
    private readonly IEmailService _service;

    public SendEmailV1Controller(IEmailService service)
    {
        if (service is null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Send(IEnumerable<Email> emails, CancellationToken cancellationToken)
    {
        if (emails is null)
        {
            throw new MissingBodyException($"{nameof(emails)} is required for {nameof(Send)}.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        await _service.Send(emails, cancellationToken);

        return new NoContentResult();
    }
}
