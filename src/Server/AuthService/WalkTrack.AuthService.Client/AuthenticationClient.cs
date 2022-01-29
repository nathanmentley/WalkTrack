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
using WalkTrack.Framework.Common.Resources;
using WalkTrack.AuthService.Common;

namespace WalkTrack.AuthService.Client;

internal sealed class AuthenticationClient: BaseClient, IAuthenticationClient, IDisposable
{
    private readonly ITranscoderProcessor _transcoder;
    private readonly HttpClient _httpClient;

    public AuthenticationClient(string url, ITranscoderProcessor transcoder)
    {
        _transcoder = transcoder ??
            throw new ArgumentNullException(nameof(transcoder));

        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(url)
        };
    }

    public async Task<AuthenticateResponse> Login(
        AuthenticateRequest request,
        CancellationToken cancellationToken = default
    ) =>
            await new RequestBuilder(_transcoder)
                .WithBody(request)
                .WithContentTypes(MediaTypes.AuthenticateRequest)
                .WithMethod(HttpMethod.Post)
                .WithUrl(
                    new Url()
                        .AppendPathSegment("v1")
                        .AppendPathSegment("authenticate")
                )
                .WithAcceptType(MediaTypes.AuthenticateResponse)
                .WithAcceptType(MediaTypes.ApiError)
                .WithErrorHandler(new ResourceNotFoundErrorHandler())
                .Fetch<AuthenticateRequest, AuthenticateResponse>(_httpClient, cancellationToken);

    public async Task<Token> RefreshToken(
        Token request,
        CancellationToken cancellationToken = default
    ) =>
            await new RequestBuilder(_transcoder)
                .WithBody(request)
                .WithContentTypes(MediaTypes.Token)
                .WithMethod(HttpMethod.Put)
                .WithUrl(
                    new Url()
                        .AppendPathSegment("v1")
                        .AppendPathSegment("token")
                )
                .WithAcceptType(MediaTypes.Token)
                .WithAcceptType(MediaTypes.ApiError)
                .WithErrorHandler(new ForbiddenErrorHandler())
                .WithErrorHandler(new UnauthorizedErrorHandler())
                .Fetch<Token, Token>(_httpClient, cancellationToken);

    public async Task RequestForgottenPassword(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken = default
    ) =>
        await new RequestBuilder(_transcoder)
            .WithBody(request)
            .WithContentTypes(MediaTypes.ForgotPasswordRequest)
            .WithMethod(HttpMethod.Post)
            .WithUrl(
                new Url()
                    .AppendPathSegment("v1")
                    .AppendPathSegment("password")
                    .AppendPathSegment("forgot")
            )
            .WithAcceptType(MediaTypes.ApiError)
            .WithErrorHandler(new ForbiddenErrorHandler())
            .WithErrorHandler(new UnauthorizedErrorHandler())
            .Fetch<ForgotPasswordRequest>(_httpClient, cancellationToken);

    public async Task<AuthenticateResponse> ResetPassword(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default
    ) =>
            await new RequestBuilder(_transcoder)
                .WithBody(request)
                .WithContentTypes(MediaTypes.ResetPasswordRequest)
                .WithMethod(HttpMethod.Post)
                .WithUrl(
                    new Url()
                        .AppendPathSegment("v1")
                        .AppendPathSegment("password")
                        .AppendPathSegment("reset")
                )
                .WithAcceptType(MediaTypes.AuthenticateRequest)
                .WithAcceptType(MediaTypes.ApiError)
                .WithErrorHandler(new ForbiddenErrorHandler())
                .WithErrorHandler(new UnauthorizedErrorHandler())
                .Fetch<ResetPasswordRequest, AuthenticateResponse>(_httpClient, cancellationToken);

    public void Dispose() =>
        _httpClient.Dispose();
}