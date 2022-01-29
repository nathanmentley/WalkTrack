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

namespace WalkTrack.Framework.Server.Exceptions;

/// <summary>
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class InvalidRequestException: BaseServerWalkTrackException
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="message">
    /// A message describing the exceptional situation in detail.
    /// </param>
    /// <param name="innerException">
    /// Another exception that brought this exception to light.
    /// </param>
    public InvalidRequestException(string message, Exception innerException):
        base(message, innerException) {}

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="message">
    /// A message describing the exceptional situation in detail.
    /// </param>
    public InvalidRequestException(string message):
        base(message) {}

    /// <summary>
    /// Constructor
    /// </summary>
    public InvalidRequestException():
        base() {}
}
