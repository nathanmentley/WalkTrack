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

using WalkTrack.Framework.Common.Resources;

namespace WalkTrack.Utils.DataSeeder.Loaders;

public abstract class BaseDataLoader<T> : IDataLoader
{
    protected abstract WalkTrackMediaType MediaType { get; }
    protected abstract string Directory { get; }

    private readonly ITranscoderProcessor _transcoderProcessor;

    public BaseDataLoader(ITranscoderProcessor transcoderProcessor)
    {
        _transcoderProcessor = transcoderProcessor ??
            throw new ArgumentNullException(nameof(transcoderProcessor));
    }

    public async Task Load(CancellationToken cancellationToken = default)
    {
        foreach(string filename in GetFiles())
        {
            T record = await ReadRecord(filename, cancellationToken);

            await LoadRecord(record, cancellationToken);
        }
    }

    protected IEnumerable<string> GetFiles() =>
        new DirectoryInfo(Directory)
            .GetFiles()
            .Where(file =>
                file.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            )
            .Select(file => file.FullName);

    protected async Task<T> ReadRecord(
        string filename,
        CancellationToken cancellationToken = default
    )
    {
        using FileStream stream = File.OpenRead(filename);

        return await _transcoderProcessor.Decode<T>(
            MediaType,
            stream,
            cancellationToken
        );
    }

    protected abstract Task LoadRecord(
        T record,
        CancellationToken cancellationToken = default
    );
}