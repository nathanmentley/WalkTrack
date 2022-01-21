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
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using WalkTrack.Framework.Server.Exceptions;

namespace WalkTrack.Framework.Server.Hosting.ExceptionFilters.Core;

/// <summary>
/// An abstract excpetion handler implementation to transform exceptions into http status codes.
/// </summary>
internal sealed class WalkTrackExceptionFilter: ExceptionFilterAttribute
{
    private readonly ILogger _Logger;
    private readonly IEnumerable<IExceptionHandler> _handlers;
    private readonly ILastResortExceptionHandler _lastResortHandler;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">
    /// An <see cref="ILogger"/> instance for logging
    /// </param>
    public WalkTrackExceptionFilter(
        ILogger<WalkTrackExceptionFilter> logger,
        IEnumerable<IExceptionHandler> handlers,
        ILastResortExceptionHandler lastResortHandler
    )
    {
        _Logger = logger ??
            throw new ArgumentNullException(nameof(logger));

        _handlers = handlers ??
            throw new ArgumentNullException(nameof(handlers));

        _lastResortHandler = lastResortHandler ??
            throw new ArgumentNullException(nameof(lastResortHandler));
    }

    /// <summary>
    /// Tests and runs this handle for an exceptional event
    /// </summary>
    /// <param name="context">
    /// The exception context of the exceptional event.
    /// </param>
    public override async Task OnExceptionAsync(ExceptionContext context)
    {
        IExceptionHandler? handler = _handlers.FirstOrDefault(handler => handler.CanHandle(context.Exception));

        Console.Write($"{context.Exception}");

        if (handler is not null)
        {
            _Logger.LogDebug(
                "Running {handler} for {exception}.",
                handler.GetType(),
                context.Exception.GetType()
            );

            context.Result = new ObjectResult(handler.BuildErrorResponse(context.Exception))
            {
                StatusCode = handler.StatusCode
            };
        }
        else
        {
            context.Result = new ObjectResult(_lastResortHandler.BuildErrorResponse(context.Exception))
            {
                StatusCode = _lastResortHandler.StatusCode
            };
        }

        await base.OnExceptionAsync(context);
    }
}