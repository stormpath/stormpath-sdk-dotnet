// <copyright file="IGroupCreationActions.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;

namespace Stormpath.SDK.Group
{
    /// <summary>
    /// Represents common <see cref="IGroup">Group</see> creation actions.
    /// </summary>
    public interface IGroupCreationActions
    {
        /// <summary>
        /// Creates a new <see cref="IGroup">Group</see> that may be used by the <see cref="Application.IApplication">Application</see> or <see cref="Organization.IOrganization">Organization</see>.
        /// </summary>
        /// <param name="group">The Group to create.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The new <see cref="IGroup">Group</see>.</returns>
        Task<IGroup> CreateGroupAsync(IGroup group, CancellationToken cancellationToken = default(CancellationToken));
        
        /// <summary>
        /// Creates a new <see cref="IGroup">Group</see> that may be used by the <see cref="Application.IApplication">Application</see> or <see cref="Organization.IOrganization">Organization</see>.
        /// </summary>
        /// <param name="group">The Group to create.</param>
        /// <param name="creationOptions">An <see cref="IGroupCreationOptions"/> instance to use when sending the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The new <see cref="IGroup">Group</see>.</returns>
        Task<IGroup> CreateGroupAsync(IGroup group, IGroupCreationOptions creationOptions, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new enabled <see cref="IGroup">Group</see> that may be used by the <see cref="Application.IApplication">Application</see> or <see cref="Organization.IOrganization">Organization</see>.
        /// </summary>
        /// <param name="name">The new Group's name.</param>
        /// <param name="description">The new Group's description text.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The new <see cref="IGroup">Group</see>.</returns>
        Task<IGroup> CreateGroupAsync(string name, string description, CancellationToken cancellationToken = default(CancellationToken));
    }
}
