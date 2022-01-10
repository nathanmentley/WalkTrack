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
using Microsoft.Extensions.DependencyInjection;
using WalkTrack.Server.DAL.Entries;
using WalkTrack.Server.DAL.Goals;
using WalkTrack.Server.DAL.Users;
using WalkTrack.Server.Entries;
using WalkTrack.Server.Goals;
using WalkTrack.Server.Users;

namespace WalkTrack.Server.DAL;

/// <summary>
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    public static IServiceCollection WithWalkTrackDAL(this IServiceCollection collection) =>
        collection
            .AddSingleton<IEntryRepository, EntryRepository>()
            .AddSingleton<IGoalRepository, GoalRepository>()
            .AddSingleton<IUserRepository, UserRepository>();
}