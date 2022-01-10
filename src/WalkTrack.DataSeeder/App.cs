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

using WalkTrack.Client.Goals;
using WalkTrack.Common.Goals;

namespace WalkTrack.DataSeeder;

public class App
{
    private readonly IGoalClient _goalClient;

    public App(IGoalClient goalClient)
    {
        _goalClient = goalClient ??
            throw new ArgumentNullException(nameof(goalClient));
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        foreach (Goal goal in await FetchGoals())
        {
            await ProcessGoal(goal, cancellationToken);
        }
    }

    private async Task ProcessGoal(Goal goal, CancellationToken cancellationToken)
    {
        if (await _goalClient.Fetch(goal.Id, cancellationToken) is null)
        {
            await _goalClient.Create(goal, cancellationToken);
        }
        else
        {
            await _goalClient.Update(goal, cancellationToken);
        }
    }

    private static Task<IEnumerable<Goal>> FetchGoals() =>
        Task.FromResult(Enumerable.Empty<Goal>());
}