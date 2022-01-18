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

using System.Text;
using CouchDB.Driver;
using CouchDB.Driver.Extensions;
using WalkTrack.Framework.Common.Criteria;
using WalkTrack.Framework.Common.Resources;
using WalkTrack.Framework.Server.DAL.CouchDb.Criteria;
using WalkTrack.Framework.Server.Exceptions;

namespace WalkTrack.Framework.Server.DAL.CouchDb;

public abstract class BaseRepository<TResource, TPersisted> : IResourceRepository<TResource>, IAsyncDisposable
    where TResource: IResource
    where TPersisted: BasePersistedDocument<TResource>, new()
{
    private readonly CriterionProcessor<TPersisted> _criterionProcessor;
    private readonly CouchClient _client;
    private readonly string _dbName;

    protected BaseRepository(
        string dbName,
        string connectionString,
        string username,
        string password,
        IEnumerable<ICriterionHandler<TPersisted>> criterionHandlers
    )
    {
        if (string.IsNullOrWhiteSpace(dbName))
        {
            throw new ArgumentNullException(nameof(dbName));
        }

        if (criterionHandlers is null)
        {
            throw new ArgumentNullException(nameof(criterionHandlers));
        }

        _dbName = dbName;

        _criterionProcessor = new CriterionProcessor<TPersisted>(criterionHandlers);

        _client = new CouchClient(
            connectionString,
            builder => builder.UseBasicAuthentication(username, password)
        );
    }

    public async Task<TResource> Create(TResource resource, CancellationToken cancellationToken = default) =>
        await Update(resource, cancellationToken);

    public async Task Delete(string id, CancellationToken cancellationToken = default)
    {
        ICouchDatabase<TPersisted> db = await GetDb(cancellationToken);

        TPersisted? record = db.FirstOrDefault(x => x.Id == id);

        if (record is null)
        {
            throw new ResourceNotFoundException();
        }

        await db.RemoveAsync(record, false, cancellationToken);
    }

    public async Task<TResource> Fetch(string id, CancellationToken cancellationToken = default)
    {
        ICouchDatabase<TPersisted> db = await GetDb(cancellationToken);

        TPersisted? record = db.FirstOrDefault(x => x.Id == id);

        if (record is null)
        {
            throw new ResourceNotFoundException();
        }

        return await DecodeResource(record.Content, GetSupportedMediaType(), cancellationToken);
    }

    public async Task<IEnumerable<TResource>> Search(
        IEnumerable<ICriterion> criteria,
        CancellationToken cancellationToken = default
    )
    {
        IList<TResource> resources = new List<TResource>();

        ICouchDatabase<TPersisted> db = await GetDb(cancellationToken);

        IQueryable<TPersisted> records = db;

        foreach(ICriterion criterion in criteria)
        {
            records = records.Where(_criterionProcessor.Handle(criterion));
        }

        foreach(TPersisted record in await records.ToListAsync())
        {
            resources.Add(await DecodeResource(record.Content, GetSupportedMediaType(), cancellationToken));
        }

        return resources;
    }

    public async Task<TResource> Update(TResource resource, CancellationToken cancellationToken = default)
    {
        ICouchDatabase<TPersisted> db = await GetDb(cancellationToken);

        TPersisted persisted = new TPersisted()
        {
            Id = resource.Id,
            Content = await EncodeResource(resource, cancellationToken),
            MediaType = GetSupportedMediaType().ToString()
        };
        persisted.From(resource);

        persisted = await db.AddOrUpdateAsync(persisted, false, cancellationToken);

        return await DecodeResource(persisted.Content, GetSupportedMediaType(), cancellationToken);
    }

    private async Task<ICouchDatabase<TPersisted>> GetDb(CancellationToken cancellationToken) =>
        await _client.GetOrCreateDatabaseAsync<TPersisted>(cancellationToken: cancellationToken);

    protected abstract WalkTrackMediaType GetSupportedMediaType();

    protected abstract ITranscoder GetTranscoder();

    private async Task<string> EncodeResource(TResource resource, CancellationToken cancellationToken = default)
    {
        using MemoryStream memoryStream = new MemoryStream();

        await GetTranscoder().Encode(resource, memoryStream, cancellationToken);

        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    private async Task<TResource> DecodeResource(string content, WalkTrackMediaType mediaType, CancellationToken cancellationToken)
    {
        using MemoryStream memoryStream = new MemoryStream();
        using StreamWriter writer = new StreamWriter(memoryStream);

        writer.Write(content);
        writer.Flush();
        memoryStream.Position = 0;

        object instance = await GetTranscoder().Decode(memoryStream, cancellationToken);

        if (instance is TResource typedInstance)
        {
            return typedInstance;
        }

        throw new Exception("TODO");
    }

    public ValueTask DisposeAsync() =>
        _client.DisposeAsync();
}