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
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WalkTrack.Framework.Server.Authentications;

namespace WalkTrack.Framework.Server.Hosting.Middlewares;

internal sealed class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public JwtMiddleware(
        IOptions<AuthenticationSettings> authenticationSettings,
        RequestDelegate next
    )
    {
        if (authenticationSettings is null)
        {
            throw new ArgumentNullException(nameof(authenticationSettings));
        }

        _next = next;

        _tokenHandler = new JwtSecurityTokenHandler();

        _tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authenticationSettings.Value.JwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    }

    public async Task Invoke(HttpContext context)
    {
        string? token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token is not null)
        {
            AttachUserToContext(context, token);
        }

        await _next(context);
    }

    private void AttachUserToContext(HttpContext context, string token)
    {
        try
        {
            _tokenHandler.ValidateToken(token, _tokenValidationParameters, out SecurityToken validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken)
            {
                throw new InvalidOperationException("Cannot process authentication because the token isn't a valid JWT.");
            }

            if (jwtToken.Claims.FirstOrDefault(x => x.Type == "admin")?.Value == "admin")
            {
                context.Items["AuthenticationContext"] = new SystemAuthenticationContext(token);
            }
            else
            {
                string userId = jwtToken.Claims.First(x => x.Type == "id").Value;

                context.Items["AuthenticationContext"] = new UserAuthenticationContext(token)
                {
                    UserId = userId
                };
            }
        }
        catch
        {
        }
    }
}