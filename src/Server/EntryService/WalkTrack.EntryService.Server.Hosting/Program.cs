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

using WalkTrack.AuthService.Client;
using WalkTrack.AuthService.Common;
using WalkTrack.EntryService.Common;
using WalkTrack.EntryService.Server.Configuration;
using WalkTrack.EntryService.Server.DAL;
using WalkTrack.EntryService.Server.Services;
using WalkTrack.Framework.Server.Hosting;

WebApplicationBuilder builder =
    WebApplication
        .CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder
    .Services
        .AddOptions()
        .Configure<DalSettings>(builder.Configuration.GetSection("DalSettings"))
        .Configure<ServiceAuthenticatorSettings>(builder.Configuration.GetSection("ServiceAuthenticatorSettings"))
        .WithFramework(builder.Configuration)
        .WithEntryTranscoders()
        .WithEntryDAL()
        .WithEntryServices()

        .WithAuthTranscoders()
        .WithServiceAuthentication(builder.Configuration);

WebApplication app = builder.Build();

app.WithFramework();

app.Run();