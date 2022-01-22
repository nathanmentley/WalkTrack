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

namespace WalkTrack.EmailService.Common;

/// <summary>
/// </summary>
internal sealed class EmailJsonV1Transcoder: BaseJsonTranscoder<Email>, ITranscoder
{
    private static readonly WalkTrackMediaType _supportedMediaType =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Email")
            .WithVersion(1)
            .Build();

    protected override WalkTrackMediaType GetSupportedMediaType() =>
        _supportedMediaType;

    public override JsonObject Encode(Email resource) =>
        new JsonObjectBuilder()
            .With("from", resource.From)
            .With("fromAddress", resource.FromAddress)
            .With("to", resource.To)
            .With("toAddress", resource.ToAddress)
            .With("subject", resource.Subject)
            .With("htmlMessage", resource.HtmlMessage)
            .With("textMessage", resource.TextMessage)
            .Build();

    public override Email Decode(JsonObject jsonObject) =>
        new Email()
        {
            From = GetValue(jsonObject, "from", string.Empty),
            FromAddress = GetValue(jsonObject, "fromAddress", string.Empty),
            To = GetValue(jsonObject, "to", string.Empty),
            ToAddress = GetValue(jsonObject, "toAddress", string.Empty),
            Subject = GetValue(jsonObject, "subject", string.Empty),
            HtmlMessage = GetValue(jsonObject, "htmlMessage", string.Empty),
            TextMessage = GetValue(jsonObject, "textMessage", string.Empty)
        };
}