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

using WalkTrack.Common.Criteria;
using WalkTrack.Common.Users;
using WalkTrack.Server.Authentications;
using WalkTrack.Server.Users;

namespace WalkTrack.Server.Services.Users;

internal sealed class UserService: IUserService
{
    private readonly IUserRepository _repository;
    private readonly IHashingUtility _hashingUtility;

    public UserService(IUserRepository repository, IHashingUtility hashingUtility)
    {
        _repository = repository ??
            throw new ArgumentNullException(nameof(repository));

        _hashingUtility = hashingUtility ??
            throw new ArgumentNullException(nameof(hashingUtility));
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

    public Task<User> Create(
        User resource,
        CancellationToken cancellationToken = default
    )
    {
        string salt = Guid.NewGuid().ToString();

        return _repository.Create(
            resource with {
                Id = Guid.NewGuid().ToString(),
                Salt = salt,
                Password = _hashingUtility.Hash(resource.Password, salt)
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

    public async Task<User> UpdatePassword(
        AuthenticationContext authenticationContext,
        string userId,
        string password,
        CancellationToken cancellationToken = default
    )
    {
        User user = await FetchRecordWithAuth(authenticationContext, userId, cancellationToken);

        user = user with {
            Password = _hashingUtility.Hash(password, user.Salt)
        };

        return await _repository.Update(user, cancellationToken);
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