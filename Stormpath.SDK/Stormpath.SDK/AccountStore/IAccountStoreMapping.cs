// <copyright file="IAccountStoreMapping.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Application;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.AccountStore
{
    /// <summary>
    /// Represents the assignment of an <see cref="IAccountStore"/> AccountStore (either a <see cref="Group.IGroup"/> or <see cref="Directory.IDirectory"/>) to an <see cref="IApplication"/>.
    /// <para>When an <see cref="IAccountStoreMapping"/> is created, the accounts in the account store are granted access to become users of the linked <see cref="IApplication"/>. The order in which AccountStores are assigned to an application determines how login attempts work in Stormpath.</para>
    /// </summary>
    public interface IAccountStoreMapping : IResource, ISaveable<IAccountStoreMapping>, IDeletable
    {
        /// <summary>
        /// Sets the <see cref="IApplication"/> represented by this <see cref="IAccountStoreMapping"/> resource.
        /// </summary>
        /// <param name="application">The <see cref="IApplication"/> represented by this <see cref="IAccountStoreMapping"/> resource.</param>
        /// <returns>This instance for method chaining.</returns>
        IAccountStoreMapping SetApplication(IApplication application);

        /// <summary>
        /// Sets this mapping's <see cref="IAccountStore"/> (either a <see cref="Group.IGroup"/> or <see cref="Directory.IDirectory"/>),
        /// to be assigned to the application.
        /// </summary>
        /// <param name="accountStore">The <see cref="IAccountStore"/> to be assigned to the application.</param>
        /// <returns>This instance for method chaining.</returns>
        IAccountStoreMapping SetAccountStore(IAccountStore accountStore);

        /// <summary>
        /// Updates the zero-based order in which the associated <see cref="IAccountStore"/> will be consulted
        /// by the linked <see cref="IApplication"/> during an account authentication attempt.
        /// <para>
        /// If you use this setter, you will invalidate the cache for all of the associated Application's
        /// other AccountStoreMappings.
        /// </para>
        /// </summary>
        /// <param name="listIndex">
        /// If <c>0</c> is passed, the account store mapping will be the first item in the list.
        /// If <c>1</c> or greater is passed, the mapping will be inserted at that index. Because list indices
        /// are zero-based, the account store will be in the list at position <c><paramref name="listIndex"/> - 1</c>.
        /// If a negative number is passed, an <see cref="System.ArgumentException"/> will be thrown.
        /// </param>
        /// <returns>This instance for method chaining.</returns>
        IAccountStoreMapping SetListIndex(int listIndex);

        /// <summary>
        /// Sets whether or not the associated <see cref="IAccountStore"/> is designated as the Application's
        /// default account store.
        /// <para>
        /// A <see langword="true"/> value indicates that any accounts created directly by the application will be dispatched
        /// to and saved in the associated <see cref="IAccountStore"/>, since an application cannot store accounts directly.
        /// </para>
        /// <para>
        /// If you use this setter, you will invalidate the cache for all of the associated Application's
        /// other AccountStoreMappings.
        /// </para>
        /// </summary>
        /// <param name="defaultAccountStore">Whether or not the associated <see cref="IAccountStore"/> is designated as the Application's default account store.</param>
        /// <returns>This instance for method chaining.</returns>
        IAccountStoreMapping SetDefaultAccountStore(bool defaultAccountStore);

        /// <summary>
        /// Sets whether or not the associated <see cref="IAccountStore"/> is designated as the Application's
        /// default group store.
        /// <para>
        /// A <see langword="true"/> value indicates that any groups created directly by the application will be dispatched
        /// to and saved in the associated <see cref="IAccountStore"/>, since an application cannot store groups directly.
        /// </para>
        /// <para>
        /// If you use this setter, you will invalidate the cache for all of the associated Application's
        /// other AccountStoreMappings.
        /// </para>
        /// </summary>
        /// <param name="defaultGroupStore">Whether or not the associated <see cref="IAccountStore"/> is designated as the Application's default group store.</param>
        /// <returns>This instance for method chaining.</returns>
        IAccountStoreMapping SetDefaultGroupStore(bool defaultGroupStore);

        /// <summary>
        /// Gets a value indicating whether the associated <see cref="IAccountStore"/> is designated as the application's default account store.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the associated <see cref="IAccountStore"/> is designated as the application's default account store; <see langword="false"/> otherwise.
        /// <para>A <see langword="true"/> value indicates that any accounts created directly by the application will be dispatched to and saved in the associated <see cref="IAccountStore"/>, since an <see cref="IApplication"/> cannot store accounts directly.</para>
        /// </value>
        bool IsDefaultAccountStore { get; }

        /// <summary>
        /// Gets a value indicating whether the associated <see cref="IAccountStore"/> is designated as the application's default group store.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the associated <see cref="IAccountStore"/> is designated as the application's default group store; <see langword="false"/> otherwise.
        /// <para>A <see langword="true"/> value indicates that any groups created directly by the application will be dispatched to and saved in the associated <see cref="IAccountStore"/>, since an <see cref="IApplication"/> cannot store accounts directly.</para>
        /// </value>
        bool IsDefaultGroupStore { get; }

        /// <summary>
        /// Gets the zero-based order in which the associated <see cref="IAccountStore"/> will be consulted
        /// by the linked <see cref="IApplication"/> during an account authentication attempt.
        /// </summary>
        /// <value>
        /// The lower the index, the higher precedence (the earlier it will be accessed) during an authentication attempt.
        /// The higher the index, the lower the precedence (the later it will be accessed) during an authentication attempt.
        /// </value>
        int ListIndex { get; }

        /// <summary>
        /// Gets this mapping's <see cref="IAccountStore"/> (either a <see cref="Group.IGroup"/> or <see cref="Directory.IDirectory"/>), to be assigned to the application.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The mapping's <see cref="IAccountStore"/>.</returns>
        Task<IAccountStore> GetAccountStoreAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the <see cref="IApplication"/> represented by this mapping.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The mapping's <see cref="IApplication"/>.</returns>
        Task<IApplication> GetApplicationAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
