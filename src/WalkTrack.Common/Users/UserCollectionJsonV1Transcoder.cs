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

namespace WalkTrack.Common.Users;

/// <summary>
/// </summary>
internal sealed class UserCollectionJsonV1Transcoder: BaseJsonTranscoder<IEnumerable<User>>, IWireTranscoder
{
    private readonly UserJsonV1Transcoder _userTranscoder;
    
    public UserCollectionJsonV1Transcoder()
    {
        _userTranscoder = new UserJsonV1Transcoder();
    }

    private static readonly WalkTrackMediaType _supportedMediaType =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Users")
            .WithVersion(1)
            .Build();

    protected override WalkTrackMediaType GetSupportedMediaType() =>
        _supportedMediaType;

    public override JsonObject Encode(IEnumerable<User> resource) =>
        new JsonObjectBuilder()
            .With("count", resource.Count())
            .With("data", BuildArray(resource))
            .Build();

    private JsonArray BuildArray(IEnumerable<User> resource) =>
        new JsonArrayBuilder()
            .With(resource, user => _userTranscoder.Encode(user))
            .Build();

    public override IEnumerable<User> Decode(JsonObject jsonObject)
    {
        JsonArray array = GetValue<JsonArray>(jsonObject, "data", new JsonArray());

        foreach(JsonObject? user in array)
        {
            if (user is not null)
            {
                yield return _userTranscoder.Decode(user);
            }
        }
    }
}