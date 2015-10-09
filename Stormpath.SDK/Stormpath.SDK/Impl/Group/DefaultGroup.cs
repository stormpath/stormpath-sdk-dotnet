// <copyright file="DefaultGroup.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.CustomData;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Group
{
    internal sealed class DefaultGroup : AbstractExtendableInstanceResource, IGroup
    {
        private static readonly string NamePropertyName = "name";
        private static readonly string DescriptionPropertyName = "description";
        private static readonly string StatusPropertyName = "status";
        private static readonly string DirectoryPropertyName = "directory";
        private static readonly string TenantPropertyName = "tenant";
        private static readonly string AccountsPropertyName = "accounts";
        private static readonly string AccountMembershipsPropertyName = "accountMemberships";

        public DefaultGroup(IInternalDataStore dataStore)
            : base(dataStore)
        {
        }

        private IGroup AsInterface => this;

        string IGroup.Name => this.GetProperty<string>(NamePropertyName);

        string IGroup.Description => this.GetProperty<string>(DescriptionPropertyName);

        GroupStatus IGroup.Status => this.GetProperty<GroupStatus>(StatusPropertyName);

        internal LinkProperty Directory => this.GetLinkProperty(DirectoryPropertyName);

        internal LinkProperty Tenant => this.GetLinkProperty(TenantPropertyName);

        internal LinkProperty Accounts => this.GetLinkProperty(AccountsPropertyName);

        internal LinkProperty AccountMemberships => this.GetLinkProperty(AccountMembershipsPropertyName);

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

        private Task<IAccount> FindAccountAsync(string hrefOrEmailOrUsername, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<IGroupMembership> IGroup.AddAccountAsync(IAccount account, CancellationToken cancellationToken)
            => DefaultGroupMembership.CreateAsync(account, this, this.GetInternalDataStore(), cancellationToken);

        async Task<IGroupMembership> IGroup.AddAccountAsync(string hrefOrEmailOrUsername, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(hrefOrEmailOrUsername))
                throw new ArgumentNullException(nameof(hrefOrEmailOrUsername));

            var account = await this.FindAccountAsync(hrefOrEmailOrUsername, cancellationToken).ConfigureAwait(false);
            if (account == null)
                throw new InvalidOperationException($"No matching account for {nameof(hrefOrEmailOrUsername)} '{hrefOrEmailOrUsername}' found.");

            return await DefaultGroupMembership.CreateAsync(account, this, this.GetInternalDataStore(), cancellationToken).ConfigureAwait(false);
        }

        async Task<bool> IGroup.RemoveAccountAsync(IAccount account, CancellationToken cancellationToken)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            IGroupMembership foundMembership = null;
            await this.AsInterface.GetAccountMemberships().ForEachAsync(
                item =>
            {
                if ((item as IInternalGroupMembership).AccountHref.Equals(this.AsInterface.Href, StringComparison.InvariantCultureIgnoreCase))
                    foundMembership = item;

                return foundMembership != null;
            }, cancellationToken).ConfigureAwait(false);

            if (foundMembership == null)
                throw new InvalidOperationException("The specified account does not belong to this group.");

            return await foundMembership.DeleteAsync(cancellationToken).ConfigureAwait(false);
        }

        Task<bool> IGroup.RemoveAccountAsync(string hrefOrEmailOrUsername, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<IDirectory> IGroup.GetDirectoryAsync(CancellationToken cancellationToken)
            => this.GetInternalDataStore().GetResourceAsync<IDirectory>(this.Directory.Href, cancellationToken);

        IAsyncQueryable<IGroupMembership> IGroup.GetAccountMemberships()
            => new CollectionResourceQueryable<IGroupMembership>(this.AccountMemberships.Href, this.GetInternalDataStore());

        IAsyncQueryable<IAccount> IGroup.GetAccounts()
            => new CollectionResourceQueryable<IAccount>(this.Accounts.Href, this.GetInternalDataStore());

        Task<IGroup> ISaveable<IGroup>.SaveAsync(CancellationToken cancellationToken)
            => this.SaveAsync<IGroup>(cancellationToken);

        Task<bool> IDeletable.DeleteAsync(CancellationToken cancellationToken)
            => this.GetInternalDataStore().DeleteAsync(this, cancellationToken);
    }
}
