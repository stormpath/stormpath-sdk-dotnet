// <copyright file="DefaultGroupMembership.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Group
{
    internal sealed class DefaultGroupMembership : AbstractInstanceResource, IGroupMembership
    {
        private static readonly string GroupPropertyName = "group";
        private static readonly string AccountPropertyName = "account";

        internal LinkProperty Account => this.GetLinkProperty(AccountPropertyName);

        internal LinkProperty Group => this.GetLinkProperty(GroupPropertyName);

        public DefaultGroupMembership(IInternalDataStore dataStore)
            : base(dataStore)
        {
        }

        Task<bool> IDeletable.DeleteAsync(CancellationToken cancellationToken)
            => this.GetInternalDataStore().DeleteAsync(this, cancellationToken);

        Task<IAccount> IGroupMembership.GetAccountAsync(CancellationToken cancellationToken)
            => this.GetInternalDataStore().GetResourceAsync<IAccount>(this.Account.Href, cancellationToken);

        Task<IGroup> IGroupMembership.GetGroupAsync(CancellationToken cancellationToken)
            => this.GetInternalDataStore().GetResourceAsync<IGroup>(this.Group.Href, cancellationToken);
    }
}
