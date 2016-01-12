// <copyright file="DefaultTenant.TenantActionsSync.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
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
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Directory;
using Stormpath.SDK.Organization;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Sync;

namespace Stormpath.SDK.Impl.Tenant
{
    internal sealed partial class DefaultTenant
    {
        IApplication ITenantActionsSync.CreateApplication(IApplication application, Action<ApplicationCreationOptionsBuilder> creationOptionsAction)
        {
            var builder = new ApplicationCreationOptionsBuilder();
            creationOptionsAction(builder);
            var options = builder.Build();

            return this.GetInternalSyncDataStore().Create(ApplicationsPropertyName, application, options);
        }

        IApplication ITenantActionsSync.CreateApplication(IApplication application, IApplicationCreationOptions creationOptions)
            => this.GetInternalSyncDataStore().Create(ApplicationsPropertyName, application, creationOptions);

        IApplication ITenantActionsSync.CreateApplication(IApplication application)
            => this.GetInternalSyncDataStore().Create(ApplicationsPropertyName, application);

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

        IDirectory ITenantActionsSync.CreateDirectory(IDirectory directory)
            => this.GetInternalSyncDataStore().Create(DirectoriesPropertyName, directory);

        IDirectory ITenantActionsSync.CreateDirectory(IDirectory directory, Action<DirectoryCreationOptionsBuilder> creationOptionsAction)
        {
            var builder = new DirectoryCreationOptionsBuilder();
            creationOptionsAction(builder);
            var options = builder.Build();

            return this.CreateDirectory(directory, options);
        }

        IDirectory ITenantActionsSync.CreateDirectory(IDirectory directory, IDirectoryCreationOptions creationOptions)
        {
            if (directory == null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (creationOptions?.Provider != null)
            {
                (directory as DefaultDirectory).SetProvider(creationOptions.Provider);
            }

            return this.GetInternalSyncDataStore().Create(DirectoriesPropertyName, directory, creationOptions);
        }

        IDirectory ITenantActionsSync.CreateDirectory(string name, string description, DirectoryStatus status)
        {
            var directory = this.GetInternalAsyncDataStore().Instantiate<IDirectory>();
            directory.SetName(name);
            directory.SetDescription(description);
            directory.SetStatus(status);

            return this.AsTenantActionSyncInterface.CreateDirectory(directory);
        }

        IOrganization ITenantActionsSync.CreateOrganization(IOrganization organization)
            => this.GetInternalSyncDataStore().Create(OrganizationsPropertyName, organization);

        IOrganization ITenantActionsSync.CreateOrganization(IOrganization organization, Action<OrganizationCreationOptionsBuilder> creationOptionsAction)
        {
            var builder = new OrganizationCreationOptionsBuilder();
            creationOptionsAction(builder);
            var options = builder.Build();

            return this.CreateOrganization(organization, options);
        }

        IOrganization ITenantActionsSync.CreateOrganization(IOrganization organization, IOrganizationCreationOptions creationOptions)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }

            return this.GetInternalSyncDataStore().Create(OrganizationsPropertyName, organization, creationOptions);
        }

        IOrganization ITenantActionsSync.CreateOrganization(string name, string description)
        {
            var organization = this.GetInternalAsyncDataStore().Instantiate<IOrganization>();
            organization.SetName(name);
            organization.SetDescription(description);

            return this.AsTenantActionSyncInterface.CreateOrganization(organization);
        }

        IAccount ITenantActionsSync.VerifyAccountEmail(string token)
        {
            var href = $"/accounts/emailVerificationTokens/{token}";

            var tokenResponse = this.GetInternalSyncDataStore().Create<IResource, IEmailVerificationToken>(href, null);
            return this.GetInternalSyncDataStore().GetResource<IAccount>(tokenResponse.Href);
        }

        IAccount ITenantActionsSync.GetAccount(string href)
            => this.GetInternalSyncDataStore().GetResource<IAccount>(href);

        IApplication ITenantActionsSync.GetApplication(string href)
            => this.GetInternalSyncDataStore().GetResource<IApplication>(href);

        IDirectory ITenantActionsSync.GetDirectory(string href)
            => this.GetInternalSyncDataStore().GetResource<IDirectory>(href);

        IGroup ITenantActionsSync.GetGroup(string href)
            => this.GetInternalSyncDataStore().GetResource<IGroup>(href);

        IOrganization ITenantActionsSync.GetOrganization(string href)
            => this.GetInternalSyncDataStore().GetResource<IOrganization>(href);
    }
}
