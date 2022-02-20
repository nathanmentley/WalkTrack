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

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WalkTrack.AuthService.Common;
using WalkTrack.Framework.Common.Criteria;
using WalkTrack.Framework.Server;

namespace WalkTrack.AuthService.Server.Services.Authentications;

/// <summary>
/// </summary>
internal sealed class AuthorizationService : IAuthorizationService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public AuthorizationService(
        IOptions<AuthenticationSettings> authenticationSettings,
        IPermissionRepository permissionRepository,
        IRoleRepository roleRepository
    )
    {
        if (authenticationSettings is null)
        {
            throw new ArgumentNullException(nameof(authenticationSettings));
        }

        _permissionRepository = permissionRepository ??
            throw new ArgumentNullException(nameof(permissionRepository));

        _roleRepository = roleRepository ??
            throw new ArgumentNullException(nameof(roleRepository));

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

    public async Task<AuthorizeResponse> Authorize(
        AuthorizeRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (IsTokenAdmin(request.Token))
        {
            return new AuthorizeResponse()
            {
                IsAuthorized = true
            };
        }

        Permission? permission =
            await GetPermission(request.Permission, cancellationToken);

        string? roleId = GetRoleId(request.Token);

        if (permission is not null && roleId is not null)
        {
            return new AuthorizeResponse()
            {
                IsAuthorized =
                    await _roleRepository.DoesRoleHavePermission(
                        roleId,
                        permission.Id,
                        cancellationToken
                    )
            };
        }

        return new AuthorizeResponse()
        {
            IsAuthorized = false
        };
    }

    private bool IsTokenAdmin(string token)
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
                return true;
            }
        }
        catch
        {
        }

        return false;
    }

    private string? GetRoleId(string token)
    {
        try
        {
            _tokenHandler.ValidateToken(token, _tokenValidationParameters, out SecurityToken validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken)
            {
                throw new InvalidOperationException("Cannot process authentication because the token isn't a valid JWT.");
            }

            return jwtToken.Claims.FirstOrDefault(x => x.Type == "roleId")?.Value;
        }
        catch
        {
        }

        return null;
    }

    private async Task<Permission?> GetPermission(
        string permissionName,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<Permission> permissions =
            await _permissionRepository.Search(
                Enumerable.Empty<ICriterion>(),
                cancellationToken
            );

        return permissions.FirstOrDefault(
            permission => string.Equals(permission.Name, permissionName)
        );
    }
}