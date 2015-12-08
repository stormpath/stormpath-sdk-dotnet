// <copyright file="IAccountStoreContainerSync.cs" company="Stormpath, Inc.">
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

using System;
using System.Linq;
using Stormpath.SDK.AccountStore;

namespace Stormpath.SDK.Impl.AccountStore
{
    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on <see cref="IAccountStoreContainer"/>.
    /// </summary>
    internal interface IAccountStoreContainerSync
    {
        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountStoreContainer.GetDefaultAccountStoreAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The default Account store, or <see langword="null"/>.</returns>
        IAccountStore GetDefaultAccountStore();

        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountStoreContainer.SetDefaultAccountStoreAsync(IAccountStore, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="accountStore">The <see cref="IAccountStore"/> used to persist new accounts.</param>
        void SetDefaultAccountStore(IAccountStore accountStore);

        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountStoreContainer.GetDefaultGroupStoreAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The default Group store, or <see langword="null"/>.</returns>
        IAccountStore GetDefaultGroupStore();

        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountStoreContainer.SetDefaultGroupStoreAsync(IAccountStore, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="accountStore">The <see cref="IAccountStore"/> used to persist new groups.</param>
        void SetDefaultGroupStore(IAccountStore accountStore);

        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountStoreContainer.CreateAccountStoreMappingAsync(IAccountStoreMapping, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="mapping">The new <see cref="IAccountStoreMapping"/> resource to add to the AccountStoreMapping list.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>.</returns>
        IAccountStoreMapping CreateAccountStoreMapping(IAccountStoreMapping mapping);

        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountStoreContainer.AddAccountStoreAsync(IAccountStore, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="accountStore">The new <see cref="IAccountStore"/> resource to add to the AccountStoreMapping list.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>.</returns>
        IAccountStoreMapping AddAccountStore(IAccountStore accountStore);

        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountStoreContainer.AddAccountStoreAsync(string, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="hrefOrName">Either the <c>href</c> or name of the desired <see cref="SDK.Directory.IDirectory"/> or <see cref="SDK.Group.IGroup"/>.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>.</returns>
        IAccountStoreMapping AddAccountStore(string hrefOrName);

        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountStoreContainer.AddAccountStoreAsync{T}(Func{SDK.Linq.IAsyncQueryable{T}, SDK.Linq.IAsyncQueryable{T}}, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <typeparam name="T">The type of resource (either a <see cref="SDK.Directory.IDirectory"/> or a <see cref="SDK.Group.IGroup"/>) to query for.</typeparam>
        /// <param name="query">Query to search for a resource of type <typeparamref name="T"/> in the current Tenant.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>, or <see langword="null"/> if there is no resource matching the query.</returns>
        IAccountStoreMapping AddAccountStore<T>(Func<IQueryable<T>, IQueryable<T>> query)
            where T : IAccountStore;
    }
}
