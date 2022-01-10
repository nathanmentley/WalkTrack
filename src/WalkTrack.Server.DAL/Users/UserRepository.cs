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

using Couchbase.Lite;
using WalkTrack.Common.Resources;
using WalkTrack.Common.Users;
using WalkTrack.Server.DAL.Criteria.Core;
using WalkTrack.Server.DAL.Resources;
using WalkTrack.Server.Users;

namespace WalkTrack.Server.DAL.Users;

internal sealed class UserRepository: BaseRepository<User>, IUserRepository
{
    private static readonly IEnumerable<ICriterionHandler> CriterionHandlers =
        new ICriterionHandler[] {
            new IdCriterionHandler(),
            new UsernameCriterionHandler()
        };

    private static readonly WalkTrackMediaType _supportedMediaType =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.SecureUser")
            .WithVersion(1)
            .Build();

    public UserRepository(ITranscoderProcessor transcoderProcessor):
        base("userdb", transcoderProcessor, CriterionHandlers) {}

    protected override WalkTrackMediaType GetSupportedMediaType() =>
        _supportedMediaType;

    protected override async Task BuildDocument(
        MutableDocument mutableDocument,
        User resource,
        CancellationToken cancellationToken
    )
    {
        await base.BuildDocument(mutableDocument, resource, cancellationToken);

        mutableDocument.SetString("username", resource.Username);
        mutableDocument.SetString("password", resource.Password);
    }
}
