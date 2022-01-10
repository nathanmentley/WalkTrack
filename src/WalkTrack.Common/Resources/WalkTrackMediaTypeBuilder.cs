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
public sealed class WalkTrackMediaTypeBuilder
{
    private string _type = string.Empty;
    private string _subType = string.Empty;
    private string _structure = string.Empty;
    private int _version = 0;

    public WalkTrackMediaTypeBuilder WithType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentNullException(nameof(type));
        }

        _type = type;

        return this;
    }

    public WalkTrackMediaTypeBuilder WithSubType(string subType)
    {
        if (string.IsNullOrWhiteSpace(subType))
        {
            throw new ArgumentNullException(nameof(subType));
        }

        _subType = subType;

        return this;
    }

    public WalkTrackMediaTypeBuilder WithStructure(string structure)
    {
        if (string.IsNullOrWhiteSpace(structure))
        {
            throw new ArgumentNullException(nameof(structure));
        }

        _structure = structure;

        return this;
    }

    public WalkTrackMediaTypeBuilder WithVersion(int version)
    {
        if (version < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(version),
                version,
                $"{nameof(version)} is set to {version}, however it cannot be less than zero."
            );
        }

        _version = version;

        return this;
    }

    public WalkTrackMediaType Build() =>
        new WalkTrackMediaType()
        {
            Type = _type,
            SubType = _subType,
            Structure = _structure,
            Version = _version
        };
}
