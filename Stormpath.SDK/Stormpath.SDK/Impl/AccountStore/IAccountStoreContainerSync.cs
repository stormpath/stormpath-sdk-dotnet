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
    /// available on <see cref="IAccountStoreContainer{T}"/>.
    /// </summary>
    /// <typeparam name="T">The Account Store Mapping type.</typeparam>
    internal interface IAccountStoreContainerSync<T>
        where T : IAccountStoreMapping<T>
    {
        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountStoreContainer{T}.GetDefaultAccountStoreAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The default Account store, or <see langword="null"/>.</returns>
        IAccountStore GetDefaultAccountStore();

        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountStoreContainer{T}.SetDefaultAccountStoreAsync(IAccountStore, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="accountStore">The <see cref="IAccountStore"/> used to persist new accounts.</param>
        void SetDefaultAccountStore(IAccountStore accountStore);

        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountStoreContainer{T}.GetDefaultGroupStoreAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The default Group store, or <see langword="null"/>.</returns>
        IAccountStore GetDefaultGroupStore();

        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountStoreContainer{T}.SetDefaultGroupStoreAsync(IAccountStore, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="accountStore">The <see cref="IAccountStore"/> used to persist new groups.</param>
        void SetDefaultGroupStore(IAccountStore accountStore);

        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountStoreContainer{T}.CreateAccountStoreMappingAsync(T, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="mapping">The new <see cref="IAccountStoreMapping"/> resource to add to the AccountStoreMapping list.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>.</returns>
        T CreateAccountStoreMapping(T mapping);

        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountStoreContainer{T}.AddAccountStoreAsync(IAccountStore, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="accountStore">The new <see cref="IAccountStore"/> resource to add to the AccountStoreMapping list.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>.</returns>
        T AddAccountStore(IAccountStore accountStore);

        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountStoreContainer{T}.AddAccountStoreAsync(string, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="hrefOrName">Either the <c>href</c> or name of the desired <see cref="SDK.Directory.IDirectory"/> or <see cref="SDK.Group.IGroup"/>.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>.</returns>
        T AddAccountStore(string hrefOrName);

        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountStoreContainer{T}.AddAccountStoreAsync{T}(Func{SDK.Linq.IAsyncQueryable{T}, SDK.Linq.IAsyncQueryable{T}}, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of resource (either a <see cref="SDK.Directory.IDirectory"/> or a <see cref="SDK.Group.IGroup"/>) to query for.</typeparam>
        /// <param name="query">Query to search for a resource of type <typeparamref name="TSource"/> in the current Tenant.</param>
        /// <returns>The newly-created <see cref="IAccountStoreMapping"/>, or <see langword="null"/> if there is no resource matching the query.</returns>
        T AddAccountStore<TSource>(Func<IQueryable<TSource>, IQueryable<TSource>> query)
            where TSource : IAccountStore;
    }
}
