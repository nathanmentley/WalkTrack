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

namespace WalkTrack.Common.Resources;

/// <summary>
/// </summary>
public sealed class WalkTrackMediaType
{
    /// <summary>
    /// The key used for the structure parameterr in the media type.
    /// </summary>
    /// <value>
    /// structure
    /// </value>
    private readonly static string StructureKey =
        "structure";
    
    /// <summary>
    /// The key used for the version parameterr in the media type.
    /// </summary>
    /// <value>
    /// version
    /// </value>
    private readonly static string VersionKey =
        "version";

    /// <summary>
    /// The mediatype type
    /// </summary>
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// The mediatype sub-type
    /// </summary>
    public string SubType { get; init; } = string.Empty;

    /// <summary>
    /// The structure defined by the type
    /// </summary>
    public string Structure { get; init; } = string.Empty;

    /// <summary>
    /// The version of the structure defined by the type.
    /// </summary>
    public int Version { get; init; } = 0;

    internal WalkTrackMediaType()
    {
    }

    /// <summary>
    /// Tests if an instance is equal to this instance.
    /// </summary>
    /// <param name="obj">
    /// Instance to compare against
    /// </param>
    /// <returns>
    /// If <paramref name="obj"/> is equal true is returned.
    /// </returns>
    public override bool Equals(object? obj) =>
        obj is WalkTrackMediaType &&
        string.Equals(ToString(), obj.ToString(), StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Fetches a unique hash code for the value of this instance.
    /// </summary>
    public override int GetHashCode() =>
        ToString().ToLower().GetHashCode();

    /// <summary>
    /// Fetches a string representation of this instance.
    /// </summary>
    public override string ToString() =>
        $"{Type}/{SubType}; structure={Structure}; version={Version}";


    /// <summary>
    /// Fetches a <see cref="WalkTrackMediaType"/> to represent <paramref name="mediaType"/>.
    /// </summary>
    /// <param name="mediaType">
    /// A string mediatype
    /// </param>
    /// <returns>
    /// The <see cref="WalkTrackMediaType"/> representation
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="mediaType"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="mediaType"/> is invalid.
    /// </exception>
    public static WalkTrackMediaType Parse(string mediaType)
    {
        if (string.IsNullOrWhiteSpace(mediaType))
        {
            throw new ArgumentNullException(nameof(mediaType));
        }

        IEnumerable<string> parts = mediaType.Split(";").Select(part => part.Trim());

        IEnumerable<string> typeParts = parts.First().Split("/").Select(part => part.Trim());

        if (typeParts.Count() != 2)
        {
            throw new ArgumentException(
                "invalid media type format",
                nameof(mediaType)
            );
        }
        
        string type = typeParts.First();
        string subtype = typeParts.Last();
        string structure = string.Empty;
        int version = 0;

        foreach(string param in parts.Skip(1))
        {
            IEnumerable<string> paramParts = param.Split("=").Select(part => part.Trim());

            if (paramParts.Count() != 2)
            {
                throw new ArgumentException(
                    "invalid media type format",
                    nameof(mediaType)
                );
            }

            if (string.Equals(paramParts.First(), StructureKey, StringComparison.OrdinalIgnoreCase))
            {
                structure = paramParts.Last();
            }

            if (string.Equals(paramParts.First(), VersionKey, StringComparison.OrdinalIgnoreCase))
            {
                version = Convert.ToInt32(paramParts.Last());
            }
        }

        return new WalkTrackMediaTypeBuilder()
            .WithType(type)
            .WithSubType(subtype)
            .WithStructure(structure)
            .WithVersion(version)
            .Build();
    }
}