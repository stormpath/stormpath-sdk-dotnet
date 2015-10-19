// <copyright file="DefaultDirectory.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.Provider;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Directory
{
    internal sealed class DefaultDirectory : AbstractExtendableInstanceResource, IDirectory, IDirectorySync
    {
        private static readonly string AccountCreationPolicyPropertyName = "accountCreationPolicy";
        private static readonly string AccountsPropertyName = "accounts";
        private static readonly string ApplicationMappingsPropertyName = "applicationMappings";
        private static readonly string ApplicationsPropertyName = "applications";
        private static readonly string DescriptionPropertyName = "description";
        private static readonly string GroupsPropertyName = "groups";
        private static readonly string NamePropertyName = "name";
        private static readonly string PasswordPolicyPropertyName = "passwordPolicy";
        private static readonly string ProviderPropertyName = "provider";
        private static readonly string StatusPropertyName = "status";
        private static readonly string TenantPropertyName = "tenant";

        public DefaultDirectory(ResourceData data)
            : base(data)
        {
        }

        internal LinkProperty AccountCreationPolicy => this.GetLinkProperty(AccountCreationPolicyPropertyName);

        internal LinkProperty Accounts => this.GetLinkProperty(AccountsPropertyName);

        internal LinkProperty ApplicationMappings => this.GetLinkProperty(ApplicationMappingsPropertyName);

        internal LinkProperty Applications => this.GetLinkProperty(ApplicationsPropertyName);

        string IDirectory.Description => this.GetProperty<string>(DescriptionPropertyName);

        internal LinkProperty Groups => this.GetLinkProperty(GroupsPropertyName);

        string IDirectory.Name => this.GetProperty<string>(NamePropertyName);

        internal LinkProperty PasswordPolicy => this.GetLinkProperty(PasswordPolicyPropertyName);

        internal LinkProperty Provider => this.GetLinkProperty(ProviderPropertyName);

        DirectoryStatus IDirectory.Status => this.GetProperty<DirectoryStatus>(StatusPropertyName);

        internal LinkProperty Tenant => this.GetLinkProperty(TenantPropertyName);

        IDirectory IDirectory.SetDescription(string description)
        {
            this.SetProperty(DescriptionPropertyName, description);
            return this;
        }

        IDirectory IDirectory.SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            this.SetProperty(NamePropertyName, name);
            return this;
        }

        IDirectory IDirectory.SetStatus(DirectoryStatus status)
        {
            this.SetProperty(StatusPropertyName, status);
            return this;
        }

        internal IDirectory SetProvider(IProvider provider)
        {
            if (!string.IsNullOrEmpty(this.AsInterface.Href))
                throw new ApplicationException("Cannot change the provider of an existing Directory");

            this.SetProperty(ProviderPropertyName, provider);
            return this;
        }

        Task<IAccount> IAccountCreationActions.CreateAccountAsync(IAccount account, Action<AccountCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken)
            => AccountCreationActionsShared.CreateAccountAsync(this.GetInternalAsyncDataStore(), this.Accounts.Href, account, creationOptionsAction, cancellationToken);

        IAccount IAccountCreationActionsSync.CreateAccount(IAccount account, Action<AccountCreationOptionsBuilder> creationOptionsAction)
            => AccountCreationActionsShared.CreateAccount(this.GetInternalSyncDataStore(), this.Accounts.Href, account, creationOptionsAction);

        Task<IAccount> IAccountCreationActions.CreateAccountAsync(IAccount account, IAccountCreationOptions creationOptions, CancellationToken cancellationToken)
             => AccountCreationActionsShared.CreateAccountAsync(this.GetInternalAsyncDataStore(), this.Accounts.Href, account, creationOptions, cancellationToken);

        IAccount IAccountCreationActionsSync.CreateAccount(IAccount account, IAccountCreationOptions creationOptions)
             => AccountCreationActionsShared.CreateAccount(this.GetInternalSyncDataStore(), this.Accounts.Href, account, creationOptions);

        Task<IAccount> IAccountCreationActions.CreateAccountAsync(IAccount account, CancellationToken cancellationToken)
             => AccountCreationActionsShared.CreateAccountAsync(this.GetInternalAsyncDataStore(), this.Accounts.Href, account, cancellationToken);

        IAccount IAccountCreationActionsSync.CreateAccount(IAccount account)
             => AccountCreationActionsShared.CreateAccount(this.GetInternalSyncDataStore(), this.Accounts.Href, account);

        Task<IAccount> IAccountCreationActions.CreateAccountAsync(string givenName, string surname, string email, string password, object customData, CancellationToken cancellationToken)
            => AccountCreationActionsShared.CreateAccountAsync(this.GetInternalAsyncDataStore(), this.Accounts.Href, givenName, surname, email, password, customData, cancellationToken);

        Task<IAccount> IAccountCreationActions.CreateAccountAsync(string givenName, string surname, string email, string password, CancellationToken cancellationToken)
            => AccountCreationActionsShared.CreateAccountAsync(this.GetInternalAsyncDataStore(), this.Accounts.Href, givenName, surname, email, password, null, cancellationToken);

        IAccount IAccountCreationActionsSync.CreateAccount(string givenName, string surname, string email, string password, object customData)
            => AccountCreationActionsShared.CreateAccount(this.GetInternalSyncDataStore(), this.Accounts.Href, givenName, surname, email, password, customData);

        Task<IGroup> IDirectory.CreateGroupAsync(IGroup group, CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().CreateAsync(this.Groups.Href, group, cancellationToken);

        IGroup IDirectorySync.CreateGroup(IGroup group)
            => this.GetInternalSyncDataStore().Create(this.Groups.Href, group);

        Task<IProvider> IDirectory.GetProviderAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<IProvider>(this.Provider.Href, ProviderTypeConverter.TypeLookup, cancellationToken);

        IProvider IDirectorySync.GetProvider()
            => this.GetInternalSyncDataStore().GetResource<IProvider>(this.Provider.Href, ProviderTypeConverter.TypeLookup);

        Task<ITenant> IDirectory.GetTenantAsync(CancellationToken cancellationToken)
            => this.GetTenantAsync(this.Tenant.Href, cancellationToken);

        ITenant IDirectorySync.GetTenant()
            => this.GetTenant(this.Tenant.Href);

        Task<bool> IDeletable.DeleteAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().DeleteAsync(this, cancellationToken);

        bool IDeletableSync.Delete()
            => this.GetInternalSyncDataStore().Delete(this);

        Task<IDirectory> ISaveable<IDirectory>.SaveAsync(CancellationToken cancellationToken)
            => this.SaveAsync<IDirectory>(cancellationToken);

        IDirectory ISaveableSync<IDirectory>.Save()
            => this.Save<IDirectory>();

        IAsyncQueryable<IAccount> IDirectory.GetAccounts()
            => new CollectionResourceQueryable<IAccount>(this.Accounts.Href, this.GetInternalAsyncDataStore());

        IAsyncQueryable<IGroup> IDirectory.GetGroups()
            => new CollectionResourceQueryable<IGroup>(this.Groups.Href, this.GetInternalAsyncDataStore());
    }
}
