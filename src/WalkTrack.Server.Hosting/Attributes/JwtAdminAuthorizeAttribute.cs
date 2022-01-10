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
using Microsoft.AspNetCore.Mvc.Filters;
using WalkTrack.Server.Authentications;

namespace WalkTrack.Server.Hosting.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class JwtAdminAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.Items["AuthenticationContext"] is null)
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
        }

        if (context.HttpContext.Items["AuthenticationContext"] is not SystemAuthenticationContext)
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
        }
    }
}