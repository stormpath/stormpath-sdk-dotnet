// <copyright file="TenantActionsCompatibilityExtensions.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Organization;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK
{
    public static class TenantActionsCompatibilityExtensions
    {
        /// <summary>
        /// Creates a new <see cref="Application.IApplication">Application</see> resource in the current tenant.
        /// </summary>
        /// <param name="application">The <see cref="IApplication">Application</see> to create.</param>
        /// <param name="creationOptionsAction">An inline builder for an instance of <see cref="IApplicationCreationOptions"/>,
        /// which will be used when sending the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IApplication">Application</see>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the application.</exception>
        public static Task<IApplication> CreateApplicationAsync(this ITenantActions tenantActions, IApplication application, Action<ApplicationCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken = default(CancellationToken))
        {
            var builder = new ApplicationCreationOptionsBuilder();
            creationOptionsAction(builder);
            var options = builder.Build();

            return (tenantActions as AbstractResource).GetInternalAsyncDataStore().CreateAsync("applications", application, options, cancellationToken);
        }


        /// <summary>
        /// Creates a new Cloud- or Provider-based Directory resource in the Tenant.
        /// </summary>
        /// <param name="directory">The <see cref="IDirectory">Directory</see> to create.</param>
        /// <param name="creationOptionsAction">An inline builder for an instance of <see cref="IDirectoryCreationOptions"/>,
        /// which will be used when sending the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IDirectory">Directory</see>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the directory.</exception>
        public static Task<IDirectory> CreateDirectoryAsync(this ITenantActions tenantActions, IDirectory directory, Action<DirectoryCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken = default(CancellationToken))
        {
            var builder = new DirectoryCreationOptionsBuilder();
            creationOptionsAction(builder);
            var options = builder.Build();

            return tenantActions.CreateDirectoryAsync(directory, options, cancellationToken);
        }

        /// <summary>
        /// Creates a new <see cref="IOrganization">Organization</see> resource in the Tenant.
        /// </summary>
        /// <param name="organization">The Organization to create.</param>
        /// <param name="creationOptionsAction">An inline builder for an instance of <see cref="IDirectoryCreationOptions"/>,
        /// which will be used when sending the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IOrganization">Organization</see>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the Organization.</exception>
        public static Task<IOrganization> CreateOrganizationAsync(this ITenantActions tenantActions, IOrganization organization, Action<OrganizationCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken = default(CancellationToken))
        {
            var builder = new OrganizationCreationOptionsBuilder();
            creationOptionsAction(builder);
            var options = builder.Build();

            return tenantActions.CreateOrganizationAsync(organization, options, cancellationToken);
        }
    }
}
