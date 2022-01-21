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

using WalkTrack.App.State;
using WalkTrack.GoalService.Common;

namespace WalkTrack.App.Pages.Models;

public class GoalModel
{
    public string Name { get; set; } = string.Empty;

    public bool Collapsed { get; set; } = true;
    public bool Loading { get; set; } = false;

    public IEnumerable<Goal> Goals { get; private set; } = Enumerable.Empty<Goal>();

    public void SetGoals(IEnumerable<Goal> goals)
    {
        Goals = goals;
    }

    public Goal ToGoal() =>
        new Goal()
        {
            Name = Name,
            UserId = AppState.UserId
        };
}