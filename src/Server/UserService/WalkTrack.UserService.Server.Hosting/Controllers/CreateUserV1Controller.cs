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
[Route("v1/user")]
public sealed class CreateUserV1Controller: ControllerBase
{
    private readonly IUserService _service;

    public CreateUserV1Controller(IUserService service)
    {
        if (service is null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequest createUserRequest,
        CancellationToken cancellationToken
    )
    {
        if (createUserRequest is null)
        {
            throw new MissingBodyException($"{nameof(createUserRequest)} is required for {nameof(Create)}.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        User user = FromCreateUserRequest(createUserRequest);

        User createdUser = await _service.Create(user, cancellationToken);

        return new ObjectResult(createdUser) { StatusCode = StatusCodes.Status201Created };
    }

    private static User FromCreateUserRequest(CreateUserRequest createUserRequest) =>
        new User()
        {
            Username = createUserRequest.Username,
            Email = createUserRequest.Email,
            IsPublic = createUserRequest.IsPublic,
            Password = createUserRequest.Password
        };
}