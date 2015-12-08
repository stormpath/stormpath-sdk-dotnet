// <copyright file="DefaultApplication.AccountStoreContainer.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Error;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.AccountStore;
using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Linq;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed partial class DefaultApplication
    {
        IAsyncQueryable<IAccountStoreMapping> IAccountStoreContainer.GetAccountStoreMappings()
            => new CollectionResourceQueryable<IAccountStoreMapping>(this.AccountStoreMappings.Href, this.GetInternalAsyncDataStore());

        Task<IAccountStore> IAccountStoreContainer.GetDefaultAccountStoreAsync(CancellationToken cancellationToken)
            => AccountStoreContainerShared.GetDefaultStoreAsync(this.DefaultAccountStoreMapping.Href, this.GetInternalAsyncDataStore(), cancellationToken);

        Task<IAccountStore> IAccountStoreContainer.GetDefaultGroupStoreAsync(CancellationToken cancellationToken)
            => AccountStoreContainerShared.GetDefaultStoreAsync(this.DefaultGroupStoreMapping.Href, this.GetInternalAsyncDataStore(), cancellationToken);

        Task IAccountStoreContainer.SetDefaultAccountStoreAsync(IAccountStore accountStore, CancellationToken cancellationToken)
            => AccountStoreContainerShared.SetDefaultStoreAsync(this, accountStore, isAccountStore: true, cancellationToken: cancellationToken);

        Task IAccountStoreContainer.SetDefaultGroupStoreAsync(IAccountStore groupStore, CancellationToken cancellationToken)
            => AccountStoreContainerShared.SetDefaultStoreAsync(this, groupStore, isAccountStore: false, cancellationToken: cancellationToken);

        Task<IAccountStoreMapping> IAccountStoreContainer.CreateAccountStoreMappingAsync(IAccountStoreMapping mapping, CancellationToken cancellationToken)
            => AccountStoreContainerShared.CreateAccountStoreMappingAsync(this, this.GetInternalAsyncDataStore(), mapping, cancellationToken);

        Task<IAccountStoreMapping> IAccountStoreContainer.AddAccountStoreAsync(IAccountStore accountStore, CancellationToken cancellationToken)
            => AccountStoreContainerShared.AddAccountStoreAsync(this, this.GetInternalAsyncDataStore(), accountStore, cancellationToken);

        Task<IAccountStoreMapping> IAccountStoreContainer.AddAccountStoreAsync(string hrefOrName, CancellationToken cancellationToken)
            => AccountStoreContainerShared.AddAccountStoreAsync(this, this.GetInternalAsyncDataStore(), hrefOrName, cancellationToken);

        Task<IAccountStoreMapping> IAccountStoreContainer.AddAccountStoreAsync<T>(Func<IAsyncQueryable<T>, IAsyncQueryable<T>> query, CancellationToken cancellationToken)
            => AccountStoreContainerShared.AddAccountStoreAsync(this, this.GetInternalAsyncDataStore(), query, cancellationToken);
    }
}
