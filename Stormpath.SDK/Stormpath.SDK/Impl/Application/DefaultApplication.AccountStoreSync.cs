// <copyright file="DefaultApplication.AccountStoreSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.AccountStore;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed partial class DefaultApplication
    {
        IAccountStore IAccountStoreContainerSync.GetDefaultAccountStore()
            => AccountStoreContainerShared.GetDefaultAccountStore(this.DefaultAccountStoreMapping.Href, this.GetInternalSyncDataStore());

        IAccountStore IAccountStoreContainerSync.GetDefaultGroupStore()
            => AccountStoreContainerShared.GetDefaultAccountStore(this.DefaultGroupStoreMapping.Href, this.GetInternalSyncDataStore());

        void IAccountStoreContainerSync.SetDefaultAccountStore(IAccountStore accountStore)
            => AccountStoreContainerShared.SetDefaultStore(this, accountStore, isAccountStore: true);

        void IAccountStoreContainerSync.SetDefaultGroupStore(IAccountStore accountStore)
            => AccountStoreContainerShared.SetDefaultStore(this, accountStore, isAccountStore: false);

        IAccountStoreMapping IAccountStoreContainerSync.CreateAccountStoreMapping(IAccountStoreMapping mapping)
            => AccountStoreContainerShared.CreateAccountStoreMapping(this, this.GetInternalSyncDataStore(), mapping);

        IAccountStoreMapping IAccountStoreContainerSync.AddAccountStore(IAccountStore accountStore)
            => AccountStoreContainerShared.AddAccountStore(this, this.GetInternalSyncDataStore(), accountStore);

        IAccountStoreMapping IAccountStoreContainerSync.AddAccountStore(string hrefOrName)
            => AccountStoreContainerShared.AddAccountStore(this, this.GetInternalSyncDataStore(), hrefOrName);

        IAccountStoreMapping IAccountStoreContainerSync.AddAccountStore<T>(Func<IQueryable<T>, IQueryable<T>> query)
            => AccountStoreContainerShared.AddAccountStore(this, this.GetInternalSyncDataStore(), query);
    }
}
