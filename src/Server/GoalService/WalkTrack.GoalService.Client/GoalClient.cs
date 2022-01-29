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

using Flurl;
using WalkTrack.Framework.Client;
using WalkTrack.Framework.Client.Authentications;
using WalkTrack.Framework.Common.Resources;
using WalkTrack.GoalService.Common;

namespace WalkTrack.GoalService.Client;

internal sealed class GoalClient: BaseClient, IGoalClient, IDisposable
{
    private readonly IAuthenticator _authenicator;
    private readonly ITranscoderProcessor _transcoder;
    private readonly HttpClient _httpClient;

    private static readonly WalkTrackMediaType _goalMediaType =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Goal")
            .WithVersion(1)
            .Build();

    private static readonly WalkTrackMediaType _goalsMediaType =
        new WalkTrackMediaTypeBuilder()
            .WithType(WalkTrackMediaTypeTypes.Application)
            .WithSubType(WalkTrackMediaTypeSubTypes.Json)
            .WithStructure("WalkTrack.Goals")
            .WithVersion(1)
            .Build();

    public GoalClient(string url, IAuthenticator authenicator, ITranscoderProcessor transcoder)
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

    public async Task<Goal> Fetch(string id, CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithMethod(HttpMethod.Get)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("goal")
                    .AppendPathSegment(id)
            )
            .WithAcceptType(_goalMediaType)
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthenticator(_authenicator)
            .WithErrorHandler(new ResourceNotFoundErrorHandler())
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Fetch<Goal>(_httpClient, cancellationToken);

    public async Task<IEnumerable<Goal>> Search(CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithMethod(HttpMethod.Get)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("goal")
            )
            .WithAcceptType(_goalsMediaType)
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthenticator(_authenicator)
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Fetch<IEnumerable<Goal>>(_httpClient, cancellationToken);

    public async Task<Goal> Create(Goal goal, CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithBody(goal)
            .WithContentTypes(_goalMediaType)
            .WithMethod(HttpMethod.Post)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("goal")
            )
            .WithAcceptType(_goalMediaType)
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthenticator(_authenicator)
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Fetch<Goal, Goal>(_httpClient, cancellationToken);

    public async Task Update(Goal goal, CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithBody(goal)
            .WithContentTypes(_goalMediaType)
            .WithMethod(HttpMethod.Put)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("goal")
            )
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthenticator(_authenicator)
            .WithErrorHandler(new ResourceNotFoundErrorHandler())
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Send<Goal>(_httpClient, cancellationToken);

    public async Task Delete(string id, CancellationToken cancellationToken = default) =>
        await new RequestBuilder(_transcoder)
            .WithMethod(HttpMethod.Delete)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("goal")
                    .AppendPathSegment(id)
            )
            .WithAcceptType(_goalMediaType)
            .WithAcceptType(MediaTypes.ApiError)
            .WithAuthenticator(_authenicator)
            .WithErrorHandler(new ResourceNotFoundErrorHandler())
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Send(_httpClient, cancellationToken);

    public void Dispose() =>
        _httpClient.Dispose();
}