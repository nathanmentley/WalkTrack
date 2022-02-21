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
using WalkTrack.Framework.Client.Exceptions;
using WalkTrack.Framework.Common.Resources;

namespace WalkTrack.Utils.DataSeeder.Loaders;

public sealed class AuthenticationDataLoader: BaseDataLoader<CreateAuthRequest>
{
    protected override WalkTrackMediaType MediaType =>
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.CreateAuthRequest")
            .WithVersion(1)
            .Build();

    protected override string Directory =>
        "data/authentications";

    private readonly IAuthenticationClient _authenticationClient;

    public AuthenticationDataLoader(
        IAuthenticationClient authenticationClient,
        ITranscoderProcessor transcoderProcessor
    ):
        base(transcoderProcessor)
    {
        _authenticationClient = authenticationClient ??
            throw new ArgumentNullException(nameof(authenticationClient));
    }

    protected override async Task LoadRecord(
        CreateAuthRequest record,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await _authenticationClient.Create(record, cancellationToken);
        }
        catch(UnhandledResponseErrorClientException)
        {
        }
    }
}