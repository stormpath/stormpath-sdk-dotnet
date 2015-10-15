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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Directory;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Sync;
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

        public DefaultTenant(ResourceData data)
            : base(data)
        {
        }

        private new ITenant AsInterface => this;

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

            return this.GetInternalAsyncDataStore().CreateAsync(this.applicationsResourceBase, application, options, cancellationToken);
        }

        IApplication ITenantActionsSync.CreateApplication(IApplication application, Action<ApplicationCreationOptionsBuilder> creationOptionsAction)
        {
            var builder = new ApplicationCreationOptionsBuilder();
            creationOptionsAction(builder);
            var options = builder.Build();

            return this.GetInternalSyncDataStore().Create(this.applicationsResourceBase, application, options);
        }

        Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application, IApplicationCreationOptions creationOptions, CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().CreateAsync(this.applicationsResourceBase, application, creationOptions, cancellationToken);

        IApplication ITenantActionsSync.CreateApplication(IApplication application, IApplicationCreationOptions creationOptions)
            => this.GetInternalSyncDataStore().Create(this.applicationsResourceBase, application, creationOptions);

        Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application, CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().CreateAsync(this.applicationsResourceBase, application, cancellationToken);

        IApplication ITenantActionsSync.CreateApplication(IApplication application)
            => this.GetInternalSyncDataStore().Create(this.applicationsResourceBase, application);

        Task<IApplication> ITenantActions.CreateApplicationAsync(string name, bool createDirectory, CancellationToken cancellationToken)
        {
            var application = this.GetInternalAsyncDataStore().Instantiate<IApplication>();
            application.SetName(name);

            var options = new ApplicationCreationOptionsBuilder()
            {
                CreateDirectory = createDirectory
            }.Build();

            return this.AsInterface.CreateApplicationAsync(application, options, cancellationToken);
        }

        IApplication ITenantActionsSync.CreateApplication(string name, bool createDirectory)
        {
            var application = this.GetInternalAsyncDataStore().Instantiate<IApplication>();
            application.SetName(name);

            var options = new ApplicationCreationOptionsBuilder()
            {
                CreateDirectory = createDirectory
            }.Build();

            return this.AsTenantActionSyncInterface.CreateApplication(application, options);
        }

        Task<IDirectory> ITenantActions.CreateDirectoryAsync(IDirectory directory, CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().CreateAsync(this.directoriesResourceBase, directory, cancellationToken);

        IDirectory ITenantActionsSync.CreateDirectory(IDirectory directory)
            => this.GetInternalSyncDataStore().Create(this.directoriesResourceBase, directory);

        Task<IDirectory> ITenantActions.CreateDirectoryAsync(IDirectory directory, Action<DirectoryCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken)
        {
            var builder = new DirectoryCreationOptionsBuilder();
            creationOptionsAction(builder);
            var options = builder.Build();

            return this.AsInterface.CreateDirectoryAsync(directory, options, cancellationToken);
        }

        IDirectory ITenantActionsSync.CreateDirectory(IDirectory directory, Action<DirectoryCreationOptionsBuilder> creationOptionsAction)
        {
            var builder = new DirectoryCreationOptionsBuilder();
            creationOptionsAction(builder);
            var options = builder.Build();

            return this.CreateDirectory(directory, options);
        }

        Task<IDirectory> ITenantActions.CreateDirectoryAsync(IDirectory directory, IDirectoryCreationOptions creationOptions, CancellationToken cancellationToken)
        {
            if (directory == null)
                throw new ArgumentNullException(nameof(directory));

            if (creationOptions?.Provider != null)
                (directory as DefaultDirectory).SetProvider(creationOptions.Provider);

            return this.AsInterface.CreateDirectoryAsync(directory, cancellationToken);
        }

        IDirectory ITenantActionsSync.CreateDirectory(IDirectory directory, IDirectoryCreationOptions creationOptions)
        {
            if (directory == null)
                throw new ArgumentNullException(nameof(directory));

            if (creationOptions?.Provider != null)
                (directory as DefaultDirectory).SetProvider(creationOptions.Provider);

            return this.CreateDirectory(directory);
        }

        Task<IDirectory> ITenantActions.CreateDirectoryAsync(string name, string description, DirectoryStatus status, CancellationToken cancellationToken)
        {
            var directory = this.GetInternalAsyncDataStore().Instantiate<IDirectory>();
            directory.SetName(name);
            directory.SetDescription(description);
            directory.SetStatus(status);

            return this.AsInterface.CreateDirectoryAsync(directory, cancellationToken);
        }

        IDirectory ITenantActionsSync.CreateDirectory(string name, string description, DirectoryStatus status)
        {
            var directory = this.GetInternalAsyncDataStore().Instantiate<IDirectory>();
            directory.SetName(name);
            directory.SetDescription(description);
            directory.SetStatus(status);

            return this.AsTenantActionSyncInterface.CreateDirectory(directory);
        }

        async Task<IAccount> ITenantActions.VerifyAccountEmailAsync(string token, CancellationToken cancellationToken)
        {
            var href = $"/accounts/emailVerificationTokens/{token}";

            var emailVerificationToken = this.GetInternalAsyncDataStore().Instantiate<IEmailVerificationToken>();
            var tokenResponse = await this.GetInternalAsyncDataStore().CreateAsync(href, emailVerificationToken, cancellationToken).ConfigureAwait(false);
            return await this.GetInternalAsyncDataStore().GetResourceAsync<IAccount>(tokenResponse.Href, cancellationToken).ConfigureAwait(false);
        }

        IAccount ITenantActionsSync.VerifyAccountEmail(string token)
        {
            var href = $"/accounts/emailVerificationTokens/{token}";

            var emailVerificationToken = this.GetInternalAsyncDataStore().Instantiate<IEmailVerificationToken>();
            var tokenResponse = this.GetInternalSyncDataStore().Create(href, emailVerificationToken);
            return this.GetInternalSyncDataStore().GetResource<IAccount>(tokenResponse.Href);
        }

        IAsyncQueryable<IApplication> ITenantActions.GetApplications()
            => new CollectionResourceQueryable<IApplication>(this.Applications.Href, this.GetInternalAsyncDataStore());

        IAsyncQueryable<IDirectory> ITenantActions.GetDirectories()
            => new CollectionResourceQueryable<IDirectory>(this.Directories.Href, this.GetInternalAsyncDataStore());

        IAsyncQueryable<IAccount> ITenantActions.GetAccounts()
            => new CollectionResourceQueryable<IAccount>(this.Accounts.Href, this.GetInternalAsyncDataStore());

        IAsyncQueryable<IGroup> ITenantActions.GetGroups()
            => new CollectionResourceQueryable<IGroup>(this.Groups.Href, this.GetInternalAsyncDataStore());
    }
}
