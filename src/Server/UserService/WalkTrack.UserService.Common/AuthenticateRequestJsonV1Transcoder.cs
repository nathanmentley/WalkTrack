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

namespace WalkTrack.UserService.Common;

/// <summary>
/// </summary>
internal sealed class AuthenticateRequestJsonV1Transcoder: BaseJsonTranscoder<AuthenticateRequest>, ITranscoder
{
    private static readonly WalkTrackMediaType _supportedMediaType =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.AuthenticateRequest")
            .WithVersion(1)
            .Build();

    protected override WalkTrackMediaType GetSupportedMediaType() =>
        _supportedMediaType;

    public override JsonObject Encode(AuthenticateRequest resource) =>
        new JsonObjectBuilder()
            .With("username", resource.Username)
            .With("password", resource.Password)
            .Build();

    public override AuthenticateRequest Decode(JsonObject jsonObject) =>
        new AuthenticateRequest()
        {
            Username = GetValue(jsonObject, "username", string.Empty),
            Password = GetValue(jsonObject, "password", string.Empty)
        };
}