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

using WalkTrack.AuthService.Client;
using WalkTrack.AuthService.Common;
using WalkTrack.Framework.Common.Criteria;
using WalkTrack.Framework.Server.Authentications;
using WalkTrack.Framework.Server.Exceptions;
using WalkTrack.UserService.Common;
using WalkTrack.UserService.Common.Criteria;

namespace WalkTrack.UserService.Server.Services;

internal sealed class UserService: IUserService
{
    private readonly IAuthenticationClient _authenticationClient;
    private readonly IUserRepository _repository;

    public UserService(
        IAuthenticationClient authenticationClient,
        IRoleClient roleClient,
        IUserRepository repository
    )
    {
        _authenticationClient = authenticationClient ??
            throw new ArgumentNullException(nameof(authenticationClient));

        _repository = repository ??
            throw new ArgumentNullException(nameof(repository));
    }
 
    public Task<User> Fetch(
        AuthenticationContext authenticationContext,
        string id,
        CancellationToken cancellationToken = default
    ) =>
        FetchRecordWithAuth(authenticationContext, id, cancellationToken);

    public Task<IEnumerable<User>> Search(
        AuthenticationContext authenticationContext,
        IEnumerable<ICriterion> criteria,
        CancellationToken cancellationToken = default
    )
    {
        if (authenticationContext is not SystemAuthenticationContext)
        {
            throw new NotSupportedException();
        }

        return _repository.Search(criteria, cancellationToken);
    }

    public async Task<User> Create(
        CreateUserRequest resource,
        CancellationToken cancellationToken = default
    )
    {
        if(
            (
                await _repository.Search(
                    new ICriterion[] {
                        new EmailCriterion(resource.Email)
                    },
                    cancellationToken
                )
            ).Any()
        )
        {
            throw new InvalidRequestException("Email already exists");
        }

        if(
            (
                await _repository.Search(
                    new ICriterion[] {
                        new UsernameCriterion(resource.Username)
                    },
                    cancellationToken
                )
            ).Any()
        )
        {
            throw new InvalidRequestException("Email already exists");
        }

        await _authenticationClient.Create(
            new CreateAuthRequest()
            {
                Username = resource.Username,
                Password = resource.Password,
                RoleName = "user"
            },
            cancellationToken
        );

        return await _repository.Create(
            new User()
            {
                Id = Guid.NewGuid().ToString(),
                Username = resource.Username,
                Email = resource.Email,
                IsPublic = resource.IsPublic,
            },
            cancellationToken
        );
    }

    public async Task<User> Update(
        AuthenticationContext authenticationContext,
        User user,
        CancellationToken cancellationToken = default
    )
    {
        User currentUser = await FetchRecordWithAuth(authenticationContext, user.Id, cancellationToken);

        User userWithUpdates = currentUser with {
            Username = user.Username,
            Email = user.Email,
            IsPublic = user.IsPublic
        };

        return await _repository.Update(userWithUpdates, cancellationToken);
    }

    public async Task Delete(
        AuthenticationContext authenticationContext,
        string id,
        CancellationToken cancellationToken = default
    )
    {
        User user = await FetchRecordWithAuth(authenticationContext, id, cancellationToken);

        await _repository.Delete(user.Id, cancellationToken);
    }

    public async Task<User> FetchRecordWithAuth(
        AuthenticationContext authenticationContext,
        string id,
        CancellationToken cancellationToken = default
    ) =>
        authenticationContext switch {
            SystemAuthenticationContext _ =>
                await _repository.Fetch(id, cancellationToken),
            UserAuthenticationContext userAuthenticationContext =>
                await _repository.Fetch(userAuthenticationContext.UserId, cancellationToken),
            _ =>
                throw new NotSupportedException()
        };
}