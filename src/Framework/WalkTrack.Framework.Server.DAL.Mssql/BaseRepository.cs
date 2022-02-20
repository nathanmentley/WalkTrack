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

using System.Data.SqlClient;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
using WalkTrack.Framework.Common.Criteria;
using WalkTrack.Framework.Common.Resources;
using WalkTrack.Framework.Server.DAL.Mssql.Criteria;
using WalkTrack.Framework.Server.Exceptions;

namespace WalkTrack.Framework.Server.DAL.Mssql;

public abstract class BaseRepository<TResource, TPersistedResource>: IResourceRepository<TResource>, IDisposable
    where TResource: IResource
    where TPersistedResource: BasePresistedResource
{
    private readonly string _tableName;
    private readonly QueryFactory _database;
    private readonly CriterionProcessor _criterionProcessor;

    protected BaseRepository(
        string tableName,
        string connectionString,
        IEnumerable<ICriterionHandler> handlers
    )
    {
        if (string.IsNullOrWhiteSpace(nameof(tableName)))
        {
            throw new ArgumentNullException(nameof(tableName));
        }

        if (handlers is null)
        {
            throw new ArgumentNullException(nameof(handlers));
        }

        _tableName = tableName;

        _database = new QueryFactory(new SqlConnection(connectionString), new SqlServerCompiler());

        _criterionProcessor = new CriterionProcessor(handlers);
    }

    public async Task<TResource> Create(TResource resource, CancellationToken cancellationToken = default)
    {
        await GetQuery()
            .InsertAsync(
                ConvertToRecord(resource),
                cancellationToken: cancellationToken
            );

        return await Fetch(resource.Id, cancellationToken);
    }

    public async Task Delete(string id, CancellationToken cancellationToken = default) =>
        await GetQuery()
            .Where("id", id)
            .DeleteAsync(cancellationToken: cancellationToken);

    public async Task<TResource> Fetch(string id, CancellationToken cancellationToken = default)
    {
        TPersistedResource? record =
            await GetQuery()
                .Where("id", id)
                .FirstOrDefaultAsync<TPersistedResource>(cancellationToken: cancellationToken);

        if (record is null)
        {
            throw new ResourceNotFoundException();
        }

        return FromRecord(record);
    }

    public async Task<IEnumerable<TResource>> Search(
        IEnumerable<ICriterion> criteria,
        CancellationToken cancellationToken = default
    )
    {
        IList<TResource> resources = new List<TResource>();

        Query query = GetQuery();

        foreach(ICriterion criterion in criteria)
        {
            query = _criterionProcessor.Handle(criterion, query);
        }

        IEnumerable<TPersistedResource> records = await query.GetAsync<TPersistedResource>();

        foreach(TPersistedResource record in records)
        {
            resources.Add(FromRecord(record));
        }

        return resources;
    }

    public async Task<TResource> Update(TResource resource, CancellationToken cancellationToken = default)
    {
        await GetQuery()
            .Where("id", resource.Id)
            .UpdateAsync(ConvertToRecord(resource), cancellationToken: cancellationToken);

        return await Fetch(resource.Id, cancellationToken);
    }

    protected abstract TPersistedResource ConvertToRecord(TResource resource);

    protected abstract TResource FromRecord(TPersistedResource record);

    protected Query GetQuery() =>
        _database.Query(_tableName);

    protected QueryFactory GetQueryFactory() =>
        _database;

    public void Dispose() =>
        _database.Dispose();
}