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

using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WalkTrack.App;
using WalkTrack.App.Authenticator;
using WalkTrack.Common;
using WalkTrack.EntryService.Client;
using WalkTrack.EntryService.Common;
using WalkTrack.Framework.Client.Authentications;
using WalkTrack.GoalService.Client;
using WalkTrack.GoalService.Common;
using WalkTrack.UserService.Client;
using WalkTrack.UserService.Common;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .WithWalkTrackClient()
    .WithEntryTranscoders()
    .WithEntryClient(builder.Configuration.GetValue<string>("EntryServiceUrl"))
    .WithGoalTranscoders()
    .WithGoalClient(builder.Configuration.GetValue<string>("GoalServiceUrl"))
    .WithUserTranscoders()
    .WithUserClient(
        builder.Configuration.GetValue<string>("AuthServiceUrl"),
        builder.Configuration.GetValue<string>("UserServiceUrl")
    )

    .AddSingleton<AppAuthenticator, CachedAppAuthenticator>()
    .AddSingleton<IAuthenticator>(sp => sp.GetRequiredService<CachedAppAuthenticator>())

    .AddBlazoredLocalStorage();

await builder.Build().RunAsync();
