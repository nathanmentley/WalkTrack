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
internal sealed class CreateUserRequestJsonV1Transcoder : BaseJsonTranscoder<CreateUserRequest>, IWireTranscoder
{
    private static readonly WalkTrackMediaType _supportedMediaType =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.CreateUserRequest")
            .WithVersion(1)
            .Build();

    protected override WalkTrackMediaType GetSupportedMediaType() =>
        _supportedMediaType;

    public override JsonObject Encode(CreateUserRequest resource) =>
        new JsonObjectBuilder()
            .With("email", resource.Email)
            .With("username", resource.Username)
            .With("password", resource.Password)
            .With("isPublic", resource.IsPublic)
            .Build();

    public override CreateUserRequest Decode(JsonObject jsonObject) =>
        new CreateUserRequest()
        {
            Email = GetValue(jsonObject, "email", string.Empty),
            Username = GetValue(jsonObject, "username", string.Empty),
            Password = GetValue(jsonObject, "password", string.Empty),
            IsPublic = GetValue(jsonObject, "isPublic", false)
        };
}