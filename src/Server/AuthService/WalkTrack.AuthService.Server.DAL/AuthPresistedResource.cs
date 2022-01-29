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

using System.Data.SqlTypes;
using WalkTrack.Framework.Server.DAL.Mssql;

namespace WalkTrack.AuthService.Server.DAL;

internal sealed class AuthPresistedResource: BasePresistedResource
{
    private readonly static DateTime MinSqlDataTime =
        SqlDateTime.MinValue.Value;

    internal Guid Id { get; init; } = Guid.Empty;
    internal string Username { get; init; } = string.Empty;
    internal string Email { get; init; } = string.Empty;
    internal string Password { get; init; } = string.Empty;
    internal string Salt { get; init; } = string.Empty;
    internal string ResetToken { get; init; } = string.Empty;
    private DateTime _resetTokenExpiresAt = MinSqlDataTime;
    internal DateTime ResetTokenExpiresAt {
        get => _resetTokenExpiresAt;
        init {
            _resetTokenExpiresAt =
                value < MinSqlDataTime ?
                    MinSqlDataTime:
                    value;
        }
    }
}