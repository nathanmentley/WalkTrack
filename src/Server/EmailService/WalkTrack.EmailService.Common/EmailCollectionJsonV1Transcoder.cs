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
internal sealed class EmailCollectionJsonV1Transcoder: BaseJsonTranscoder<IEnumerable<Email>>, ITranscoder
{
    private readonly EmailJsonV1Transcoder _emailTranscoder;
    
    public EmailCollectionJsonV1Transcoder()
    {
        _emailTranscoder = new EmailJsonV1Transcoder();
    }

    private static readonly WalkTrackMediaType _supportedMediaType =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Emails")
            .WithVersion(1)
            .Build();

    protected override WalkTrackMediaType GetSupportedMediaType() =>
        _supportedMediaType;

    public override JsonObject Encode(IEnumerable<Email> resource) =>
        new JsonObjectBuilder()
            .With("count", resource.Count())
            .With("data", BuildArray(resource))
            .Build();

    private JsonArray BuildArray(IEnumerable<Email> resource) =>
        new JsonArrayBuilder()
            .With(resource, email => _emailTranscoder.Encode(email))
            .Build();

    public override IEnumerable<Email> Decode(JsonObject jsonObject)
    {
        foreach(JsonObject? email in jsonObject["data"]?.AsArray() ?? new JsonArray())
        {
            if (email is not null)
            {
                yield return _emailTranscoder.Decode(email);
            }
        }
    }
}