// <copyright file="IAccountStoreMappingSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.AccountStore
{
    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on <see cref="IAccountStoreMapping{T}">Account Store Mapping</see>.
    /// </summary>
    [Obsolete("This interface will be removed in 1.0. Use IApplicationAccountStoreMappingSync instead.")]
    internal interface IAccountStoreMappingSync : IAccountStoreMappingSync<IApplicationAccountStoreMapping>
    {
    }

    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on <see cref="IAccountStoreMapping{T}">Account Store Mapping</see>.
    /// </summary>
    /// <typeparam name="T">The Account Store type.</typeparam>
    internal interface IAccountStoreMappingSync<T> :
        ISaveableSync<T>,
        IDeletableSync
        where T : IAccountStoreMapping<T>
    {
        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountStoreMapping{T}.GetAccountStoreAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The Account Store.</returns>
        IAccountStore GetAccountStore();

        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountStoreMapping{T}.GetApplicationAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The Application.</returns>
        IApplication GetApplication();
    }
}
