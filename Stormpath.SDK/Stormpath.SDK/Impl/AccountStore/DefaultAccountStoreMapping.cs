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

using System;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

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

        IAccountStoreMapping IAccountStoreMapping.SetApplication(IApplication application)
        {
            if (string.IsNullOrEmpty(application?.Href))
            {
                throw new ArgumentNullException(nameof(application.Href));
            }

            this.SetLinkProperty(ApplicationPropertyName, application.Href);

            return this;
        }

        IAccountStoreMapping IAccountStoreMapping.SetAccountStore(IAccountStore accountStore)
        {
            if (string.IsNullOrEmpty(accountStore?.Href))
            {
                throw new ArgumentNullException(nameof(accountStore.Href));
            }

            this.SetLinkProperty(AccountStorePropertyName, accountStore.Href);

            return this;
        }

        IAccountStoreMapping IAccountStoreMapping.SetListIndex(int listIndex)
        {
            if (listIndex < 0)
            {
                throw new ArgumentException("Must be greater than 0.", nameof(listIndex));
            }

            this.SetProperty(ListIndexPropertyName, listIndex);

            return this;
        }

        IAccountStoreMapping IAccountStoreMapping.SetDefaultAccountStore(bool defaultAccountStore)
        {
            this.SetProperty(IsDefaultAccountStorePropertyName, defaultAccountStore);

            return this;
        }

        IAccountStoreMapping IAccountStoreMapping.SetDefaultGroupStore(bool defaultGroupStore)
        {
            this.SetProperty(IsDefaultGroupStorePropertyName, defaultGroupStore);

            return this;
        }

        async Task<IAccountStore> IAccountStoreMapping.GetAccountStoreAsync(CancellationToken cancellationToken)
        {
            var href = this.AccountStore?.Href ?? string.Empty;
            IAccountStore accountStore = null;

            if (href.Contains("directories"))
            {
                accountStore = await this.GetInternalAsyncDataStore().GetResourceAsync<IDirectory>(href, cancellationToken).ConfigureAwait(false);
            }
            else if (href.Contains("groups"))
            {
                accountStore = await this.GetInternalAsyncDataStore().GetResourceAsync<IGroup>(href, cancellationToken).ConfigureAwait(false);
            }

            return accountStore;
        }

        IAccountStore IAccountStoreMappingSync.GetAccountStore()
        {
            var href = this.AccountStore?.Href ?? string.Empty;
            IAccountStore accountStore = null;

            if (href.Contains("directories"))
            {
                accountStore = this.GetInternalSyncDataStore().GetResource<IDirectory>(href);
            }
            else if (href.Contains("groups"))
            {
                accountStore = this.GetInternalSyncDataStore().GetResource<IGroup>(href);
            }

            return accountStore;
        }

        Task<IApplication> IAccountStoreMapping.GetApplicationAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<IApplication>(this.Application.Href, cancellationToken);

        IApplication IAccountStoreMappingSync.GetApplication()
            => this.GetInternalSyncDataStore().GetResource<IApplication>(this.Application.Href);

        Task<IAccountStoreMapping> ISaveable<IAccountStoreMapping>.SaveAsync(CancellationToken cancellationToken)
            => this.SaveAsync<IAccountStoreMapping>(cancellationToken);

        IAccountStoreMapping ISaveableSync<IAccountStoreMapping>.Save()
            => this.Save<IAccountStoreMapping>();

        Task<bool> IDeletable.DeleteAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().DeleteAsync(this, cancellationToken);

        bool IDeletableSync.Delete()
            => this.GetInternalSyncDataStore().Delete(this);
    }
}
