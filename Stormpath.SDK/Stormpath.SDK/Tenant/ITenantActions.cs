// <copyright file="ITenantActions.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Linq;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Organization;

namespace Stormpath.SDK.Tenant
{
    /// <summary>
    /// Represents common tenant actions that can be executed on a <see cref="ITenant">Tenant</see> instance
    /// <i>or</i> a <see cref="Client.IClient">Client</see> instance acting on behalf of its current tenant.
    /// </summary>
    public interface ITenantActions
    {
        /// <summary>
        /// Creates a new <see cref="Application.IApplication">Application</see> resource in the current tenant, with the default creation options.
        /// </summary>
        /// <param name="application">The <see cref="IApplication">Application</see> to create.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IApplication">Application</see>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the application.</exception>
        Task<IApplication> CreateApplicationAsync(IApplication application, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new <see cref="Application.IApplication">Application</see> resource in the current tenant.
        /// </summary>
        /// <param name="application">The <see cref="IApplication">Application</see> to create.</param>
        /// <param name="creationOptionsAction">An inline builder for an instance of <see cref="IApplicationCreationOptions"/>,
        /// which will be used when sending the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IApplication">Application</see>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the application.</exception>
        Task<IApplication> CreateApplicationAsync(IApplication application, Action<ApplicationCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new <see cref="Application.IApplication">Application</see> resource in the current tenant.
        /// </summary>
        /// <param name="application">The <see cref="IApplication">Application</see> to create.</param>
        /// <param name="creationOptions">An <see cref="IApplicationCreationOptions"/> instance to use when sending the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IApplication">Application</see>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the application.</exception>
        Task<IApplication> CreateApplicationAsync(IApplication application, IApplicationCreationOptions creationOptions, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Convenience method. Creates a new <see cref="Application.IApplication">Application</see> resource in the current tenant.
        /// </summary>
        /// <param name="name">The name of the application.</param>
        /// <param name="createDirectory">Whether a default directory should be created automatically.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IApplication">Application</see>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the application.</exception>
        Task<IApplication> CreateApplicationAsync(string name, bool createDirectory, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new Cloud- or Provider-based Directory resource in the Tenant.
        /// </summary>
        /// <param name="directory">The Directory resource to create.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IDirectory">Directory</see>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the directory.</exception>
        Task<IDirectory> CreateDirectoryAsync(IDirectory directory, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new Cloud- or Provider-based Directory resource in the Tenant.
        /// </summary>
        /// <param name="directory">The <see cref="IDirectory">Directory</see> to create.</param>
        /// <param name="creationOptionsAction">An inline builder for an instance of <see cref="IDirectoryCreationOptions"/>,
        /// which will be used when sending the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IDirectory">Directory</see>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the directory.</exception>
        Task<IDirectory> CreateDirectoryAsync(IDirectory directory, Action<DirectoryCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new Cloud- or Provider-based Directory resource in the Tenant.
        /// </summary>
        /// <param name="directory">The <see cref="IDirectory">Directory</see> to create.</param>
        /// <param name="creationOptions">A <see cref="IDirectoryCreationOptions"/> instance to use when sending the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IDirectory">Directory</see>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the directory.</exception>
        Task<IDirectory> CreateDirectoryAsync(IDirectory directory, IDirectoryCreationOptions creationOptions, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new Cloud Directory resource in the Tenant.
        /// </summary>
        /// <param name="name">The directory name.</param>
        /// <param name="description">The directory description.</param>
        /// <param name="status">The initial directory status.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IDirectory">Directory</see>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the directory.</exception>
        Task<IDirectory> CreateDirectoryAsync(string name, string description, DirectoryStatus status, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new <see cref="IOrganization">Organization</see> resource in the Tenant.
        /// </summary>
        /// <param name="organization">The Organization to create.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IOrganization">Organization</see>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the Organization.</exception>
        Task<IOrganization> CreateOrganizationAsync(IOrganization organization, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new <see cref="IOrganization">Organization</see> resource in the Tenant.
        /// </summary>
        /// <param name="organization">The Organization to create.</param>
        /// <param name="creationOptionsAction">An inline builder for an instance of <see cref="IDirectoryCreationOptions"/>,
        /// which will be used when sending the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IOrganization">Organization</see>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the Organization.</exception>
        Task<IOrganization> CreateOrganizationAsync(IOrganization organization, Action<OrganizationCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new <see cref="IOrganization">Organization</see> resource in the Tenant.
        /// </summary>
        /// <param name="organization">The Organization to create.</param>
        /// <param name="creationOptions">A <see cref="IDirectoryCreationOptions"/> instance to use when sending the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IOrganization">Organization</see>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the Organization.</exception>
        Task<IOrganization> CreateOrganizationAsync(IOrganization organization, IOrganizationCreationOptions creationOptions, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new <see cref="IOrganization">Organization</see> resource in the Tenant.
        /// </summary>
        /// <param name="name">The Organization's name.</param>
        /// <param name="nameKey">The Organization's <c>nameKey</c>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created <see cref="IOrganization">Organization</see>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the Organization.</exception>
        Task<IOrganization> CreateOrganizationAsync(string name, string nameKey, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Verifies an account's email address based on a <c>sptoken</c> parameter embedded in a URL
        /// found in an account's verification email.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For example:
        /// <code>
        /// https://my.company.com/email/verify?sptoken=ExAmPleEmAilVeRiFiCaTiOnTokEnHeRE
        /// </code>
        /// </para>
        /// <para>
        /// Based on this URL, the following should be invoked:
        /// <code>
        /// tenant.VerifyAccountEmailAsync("ExAmPleEmAilVeRiFiCaTiOnTokEnHeRE");
        /// </code>
        /// </para>
        /// </remarks>
        /// <param name="token">The <c>sptoken</c> query parameter value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The verified account.</returns>
        /// <exception cref="Error.ResourceException">The token was not valid.</exception>
        Task<IAccount> VerifyAccountEmailAsync(string token, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves the <see cref="IAccount">Account</see> at the specified Stormpath URL.
        /// </summary>
        /// <remarks>This is a convenience method equivalent to <see cref="DataStore.IDataStore.GetResourceAsync{IAccount}(string, CancellationToken)"/>.</remarks>
        /// <param name="href">The resource URL of the <see cref="IAccount">Account</see> to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="IAccount">Account</see>.</returns>
        /// <exception cref="Error.ResourceException">The resource could not be found.</exception>
        Task<IAccount> GetAccountAsync(string href, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves the <see cref="IApplication">Application</see> at the specified Stormpath URL.
        /// </summary>
        /// <remarks>This is a convenience method equivalent to <see cref="DataStore.IDataStore.GetResourceAsync{IApplication}(string, CancellationToken)"/>.</remarks>
        /// <param name="href">The resource URL of the <see cref="IApplication">Application</see> to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="IApplication">Application</see>.</returns>
        /// <exception cref="Error.ResourceException">The resource could not be found.</exception>
        Task<IApplication> GetApplicationAsync(string href, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves the <see cref="IDirectory">Directory</see> at the specified Stormpath URL.
        /// </summary>
        /// <remarks>This is a convenience method equivalent to <see cref="DataStore.IDataStore.GetResourceAsync{IDirectory}(string, CancellationToken)"/>.</remarks>
        /// <param name="href">The resource URL of the <see cref="IDirectory">Directory</see> to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="IDirectory">Directory</see>.</returns>
        /// <exception cref="Error.ResourceException">The resource could not be found.</exception>
        Task<IDirectory> GetDirectoryAsync(string href, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves the <see cref="IGroup">Group</see> at the specified URL.
        /// </summary>
        /// <remarks>This is a convenience method equivalent to <see cref="DataStore.IDataStore.GetResourceAsync{IGroup}(string, CancellationToken)"/>.</remarks>
        /// <param name="href">The resource URL of the <see cref="IGroup">Group</see> to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="IGroup">Group</see>.</returns>
        /// <exception cref="Error.ResourceException">The resource could not be found.</exception>
        Task<IGroup> GetGroupAsync(string href, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves the <see cref="IOrganization">Organization</see> at the specified Stormpath URL.
        /// </summary>
        /// <remarks>This is a convenience method equivalent to <see cref="DataStore.IDataStore.GetResourceAsync{IOrganization}(string, CancellationToken)"/>.</remarks>
        /// <param name="href">The resource URL of the <see cref="IOrganization">Organization</see> to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="IOrganization">Organization</see>.</returns>
        /// <exception cref="Error.ResourceException">The resource could not be found.</exception>
        Task<IOrganization> GetOrganizationAsync(string href, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves the <see cref="IOrganization">Organization</see> by the Organization name.
        /// </summary>
        /// <remarks>This is a convenience method equivalent to <see cref="DataStore.IDataStore.GetResourceAsync{IOrganization}(string, CancellationToken)"/>.</remarks>
        /// <param name="nameKey">The name of the <see cref="IOrganization">Organization</see> to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="IOrganization">Organization</see>.</returns>
        /// <exception cref="Error.ResourceException">The resource could not be found.</exception>
        Task<IOrganization> GetOrganizationByNameAsync(string nameKey, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// Retrieves the <see cref="IAccessToken">Access Token</see> at the specified Stormpath URL.
        /// </summary>
        /// <remarks>This is a convenience method equivalent to <see cref="DataStore.IDataStore.GetResourceAsync{IAccessToken}(string, CancellationToken)"/>.</remarks>
        /// <param name="href">The resource URL of the <see cref="IAccessToken">Access Token</see> to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="IAccessToken">Access Token</see>.</returns>
        /// <exception cref="Error.ResourceException">The resource could not be found.</exception>
        Task<IAccessToken> GetAccessTokenAsync(string href, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves the <see cref="IRefreshToken">Refresh Token</see> at the specified Stormpath URL.
        /// </summary>
        /// <remarks>This is a convenience method equivalent to <see cref="DataStore.IDataStore.GetResourceAsync{IRefreshToken}(string, CancellationToken)"/>.</remarks>
        /// <param name="href">The resource URL of the <see cref="IRefreshToken">Refresh Token</see> to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="IRefreshToken">Refresh Token</see>.</returns>
        /// <exception cref="Error.ResourceException">The resource could not be found.</exception>
        Task<IRefreshToken> GetRefreshTokenAsync(string href, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a queryable list of all <see cref="IAccount">Accounts</see> in this tenant.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IAccount}"/> that may be used to asynchronously list or search Accounts.</returns>
        /// <example>
        /// var allAccounts = await myTenant.GetAccounts().ToListAsync();
        /// </example>
        IAsyncQueryable<IAccount> GetAccounts();

        /// <summary>
        /// Gets a queryable list of all <see cref="IApplication">Applications</see> in this tenant.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IApplication}"/> that may be used to asynchronously list or search Applications.</returns>
        /// <example>
        /// var allApplications = await myTenant.GetApplications().ToListAsync();
        /// </example>
        IAsyncQueryable<IApplication> GetApplications();

        /// <summary>
        /// Gets a queryable list of all <see cref="IDirectory">Directories</see> in this tenant.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IDirectory}"/> that may be used to asynchronously list or search Directories.</returns>
        /// <example>
        /// var allDirectories = await myTenant.GetDirectories().ToListAsync();
        /// </example>
        IAsyncQueryable<IDirectory> GetDirectories();

        /// <summary>
        /// Gets a queryable list of all groups in this tenant.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IGroup}"/> that may be used to asynchronously list or search Groups.</returns>
        /// <example>
        /// var allGroups = await myTenant.GetGroups().ToListAsync();
        /// </example>
        IAsyncQueryable<IGroup> GetGroups();

        /// <summary>
        /// Gets a queryable list of all <see cref="IOrganization">Organizations</see> in this tenant.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IOrganization}"/> that may be used to asynchronously list or search Organizations.</returns>
        /// <example>
        /// var allOrgs = await myTenant.GetOrganizations().ToListAsync();
        /// </example>
        IAsyncQueryable<IOrganization> GetOrganizations();
    }
}
