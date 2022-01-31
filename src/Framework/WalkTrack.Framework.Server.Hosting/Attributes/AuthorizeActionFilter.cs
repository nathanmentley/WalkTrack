

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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WalkTrack.Framework.Server.Authentications;
using WalkTrack.Framework.Server.Authorizations;

namespace WalkTrack.Framework.Server.Hosting.Attributes;

public class AuthorizeActionFilter : IAsyncActionFilter
{
    private readonly IAuthorizer _authorizer;

    public AuthorizeActionFilter(IAuthorizer authorizer)
    {
        if (authorizer is null)
        {
            throw new ArgumentNullException(nameof(authorizer));
        }

        _authorizer = authorizer;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        IEnumerable<AuthorizeAttribute> authorizeAttributes =
            context.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>();

        if (authorizeAttributes.Any())
        {
            if (context.HttpContext.Items["AuthenticationContext"] is AuthenticationContext authenticationContext)
            {
                AuthorizeAttribute authorizeAttribute = authorizeAttributes.First();

                if (!await _authorizer.Authorize(authenticationContext.Token, authorizeAttribute.PermissionKey))
                {
                    context.Result = new JsonResult(
                        new
                        {
                            message = "Unauthorized",
                            statusCode = StatusCodes.Status401Unauthorized
                        }
                    )
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };

                    return;
                }
            }
            else
            {
                context.Result = new JsonResult(
                    new
                    {
                        message = "Forbidden",
                        statusCode = StatusCodes.Status403Forbidden
                    }
                )
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };

                return;
            }
        }

        await next();
    }
}