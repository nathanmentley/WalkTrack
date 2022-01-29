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

using System.Diagnostics.CodeAnalysis;
using WalkTrack.Framework.Common.ApiErrorResponses;
using WalkTrack.Framework.Server.Exceptions;
using WalkTrack.Framework.Server.Hosting.Exceptions;

namespace WalkTrack.Framework.Server.Hosting.ExceptionFilters;

/// <summary>
/// An excpetion handler for <see cref="MissingQueryStringException"/>
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class MissingQueryStringExceptionHandler: IExceptionHandler
{
    /// <summary>
    /// The http status code to be returned from this handler
    /// </summary>
    public int StatusCode => 400;

    /// <summary>
    /// Checks if the exception can be handled by this handler
    /// </summary>
    /// <param name="exception">
    /// The exception that needs handling
    /// </param>
    /// <returns>
    /// true, if the exception can be handled
    /// </returns>
    public bool CanHandle(Exception exception) =>
        exception is MissingQueryStringException;

    public ApiErrorResponse BuildErrorResponse(Exception exception) =>
        new ApiErrorResponse()
        {
            StatusCode = StatusCode,
            Message = "Missing query string in request."
        };
}