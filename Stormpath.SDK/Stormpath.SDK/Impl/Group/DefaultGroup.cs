// <copyright file="DefaultGroup.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Error;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Group
{
    internal sealed class DefaultGroup : AbstractExtendableInstanceResource, IGroup, IGroupSync
    {
        private static readonly string NamePropertyName = "name";
        private static readonly string DescriptionPropertyName = "description";
        private static readonly string StatusPropertyName = "status";
        private static readonly string DirectoryPropertyName = "directory";
        private static readonly string TenantPropertyName = "tenant";
        private static readonly string AccountsPropertyName = "accounts";
        private static readonly string AccountMembershipsPropertyName = "accountMemberships";

        public DefaultGroup(ResourceData data)
            : base(data)
        {
        }

        private new IGroup AsInterface => this;

        string IGroup.Name => this.GetProperty<string>(NamePropertyName);

        string IGroup.Description => this.GetProperty<string>(DescriptionPropertyName);

        GroupStatus IGroup.Status => this.GetProperty<GroupStatus>(StatusPropertyName);

        internal IEmbeddedProperty Directory => this.GetLinkProperty(DirectoryPropertyName);

        internal IEmbeddedProperty Tenant => this.GetLinkProperty(TenantPropertyName);

        internal IEmbeddedProperty Accounts => this.GetLinkProperty(AccountsPropertyName);

        internal IEmbeddedProperty AccountMemberships => this.GetLinkProperty(AccountMembershipsPropertyName);

        IGroup IGroup.SetDescription(string description)
        {
            this.SetProperty(DescriptionPropertyName, description);
            return this;
        }

        IGroup IGroup.SetName(string name)
        {
            this.SetProperty(NamePropertyName, name);
            return this;
        }

        IGroup IGroup.SetStatus(GroupStatus status)
        {
            this.SetProperty(StatusPropertyName, status);
            return this;
        }

        private async Task<IAccount> FindAccountAsync(string hrefOrEmailOrUsername, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(hrefOrEmailOrUsername))
                throw new ArgumentNullException(nameof(hrefOrEmailOrUsername));

            IAccount account = null;

            bool looksLikeHref = hrefOrEmailOrUsername.Split('/').Length > 4;
            if (looksLikeHref)
            {
                try
                {
                    account = await this.GetInternalAsyncDataStore().GetResourceAsync<IAccount>(hrefOrEmailOrUsername).ConfigureAwait(false);

                    if ((account as DefaultAccount)?.Directory.Href == this.Directory.Href)
                        return account;
                }
                catch (ResourceException)
                {
                    // It looked like an href, but no group was found.
                    // We'll try looking it up by name.
                }
            }

            var directory = await this.AsInterface
                .GetDirectoryAsync(cancellationToken)
                .ConfigureAwait(false);

            account = await directory
                    .GetAccounts()
                    .Where(x => x.Email == hrefOrEmailOrUsername)
                    .FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);

            if (account == null)
            {
                account = await directory
                .GetAccounts()
                .Where(x => x.Username == hrefOrEmailOrUsername)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
            }

            return account; // or null
        }

        private IAccount FindAccount(string hrefOrEmailOrUsername)
        {
            if (string.IsNullOrEmpty(hrefOrEmailOrUsername))
                throw new ArgumentNullException(nameof(hrefOrEmailOrUsername));

            IAccount account = null;

            bool looksLikeHref = hrefOrEmailOrUsername.Split('/').Length > 4;
            if (looksLikeHref)
            {
                try
                {
                    account = this.GetInternalSyncDataStore().GetResource<IAccount>(hrefOrEmailOrUsername);

                    if ((account as DefaultAccount)?.Directory.Href == this.Directory.Href)
                        return account;
                }
                catch (ResourceException)
                {
                    // It looked like an href, but no group was found.
                    // We'll try looking it up by name.
                }
            }

            var directory = this.AsInterface.GetDirectory();

            account = directory
                    .GetAccounts()
                    .Synchronously()
                    .Where(x => x.Email == hrefOrEmailOrUsername)
                    .FirstOrDefault();

            if (account == null)
            {
                account = directory
                .GetAccounts()
                .Synchronously()
                .Where(x => x.Username == hrefOrEmailOrUsername)
                .FirstOrDefault();
            }

            return account; // or null
        }

        Task<IGroupMembership> IGroup.AddAccountAsync(IAccount account, CancellationToken cancellationToken)
            => DefaultGroupMembership.CreateAsync(account, this, this.GetInternalAsyncDataStore(), cancellationToken);

        IGroupMembership IGroupSync.AddAccount(IAccount account)
            => DefaultGroupMembership.Create(account, this, this.GetInternalSyncDataStore());

        async Task<IGroupMembership> IGroup.AddAccountAsync(string hrefOrEmailOrUsername, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(hrefOrEmailOrUsername))
                throw new ArgumentNullException(nameof(hrefOrEmailOrUsername));

            var account = await this.FindAccountAsync(hrefOrEmailOrUsername, cancellationToken).ConfigureAwait(false);
            if (account == null)
                throw new InvalidOperationException($"No matching account for {nameof(hrefOrEmailOrUsername)} '{hrefOrEmailOrUsername}' found.");

            return await DefaultGroupMembership.CreateAsync(account, this, this.GetInternalAsyncDataStore(), cancellationToken).ConfigureAwait(false);
        }

        IGroupMembership IGroupSync.AddAccount(string hrefOrEmailOrUsername)
        {
            if (string.IsNullOrEmpty(hrefOrEmailOrUsername))
                throw new ArgumentNullException(nameof(hrefOrEmailOrUsername));

            var account = this.FindAccount(hrefOrEmailOrUsername);
            if (account == null)
                throw new InvalidOperationException($"No matching account for {nameof(hrefOrEmailOrUsername)} '{hrefOrEmailOrUsername}' found.");

            return DefaultGroupMembership.Create(account, this, this.GetInternalSyncDataStore());
        }

        async Task<bool> IGroup.RemoveAccountAsync(IAccount account, CancellationToken cancellationToken)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            IGroupMembership foundMembership = null;
            await this.AsInterface.GetAccountMemberships().ForEachAsync(
                item =>
            {
                if ((item as IInternalGroupMembership).AccountHref.Equals(account.Href, StringComparison.InvariantCultureIgnoreCase))
                    foundMembership = item;

                return foundMembership != null;
            }, cancellationToken).ConfigureAwait(false);

            if (foundMembership == null)
                throw new InvalidOperationException("The specified account does not belong to this group.");

            return await foundMembership.DeleteAsync(cancellationToken).ConfigureAwait(false);
        }

        bool IGroupSync.RemoveAccount(IAccount account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            IGroupMembership foundMembership = null;
            foreach (var item in this.AsInterface.GetAccountMemberships().Synchronously())
            {
                if ((item as IInternalGroupMembership).AccountHref.Equals(account.Href, StringComparison.InvariantCultureIgnoreCase))
                    foundMembership = item;

                if (foundMembership != null)
                    break;
            }

            if (foundMembership == null)
                throw new InvalidOperationException("The specified account does not belong to this group.");

            return foundMembership.Delete();
        }

        async Task<bool> IGroup.RemoveAccountAsync(string hrefOrEmailOrUsername, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(hrefOrEmailOrUsername))
                throw new ArgumentNullException(nameof(hrefOrEmailOrUsername));

            IGroupMembership foundMembership = null;
            var iterator = this.AsInterface.GetAccountMemberships();
            while (await iterator.MoveNextAsync(cancellationToken).ConfigureAwait(false))
            {
                foreach (var item in iterator.CurrentPage)
                {
                    IAccount account = await item.GetAccountAsync(cancellationToken).ConfigureAwait(false);
                    if (account.Href.Equals(hrefOrEmailOrUsername, StringComparison.InvariantCultureIgnoreCase) ||
                        account.Email.Equals(hrefOrEmailOrUsername, StringComparison.InvariantCultureIgnoreCase) ||
                        account.Username.Equals(hrefOrEmailOrUsername, StringComparison.InvariantCultureIgnoreCase))
                        foundMembership = item;

                    if (foundMembership != null)
                        break;
                }

                if (foundMembership != null)
                    break;
            }

            if (foundMembership == null)
                throw new InvalidOperationException("The specified account does not belong to this group.");

            return await foundMembership.DeleteAsync(cancellationToken).ConfigureAwait(false);
        }

        bool IGroupSync.RemoveAccount(string hrefOrEmailOrUsername)
        {
            if (string.IsNullOrEmpty(hrefOrEmailOrUsername))
                throw new ArgumentNullException(nameof(hrefOrEmailOrUsername));

            IGroupMembership foundMembership = null;
            foreach (var item in this.AsInterface.GetAccountMemberships().Synchronously())
            {
                IAccount account = item.GetAccount();
                if (account.Href.Equals(hrefOrEmailOrUsername, StringComparison.InvariantCultureIgnoreCase) ||
                    account.Email.Equals(hrefOrEmailOrUsername, StringComparison.InvariantCultureIgnoreCase) ||
                    account.Username.Equals(hrefOrEmailOrUsername, StringComparison.InvariantCultureIgnoreCase))
                    foundMembership = item;

                if (foundMembership != null)
                    break;
            }

            if (foundMembership == null)
                throw new InvalidOperationException("The specified account does not belong to this group.");

            return foundMembership.Delete();
        }

        Task<IDirectory> IGroup.GetDirectoryAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<IDirectory>(this.Directory.Href, cancellationToken);

        IDirectory IGroupSync.GetDirectory()
            => this.GetInternalSyncDataStore().GetResource<IDirectory>(this.Directory.Href);

        Task<ITenant> IGroup.GetTenantAsync(CancellationToken cancellationToken)
            => this.GetTenantAsync(this.Tenant.Href, cancellationToken);

        ITenant IGroupSync.GetTenant()
            => this.GetTenant(this.Tenant.Href);

        IAsyncQueryable<IGroupMembership> IGroup.GetAccountMemberships()
            => new CollectionResourceQueryable<IGroupMembership>(this.AccountMemberships.Href, this.GetInternalAsyncDataStore());

        IAsyncQueryable<IAccount> IGroup.GetAccounts()
            => new CollectionResourceQueryable<IAccount>(this.Accounts.Href, this.GetInternalAsyncDataStore());

        Task<IGroup> ISaveable<IGroup>.SaveAsync(CancellationToken cancellationToken)
            => this.SaveAsync<IGroup>(cancellationToken);

        IGroup ISaveableSync<IGroup>.Save()
            => this.Save<IGroup>();

        Task<bool> IDeletable.DeleteAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().DeleteAsync(this, cancellationToken);

        bool IDeletableSync.Delete()
            => this.GetInternalSyncDataStore().Delete(this);
    }
}
