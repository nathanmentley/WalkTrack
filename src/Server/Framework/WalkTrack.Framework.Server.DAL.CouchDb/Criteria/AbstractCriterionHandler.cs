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

using System.Linq.Expressions;
using CouchDB.Driver.Types;
using WalkTrack.Framework.Common.Criteria;
using WalkTrack.Framework.Server.Exceptions;

namespace WalkTrack.Framework.Server.DAL.CouchDb.Criteria;

public abstract class AbstractCriterionHandler<TCriterion, TPersisted> : ICriterionHandler<TPersisted>
    where TCriterion: ICriterion
    where TPersisted: CouchDocument
{
    public bool CanHandle(ICriterion criterion) => criterion is TCriterion;

    public Expression<Func<TPersisted, bool>> Handle(ICriterion criterion)
    {
        if (criterion is TCriterion typedCriterion)
        {
            return Handle(typedCriterion);
        }

        throw new InvalidRequestException("TODO");
    }

    protected abstract Expression<Func<TPersisted, bool>> Handle(TCriterion criterion);
}