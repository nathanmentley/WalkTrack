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
using WalkTrack.Common.Entries;
using WalkTrack.Common.Json;
using WalkTrack.Common.Resources;

namespace WalkTrack.Server.DAL.Entries;

/// <summary>
/// </summary>
internal sealed class EntryJsonV1Transcoder: BaseJsonTranscoder<Entry>, ITranscoder
{
    private static readonly WalkTrackMediaType _supportedMediaType =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.SecureEntry")
            .WithVersion(1)
            .Build();

    protected override WalkTrackMediaType GetSupportedMediaType() =>
        _supportedMediaType;

    public override JsonObject Encode(Entry resource) =>
        new JsonObjectBuilder()
            .With("id", resource.Id)
            .With("userId", resource.UserId)
            .With("date", resource.Date.ToDateTime(TimeOnly.MinValue))
            .With("distance", resource.Distance)
            .Build();

    public override Entry Decode(JsonObject jsonObject) =>
        new Entry()
        {
            Id = GetValue(jsonObject, "id", string.Empty),
            UserId = GetValue(jsonObject, "userId", string.Empty),
            Date = DateOnly.FromDateTime(GetValue(jsonObject, "date", DateTime.MinValue)),
            Distance = GetValue(jsonObject, "distance", decimal.Zero),
        };
}