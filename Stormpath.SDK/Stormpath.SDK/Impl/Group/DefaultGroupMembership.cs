// <copyright file="DefaultGroupMembership.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Group
{
    internal sealed class DefaultGroupMembership : AbstractInstanceResource, IGroupMembership, IInternalGroupMembership, IGroupMembershipSync
    {
        private static readonly string GroupPropertyName = "group";
        private static readonly string AccountPropertyName = "account";

        internal IEmbeddedProperty Account => this.GetLinkProperty(AccountPropertyName);

        internal IEmbeddedProperty Group => this.GetLinkProperty(GroupPropertyName);

        public DefaultGroupMembership(ResourceData data)
            : base(data)
        {
        }

        string IInternalGroupMembership.AccountHref => this.Account?.Href;

        string IInternalGroupMembership.GroupHref => this.Group?.Href;

        Task<bool> IDeletable.DeleteAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().DeleteAsync(this, cancellationToken);

        bool IDeletableSync.Delete()
            => this.GetInternalSyncDataStore().Delete(this);

        Task<IAccount> IGroupMembership.GetAccountAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<IAccount>(this.Account.Href, cancellationToken);

        IAccount IGroupMembershipSync.GetAccount()
            => this.GetInternalSyncDataStore().GetResource<IAccount>(this.Account.Href);

        Task<IGroup> IGroupMembership.GetGroupAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<IGroup>(this.Group.Href, cancellationToken);

        IGroup IGroupMembershipSync.GetGroup()
            => this.GetInternalSyncDataStore().GetResource<IGroup>(this.Group.Href);

        private void SetGroup(IGroup group)
            => this.SetLinkProperty(GroupPropertyName, group.Href);

        private void SetAccount(IAccount account)
            => this.SetLinkProperty(AccountPropertyName, account.Href);

        public static Task<IGroupMembership> CreateAsync(IAccount account, IGroup group, IInternalAsyncDataStore dataStore, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(account.Href))
            {
                throw new ApplicationException("You must persist the account first before assigning it to a group.");
            }

            if (string.IsNullOrEmpty(group.Href))
            {
                throw new ApplicationException("You must persist the group first because assigning it to a group.");
            }

            var groupMembership = (DefaultGroupMembership)dataStore.Instantiate<IGroupMembership>();
            groupMembership.SetGroup(group);
            groupMembership.SetAccount(account);

            var href = "/groupMemberships";

            return dataStore.CreateAsync<IGroupMembership>(href, groupMembership, cancellationToken);
        }

        public static IGroupMembership Create(IAccount account, IGroup group, IInternalSyncDataStore dataStore)
        {
            if (string.IsNullOrEmpty(account.Href))
            {
                throw new ApplicationException("You must persist the account first before assigning it to a group.");
            }

            if (string.IsNullOrEmpty(group.Href))
            {
                throw new ApplicationException("You must persist the group first because assigning it to a group.");
            }

            var groupMembership = (DefaultGroupMembership)(dataStore as IDataStore).Instantiate<IGroupMembership>();
            groupMembership.SetGroup(group);
            groupMembership.SetAccount(account);

            var href = "/groupMemberships";

            return dataStore.Create<IGroupMembership>(href, groupMembership);
        }
    }
}
