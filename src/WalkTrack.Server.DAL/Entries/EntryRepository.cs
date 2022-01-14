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

using Couchbase.Lite;
using WalkTrack.Common.Entries;
using WalkTrack.Common.Resources;
using WalkTrack.Server.DAL.Criteria.Core;
using WalkTrack.Server.DAL.Resources;
using WalkTrack.Server.Entries;

namespace WalkTrack.Server.DAL.Entries;

internal sealed class EntryRepository: BaseRepository<Entry>, IEntryRepository
{
    private static readonly IEnumerable<ICriterionHandler> CriterionHandlers =
        new ICriterionHandler[] {
            new IdCriterionHandler(),
            new UserIdCriterionHandler()
        };

    private static readonly WalkTrackMediaType SupportedMediaType =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.SecureEntry")
            .WithVersion(1)
            .Build();

    private static readonly ITranscoder Transcoder =
        new EntryJsonV1Transcoder();

    public EntryRepository():
        base("entrydb", CriterionHandlers) {}

    protected override WalkTrackMediaType GetSupportedMediaType() =>
        SupportedMediaType;

    protected override ITranscoder GetTranscoder() =>
        Transcoder;

    protected override async Task BuildDocument(
        MutableDocument mutableDocument,
        Entry resource,
        CancellationToken cancellationToken
    )
    {
        await base.BuildDocument(mutableDocument, resource, cancellationToken);

        mutableDocument.SetString("userId", resource.UserId);
    }
}