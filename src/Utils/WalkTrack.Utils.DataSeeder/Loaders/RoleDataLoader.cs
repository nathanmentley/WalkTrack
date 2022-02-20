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
using WalkTrack.Framework.Common.Resources;

namespace WalkTrack.Utils.DataSeeder.Loaders;

public sealed class RoleDataLoader: BaseDataLoader<Role>
{
    protected override WalkTrackMediaType MediaType =>
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Role")
            .WithVersion(1)
            .Build();

    protected override string Directory =>
        "data/roles";

    private readonly IRoleClient _roleClient;

    public RoleDataLoader(
        IRoleClient roleClient,
        ITranscoderProcessor transcoderProcessor
    ):
        base(transcoderProcessor)
    {
        _roleClient = roleClient ??
            throw new ArgumentNullException(nameof(roleClient));
    }

    protected override async Task LoadRecord(
        Role record,
        CancellationToken cancellationToken = default
    )
    {
        IEnumerable<Role> roles = await _roleClient.Search(cancellationToken);

        if (!roles.Any(role => string.Equals(record.Name, role.Name)))
        {
            await _roleClient.Create(record, cancellationToken);
        }
    }
}