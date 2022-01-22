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

using WalkTrack.Framework.Common.Criteria;

namespace WalkTrack.UserService.Server.Criteria;

public sealed class ResetTokenCriterion: ICriterion
{
    public string ResetToken { get; }

    public ResetTokenCriterion(string resetToken)
    {
        if (string.IsNullOrWhiteSpace(resetToken))
        {
            throw new ArgumentNullException(nameof(resetToken));
        }

        ResetToken = resetToken;
    }
}