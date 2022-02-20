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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WalkTrack.AuthService.Client;
using WalkTrack.AuthService.Common;
using WalkTrack.Common;
using WalkTrack.EntryService.Client;
using WalkTrack.EntryService.Common;
using WalkTrack.GoalService.Client;
using WalkTrack.GoalService.Common;
using WalkTrack.UserService.Client;
using WalkTrack.UserService.Common;
using WalkTrack.Utils.DataSeeder.Loaders;

namespace WalkTrack.Utils.DataSeeder;

public static class Program
{
    public static async Task Main(string[] args) =>
        await GetServices(GetConfiguration())
            .GetRequiredService<App>()
            .Run();

    private static IServiceProvider GetServices(
        IConfiguration configuration
    ) =>
        new ServiceCollection()
            .WithWalkTrackClient()
            .WithAuthTranscoders()
            .WithEntryTranscoders()
            .WithGoalTranscoders()
            .WithUserTranscoders()

            .WithAuthClient(configuration.GetValue<string>("AuthServiceUrl"))
            .WithEntryClient(configuration.GetValue<string>("EntryServiceUrl"))
            .WithGoalClient(configuration.GetValue<string>("GoalServiceUrl"))
            .WithUserClient(configuration.GetValue<string>("UserServiceUrl"))

            .WithServiceAuthentication(configuration)

            .AddSingleton<IDataLoader, RoleDataLoader>()
            .AddSingleton<IDataLoader, PermissionDataLoader>()
            .AddSingleton<IDataLoader, RolePermissionsDataLoader>()
            .AddSingleton<IDataLoader, GoalDataLoader>()

            .AddSingleton<App>()
            .BuildServiceProvider();

    private static IConfiguration GetConfiguration() =>
        new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
}