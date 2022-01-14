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

using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;
using WalkTrack.Common.Resources;
using WalkTrack.Server.Hosting.Exceptions;

namespace WalkTrack.Server.Hosting.Formatters;

/// <summary>
/// Validates a signed <see cref="VerifiedData{TData}"/> and writes it to the http response <see cref="Stream"/>.
/// </summary>
/// <typeparam name="TData">
/// Verified data model
/// </typeparam>
public class WalkTrackOutputFormatter: TextOutputFormatter
{
    private readonly static ITranscoder _nullTranscoder =
        new NullTranscoder();

    private readonly IEnumerable<ITranscoder> _codecs;

    private readonly ILogger _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    public WalkTrackOutputFormatter(
        ILogger logger,
        IEnumerable<ITranscoder> codecs
    )
    {
        _logger = logger ??
            throw new ArgumentNullException(nameof(logger));

        _codecs = codecs ??
            throw new ArgumentNullException(nameof(logger));

        foreach(WalkTrackMediaType mediaType in _codecs.SelectMany(codec => codec.GetSupportedMediaTypes()))
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(mediaType.ToString()));
        }

        SupportedEncodings.Add(Encoding.UTF8);
    }

    /// <summary>
    /// Writes the signed and validated <see cref="VerifiedData{TData}"/> to the http response <see cref="Stream"/>.
    /// </summary>
    /// <param name="context">
    /// The <see cref="OutputFormatterWriteContext"/>.
    /// </param>
    /// <param name="selectedEncoding">
    /// The selected <see cref="Encoding"/>.
    /// </param>
    public override async Task WriteResponseBodyAsync(
        OutputFormatterWriteContext context,
        Encoding selectedEncoding
    )
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Object is null)
        {
            throw new ArgumentException(nameof(context), "Object is required to process the output.");
        }

        if (context.ObjectType is null)
        {
            throw new ArgumentException(nameof(context), "ObjectType is required to process the output.");
        }

        IEnumerable<WalkTrackMediaType> acceptTypes =
            context.HttpContext.Request.Headers.Accept
                .SelectMany(accepts => accepts.Split(",").Select(accept => accept.Trim()))
                .Select(accept => WalkTrackMediaType.Parse(accept));

        if (
            !CanHandleResponse(
                context.ObjectType,
                acceptTypes,
                out ITranscoder codec,
                out string acceptType)
            )
        {
            context.HttpContext.Response.StatusCode = 406;
        }
        else
        {
            context.HttpContext.Response.ContentType = acceptType;

            await codec.Encode(
                context.Object,
                context.HttpContext.Response.Body,
                CancellationToken.None
            );
        }
    }

    /// <summary>
    /// Checks if a <see cref="Type"/> can be handled by this <see cref="TextOutputFormatter"/>.
    /// </summary>
    /// <param name="type">
    /// The <see cref="Type"/> to test.
    /// </param>
    /// <returns>
    /// True if <paramref name="type"/> can be handled by this <see cref="TextOutputFormatter"/>.
    /// </returns>
    protected override bool CanWriteType(Type? type) =>
        _codecs.Any(codec => type is not null && codec.CanHandle(type));

    private bool CanHandleResponse(Type type, IEnumerable<WalkTrackMediaType> acceptTypes, out ITranscoder transcoder, out string mediaType)
    {
        transcoder = _nullTranscoder;
        mediaType = string.Empty;

        foreach(WalkTrackMediaType acceptType in acceptTypes)
        {
            foreach(ITranscoder codec in _codecs)
            {
                if (codec.CanHandle(type) && codec.CanHandle(acceptType))
                {
                    transcoder = codec;
                    mediaType = acceptType.ToString();

                    return true;
                }
            }
        }

        return false;
    }
}