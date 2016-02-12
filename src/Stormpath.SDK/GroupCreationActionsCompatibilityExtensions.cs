// <copyright file="GroupCreationActionsCompatibilityExtensions.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Group;

namespace Stormpath.SDK
{
    public static class GroupCreationActionsCompatibilityExtensions
    {
        /// <summary>
        /// Creates a new <see cref="IGroup">Group</see> that may be used by the <see cref="Application.IApplication">Application</see> or <see cref="Organization.IOrganization">Organization</see>.
        /// </summary>
        /// <param name="group">The group to create.</param>
        /// <param name="creationOptionsAction">
        /// An inline builder for an instance of <see cref="IGroupCreationOptions"/>, which will be used when sending the request.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The new <see cref="IGroup">Group</see>.</returns>
        public static Task<IGroup> CreateGroupAsync(this IGroupCreationActions groupCreationActions, IGroup group, Action<GroupCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken = default(CancellationToken))
        {
            var @this = groupCreationActions as IGroupCreationActionsInternal;

            return GroupCreationActionsShared.CreateGroupAsync(@this.GetInternalAsyncDataStore(), @this.Groups.Href, group, creationOptionsAction, cancellationToken);
        }
    }
}
