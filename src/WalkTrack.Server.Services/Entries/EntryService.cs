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
using WalkTrack.Common.Entries;
using WalkTrack.Server.Authentications;
using WalkTrack.Server.Entries;
using WalkTrack.Server.Exceptions;

namespace WalkTrack.Server.Services.Entries;

internal sealed class EntryService: IEntryService
{
    private readonly IEntryRepository _repository;

    public EntryService(IEntryRepository repository)
    {
        if (repository is null)
        {
            throw new ArgumentNullException(nameof(repository));
        }

        _repository = repository;
    }
 
    public async Task<Entry> Fetch(
        AuthenticationContext authenticationContext,
        string id,
        CancellationToken cancellationToken = default
    )
    {
        IEnumerable<Entry> entries = await _repository.Search(
            SetupCriteriaForAuthenticationContext(
                authenticationContext,
                new ICriterion[] {
                    new IdCriterion(id)
                }
            ),
            cancellationToken
        );

        return entries.Count() switch {
            0 =>
                throw new ResourceNotFoundException(),
            1 =>
                entries.Single(),
            _ =>
                throw new ResourceNotFoundException()
        };
    }

    public Task<IEnumerable<Entry>> Search(
        AuthenticationContext authenticationContext,
        IEnumerable<ICriterion> criteria,
        CancellationToken cancellationToken = default
    ) =>
        _repository.Search(
            SetupCriteriaForAuthenticationContext(authenticationContext, criteria),
            cancellationToken
        );

    public Task<Entry> Create(
        AuthenticationContext authenticationContext,
        Entry resource,
        CancellationToken cancellationToken = default
    ) =>
        _repository.Create(
            SetupRecordForAuthenticationContext(authenticationContext, resource) with { Id = Guid.NewGuid().ToString() },
            cancellationToken
        );

    public Task<Entry> Update(
        AuthenticationContext authenticationContext,
        Entry resource,
        CancellationToken cancellationToken = default
    ) =>
        _repository.Update(
            SetupRecordForAuthenticationContext(authenticationContext, resource),
            cancellationToken
        );

    public async Task Delete(
        AuthenticationContext authenticationContext,
        string id,
        CancellationToken cancellationToken = default
    )
    {
        IEnumerable<Entry> entries = await _repository.Search(
            SetupCriteriaForAuthenticationContext(
                authenticationContext,
                new ICriterion[] {
                    new IdCriterion(id)
                }
            ),
            cancellationToken
        );

        if (entries.Any())
        {
            await _repository.Delete(id, cancellationToken);
        }
    }

    private static IEnumerable<ICriterion> SetupCriteriaForAuthenticationContext(
        AuthenticationContext authenticationContext,
        IEnumerable<ICriterion> criteria
    )
    {
        if (authenticationContext is UserAuthenticationContext userAuthenticationContext)
        {
            List<ICriterion> newCriteria = new List<ICriterion>();

            newCriteria.AddRange(criteria);

            newCriteria.Add(new UserIdCriterion(userAuthenticationContext.UserId));

            return newCriteria;
        }

        return criteria;
    }

    private static Entry SetupRecordForAuthenticationContext(
        AuthenticationContext authenticationContext,
        Entry record
    )
    {
        if (authenticationContext is UserAuthenticationContext userAuthenticationContext)
        {
            return record with { UserId = userAuthenticationContext.UserId };
        }

        return record;
    }
}