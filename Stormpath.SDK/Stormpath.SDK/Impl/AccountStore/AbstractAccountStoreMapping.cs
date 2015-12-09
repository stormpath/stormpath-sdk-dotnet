// <copyright file="AbstractAccountStoreMapping.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Organization;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.AccountStore
{
    internal abstract class AbstractAccountStoreMapping<T> :
        AbstractInstanceResource,
        IAccountStoreMapping<T>,
        IAccountStoreMappingSync<T>,
        ISaveable<T>,
        ISaveableSync<T>,
        IDeletable,
        IDeletableSync
        where T : class, IAccountStoreMapping<T>
    {
        private static readonly string AccountStorePropertyName = "accountStore";
        private static readonly string IsDefaultAccountStorePropertyName = "isDefaultAccountStore";
        private static readonly string IsDefaultGroupStorePropertyName = "isDefaultGroupStore";
        private static readonly string ListIndexPropertyName = "listIndex";

        public AbstractAccountStoreMapping(ResourceData data)
            : base(data)
        {
        }

        internal IEmbeddedProperty AccountStore => this.GetLinkProperty(AccountStorePropertyName);

        bool IAccountStoreMapping<T>.IsDefaultAccountStore => this.GetProperty<bool>(IsDefaultAccountStorePropertyName);

        bool IAccountStoreMapping<T>.IsDefaultGroupStore => this.GetProperty<bool>(IsDefaultGroupStorePropertyName);

        int IAccountStoreMapping<T>.ListIndex => this.GetProperty<int>(ListIndexPropertyName);

        T IAccountStoreMapping<T>.SetAccountStore(IAccountStore accountStore)
        {
            if (string.IsNullOrEmpty(accountStore?.Href))
            {
                throw new ArgumentNullException(nameof(accountStore.Href));
            }

            this.SetLinkProperty(AccountStorePropertyName, accountStore.Href);

            return this as T;
        }

        T IAccountStoreMapping<T>.SetListIndex(int listIndex)
        {
            if (listIndex < 0)
            {
                throw new ArgumentException("Must be greater than 0.", nameof(listIndex));
            }

            this.SetProperty(ListIndexPropertyName, listIndex);

            return this as T;
        }

        T IAccountStoreMapping<T>.SetDefaultAccountStore(bool defaultAccountStore)
        {
            this.SetProperty(IsDefaultAccountStorePropertyName, defaultAccountStore);

            return this as T;
        }

        T IAccountStoreMapping<T>.SetDefaultGroupStore(bool defaultGroupStore)
        {
            this.SetProperty(IsDefaultGroupStorePropertyName, defaultGroupStore);

            return this as T;
        }

        async Task<IAccountStore> IAccountStoreMapping<T>.GetAccountStoreAsync(CancellationToken cancellationToken)
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
            else if (href.Contains("organizations"))
            {
                accountStore = await this.GetInternalAsyncDataStore().GetResourceAsync<IOrganization>(href, cancellationToken).ConfigureAwait(false);
            }

            return accountStore;
        }

        IAccountStore IAccountStoreMappingSync<T>.GetAccountStore()
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
            else if (href.Contains("organizations"))
            {
                accountStore = this.GetInternalSyncDataStore().GetResource<IOrganization>(href);
            }

            return accountStore;
        }

        Task<T> ISaveable<T>.SaveAsync(CancellationToken cancellationToken)
            => this.SaveAsync<T>(cancellationToken);

        T ISaveableSync<T>.Save()
            => this.Save<T>();

        Task<bool> IDeletable.DeleteAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().DeleteAsync(this, cancellationToken);

        bool IDeletableSync.Delete()
            => this.GetInternalSyncDataStore().Delete(this);

        // TODO These methods will be moved out of this class when we do a breaking version change
        public abstract T SetApplication(IApplication application);

        public abstract Task<IApplication> GetApplicationAsync(CancellationToken cancellationToken = default(CancellationToken));

        public abstract IApplication GetApplication();
    }
}
