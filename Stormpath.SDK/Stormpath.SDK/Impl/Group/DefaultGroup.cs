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
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Group
{
    internal sealed class DefaultGroup : AbstractExtendableInstanceResource, IGroup
    {
        private static readonly string AccountMembershipsPropertyName = "accountMemberships";
        private static readonly string AccountsPropertyName = "accounts";
        private static readonly string ApplicationsPropertyName = "applications";
        private static readonly string DescriptionPropertyName = "description";
        private static readonly string DirectoryPropertyName = "directory";
        private static readonly string NamePropertyName = "name";
        private static readonly string StatusPropertyName = "status";
        private static readonly string TenantPropertyName = "tenant";

        public DefaultGroup(IInternalDataStore dataStore)
            : base(dataStore)
        {
        }

        public DefaultGroup(IInternalDataStore dataStore, Hashtable properties)
            : base(dataStore, properties)
        {
        }

        internal LinkProperty AccountMemberships => GetLinkProperty(AccountMembershipsPropertyName);

        internal LinkProperty Accounts => GetLinkProperty(AccountsPropertyName);

        internal LinkProperty Applications => GetLinkProperty(ApplicationsPropertyName);

        string IGroup.Description => GetProperty<string>(DescriptionPropertyName);

        internal LinkProperty Directory => GetLinkProperty(DirectoryPropertyName);

        string IGroup.Name => GetProperty<string>(NamePropertyName);

        GroupStatus IGroup.Status => GetProperty<GroupStatus>(StatusPropertyName);

        internal LinkProperty Tenant => GetLinkProperty(TenantPropertyName);

        Task<IGroup> ISaveable<IGroup>.SaveAsync(CancellationToken cancellationToken)
        {
            return GetInternalDataStore().SaveAsync<IGroup>(this, cancellationToken);
        }
    }
}
