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

using System.Text.Json;
using System.Text.Json.Nodes;
using WalkTrack.Common.Exceptions;

namespace WalkTrack.Common.Resources;

/// <summary>
/// </summary>
public abstract class BaseJsonTranscoder<T>: ITranscoder
{
    public bool CanHandle(WalkTrackMediaType mediaType) =>
        GetSupportedMediaType().Equals(mediaType);

    public bool CanHandle(Type type) =>
        typeof(T) == type;

    public Task<object> Decode(Stream stream, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        JsonObject jsonObject = Parse(stream);

        if (Decode(jsonObject) is object result)
        {
            return Task.FromResult(result);
        }

        throw new UnparsableResourceException();
    }

    public Task Encode(object instance, Stream stream, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (instance is T resource)
        {
            JsonObject jsonObject = Encode(resource);

            using Utf8JsonWriter writer = new Utf8JsonWriter(stream);

            jsonObject.WriteTo(writer);
        }

        return Task.CompletedTask;
    }

    public IEnumerable<WalkTrackMediaType> GetSupportedMediaTypes() =>
        new [] {
            GetSupportedMediaType()
        };

    public abstract JsonObject Encode(T resource);

    public abstract T Decode(JsonObject jsonObject);

    protected abstract WalkTrackMediaType GetSupportedMediaType();

    protected static TProperty GetValue<TProperty>(JsonObject jsonObject, string key, TProperty defaultValue)
    {
        JsonNode? jsonNode = jsonObject[key];

        if (jsonNode is not null)
        {
            return jsonNode.GetValue<TProperty>();
        }

        return defaultValue;
    }

    private static JsonObject Parse(Stream stream) =>
        JsonNode.Parse(stream) as JsonObject ??
            throw new UnparsableResourceException();
}