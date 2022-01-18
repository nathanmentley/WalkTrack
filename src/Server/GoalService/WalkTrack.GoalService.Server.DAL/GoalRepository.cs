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

using Microsoft.Extensions.Options;
using WalkTrack.Framework.Common.Resources;
using WalkTrack.Framework.Server.DAL.CouchDb;
using WalkTrack.Framework.Server.DAL.CouchDb.Criteria;
using WalkTrack.GoalService.Common;
using WalkTrack.GoalService.Server.Configuration;

namespace WalkTrack.GoalService.Server.DAL;

internal sealed class GoalRepository: BaseRepository<Goal, GoalPersistedDocuemnt>, IGoalRepository
{
    private static readonly IEnumerable<ICriterionHandler<GoalPersistedDocuemnt>> CriterionHandlers =
        new ICriterionHandler<GoalPersistedDocuemnt>[] {
            new IdCriterionHandler(),
            new UserIdCriterionHandler()
        };

    private static readonly WalkTrackMediaType SupportedMediaType =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.SecureGoal")
            .WithVersion(1)
            .Build();

    private static readonly ITranscoder Transcoder =
        new SecureGoalJsonV1Transcoder();

    public GoalRepository(IOptions<DalSettings> dalSettings):
        base(
            "goalDb",
            dalSettings.Value.ConnectionString,
            dalSettings.Value.Username,
            dalSettings.Value.Password,
            CriterionHandlers
        )
    {
    }

    protected override WalkTrackMediaType GetSupportedMediaType() =>
        SupportedMediaType;

    protected override ITranscoder GetTranscoder() =>
        Transcoder;
}