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
using WalkTrack.Framework.Common.Resources;

namespace WalkTrack.GoalService.Common;

/// <summary>
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record Goal: IResource
{
    /// <summary>
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// </summary>
    public string UserId { get; init; } = string.Empty;

    /// <summary>
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// </summary>
    public int Sort { get; init; } = 0;

    /// <summary>
    /// </summary>
    public IEnumerable<Milestone> Milestones { get; init; } = Enumerable.Empty<Milestone>();
}