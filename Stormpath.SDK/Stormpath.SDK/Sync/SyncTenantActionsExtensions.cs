// <copyright file="SyncTenantActionsExtensions.cs" company="Stormpath, Inc.">
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

using System;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Impl.Tenant;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on <see cref="ITenantActions"/>.
    /// </summary>
    public static class SyncTenantActionsExtensions
    {
        /// <summary>
        /// Synchronously creates a new <see cref="IApplication"/> resource in the current tenant.
        /// </summary>
        /// <param name="tenantActions">The object supporting the <see cref="ITenantActions"/> interface.</param>
        /// <param name="application">The <see cref="IApplication"/> to create.</param>
        /// <param name="creationOptionsAction">An inline builder for an instance of <see cref="ApplicationCreationOptionsBuilder"/>,
        /// which will be used when sending the request.</param>
        /// <returns>The created <see cref="IApplication"/>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the application.</exception>
        public static IApplication CreateApplication(this ITenantActions tenantActions, IApplication application, Action<ApplicationCreationOptionsBuilder> creationOptionsAction)
            => (tenantActions as ITenantActionsSync).CreateApplication(application, creationOptionsAction);

        /// <summary>
        /// Synchronously creates a new <see cref="IApplication"/> resource in the current tenant.
        /// </summary>
        /// <param name="tenantActions">The object supporting the <see cref="ITenantActions"/> interface.</param>
        /// <param name="application">The <see cref="IApplication"/> to create.</param>
        /// <param name="creationOptions">An <see cref="IApplicationCreationOptions"/> instance to use when sending the request.</param>
        /// <returns>The created <see cref="IApplication"/>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the application.</exception>
        public static IApplication CreateApplication(this ITenantActions tenantActions, IApplication application, IApplicationCreationOptions creationOptions)
            => (tenantActions as ITenantActionsSync).CreateApplication(application, creationOptions);

        /// <summary>
        /// Synchronously creates a new <see cref="IApplication"/> resource in the current tenant, with the default creation options.
        /// </summary>
        /// <param name="tenantActions">The object supporting the <see cref="ITenantActions"/> interface.</param>
        /// <param name="application">The <see cref="IApplication"/> to create.</param>
        /// <returns>The created <see cref="IApplication"/>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the application.</exception>
        public static IApplication CreateApplication(this ITenantActions tenantActions, IApplication application)
            => (tenantActions as ITenantActionsSync).CreateApplication(application);

        /// <summary>
        /// Synchronously creates a new <see cref="IApplication"/> resource in the current tenant.
        /// </summary>
        /// <param name="tenantActions">The object supporting the <see cref="ITenantActions"/> interface.</param>
        /// <param name="name">The name of the application.</param>
        /// <param name="createDirectory">Whether a default directory should be created automatically.</param>
        /// <returns>The created <see cref="IApplication"/>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the application.</exception>
        public static IApplication CreateApplication(this ITenantActions tenantActions, string name, bool createDirectory)
            => (tenantActions as ITenantActionsSync).CreateApplication(name, createDirectory);

        /// <summary>
        /// Synchronously creates a new Cloud Directory resource in the Tenant.
        /// </summary>
        /// <param name="tenantActions">The object supporting the <see cref="ITenantActions"/> interface.</param>
        /// <param name="directory">The Directory resource to create.</param>
        /// <returns>The created <see cref="Directory.IDirectory"/>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the directory.</exception>
        public static IDirectory CreateDirectory(this ITenantActions tenantActions, IDirectory directory)
            => (tenantActions as ITenantActionsSync).CreateDirectory(directory);

        /// <summary>
        /// Synchronously creates a new Provider-based Directory resource in the Tenant.
        /// </summary>
        /// <param name="tenantActions">The object supporting the <see cref="ITenantActions"/> interface.</param>
        /// <param name="directory">The <see cref="Directory.IDirectory"/> to create.</param>
        /// <param name="creationOptionsAction">An inline builder for an instance of <see cref="IDirectoryCreationOptions"/>,
        /// which will be used when sending the request.</param>
        /// <returns>The created <see cref="Directory.IDirectory"/>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the directory.</exception>
        public static IDirectory CreateDirectory(this ITenantActions tenantActions, IDirectory directory, Action<DirectoryCreationOptionsBuilder> creationOptionsAction)
            => (tenantActions as ITenantActionsSync).CreateDirectory(directory, creationOptionsAction);

        /// <summary>
        /// Synchronously creates a new Cloud- or Provider-based Directory resource in the Tenant.
        /// </summary>
        /// <param name="tenantActions">The object supporting the <see cref="ITenantActions"/> interface.</param>
        /// <param name="directory">The <see cref="Directory.IDirectory"/> to create.</param>
        /// <param name="creationOptions">A <see cref="IDirectoryCreationOptions"/> instance to use when sending the request.</param>
        /// <returns>The created <see cref="Directory.IDirectory"/>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the directory.</exception>
        public static IDirectory CreateDirectory(this ITenantActions tenantActions, IDirectory directory, IDirectoryCreationOptions creationOptions)
            => (tenantActions as ITenantActionsSync).CreateDirectory(directory, creationOptions);

        /// <summary>
        /// Synchronously creates a new Cloud Directory resource in the Tenant.
        /// </summary>
        /// <param name="tenantActions">The object supporting the <see cref="ITenantActions"/> interface.</param>
        /// <param name="name">The directory name.</param>
        /// <param name="description">The directory description.</param>
        /// <param name="status">The initial directory status.</param>
        /// <returns>The created <see cref="Directory.IDirectory"/>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the directory.</exception>
        public static IDirectory CreateDirectory(this ITenantActions tenantActions, string name, string description, DirectoryStatus status)
            => (tenantActions as ITenantActionsSync).CreateDirectory(name, description, status);

        /// <summary>
        /// Synchronously verifies an account's email address based on a <c>sptoken</c> parameter embedded in a clickable URL
        /// found in an account's verification email.
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
        /// </summary>
        /// <param name="tenantActions">The object supporting the <see cref="ITenantActions"/> interface.</param>
        /// <param name="token">The <c>sptoken</c> query parameter value.</param>
        /// <returns>The verified account.</returns>
        /// <exception cref="Error.ResourceException">The token was not valid.</exception>
        public static IAccount VerifyAccountEmail(this ITenantActions tenantActions, string token)
            => (tenantActions as ITenantActionsSync).VerifyAccountEmail(token);
    }
}
