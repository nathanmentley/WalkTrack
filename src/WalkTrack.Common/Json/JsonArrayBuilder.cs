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

namespace WalkTrack.Common.Json;

/// <summary>
/// </summary>
internal class JsonArrayBuilder
{
    private readonly IList<JsonObject> _data;

    public JsonArrayBuilder()
    {
        _data = new List<JsonObject>();
    }

    public JsonArrayBuilder With(JsonObject? jsonObject)
    {
        if (jsonObject is not null)
        {
            _data.Add(jsonObject);
        }

        return this;
    }

    public JsonArrayBuilder With<T>(IEnumerable<T> data, Func<T, JsonObject?> logic)
    {
        foreach(T datum in data)
        {
            JsonObject? jsonObject = logic(datum);

            With(jsonObject);
        }

        return this;
    }

    public JsonArray Build()
    {
        JsonArray array = new JsonArray();

        foreach(JsonObject jsonObject in _data)
        {
            array.Add(jsonObject);
        }

        return array;
    }
}