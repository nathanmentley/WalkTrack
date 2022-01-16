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
using Couchbase.Lite;
using Couchbase.Lite.Query;
using WalkTrack.Framework.Common.Criteria;
using WalkTrack.Framework.Common.Resources;
using WalkTrack.Framework.Server.DAL.CouchDb.Criteria;
using WalkTrack.Framework.Server.Exceptions;

namespace WalkTrack.Framework.Server.DAL.CouchDb;

public abstract class BaseRepository<T> : IResourceRepository<T>, IDisposable
    where T: IResource
{
    private readonly Database _db;
    private readonly CriterionProcessor _criterionProcessor;

    protected BaseRepository(
        string dbName,
        IEnumerable<ICriterionHandler> criterionHandlers
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

        _db = new Database(dbName);
        _criterionProcessor = new CriterionProcessor(criterionHandlers);
    }

    public async Task<T> Create(T resource, CancellationToken cancellationToken = default)
    {
        using MutableDocument mutableDocument = new MutableDocument(resource.Id);

        await BuildDocument(mutableDocument, resource, cancellationToken);

        _db.Save(mutableDocument);

        return resource;
    }

    public Task Delete(string id, CancellationToken cancellationToken = default)
    {
        using Document document = _db.GetDocument(id);

        if (document is not null)
        {
            _db.Delete(document);
        }

        return Task.CompletedTask;
    }

    public async Task<T> Fetch(string id, CancellationToken cancellationToken = default)
    {
        using Document document = _db.GetDocument(id);

        if (document is null)
        {
            throw new ResourceNotFoundException();
        }

        string content = document.GetString("content");

        WalkTrackMediaType mediaType = WalkTrackMediaType.Parse(document.GetString("mediatype"));

        return await DecodeResource(content, mediaType, cancellationToken);
    }

    public async Task<IEnumerable<T>> Search(IEnumerable<ICriterion> criteria, CancellationToken cancellationToken = default)
    {
        IList<T> list = new List<T>();

        IFrom from =
            QueryBuilder.Select(
                SelectResult.Property("content"),
                SelectResult.Property("mediatype")
            )
            .From(DataSource.Database(_db));

        IQuery query = ProcessQuery(from, criteria);

        foreach(Result result in query.Execute().Where(result => result is not null))
        {
            string content = result.GetString("content");

            if (string.IsNullOrWhiteSpace(content))
            {
                continue;
            }

            WalkTrackMediaType mediaType = WalkTrackMediaType.Parse(result.GetString("mediatype"));

            T resource = await DecodeResource(content, mediaType, cancellationToken);

            list.Add(resource);
        }

        return list;
    }

    public async Task<T> Update(T resource, CancellationToken cancellationToken = default)
    {
        using Document document = _db.GetDocument(resource.Id);
        using MutableDocument mutableDocument =  document.ToMutable();

        await BuildDocument(mutableDocument, resource, cancellationToken);

        _db.Save(mutableDocument);

        return resource;
    }

    public void Dispose() =>
        _db.Dispose();

    protected abstract WalkTrackMediaType GetSupportedMediaType();

    protected abstract ITranscoder GetTranscoder();

    protected virtual async Task BuildDocument(
        MutableDocument mutableDocument,
        T resource,
        CancellationToken cancellationToken
    )
    {
        using MemoryStream memoryStream = new MemoryStream();

        await GetTranscoder().Encode(resource, memoryStream, cancellationToken);

        string content = Encoding.UTF8.GetString(memoryStream.ToArray());

        mutableDocument.SetString("content", content);
        mutableDocument.SetString("mediatype", $"{GetSupportedMediaType()}");
    }

    private IQuery ProcessQuery(IFrom query, IEnumerable<ICriterion> criteria)
    {
        IExpression expression = Expression.String("1").EqualTo(Expression.String("1"));

        foreach(ICriterion criterion in criteria)
        {
            expression = expression.And(_criterionProcessor.Handle(criterion));
        }

        return query.Where(expression);
    }

    private async Task<T> DecodeResource(string content, WalkTrackMediaType mediaType, CancellationToken cancellationToken)
    {
        using MemoryStream memoryStream = new MemoryStream();
        using StreamWriter writer = new StreamWriter(memoryStream);

        writer.Write(content);
        writer.Flush();
        memoryStream.Position = 0;

        object instance = await GetTranscoder().Decode(memoryStream, cancellationToken);

        if (instance is T typedInstance)
        {
            return typedInstance;
        }

        throw new Exception("TODO");
    }
}