// <copyright file="DefaultAccountStoreMapping.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.AccountStore
{
    internal sealed class DefaultAccountStoreMapping : AbstractInstanceResource, IAccountStoreMapping, IAccountStoreMappingSync
    {
        private static readonly string AccountStorePropertyName = "accountStore";
        private static readonly string ApplicationPropertyName = "application";
        private static readonly string IsDefaultAccountStorePropertyName = "isDefaultAccountStore";
        private static readonly string IsDefaultGroupStorePropertyName = "isDefaultGroupStore";
        private static readonly string ListIndexPropertyName = "listIndex";

        public DefaultAccountStoreMapping(ResourceData data)
            : base(data)
        {
        }

        internal IEmbeddedProperty AccountStore => this.GetLinkProperty(AccountStorePropertyName);

        internal IEmbeddedProperty Application => this.GetLinkProperty(ApplicationPropertyName);

        bool IAccountStoreMapping.IsDefaultAccountStore => this.GetProperty<bool>(IsDefaultAccountStorePropertyName);

        bool IAccountStoreMapping.IsDefaultGroupStore => this.GetProperty<bool>(IsDefaultGroupStorePropertyName);

        int IAccountStoreMapping.ListIndex => this.GetProperty<int>(ListIndexPropertyName);

        Task<IAccountStore> IAccountStoreMapping.GetAccountStoreAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<IAccountStore>(this.AccountStore.Href, cancellationToken);

        IAccountStore IAccountStoreMappingSync.GetAccountStore()
            => this.GetInternalSyncDataStore().GetResource<IAccountStore>(this.AccountStore.Href);

        Task<IApplication> IAccountStoreMapping.GetApplicationAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<IApplication>(this.Application.Href, cancellationToken);

        IApplication IAccountStoreMappingSync.GetApplication()
            => this.GetInternalSyncDataStore().GetResource<IApplication>(this.Application.Href);
    }
}
