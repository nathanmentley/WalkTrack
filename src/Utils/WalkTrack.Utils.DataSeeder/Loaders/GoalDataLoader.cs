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

using WalkTrack.Framework.Common.Resources;
using WalkTrack.GoalService.Client;
using WalkTrack.GoalService.Common;

namespace WalkTrack.Utils.DataSeeder.Loaders;

public sealed class GoalDataLoader: BaseDataLoader<Goal>
{
    protected override WalkTrackMediaType MediaType =>
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Goal")
            .WithVersion(1)
            .Build();

    protected override string Directory =>
        "data/goals";

    private readonly IGoalClient _goalClient;

    public GoalDataLoader(
        IGoalClient goalClient,
        ITranscoderProcessor transcoderProcessor
    ):
        base(transcoderProcessor)
    {
        _goalClient = goalClient ??
            throw new ArgumentNullException(nameof(goalClient));
    }

    protected override async Task LoadRecord(
        Goal record,
        CancellationToken cancellationToken = default
    )
    {
        IEnumerable<Goal> goals =
            await _goalClient.Search(cancellationToken);

        Goal? existing =
            goals.FirstOrDefault(
                goal =>
                    string.Equals(record.Name, goal.Name) &&
                    string.IsNullOrWhiteSpace(record.UserId)
            );

        if (existing is null)
        {
            await _goalClient.Create(record, cancellationToken);
        }
        else
        {
            await _goalClient.Update(
                record with { Id = existing.Id },
                cancellationToken
            );
        }
    }
}