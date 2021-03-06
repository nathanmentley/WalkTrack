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

using WalkTrack.Framework.Common.Criteria;
using WalkTrack.Framework.Common.Resources;

namespace WalkTrack.Framework.Server;

public interface IResourceRepository<TResource>
    where TResource: IResource
{
    Task<TResource> Fetch(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TResource>> Search(IEnumerable<ICriterion> criteria, CancellationToken cancellationToken = default);
    Task<TResource> Create(TResource resource, CancellationToken cancellationToken = default);
    Task<TResource> Update(TResource resource, CancellationToken cancellationToken = default);
    Task Delete(string id, CancellationToken cancellationToken = default);
}