﻿/*
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WalkTrack.Framework.Client.Authentications;
using WalkTrack.Framework.Common.Resources;

namespace WalkTrack.AuthService.Client;

/// <summary>
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    public static IServiceCollection WithAuthClient(this IServiceCollection collection, string authUrl) =>
        collection
            .AddSingleton<IAuthenticationClient>(
                sp => new AuthenticationClient(authUrl, sp.GetRequiredService<ITranscoderProcessor>())
            );

    /// <summary>
    /// </summary>
    public static IServiceCollection WithServiceAuthentication(
        this IServiceCollection collection,
        IConfiguration configuration
    ) =>
        collection
            .AddSingleton<IAuthenticationClient>(
                sp => new AuthenticationClient(
                    configuration
                        .GetSection("ServiceAuthenticatorSettings")
                        .GetValue<string>("AuthAddress"),
                    sp.GetRequiredService<ITranscoderProcessor>()
                )
            )
            .AddSingleton<IAuthenticator, ServiceAuthenticator>();
}