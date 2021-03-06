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
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WalkTrack.Framework.Common.Criteria;
using WalkTrack.Framework.Server;
using WalkTrack.Framework.Server.Exceptions;
using WalkTrack.UserService.Common;
using WalkTrack.UserService.Common.Criteria;
using WalkTrack.UserService.Server.Configuration;

namespace WalkTrack.UserService.Server.Services;

/// <summary>
/// </summary>
internal sealed class AuthenticationService: IAuthenticationService
{
    private readonly string _adminUsername;
    private readonly string _adminPassword;
    private readonly IUserRepository _repository;
    private readonly IHashingUtility _hashingUtility;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly SigningCredentials _signingCredentials;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public AuthenticationService(
        IOptions<AdminSettings> adminSettings,
        IOptions<AuthenticationSettings> authenticationSettings,
        IUserRepository repository,
        IHashingUtility hashingUtility
    )
    {
        if (adminSettings is null)
        {
            throw new ArgumentNullException(nameof(adminSettings));
        }

        if (authenticationSettings is null)
        {
            throw new ArgumentNullException(nameof(authenticationSettings));
        }

        if (repository is null)
        {
            throw new ArgumentNullException(nameof(repository));
        }

        if (hashingUtility is null)
        {
            throw new ArgumentNullException(nameof(hashingUtility));
        }

        _adminUsername = adminSettings.Value.AdminUsername;

        _adminPassword = adminSettings.Value.AdminPassword;

        _repository = repository;

        _hashingUtility = hashingUtility;

        _tokenHandler = new JwtSecurityTokenHandler();

        _signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authenticationSettings.Value.JwtSecret)),
            SecurityAlgorithms.HmacSha256Signature
        );

        _tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authenticationSettings.Value.JwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    }

    /// <summary>
    /// </summary>
    public async Task<AuthenticateResponse> Authenticate(
        AuthenticateRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (
            string.Equals(_adminUsername, request.Username, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(_adminPassword, request.Password, StringComparison.Ordinal)
        )
        {
            return new AuthenticateResponse()
            {
                Token = GenerateAdminJwtToken()
            };
        }

        IEnumerable<User> users = await _repository.Search(
            new ICriterion[] {
                new UsernameCriterion(request.Username)
            },
            cancellationToken
        );

        return users.Count() switch {
            0 =>
                throw new ResourceNotFoundException(),
            1 =>
                BuildResponse(users.Single(), request.Password),
            _ =>
                throw new ResourceNotFoundException()
        };
    }

    public Task<Token> RefreshToken(
        Token token,
        CancellationToken cancellationToken = default
    )
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        cancellationToken.ThrowIfCancellationRequested();

        ClaimsPrincipal indentity =
            _tokenHandler.ValidateToken(token.Id, _tokenValidationParameters, out SecurityToken validatedToken);

        if (validatedToken is not JwtSecurityToken jwtToken)
        {
            throw new InvalidOperationException("Cannot process authentication because the token isn't a valid JWT.");
        }

        Claim? idClaim =
            indentity.Claims.FirstOrDefault(claim =>
                string.Equals(claim.Type, "id", StringComparison.OrdinalIgnoreCase)
            );

        if (idClaim is null)
        {
            throw new InvalidOperationException("Cannot process authentication because the token doesn't contain required claims.");
        }

        return Task.FromResult(
            new Token() { Id = GenerateJwtToken(idClaim.Value) }
        );
    }

    private AuthenticateResponse BuildResponse(User user, string password)
    {
        string hash = _hashingUtility.Hash(password, user.Salt);

        if (string.Equals(user.Password, hash, StringComparison.Ordinal))
        {
            return new AuthenticateResponse()
            {
                Id = user.Id,
                Username = user.Username,
                Token = GenerateJwtToken(user.Id)
            };
        }
        
        throw new ResourceNotFoundException();
    }

    private string GenerateAdminJwtToken() =>
        _tokenHandler.WriteToken(
            _tokenHandler.CreateToken(
                new SecurityTokenDescriptor()
                {
                    SigningCredentials = _signingCredentials,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Subject = new ClaimsIdentity(
                        new[] {
                            new Claim("id", "n/a"),
                            new Claim("admin", "admin")
                        }
                    )
                }
            )
        );

    private string GenerateJwtToken(string userId) =>
        _tokenHandler.WriteToken(
            _tokenHandler.CreateToken(
                new SecurityTokenDescriptor()
                {
                    SigningCredentials = _signingCredentials,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Subject = new ClaimsIdentity(
                        new[] {
                            new Claim("id", userId)
                        }
                    )
                }
            )
        );
}