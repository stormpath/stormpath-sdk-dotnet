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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Tenant
{
    internal sealed class DefaultTenant : AbstractExtendableInstanceResource, ITenant, ITenantSync
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

        private string applicationsResourceBase = "applications";
#pragma warning disable CS0414 // Assigned but value is never used
        private string directoriesResourceBase = "directories";
        private string groupsResourceBase = "groups";
#pragma warning restore CS0414 // Assigned but value is never used

        public DefaultTenant(IInternalDataStore dataStore)
            : base(dataStore)
        {
        }

        public DefaultTenant(IInternalDataStore dataStore, IDictionary<string, object> properties)
             : base(dataStore, properties)
        {
        }

        private ITenant AsInterface => this;

        private ITenantActionsSync AsTenantActionSyncInterface => this;

        internal LinkProperty Accounts => this.GetLinkProperty(AccountsPropertyName);

        internal LinkProperty Agents => this.GetLinkProperty(AgentsPropertyName);

        internal LinkProperty Applications => this.GetLinkProperty(ApplicationsPropertyName);

        internal LinkProperty Directories => this.GetLinkProperty(DirectoriesPropertyName);

        internal LinkProperty Groups => this.GetLinkProperty(GroupsPropertyName);

        internal LinkProperty IdSites => this.GetLinkProperty(IdSitesPropertyName);

        string ITenant.Key => this.GetProperty<string>(KeyPropertyName);

        string ITenant.Name => this.GetProperty<string>(NamePropertyName);

        internal LinkProperty Organizations => this.GetLinkProperty(OrganizationsPropertyName);

        Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application, Action<ApplicationCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken)
        {
            var builder = new ApplicationCreationOptionsBuilder();
            creationOptionsAction(builder);
            var options = builder.Build();

            return this.GetInternalDataStore().CreateAsync(this.applicationsResourceBase, application, options, cancellationToken);
        }

        IApplication ITenantActionsSync.CreateApplication(IApplication application, Action<ApplicationCreationOptionsBuilder> creationOptionsAction)
        {
            var builder = new ApplicationCreationOptionsBuilder();
            creationOptionsAction(builder);
            var options = builder.Build();

            return this.GetInternalDataStoreSync().Create(this.applicationsResourceBase, application, options);
        }

        Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application, IApplicationCreationOptions creationOptions, CancellationToken cancellationToken)
            => this.GetInternalDataStore().CreateAsync(this.applicationsResourceBase, application, creationOptions, cancellationToken);

        IApplication ITenantActionsSync.CreateApplication(IApplication application, IApplicationCreationOptions creationOptions)
            => this.GetInternalDataStoreSync().Create(this.applicationsResourceBase, application, creationOptions);

        Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application, CancellationToken cancellationToken)
            => this.GetInternalDataStore().CreateAsync(this.applicationsResourceBase, application, cancellationToken);

        IApplication ITenantActionsSync.CreateApplication(IApplication application)
            => this.GetInternalDataStoreSync().Create(this.applicationsResourceBase, application);

        Task<IApplication> ITenantActions.CreateApplicationAsync(string name, bool createDirectory, CancellationToken cancellationToken)
        {
            var application = this.GetInternalDataStore().Instantiate<IApplication>();
            application.SetName(name);

            var options = new ApplicationCreationOptionsBuilder()
            {
                CreateDirectory = createDirectory
            }.Build();

            return this.AsInterface.CreateApplicationAsync(application, options, cancellationToken);
        }

        IApplication ITenantActionsSync.CreateApplication(string name, bool createDirectory)
        {
            var application = this.GetInternalDataStore().Instantiate<IApplication>();
            application.SetName(name);

            var options = new ApplicationCreationOptionsBuilder()
            {
                CreateDirectory = createDirectory
            }.Build();

            return this.AsTenantActionSyncInterface.CreateApplication(application, options);
        }

        Task<IAccount> ITenantActions.VerifyAccountEmailAsync(string token, CancellationToken cancellationToken)
        {
            var href = $"/accounts/emailVerificationTokens/{token}";

            var emailVerificationToken = this.GetInternalDataStore().Instantiate<IEmailVerificationToken>();
            return this.GetInternalDataStore().CreateAsync<IEmailVerificationToken, IAccount>(href, emailVerificationToken, cancellationToken);
        }

        IAsyncQueryable<IApplication> ITenantActions.GetApplications()
            => new CollectionResourceQueryable<IApplication>(this.Applications.Href, this.GetInternalDataStore());

        IAsyncQueryable<IDirectory> ITenantActions.GetDirectories()
            => new CollectionResourceQueryable<IDirectory>(this.Directories.Href, this.GetInternalDataStore());

        IAsyncQueryable<IAccount> ITenantActions.GetAccounts()
            => new CollectionResourceQueryable<IAccount>(this.Accounts.Href, this.GetInternalDataStore());
    }
}
