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

using Microsoft.Extensions.DependencyInjection;
using WalkTrack.Client;
using WalkTrack.Common;

namespace WalkTrack.DataSeeder;

public static class Program
{
    static public async Task Main()
    {
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        Console.CancelKeyPress += (_, @event) =>
            {
                Console.WriteLine("Canceling...");
                cancellationTokenSource.Cancel();

                @event.Cancel = true;
            };

        await GetApp().Run(cancellationTokenSource.Token);
    }

    static private App GetApp() =>
        new ServiceCollection()
            .WithWalkTrackTranscoders()
            .WithWalkTrackClient()
            .AddSingleton<App, App>()
            .BuildServiceProvider()
            .GetService<App>() ??
                throw new InvalidOperationException();
}