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

using WalkTrack.Framework.Client.Exceptions;
using WalkTrack.Framework.Common.ApiErrorResponses;

namespace WalkTrack.Framework.Client;

public class ResourceNotFoundErrorHandler: IErrorHandler
{
    public bool CanHandle(ApiErrorResponse apiError) =>
        apiError.StatusCode == 404;

    public Exception Handle(ApiErrorResponse apiError) =>
        new ResourceNotFoundClientException();
}