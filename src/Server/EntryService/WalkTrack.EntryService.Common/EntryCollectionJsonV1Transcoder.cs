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

using System.Text.Json.Nodes;
using WalkTrack.Framework.Common.Json;
using WalkTrack.Framework.Common.Resources;

namespace WalkTrack.EntryService.Common;

/// <summary>
/// </summary>
internal sealed class EntryCollectionJsonV1Transcoder: BaseJsonTranscoder<IEnumerable<Entry>>, ITranscoder
{
    private readonly EntryJsonV1Transcoder _entryTranscoder;
    
    public EntryCollectionJsonV1Transcoder()
    {
        _entryTranscoder = new EntryJsonV1Transcoder();
    }

    private static readonly WalkTrackMediaType _supportedMediaType =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Entries")
            .WithVersion(1)
            .Build();

    protected override WalkTrackMediaType GetSupportedMediaType() =>
        _supportedMediaType;

    public override JsonObject Encode(IEnumerable<Entry> resource) =>
        new JsonObjectBuilder()
            .With("count", resource.Count())
            .With("data", BuildArray(resource))
            .Build();

    private JsonArray BuildArray(IEnumerable<Entry> resource) =>
        new JsonArrayBuilder()
            .With(resource, entry => _entryTranscoder.Encode(entry))
            .Build();

    public override IEnumerable<Entry> Decode(JsonObject jsonObject)
    {
        foreach(JsonObject? entry in jsonObject["data"]?.AsArray() ?? new JsonArray())
        {
            if (entry is not null)
            {
                yield return _entryTranscoder.Decode(entry);
            }
        }
    }
}