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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Directory
{
    internal sealed class DefaultDirectory : AbstractExtendableInstanceResource, IDirectory
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

        public DefaultDirectory(IInternalDataStore dataStore)
            : base(dataStore)
        {
        }

        public DefaultDirectory(IInternalDataStore dataStore, IDictionary<string, object> properties)
            : base(dataStore, properties)
        {
        }

        internal LinkProperty AccountCreationPolicy => this.GetLinkProperty(AccountCreationPolicyPropertyName);

        internal LinkProperty Accounts => this.GetLinkProperty(AccountsPropertyName);

        internal LinkProperty ApplicationMappings => this.GetLinkProperty(ApplicationMappingsPropertyName);

        internal LinkProperty Applications => this.GetLinkProperty(ApplicationsPropertyName);

        string IDirectory.Description => GetProperty<string>(DescriptionPropertyName);

        internal LinkProperty Groups => this.GetLinkProperty(GroupsPropertyName);

        string IDirectory.Name => GetProperty<string>(NamePropertyName);

        internal LinkProperty PasswordPolicy => this.GetLinkProperty(PasswordPolicyPropertyName);

        internal LinkProperty Provider => this.GetLinkProperty(ProviderPropertyName);

        DirectoryStatus IDirectory.Status => GetProperty<DirectoryStatus>(StatusPropertyName);

        internal LinkProperty Tenant => this.GetLinkProperty(TenantPropertyName);

        Task<IDirectory> ISaveable<IDirectory>.SaveAsync(CancellationToken cancellationToken)
        {
            return this.GetInternalDataStore().SaveAsync<IDirectory>(this, cancellationToken);
        }

        IAsyncQueryable<IAccount> IDirectory.GetAccounts()
        {
            return new CollectionResourceQueryable<IAccount>(this.Accounts.Href, this.GetInternalDataStore());
        }

        Task<bool> IDeletable.DeleteAsync(CancellationToken cancellationToken)
        {
            return this.GetInternalDataStore().DeleteAsync(this, cancellationToken);
        }
    }
}
