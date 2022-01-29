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

using FluentMigrator.Runner;
using WalkTrack.AuthService.Client;
using WalkTrack.AuthService.Common;
using WalkTrack.AuthService.Server.Configuration;
using WalkTrack.AuthService.Server.DAL;
using WalkTrack.AuthService.Server.Services;
using WalkTrack.Framework.Server.Hosting;

namespace WalkTrack.AuthService.Server.Hosting;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder =
            WebApplication
                .CreateBuilder(args);

        builder.Configuration.AddEnvironmentVariables();

        RegisterServices(builder.Services, builder.Configuration);

        WebApplication app = builder.Build();

        app.WithFramework();

        RunMigrations(app);

        app.Run();
    }

    private static void RunMigrations(WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();

        scope
            .ServiceProvider
            .GetRequiredService<IMigrationRunner>()
            .MigrateUp();
    }

    private static void RegisterServices(IServiceCollection services, IConfiguration configuration) =>
        services
            .AddOptions()
            .Configure<DalSettings>(configuration.GetSection("DalSettings"))
            .Configure<ServiceAuthenticatorSettings>(configuration.GetSection("ServiceAuthenticatorSettings"))
            .WithFramework(configuration)
            .WithAuthTranscoders()
            .WithAuthDAL()
            .WithAuthServices(configuration)
            .WithServiceAuthentication(configuration)

            .AddFluentMigratorCore()
            .ConfigureRunner(
                runnerBuilder =>
                    runnerBuilder
                        .AddSqlServer()
                        .WithGlobalConnectionString(
                            configuration
                                .GetSection("DalSettings")
                                .GetValue<string>("ConnectionString")
                        )
                        .ScanIn(typeof(Program).Assembly).For.Migrations()
            )
            .AddLogging(loggingBuilder => loggingBuilder.AddFluentMigratorConsole());
}