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

using SendGrid;
using SendGrid.Helpers.Mail;
using WalkTrack.EmailService.Common;
using WalkTrack.EmailService.Server.Configuration;

namespace WalkTrack.EmailService.Server.Processor;

internal sealed class EmailProcessor : IEmailProcessor
{
    private readonly ISendGridClient _client;

    public EmailProcessor(ConnectionSettings connectionSettings)
    {
        _client = new SendGridClient(connectionSettings.ApiKey);
    }

    public async Task Send(IEnumerable<Email> emails, CancellationToken cancellationToken = default) =>
        await Task.WhenAll(
            emails.Select(
                email => Send(email, cancellationToken)
            )
        );

    private async Task Send(Email email, CancellationToken cancellationToken) =>
        await _client.SendEmailAsync(ToMessage(email), cancellationToken);

    public static SendGridMessage ToMessage(Email email) =>
        MailHelper.CreateSingleEmail(
            new EmailAddress(email.FromAddress, email.From),
            new EmailAddress(email.ToAddress, email.To),
            email.Subject,
            email.TextMessage,
            email.HtmlMessage
        );
}