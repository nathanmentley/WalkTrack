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
using WalkTrack.Common.Authentications;
using WalkTrack.Common.Criteria;
using WalkTrack.Common.Users;
using WalkTrack.Server.Authentications;
using WalkTrack.Server.Exceptions;
using WalkTrack.Server.Users;

namespace WalkTrack.Server.Services.Authentications;

/// <summary>
/// </summary>
internal sealed class AuthenticationService: IAuthenticationService
{
    private readonly IUserRepository _repository;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly SigningCredentials _signingCredentials;
    private readonly AppSettings _appSettings;
    private readonly IHashingUtility _hashingUtility;

    public AuthenticationService(
        IOptions<AppSettings> appSettings,
        IUserRepository repository,
        IHashingUtility hashingUtility
    )
    {
        _repository = repository ??
            throw new ArgumentNullException(nameof(repository));

        _hashingUtility = hashingUtility ??
            throw new ArgumentNullException(nameof(hashingUtility));

        _appSettings = appSettings.Value;

        _tokenHandler = new JwtSecurityTokenHandler();

        _signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret)),
            SecurityAlgorithms.HmacSha256Signature
        );
    }

    /// <summary>
    /// </summary>
    public async Task<AuthenticateResponse> Authenticate(
        AuthenticateRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (
            string.Equals(_appSettings.AdminUsername, request.Username, StringComparison.Ordinal) &&
            string.Equals(_appSettings.AdminUsername, request.Password, StringComparison.Ordinal)
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

    private AuthenticateResponse BuildResponse(User user, string password)
    {
        string hash = _hashingUtility.Hash(password, user.Salt);

        if (string.Equals(user.Password, hash, StringComparison.Ordinal))
        {
            return new AuthenticateResponse()
            {
                Id = user.Id,
                Username = user.Username,
                Token = GenerateJwtToken(user)
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
                            new Claim("id", "id"),
                            new Claim("admin", "admin")
                        }
                    )
                }
            )
        );

    private string GenerateJwtToken(User user) =>
        _tokenHandler.WriteToken(
            _tokenHandler.CreateToken(
                new SecurityTokenDescriptor()
                {
                    SigningCredentials = _signingCredentials,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Subject = new ClaimsIdentity(
                        new[] {
                            new Claim("id", user.Id)
                        }
                    )
                }
            )
        );
}