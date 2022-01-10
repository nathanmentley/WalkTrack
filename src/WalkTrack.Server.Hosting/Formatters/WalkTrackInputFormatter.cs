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
///
/// </summary>
public class WalkTrackInputFormatter: TextInputFormatter
{
    private readonly IEnumerable<IWireTranscoder> _dataCodecs;
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    public WalkTrackInputFormatter(
        ILogger logger,
        IEnumerable<IWireTranscoder> dataCodecs
    )
    {
        _logger = logger ??
            throw new ArgumentNullException(nameof(logger));

        _dataCodecs = dataCodecs ??
            throw new ArgumentNullException(nameof(dataCodecs));

        foreach(WalkTrackMediaType mediaType in _dataCodecs.SelectMany(codec => codec.GetSupportedMediaTypes()))
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(mediaType.ToString()));
        }

        SupportedEncodings.Add(Encoding.UTF8);
    }

    /// <summary>
    /// Reads a <see cref="SignedData"/> from a http request <see cref="Stream"/>.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InputFormatterContext"/>.
    /// </param>
    /// <param name="encoding">
    /// The selected <see cref="Encoding"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> with the signed and validated <see cref="VerifiedData{TData}"/>.
    /// </returns>
    public override async Task<InputFormatterResult> ReadRequestBodyAsync(
        InputFormatterContext context,
        Encoding encoding
    )
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.HttpContext.Request.ContentType is null)
        {
            throw new ArgumentException(nameof(context), "ContentType is required to process the input.");
        }

        if (context.HttpContext.Request.Body is null)
        {
            throw new ArgumentException(nameof(context), "Body is required to process the input.");
        }

        WalkTrackMediaType mediaType = WalkTrackMediaType.Parse(context.HttpContext.Request.ContentType);

        IWireTranscoder? codec = _dataCodecs.FirstOrDefault(codec => codec.CanHandle(mediaType));

        if (codec is null)
        {
            return await InputFormatterResult.FailureAsync();
        }

        object? result = await codec.Decode(context.HttpContext.Request.Body, CancellationToken.None);

        return await InputFormatterResult.SuccessAsync(result);
    }

    /// <summary>
    /// Checks if a <see cref="Type"/> can be handled by this <see cref="TextInputFormatter"/>.
    /// </summary>
    /// <param name="type">
    /// The <see cref="Type"/> to test.
    /// </param>
    /// <returns>
    /// True if <paramref name="type"/> can be handled by this <see cref="TextInputFormatter"/>.
    /// </returns>
    protected override bool CanReadType(Type type) =>
        _dataCodecs.Any(codec => codec.CanHandle(type));
}