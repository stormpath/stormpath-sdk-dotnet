﻿// <copyright file="DefaultTenant.TenantActions.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Directory;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Organization;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Tenant
{
    internal sealed partial class DefaultTenant
    {
        Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application, IApplicationCreationOptions creationOptions, CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().CreateAsync(ApplicationsPropertyName, application, creationOptions, cancellationToken);

        Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application, CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().CreateAsync(ApplicationsPropertyName, application, cancellationToken);

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

        Task<IDirectory> ITenantActions.CreateDirectoryAsync(IDirectory directory, CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().CreateAsync(DirectoriesPropertyName, directory, cancellationToken);

        Task<IDirectory> ITenantActions.CreateDirectoryAsync(IDirectory directory, IDirectoryCreationOptions creationOptions, CancellationToken cancellationToken)
        {
            if (directory == null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (creationOptions?.Provider != null)
            {
                (directory as DefaultDirectory).SetProvider(creationOptions.Provider);
            }

            return this.GetInternalAsyncDataStore().CreateAsync(DirectoriesPropertyName, directory, creationOptions, cancellationToken);
        }

        Task<IDirectory> ITenantActions.CreateDirectoryAsync(string name, string description, DirectoryStatus status, CancellationToken cancellationToken)
        {
            var directory = this.GetInternalAsyncDataStore().Instantiate<IDirectory>();
            directory.SetName(name);
            directory.SetDescription(description);
            directory.SetStatus(status);

            return this.AsInterface.CreateDirectoryAsync(directory, cancellationToken);
        }

        Task<IOrganization> ITenantActions.CreateOrganizationAsync(IOrganization organization, CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().CreateAsync(OrganizationsPropertyName, organization, cancellationToken);

        Task<IOrganization> ITenantActions.CreateOrganizationAsync(IOrganization organization, IOrganizationCreationOptions creationOptions, CancellationToken cancellationToken)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }

            return this.GetInternalAsyncDataStore().CreateAsync(OrganizationsPropertyName, organization, creationOptions, cancellationToken);
        }

        Task<IOrganization> ITenantActions.CreateOrganizationAsync(string name, string nameKey, CancellationToken cancellationToken)
        {
            var organzation = this.GetInternalAsyncDataStore().Instantiate<IOrganization>();
            organzation.SetName(name);
            organzation.SetNameKey(nameKey);

            return this.AsInterface.CreateOrganizationAsync(organzation, cancellationToken);
        }

        async Task<IAccount> ITenantActions.VerifyAccountEmailAsync(string token, CancellationToken cancellationToken)
        {
            var href = $"/accounts/emailVerificationTokens/{token}";

            var tokenResponse = await this.GetInternalAsyncDataStore().CreateAsync<IResource, IEmailVerificationToken>(href, null, cancellationToken).ConfigureAwait(false);
            return await this.GetInternalAsyncDataStore().GetResourceAsync<IAccount>(tokenResponse.Href, cancellationToken).ConfigureAwait(false);
        }

        Task<IAccount> ITenantActions.GetAccountAsync(string href, CancellationToken cancellationToken)
            => TenantActionsShared.GetAccountAsync(this.GetInternalAsyncDataStore(), href, cancellationToken);

        Task<IAccount> ITenantActions.GetAccountAsync(string href, Action<IRetrievalOptions<IAccount>> retrievalOptions, CancellationToken cancellationToken)
            => TenantActionsShared.GetAccountAsync(this.GetInternalAsyncDataStore(), href, retrievalOptions, cancellationToken);

        Task<IApplication> ITenantActions.GetApplicationAsync(string href, CancellationToken cancellationToken)
            => TenantActionsShared.GetApplicationAsync(this.GetInternalAsyncDataStore(), href, cancellationToken);

        Task<IDirectory> ITenantActions.GetDirectoryAsync(string href, CancellationToken cancellationToken)
            => TenantActionsShared.GetDirectoryAsync(this.GetInternalAsyncDataStore(), href, cancellationToken);

        Task<IGroup> ITenantActions.GetGroupAsync(string href, CancellationToken cancellationToken)
            => TenantActionsShared.GetGroupAsync(this.GetInternalAsyncDataStore(), href, cancellationToken);

        Task<IOrganization> ITenantActions.GetOrganizationAsync(string href, CancellationToken cancellationToken)
            => TenantActionsShared.GetOrganizationAsync(this.GetInternalAsyncDataStore(), href, cancellationToken);

        Task<IAccessToken> ITenantActions.GetAccessTokenAsync(string href, CancellationToken cancellationToken)
            => TenantActionsShared.GetAccessTokenAsync(this.GetInternalAsyncDataStore(), href, cancellationToken);

        Task<IRefreshToken> ITenantActions.GetRefreshTokenAsync(string href, CancellationToken cancellationToken)
            => TenantActionsShared.GetRefreshTokenAsync(this.GetInternalAsyncDataStore(), href, cancellationToken);

        Task<IOrganization> ITenantActions.GetOrganizationByNameKeyAsync(string nameKey, CancellationToken cancellationToken)
        {
            return this.AsInterface.GetOrganizations().Where(org => org.NameKey == nameKey).SingleOrDefaultAsync();
        }
    }
}
