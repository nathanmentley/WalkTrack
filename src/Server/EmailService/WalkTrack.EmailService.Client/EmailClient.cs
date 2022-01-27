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

using Flurl;
using WalkTrack.EmailService.Common;
using WalkTrack.Framework.Client;
using WalkTrack.Framework.Client.Authentications;
using WalkTrack.Framework.Common.Resources;

namespace WalkTrack.EmailService.Client;

internal sealed class EmailClient: BaseClient, IEmailClient, IDisposable
{
    private readonly IAuthenticator _authenicator;
    private readonly ITranscoderProcessor _transcoder;
    private readonly HttpClient _httpClient;

    public EmailClient(string url, IAuthenticator authenicator, ITranscoderProcessor transcoder)
    {
        _transcoder = transcoder ??
            throw new ArgumentNullException(nameof(transcoder));

        _authenicator = authenicator ??
            throw new ArgumentNullException(nameof(authenicator));

        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(url)
        };
    }

    public async Task Send(IEnumerable<Email> emails, CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithBody(emails)
            .WithContentTypes(MediaTypes.Emails)
            .WithMethod(HttpMethod.Post)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("email")
                    .AppendPathSegment("send")
            )
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthenticator(_authenicator)
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Send<Email>(_httpClient, cancellationToken);

    public void Dispose() =>
        _httpClient.Dispose();
}