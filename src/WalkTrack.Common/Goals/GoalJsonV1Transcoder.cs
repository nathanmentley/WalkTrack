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
using WalkTrack.Common.Json;
using WalkTrack.Common.Resources;

namespace WalkTrack.Common.Goals;

/// <summary>
/// </summary>
internal sealed class GoalJsonV1Transcoder: BaseJsonTranscoder<Goal>, IWireTranscoder, IPersistTranscoder
{
    private static readonly WalkTrackMediaType _supportedMediaType =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Goal")
            .WithVersion(1)
            .Build();

    protected override WalkTrackMediaType GetSupportedMediaType() =>
        _supportedMediaType;

    public override JsonObject Encode(Goal resource) =>
        new JsonObjectBuilder()
            .With("id", resource.Id)
            .With("userId", resource.UserId)
            .With("name", resource.Name)
            .With(
                "milestones",
                new JsonArrayBuilder()
                    .With(
                        resource.Milestones,
                        milestone =>
                            new JsonObjectBuilder()
                                .With("name", milestone.Name)
                                .With("description", milestone.Description)
                                .With("distance", milestone.Distance)
                                .Build()
                    )
                    .Build()
            )
            .Build();

    public override Goal Decode(JsonObject jsonObject) =>
        new Goal()
        {
            Id = GetValue(jsonObject, "id", string.Empty),
            UserId = GetValue(jsonObject, "userId", string.Empty),
            Name = GetValue(jsonObject, "name", string.Empty),
            Milestones = GetMilestones(jsonObject["milestones"] as JsonArray)
        };

    private static IEnumerable<Milestone> GetMilestones(JsonArray? array)
    {
        if (array is null)
        {
            return Enumerable.Empty<Milestone>();
        }

        IList<Milestone> milestones = new List<Milestone>();

        foreach(JsonNode? node in array ?? Enumerable.Empty<JsonNode?>())
        {
            if (node is JsonObject jsonObject)
            {
                milestones.Add(
                    new Milestone()
                    {
                        Name = GetValue(jsonObject, "name", string.Empty),
                        Description = GetValue(jsonObject, "description", string.Empty),
                        Distance = GetValue(jsonObject, "distance", decimal.Zero)
                    }
                );
            }
        }

        return milestones;
    }
}