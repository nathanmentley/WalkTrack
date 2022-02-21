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

namespace WalkTrack.AuthService.Client;

internal static class MediaTypes
{
    public static readonly WalkTrackMediaType ApiError =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.ApiError")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType CreateAuthRequest =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.CreateAuthRequest")
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

    public static readonly WalkTrackMediaType AuthorizeRequest =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.AuthorizeRequest")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType AuthorizeResponse =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.AuthorizeResponse")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType Token =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Token")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType ForgotPasswordResponse =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.ForgotPasswordResponse")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType ForgotPasswordRequest =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.ForgotPasswordRequest")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType ResetPasswordRequest =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.ResetPasswordRequest")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType Role =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Role")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType Roles =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Roles")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType RoleLinkRequest =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.RoleLinkRequest")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType Permission =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Permission")
            .WithVersion(1)
            .Build();

    public static readonly WalkTrackMediaType Permissions =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Permissions")
            .WithVersion(1)
            .Build();
}