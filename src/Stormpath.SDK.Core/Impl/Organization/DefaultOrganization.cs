// <copyright file="DefaultOrganization.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
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
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.AccountStore;
using Stormpath.SDK.Impl.Group;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Organization;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Organization
{
    internal sealed partial class DefaultOrganization : AbstractExtendableInstanceResource, IOrganization, IOrganizationSync, IAccountCreationActionsInternal, IGroupCreationActionsInternal
    {
        private static readonly string AccountStoreMappingsPropertyName = "accountStoreMappings";
        private static readonly string AccountsPropertyName = "accounts";
        private static readonly string DefaultAccountStoreMappingPropertyName = AccountStoreContainerShared.DefaultAccountStoreMappingPropertyName;
        private static readonly string DefaultGroupStoreMappingPropertyName = AccountStoreContainerShared.DefaultGroupStoreMappingPropertyName;
        private static readonly string DescriptionPropertyName = "description";
        private static readonly string GroupsPropertyName = "groups";
        private static readonly string NamePropertyName = "name";
        private static readonly string NameKeyPropertyName = "nameKey";
        private static readonly string StatusPropertyName = "status";

        public DefaultOrganization(ResourceData data)
            : base(data)
        {
        }

        internal IEmbeddedProperty AccountStoreMappings => this.GetLinkProperty(AccountStoreMappingsPropertyName);

        // todo: internal after 1.0
        public IEmbeddedProperty Accounts => this.GetLinkProperty(AccountsPropertyName);

        internal IEmbeddedProperty DefaultAccountStoreMapping => this.GetLinkProperty(DefaultAccountStoreMappingPropertyName);

        internal IEmbeddedProperty DefaultGroupStoreMapping => this.GetLinkProperty(DefaultGroupStoreMappingPropertyName);

        string IOrganization.Description => this.GetStringProperty(DescriptionPropertyName);

        // todo: internal after 1.0
        public IEmbeddedProperty Groups => this.GetLinkProperty(GroupsPropertyName);

        string IOrganization.Name => this.GetStringProperty(NamePropertyName);

        string IOrganization.NameKey => this.GetStringProperty(NameKeyPropertyName);

        OrganizationStatus IOrganization.Status => GetEnumProperty<OrganizationStatus>(StatusPropertyName);

        internal IEmbeddedProperty Tenant => this.GetLinkProperty(TenantPropertyName);

        IOrganization IOrganization.SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.SetProperty(NamePropertyName, name);
            return this;
        }

        IOrganization IOrganization.SetNameKey(string nameKey)
        {
            if (string.IsNullOrEmpty(nameKey))
            {
                throw new ArgumentNullException(nameof(nameKey));
            }

            this.SetProperty(NameKeyPropertyName, nameKey);
            return this;
        }

        IOrganization IOrganization.SetStatus(OrganizationStatus status)
        {
            this.SetProperty(StatusPropertyName, status);
            return this;
        }

        IOrganization IOrganization.SetDescription(string description)
        {
            this.SetProperty(DescriptionPropertyName, description);
            return this;
        }

        Task<IOrganization> ISaveable<IOrganization>.SaveAsync(CancellationToken cancellationToken)
            => this.SaveAsync<IOrganization>(cancellationToken);

        Task<IOrganization> ISaveableWithOptions<IOrganization>.SaveAsync(Action<IRetrievalOptions<IOrganization>> options, CancellationToken cancellationToken)
            => this.SaveAsync(options, cancellationToken);

        IOrganization ISaveableSync<IOrganization>.Save()
            => this.Save<IOrganization>();

        IOrganization ISaveableWithOptionsSync<IOrganization>.Save(Action<IRetrievalOptions<IOrganization>> options)
             => this.Save(options);

        Task<bool> IDeletable.DeleteAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().DeleteAsync(this, cancellationToken);

        bool IDeletableSync.Delete()
            => this.GetInternalSyncDataStore().Delete(this);
    }
}
