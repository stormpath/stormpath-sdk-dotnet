// <copyright file="DefaultOrganization.AccountStoreSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Organization;

namespace Stormpath.SDK.Impl.Organization
{
    internal sealed partial class DefaultOrganization
    {
        IAccountStore IAccountStoreContainerSync<IOrganizationAccountStoreMapping>.GetDefaultAccountStore()
            => AccountStoreContainerShared.GetDefaultStore(this.DefaultAccountStoreMapping.Href, this.GetInternalSyncDataStore());

        IAccountStore IAccountStoreContainerSync<IOrganizationAccountStoreMapping>.GetDefaultGroupStore()
            => AccountStoreContainerShared.GetDefaultStore(this.DefaultGroupStoreMapping.Href, this.GetInternalSyncDataStore());

        void IAccountStoreContainerSync<IOrganizationAccountStoreMapping>.SetDefaultAccountStore(IAccountStore accountStore)
            => AccountStoreContainerShared.SetDefaultStore<IOrganization, IOrganizationAccountStoreMapping>(this, accountStore, isAccountStore: true);

        void IAccountStoreContainerSync<IOrganizationAccountStoreMapping>.SetDefaultGroupStore(IAccountStore accountStore)
            => AccountStoreContainerShared.SetDefaultStore<IOrganization, IOrganizationAccountStoreMapping>(this, accountStore, isAccountStore: false);

        IOrganizationAccountStoreMapping IAccountStoreContainerSync<IOrganizationAccountStoreMapping>.CreateAccountStoreMapping(IOrganizationAccountStoreMapping mapping)
            => AccountStoreContainerShared.CreateAccountStoreMapping(this, this.GetInternalSyncDataStore(), mapping);

        IOrganizationAccountStoreMapping IAccountStoreContainerSync<IOrganizationAccountStoreMapping>.AddAccountStore(IAccountStore accountStore)
            => AccountStoreContainerShared.AddAccountStore(this, this.GetInternalSyncDataStore(), accountStore);

        IOrganizationAccountStoreMapping IAccountStoreContainerSync<IOrganizationAccountStoreMapping>.AddAccountStore(string hrefOrName)
            => AccountStoreContainerShared.AddAccountStore(this, this.GetInternalSyncDataStore(), hrefOrName);

        IOrganizationAccountStoreMapping IAccountStoreContainerSync<IOrganizationAccountStoreMapping>.AddAccountStore<TSource>(Func<IQueryable<TSource>, IQueryable<TSource>> query)
            => AccountStoreContainerShared.AddAccountStore(this, this.GetInternalSyncDataStore(), query);
    }
}
