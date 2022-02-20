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

using WalkTrack.Utils.DataSeeder.Loaders;

namespace WalkTrack.Utils.DataSeeder;

public class App
{
    private readonly IEnumerable<IDataLoader> _dataLoaders;

    public App(IEnumerable<IDataLoader> dataLoaders)
    {
        _dataLoaders = dataLoaders ??
            throw new ArgumentNullException(nameof(dataLoaders));
    }

    public async Task Run()
    {
        foreach(IDataLoader dataLoader in _dataLoaders)
        {
            await dataLoader.Load();
        }
    }
}