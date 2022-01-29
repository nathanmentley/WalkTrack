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

using System.Text.Json.Nodes;

namespace WalkTrack.Framework.Common.Json;

/// <summary>
/// </summary>
public sealed class JsonObjectBuilder
{
    private readonly IDictionary<string, JsonNode?> _data;

    public JsonObjectBuilder()
    {
        _data = new Dictionary<string, JsonNode?>();
    }

    public JsonObjectBuilder With(string key, JsonNode? value)
    {
        if (value is not null)
        {
            _data.Add(key, value);
        }

        return this;
    }

    public JsonObject Build() => new JsonObject(_data);
}