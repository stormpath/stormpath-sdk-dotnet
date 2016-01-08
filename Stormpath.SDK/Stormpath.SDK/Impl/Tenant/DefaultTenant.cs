// <copyright file="DefaultTenant.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Tenant
{
    internal sealed partial class DefaultTenant : AbstractExtendableInstanceResource, ITenant, ITenantSync
    {
        private static readonly string AccountsPropertyName = "accounts";
        private static readonly string AgentsPropertyName = "agents";
        private static readonly string ApplicationsPropertyName = "applications";
        private static readonly string DirectoriesPropertyName = "directories";
        private static readonly string GroupsPropertyName = "groups";
        private static readonly string IdSitesPropertyName = "idSites";
        private static readonly string KeyPropertyName = "key";
        private static readonly string NamePropertyName = "name";
        private static readonly string OrganizationsPropertyName = "organizations";

        public DefaultTenant(ResourceData data)
            : base(data)
        {
        }

        private new ITenant AsInterface => this;

        private ITenantActionsSync AsTenantActionSyncInterface => this;

        internal IEmbeddedProperty Accounts => this.GetLinkProperty(AccountsPropertyName);

        internal IEmbeddedProperty Agents => this.GetLinkProperty(AgentsPropertyName);

        internal IEmbeddedProperty Applications => this.GetLinkProperty(ApplicationsPropertyName);

        internal IEmbeddedProperty Directories => this.GetLinkProperty(DirectoriesPropertyName);

        internal IEmbeddedProperty Groups => this.GetLinkProperty(GroupsPropertyName);

        internal IEmbeddedProperty IdSites => this.GetLinkProperty(IdSitesPropertyName);

        string ITenant.Key => this.GetStringProperty(KeyPropertyName);

        string ITenant.Name => this.GetStringProperty(NamePropertyName);

        internal IEmbeddedProperty Organizations => this.GetLinkProperty(OrganizationsPropertyName);
    }
}
