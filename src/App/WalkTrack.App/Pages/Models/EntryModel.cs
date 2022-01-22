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

using WalkTrack.EntryService.Common;

namespace WalkTrack.App.Pages.Models;

public sealed class EntryModel
{
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public decimal Distance { get; set; } = 0;

    public bool Collapsed { get; set; } = true;
    public bool Loading { get; set; } = false;

    public IEnumerable<Entry> Entries { get; private set; } = Enumerable.Empty<Entry>();

    public void SetEntries(IEnumerable<Entry> entries)
    {
        Entries = entries;
    }

    public Entry ToEntry() =>
        new Entry()
        {
            Date = Date,
            Distance = Distance
        };
}
