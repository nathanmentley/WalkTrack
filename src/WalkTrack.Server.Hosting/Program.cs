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

namespace WalkTrack.Server.Hosting;

/// <summary>
/// The entrypoint class
/// </summary>
[ExcludeFromCodeCoverage]
public class Program
{
    /// <summary>
    /// The entrypoint method
    /// </summary>
    /// <param name="args">
    /// Arguments passed to the program when it was called.
    /// </param>
    public static void Main(string[] args) =>
        CreateHostBuilder(args)
            .Build()
            .Run();

    /// <summary>
    /// Setsup the post builder for the api server.
    /// </summary>
    /// <param name="args">
    /// Arguments passed to the program when it was called.
    /// </param>
    /// <returns>
    /// An <see cref="IHostBuilder"/> configured to build the api server host.
    /// </returns>
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logBuilder => {
                logBuilder.ClearProviders();
                logBuilder.AddConsole();  
            })
            .ConfigureWebHostDefaults(webBuilder => {
                webBuilder.UseStartup<Startup>();
            });
}