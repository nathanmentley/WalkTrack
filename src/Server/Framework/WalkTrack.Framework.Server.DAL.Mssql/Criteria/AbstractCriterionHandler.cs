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

using SqlKata;
using WalkTrack.Framework.Common.Criteria;
using WalkTrack.Framework.Server.Exceptions;

namespace WalkTrack.Framework.Server.DAL.Mssql.Criteria;

public abstract class AbstractCriterionHandler<TCriterion> : ICriterionHandler
    where TCriterion: ICriterion
{
    public bool CanHandle(ICriterion criterion) => criterion is TCriterion;

    public Query Handle(ICriterion criterion, Query query)
    {
        if (criterion is TCriterion typedCriterion)
        {
            return Handle(typedCriterion, query);
        }

        throw new InvalidQueryRequestException();
    }

    protected abstract Query Handle(TCriterion criterion, Query query);
}