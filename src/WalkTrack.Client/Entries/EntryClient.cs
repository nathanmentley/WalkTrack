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

using WalkTrack.Common.Entries;

namespace WalkTrack.Client.Entries;

internal sealed class EntryClient: IEntryClient
{
    public Task<Entry> Fetch(string id, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task<IEnumerable<Entry>> Fetch(CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task<Entry> Create(Entry entry, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task<Entry> Update(Entry entry, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task Delete(Entry entry, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();
}