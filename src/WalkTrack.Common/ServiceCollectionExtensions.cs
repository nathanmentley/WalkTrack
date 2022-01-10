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

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using WalkTrack.Common.ApiErrorResponses;
using WalkTrack.Common.Authentications;
using WalkTrack.Common.Entries;
using WalkTrack.Common.Goals;
using WalkTrack.Common.Resources;
using WalkTrack.Common.Users;

namespace WalkTrack.Common;

/// <summary>
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    public static IServiceCollection WithWalkTrackTranscoders(this IServiceCollection collection) =>
        collection
            .AddSingleton<IWireTranscoder, ApiErrorResponseJsonV1Transcoder>()
            .AddSingleton<IWireTranscoder, AuthenticateRequestJsonV1Transcoder>()
            .AddSingleton<IWireTranscoder, AuthenticateResponseJsonV1Transcoder>()
            .AddSingleton<IWireTranscoder, EntryJsonV1Transcoder>()
            .AddSingleton<IWireTranscoder, EntryCollectionJsonV1Transcoder>()
            .AddSingleton<IWireTranscoder, GoalCollectionJsonV1Transcoder>()
            .AddSingleton<IWireTranscoder, GoalJsonV1Transcoder>()
            .AddSingleton<IWireTranscoder, UserJsonV1Transcoder>()
            .AddSingleton<IWireTranscoder, UserCollectionJsonV1Transcoder>()
            .AddSingleton<IWireTranscoder, CreateUserRequestJsonV1Transcoder>()
            .AddSingleton<IWireTranscoder, UpdatePasswordRequestJsonV1Transcoder>()
            .AddSingleton<IPersistTranscoder, EntryJsonV1Transcoder>()
            .AddSingleton<IPersistTranscoder, GoalJsonV1Transcoder>()
            .AddSingleton<IPersistTranscoder, SecureUserJsonV1Transcoder>()
            .AddSingleton<ITranscoderProcessor, TranscoderProcessor>();
}