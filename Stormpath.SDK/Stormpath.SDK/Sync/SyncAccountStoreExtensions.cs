// <copyright file="SyncAccountStoreExtensions.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.AccountStore;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Sync
{
    public static class SyncAccountStoreExtensions
    {
        /// <summary>
        /// Synchronously gets the Stormpath <see cref="ITenant"/> that owns this Account Store resource.
        /// </summary>
        /// <param name="accountStore">The account store (a <see cref="Directory.IDirectory"/> or <see cref="Group.IGroup"/>).</param>
        /// <returns>This account's tenant.</returns>
        public static ITenant GetTenant(this IAccountStore accountStore)
            => (accountStore as IAccountStoreSync).GetTenant();
    }
}
