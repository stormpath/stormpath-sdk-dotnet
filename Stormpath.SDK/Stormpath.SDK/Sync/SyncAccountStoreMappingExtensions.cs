// <copyright file="SyncAccountStoreMappingExtensions.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.AccountStore;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on <see cref="IAccountStoreMapping"/>.
    /// </summary>
    public static class SyncAccountStoreMappingExtensions
    {
        /// <summary>
        /// Synchronously gets this mapping's <see cref="IAccountStore"/> (either a <see cref="Group.IGroup"/> or <see cref="Directory.IDirectory"/>), to be assigned to the application.
        /// </summary>
        /// <param name="accountStoreMapping">The account store mapping.</param>
        /// <returns>The mapping's <see cref="IAccountStore"/>.</returns>
        public static IAccountStore GetAccountStore(this IApplicationAccountStoreMapping accountStoreMapping)
            => (accountStoreMapping as IAccountStoreMappingSync).GetAccountStore();

        /// <summary>
        /// Synchronously gets the <see cref="IApplication"/> represented by this mapping.
        /// </summary>
        /// <param name="accountStoreMapping">The account store mapping.</param>
        /// <returns>The mapping's <see cref="IApplication"/>.</returns>
        public static IApplication GetApplication(this IApplicationAccountStoreMapping accountStoreMapping)
            => (accountStoreMapping as IAccountStoreMappingSync).GetApplication();
    }
}
