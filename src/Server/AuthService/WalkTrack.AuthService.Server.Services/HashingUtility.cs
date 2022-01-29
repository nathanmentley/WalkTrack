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

using System.Security.Cryptography;
using System.Text;

namespace WalkTrack.AuthService.Server.Services;

/// <summary>
/// </summary>
internal sealed class HashingUtility: IHashingUtility
{
    /// <summary>
    /// </summary>
    public string Hash(string value, string salt) =>
        CalculateHash($"{value}{salt}");

    private static string CalculateHash(string input)  
    {  
        using SHA256 sha256Hash = SHA256.Create();

        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
 
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < bytes.Length; i++)
        {  
            builder.Append(bytes[i].ToString("x2"));
        }

        return builder.ToString();
    } 
}