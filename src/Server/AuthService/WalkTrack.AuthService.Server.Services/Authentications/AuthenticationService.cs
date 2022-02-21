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
using WalkTrack.AuthService.Common;
using WalkTrack.AuthService.Common.Criteria;
using WalkTrack.AuthService.Server.Configuration;
using WalkTrack.AuthService.Server.Criteria;

namespace WalkTrack.AuthService.Server.Services.Authentications;

/// <summary>
/// </summary>
internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly string _adminUsername;
    private readonly string _adminPassword;
    private readonly IAuthenticationRepository _repository;
    private readonly IRoleRepository _roleRepository;
    private readonly IHashingUtility _hashingUtility;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly SigningCredentials _signingCredentials;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public AuthenticationService(
        IOptions<AdminSettings> adminSettings,
        IOptions<AuthenticationSettings> authenticationSettings,
        IAuthenticationRepository repository,
        IRoleRepository roleRepository,
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

        if (roleRepository is null)
        {
            throw new ArgumentNullException(nameof(roleRepository));
        }

        if (hashingUtility is null)
        {
            throw new ArgumentNullException(nameof(hashingUtility));
        }

        _adminUsername = adminSettings.Value.AdminUsername;

        _adminPassword = adminSettings.Value.AdminPassword;

        _repository = repository;

        _roleRepository = roleRepository;

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

    public async Task<Authentication> Create(
        CreateAuthRequest resource,
        CancellationToken cancellationToken = default
    )
    {
        if(
            (
                await _repository.Search(
                    new ICriterion[] {
                        new UsernameCriterion(resource.Username)
                    },
                    cancellationToken
                )
            )
                .Any()
        )
        {
            throw new InvalidRequestException("Authentication already exists");
        }

        string? roleId = await GetRoleId(resource.RoleName, cancellationToken);

        if (roleId is null)
        {
            throw new InvalidRequestException($"Role {resource.RoleName} does not exist.");
        }

        string salt = Guid.NewGuid().ToString();

        return await _repository.Create(
            new Authentication() {
                Id = Guid.NewGuid().ToString(),
                Salt = salt,
                Username = resource.Username,
                RoleId = roleId,
                Password = _hashingUtility.Hash(resource.Password, salt)
            },
            cancellationToken
        );
    }

    private async Task<string?> GetRoleId(string roleName, CancellationToken cancellationToken)
    {
        IEnumerable<Role> roles =
            await _roleRepository.Search(Enumerable.Empty<ICriterion>(), cancellationToken);

        Role? role = roles.FirstOrDefault(
            role => string.Equals(role.Name, roleName)
        );

        return role is not null ?
            role.Id:
            null;
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

        IEnumerable<Authentication> auths = await _repository.Search(
            new ICriterion[] {
                new UsernameCriterion(request.Username)
            },
            cancellationToken
        );

        return auths.Count() switch {
            0 =>
                throw new ResourceNotFoundException(),
            1 =>
                BuildResponse(auths.Single(), request.Password),
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

        Claim? roleClaim =
            indentity.Claims.FirstOrDefault(claim =>
                string.Equals(claim.Type, "roleId", StringComparison.OrdinalIgnoreCase)
            );

        if (idClaim is null || roleClaim is null)
        {
            throw new InvalidOperationException("Cannot process authentication because the token doesn't contain required claims.");
        }

        return Task.FromResult(
            new Token() { Id = GenerateJwtToken(idClaim.Value, roleClaim.Value) }
        );
    }

    /// <summary>
    /// </summary>
    public async Task RequestForgottenPassword(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken = default
    )
    {
        IEnumerable<Authentication> users = await _repository.Search(
            new []
            {
                new UsernameCriterion(request.Username)
            },
            cancellationToken
        );

        Authentication? auth = users.FirstOrDefault();

        if (auth is null)
        {
            return;
        }

        auth = auth with {
            ResetToken = Guid.NewGuid().ToString(),
            ResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        auth = await _repository.Update(auth, cancellationToken);

        //await SendForgotPasswordEmail(auth, request, cancellationToken);
    }
/*
        private async Task SendForgotPasswordEmail(
            Auth auth,
            ForgotPasswordRequest request,
            CancellationToken cancellationToken
        ) =>
            await _emailClient.Send(
                new [] {
                    new Email()
                    {
                        To = auth.Username,
                        ToAddress = request.Email,
                        From = "WalkTrack",
                        FromAddress = "noreply@Walktrack.PokeTriRx.com",
                        Subject = "Password Reset Request",
                        HtmlMessage = $"Token: {auth.ResetToken}",
                        TextMessage = $"Token: {auth.ResetToken}"
                    }
                },
                cancellationToken
            );
*/
    /// <summary>
    /// </summary>
    public async Task<AuthenticateResponse> ResetPassword(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default
    )
    {
        IEnumerable<Authentication> auths = await _repository.Search(
            new ICriterion[]
            {
                new UsernameCriterion(request.Username),
                new ResetTokenCriterion(request.Token)
            },
            cancellationToken
        );

        Authentication? auth = auths.FirstOrDefault();

        if (auth is null)
        {
            throw new Exception("TODO");
        }

        string salt = Guid.NewGuid().ToString();

        auth = auth with { Password = _hashingUtility.Hash(request.Password, salt), Salt = salt };

        await _repository.Update(auth, cancellationToken);

        return BuildResponse(auth, request.Password);
    }

    private AuthenticateResponse BuildResponse(Authentication auth, string password)
    {
        string hash = _hashingUtility.Hash(password, auth.Salt);

        if (string.Equals(auth.Password, hash, StringComparison.Ordinal))
        {
            return new AuthenticateResponse()
            {
                Id = auth.Id,
                Username = auth.Username,
                Token = GenerateJwtToken(auth.Id, auth.RoleId)
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

    private string GenerateJwtToken(string authId, string roleId) =>
        _tokenHandler.WriteToken(
            _tokenHandler.CreateToken(
                new SecurityTokenDescriptor()
                {
                    SigningCredentials = _signingCredentials,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Subject = new ClaimsIdentity(
                        new[] {
                            new Claim("id", authId),
                            new Claim("roleId", roleId)
                        }
                    )
                }
            )
        );
}