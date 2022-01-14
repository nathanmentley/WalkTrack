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

using WalkTrack.Common.Resources;

namespace WalkTrack.Client;

internal static class MediaTypes
{
    public static readonly WalkTrackMediaType ApiError =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.ApiError")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType AuthenticateRequest =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.AuthenticateRequest")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType AuthenticateResponse =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.AuthenticateResponse")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType Entry =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Entry")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType Entries =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Entries")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType Goal =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Goal")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType Goals =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Goals")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType User =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.User")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType Users =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Users")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType CreateUser =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.CreateUserRequest")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType UpdatePassword =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.UpdatePasswordRequest")
            .WithVersion(1)
            .Build();
}