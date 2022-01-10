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
internal sealed class SecureUserJsonV1Transcoder : BaseJsonTranscoder<User>, IPersistTranscoder
{
    private static readonly WalkTrackMediaType _supportedMediaType =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.SecureUser")
            .WithVersion(1)
            .Build();

    protected override WalkTrackMediaType GetSupportedMediaType() =>
        _supportedMediaType;

    public override JsonObject Encode(User resource) =>
        new JsonObjectBuilder()
            .With("id", resource.Id)
            .With("email", resource.Email)
            .With("username", resource.Username)
            .With("password", resource.Password)
            .With("salt", resource.Salt)
            .With("isPublic", resource.IsPublic)
            .Build();

    public override User Decode(JsonObject jsonObject) =>
        new User()
        {
            Id = GetValue(jsonObject, "id", string.Empty),
            Email = GetValue(jsonObject, "email", string.Empty),
            Username = GetValue(jsonObject, "username", string.Empty),
            Password = GetValue(jsonObject, "password", string.Empty),
            Salt = GetValue(jsonObject, "salt", string.Empty),
            IsPublic = GetValue(jsonObject, "isPublic", false)
        };
}