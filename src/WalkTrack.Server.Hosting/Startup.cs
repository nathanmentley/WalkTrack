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
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using WalkTrack.Common;
using WalkTrack.Server.DAL;
using WalkTrack.Server.Hosting.ExceptionFilters;
using WalkTrack.Server.Hosting.Middlewares;
using WalkTrack.Server.Services;

namespace WalkTrack.Server.Hosting;

/// <summary>
/// Startup logic to execute when spinning up the api.
/// </summary>
[ExcludeFromCodeCoverage]
public class Startup
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="configuration">
    /// An <see cref="IConfiguration"/> instance to use when configuring the server.
    /// </param>
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Configures and sets up DI for the server.
    /// </summary>
    /// <param name="services">
    /// An <see cref="IServiceCollection"/> to use to register services against.
    /// </param>
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddLogging()

            .Configure<AppSettings>(_configuration.GetSection("AppSettings"))

            .WithWalkTrackTranscoders()
            .WithWalkTrackDAL()
            .WithWalkTrackServices()
            .WithExceptionHandlers()

            .AddSingleton<IConfigureOptions<MvcOptions>, ConfigureMvcOptions>()

            .AddCors(options =>
                options.AddDefaultPolicy(
                    builder =>
                        builder
                            .WithOrigins("*")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                )
            )

            .Configure<KestrelServerOptions>(options => options.AllowSynchronousIO = true)

            .AddControllers();
    }

    /// <summary>
    /// Configures teh application
    /// </summary>
    /// <param name="app">
    /// The <see cref="IApplicationBuilder"/> to configure
    /// </param>
    /// <param name="env">
    /// The <see cref="IWebHostEnvironment"/> to configure against.
    /// </param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) =>
        app
            .UseRouting()
            .UseMiddleware<JwtMiddleware>()
            .UseCors()
            .UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                }
            );
}