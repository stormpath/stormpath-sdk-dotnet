// <copyright file="IAccountStoreMapping.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Application;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.AccountStore
{
    /// <summary>
    /// Represents the assignment of an <see cref="IAccountStore"/> AccountStore (either a Group or <see cref="Directory.IDirectory"/>) to an <see cref="IApplication"/>.
    /// <para>When an <see cref="IAccountStoreMapping"/> is created, the accounts in the account store are granted access to become users of the linked <see cref="IApplication"/>. The order in which AccountStores are assigned to an application determines how login attempts work in Stormpath.</para>
    /// </summary>
    public interface IAccountStoreMapping : IResource
    {
        /// <summary>
        /// True if the associated <see cref="IAccountStore"/> is designated as the application's default account store, false otherwise.
        /// <para>A <c>true</c> value indicates that any accounts created directly by the application will be dispatched to and saved in the associated <see cref="IAccountStore"/>, since an <see cref="IApplication"/> cannot store accounts directly.</para>
        /// </summary>
        bool IsDefaultAccountStore { get; }

        /// <summary>
        /// True if the associated <see cref="IAccountStore"/> is designated as the application's default group store, false otherwise.
        /// <para>A <c>true</c> value indicates that any groups created directly by the application will be dispatched to and saved in the associated <see cref="IAccountStore"/>, since an <see cref="IApplication"/> cannot store accounts directly.</para>
        /// </summary>
        bool IsDefaultGroupStore { get; }

        /// <summary>
        /// The zero-based order in which the associated <see cref="IAccountStore"/> will be consulted by the linked <see cref="IApplication"/> during an account authentication attempt.
        /// <para>The lower the index, the higher precedence (the earlier it will be accessed) during an authentication attempt. The higher the index, the lower the precedence (the later it will be accessed) during an authentication attempt.</para>
        /// </summary>
        int ListIndex { get; }

        /// <summary>
        /// Gets this mapping's <see cref="IAccountStore"/> (either a Group or <see cref="Directory.IDirectory"/>), to be assigned to the application.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the mapping's <see cref="IAccountStore"/>.</returns>
        Task<IAccountStore> GetAccountStoreAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the <see cref="IApplication"/> represented by this mapping.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the mapping's <see cref="IApplication"/>.</returns>
        Task<IApplication> GetApplicationAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
