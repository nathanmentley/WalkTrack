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

using WalkTrack.EmailService.Common;

namespace WalkTrack.EmailService.Server.Services;

internal sealed class EmailService: IEmailService
{
    private readonly IEmailProcessor _processor;

    public EmailService(IEmailProcessor processor)
    {
        if (processor is null)
        {
            throw new ArgumentNullException(nameof(processor));
        }

        _processor = processor;
    }
 
    public Task Send(IEnumerable<Email> emails, CancellationToken cancellationToken = default) =>
        _processor.Send(emails, cancellationToken);
}