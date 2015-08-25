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

using System.Collections;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;

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

        public DefaultDirectory(IDataStore dataStore)
            : base(dataStore)
        {
        }

        public DefaultDirectory(IDataStore dataStore, Hashtable properties)
            : base(dataStore, properties)
        {
        }

        internal LinkProperty AccountCreationPolicy => GetLinkProperty(AccountCreationPolicyPropertyName);

        internal LinkProperty Accounts => GetLinkProperty(AccountsPropertyName);

        internal LinkProperty ApplicationMappings => GetLinkProperty(ApplicationMappingsPropertyName);

        internal LinkProperty Applications => GetLinkProperty(ApplicationsPropertyName);

        string IDirectory.Description => GetProperty<string>(DescriptionPropertyName);

        internal LinkProperty Groups => GetLinkProperty(GroupsPropertyName);

        string IDirectory.Name => GetProperty<string>(NamePropertyName);

        internal LinkProperty PasswordPolicy => GetLinkProperty(PasswordPolicyPropertyName);

        internal LinkProperty Provider => GetLinkProperty(ProviderPropertyName);

        DirectoryStatus IDirectory.Status => GetProperty<DirectoryStatus>(StatusPropertyName);

        internal LinkProperty Tenant => GetLinkProperty(TenantPropertyName);
    }
}
