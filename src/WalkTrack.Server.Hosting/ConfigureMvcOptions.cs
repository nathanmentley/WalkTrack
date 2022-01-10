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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using WalkTrack.Common.Resources;
using WalkTrack.Server.Exceptions;
using WalkTrack.Server.Hosting.ExceptionFilters.Core;
using WalkTrack.Server.Hosting.Formatters;

namespace WalkTrack.Server.Hosting;

/// <summary>
/// 
/// </summary>
[ExcludeFromCodeCoverage]
public class ConfigureMvcOptions : IConfigureOptions<MvcOptions>
{
    private readonly ILogger<ConfigureMvcOptions> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IEnumerable<IWireTranscoder> _wireTranscoders;
    private readonly IEnumerable<IExceptionHandler> _handlers;

    public ConfigureMvcOptions(
        ILogger<ConfigureMvcOptions> logger,
        ILoggerFactory loggerFactory,
        IEnumerable<IWireTranscoder> wireTranscoders,
        IEnumerable<IExceptionHandler> handlers
    )
    {
        _logger = logger ??
            throw new ArgumentNullException(nameof(logger));

        _loggerFactory = loggerFactory ??
            throw new ArgumentNullException(nameof(loggerFactory));

        _wireTranscoders = wireTranscoders ??
            throw new ArgumentNullException(nameof(wireTranscoders));

        _handlers = handlers ??
            throw new ArgumentNullException(nameof(handlers));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options">
    /// 
    /// </param>
    public void Configure(MvcOptions options)
    {
        _logger.LogTrace("Setting up input formatters.");

        options.InputFormatters.Insert(
            0,
            new WalkTrackInputFormatter(
                _loggerFactory.CreateLogger<WalkTrackInputFormatter>(),
                _wireTranscoders
            )
        );

        _logger.LogTrace("Setting up output formatters.");

        options.OutputFormatters.Insert(
            0,
            new WalkTrackOutputFormatter(
                _loggerFactory.CreateLogger<WalkTrackOutputFormatter>(),
                _wireTranscoders
            )
        );

        _logger.LogTrace("Setting up exception handlers.");

        options.Filters.Insert(
            0,
            new WalkTrackExceptionFilter(_loggerFactory.CreateLogger<WalkTrackExceptionFilter>(), _handlers)
        );
    }
}