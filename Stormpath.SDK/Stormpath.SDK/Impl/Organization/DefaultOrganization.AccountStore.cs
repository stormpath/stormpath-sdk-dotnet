// <copyright file="DefaultOrganization.AccountStore.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Impl.AccountStore;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Organization;

namespace Stormpath.SDK.Impl.Organization
{
    internal sealed partial class DefaultOrganization
    {
        Task<IAccountStore> IAccountStoreContainer<IOrganizationAccountStoreMapping>.GetDefaultAccountStoreAsync(CancellationToken cancellationToken)
            => AccountStoreContainerShared.GetDefaultStoreAsync(this.DefaultAccountStoreMapping.Href, this.GetInternalAsyncDataStore(), cancellationToken);

        Task<IAccountStore> IAccountStoreContainer<IOrganizationAccountStoreMapping>.GetDefaultGroupStoreAsync(CancellationToken cancellationToken)
            => AccountStoreContainerShared.GetDefaultStoreAsync(this.DefaultGroupStoreMapping.Href, this.GetInternalAsyncDataStore(), cancellationToken);

        Task IAccountStoreContainer<IOrganizationAccountStoreMapping>.SetDefaultAccountStoreAsync(IAccountStore accountStore, CancellationToken cancellationToken)
            => AccountStoreContainerShared.SetDefaultStoreAsync<IOrganization, IOrganizationAccountStoreMapping>(this, accountStore, isAccountStore: true, cancellationToken: cancellationToken);

        Task IAccountStoreContainer<IOrganizationAccountStoreMapping>.SetDefaultGroupStoreAsync(IAccountStore accountStore, CancellationToken cancellationToken)
            => AccountStoreContainerShared.SetDefaultStoreAsync<IOrganization, IOrganizationAccountStoreMapping>(this, accountStore, isAccountStore: false, cancellationToken: cancellationToken);

        Task<IOrganizationAccountStoreMapping> IAccountStoreContainer<IOrganizationAccountStoreMapping>.CreateAccountStoreMappingAsync(IOrganizationAccountStoreMapping mapping, CancellationToken cancellationToken)
            => AccountStoreContainerShared.CreateAccountStoreMappingAsync(this, this.GetInternalAsyncDataStore(), mapping, cancellationToken);

        Task<IOrganizationAccountStoreMapping> IAccountStoreContainer<IOrganizationAccountStoreMapping>.AddAccountStoreAsync(IAccountStore accountStore, CancellationToken cancellationToken)
            => AccountStoreContainerShared.AddAccountStoreAsync(this, this.GetInternalAsyncDataStore(), accountStore, cancellationToken);

        Task<IOrganizationAccountStoreMapping> IAccountStoreContainer<IOrganizationAccountStoreMapping>.AddAccountStoreAsync(string hrefOrName, CancellationToken cancellationToken)
            => AccountStoreContainerShared.AddAccountStoreAsync(this, this.GetInternalAsyncDataStore(), hrefOrName, cancellationToken);

        Task<IOrganizationAccountStoreMapping> IAccountStoreContainer<IOrganizationAccountStoreMapping>.AddAccountStoreAsync<T>(Func<IAsyncQueryable<T>, IAsyncQueryable<T>> query, CancellationToken cancellationToken)
            => AccountStoreContainerShared.AddAccountStoreAsync(this, this.GetInternalAsyncDataStore(), query, cancellationToken);
    }
}
