// <copyright file="DefaultTenant.cs" company="Stormpath, Inc.">
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
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;
using System.Threading;

namespace Stormpath.SDK.Impl.Tenant
{
    internal sealed class DefaultTenant : AbstractExtendableInstanceResource, ITenant
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

        public DefaultTenant(IInternalDataStore dataStore)
            : base(dataStore)
        {
        }

        public DefaultTenant(IInternalDataStore dataStore, Hashtable properties)
             : base(dataStore, properties)
        {
        }

        private ITenant This => this;

        internal LinkProperty Accounts => GetLinkProperty(AccountsPropertyName);

        internal LinkProperty Agents => GetLinkProperty(AgentsPropertyName);

        internal string ApplicationResourceBase = "applications";

        internal LinkProperty Applications => GetLinkProperty(ApplicationsPropertyName);

        internal string DirectoriesResourceBase = "directories";

        internal LinkProperty Directories => GetLinkProperty(DirectoriesPropertyName);

        internal string GroupsResourceBase = "groups";

        internal LinkProperty Groups => GetLinkProperty(GroupsPropertyName);

        internal LinkProperty IdSites => GetLinkProperty(IdSitesPropertyName);

        string ITenant.Key => GetProperty<string>(KeyPropertyName);

        string ITenant.Name => GetProperty<string>(NamePropertyName);

        internal LinkProperty Organizations => GetLinkProperty(OrganizationsPropertyName);

        Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application, CancellationToken cancellationToken)
        {
            return GetInternalDataStore().CreateAsync(ApplicationResourceBase, application, cancellationToken);
        }

        Task<IApplication> ITenantActions.CreateApplicationAsync(string name, CancellationToken cancellationToken)
        {
            var application = new Application.DefaultApplication(GetInternalDataStore());
            application.SetProperty("name", name); //todo

            return This.CreateApplicationAsync(application, cancellationToken);
        }

        ICollectionResourceQueryable<IApplication> ITenantActions.GetApplications()
        {
            return new CollectionResourceQueryable<IApplication>(this.Applications.Href, this.GetInternalDataStore());
        }

        Task<IDirectory> ITenantActions.CreateDirectoryAsync(IDirectory directory, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        ICollectionResourceQueryable<IDirectory> ITenantActions.GetDirectories()
        {
            return new CollectionResourceQueryable<IDirectory>(this.Directories.Href, this.GetInternalDataStore());
        }

        Task<IAccount> ITenantActions.VerifyAccountEmailAsync(string token, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        ICollectionResourceQueryable<IAccount> ITenantActions.GetAccounts()
        {
            return new CollectionResourceQueryable<IAccount>(this.Accounts.Href, this.GetInternalDataStore());
        }

        ICollectionResourceQueryable<IGroup> ITenantActions.GetGroups()
        {
            return new CollectionResourceQueryable<IGroup>(this.Groups.Href, this.GetInternalDataStore());
        }

        Task<IDirectory> ITenantActions.CreateDirectoryAsync(string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
