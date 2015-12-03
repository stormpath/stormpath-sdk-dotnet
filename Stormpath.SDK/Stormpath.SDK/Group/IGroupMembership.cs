// <copyright file="IGroupMembership.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
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
using Stormpath.SDK.Account;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Group
{
    /// <summary>
    /// A {@code GroupMembership} represents the association of an {@link Account} and a {@link Group}.
    /// <para>
    /// Calling <see cref="IDeletable.DeleteAsync(CancellationToken)"/> on this resource will only
    /// delete the association - it will not delete the <see cref="IAccount"/> or <see cref="IGroup"/>.
    /// </para>
    /// </summary>
    public interface IGroupMembership : IResource, IDeletable
    {
        /// <summary>
        /// Gets this membership's <see cref="IAccount"/> resource.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>This membership's <see cref="IAccount"/> resource.</returns>
        Task<IAccount> GetAccountAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets this membership's <see cref="IGroup"/> resource.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>This membership's <see cref="IGroup"/> resource.</returns>
        Task<IGroup> GetGroupAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
