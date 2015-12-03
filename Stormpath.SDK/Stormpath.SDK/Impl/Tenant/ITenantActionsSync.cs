// <copyright file="ITenantActionsSync.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Impl.Tenant
{
    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on <see cref="SDK.Tenant.ITenantActions"/>.
    /// </summary>
    internal interface ITenantActionsSync
    {
        /// <summary>
        /// Synchronous counterpart to <see cref="SDK.Tenant.ITenantActions.CreateApplicationAsync(IApplication, Action{ApplicationCreationOptionsBuilder}, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="application">The <see cref="IApplication"/> to create.</param>
        /// <param name="creationOptionsAction">An inline builder for an instance of <see cref="IApplicationCreationOptions"/>,
        /// which will be used when sending the request.</param>
        /// <returns>The created <see cref="IApplication"/>.</returns>
        IApplication CreateApplication(IApplication application, Action<ApplicationCreationOptionsBuilder> creationOptionsAction);

        /// <summary>
        /// Synchronous counterpart to <see cref="SDK.Tenant.ITenantActions.CreateApplicationAsync(IApplication, IApplicationCreationOptions, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="application">The <see cref="IApplication"/> to create.</param>
        /// <param name="creationOptions">An <see cref="IApplicationCreationOptions"/> instance to use when sending the request.</param>
        /// <returns>The created <see cref="IApplication"/>.</returns>
        IApplication CreateApplication(IApplication application, IApplicationCreationOptions creationOptions);

        /// <summary>
        /// Synchronous counterpart to <see cref="SDK.Tenant.ITenantActions.CreateApplicationAsync(IApplication, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="application">The <see cref="IApplication"/> to create.</param>
        /// <returns>The created <see cref="IApplication"/>.</returns>
        IApplication CreateApplication(IApplication application);

        /// <summary>
        /// Synchronous counterpart to <see cref="SDK.Tenant.ITenantActions.CreateApplicationAsync(string, bool, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="name">The name of the application.</param>
        /// <param name="createDirectory">Whether a default directory should be created automatically.</param>
        /// <returns>The created <see cref="IApplication"/>.</returns>
        IApplication CreateApplication(string name, bool createDirectory);

        /// <summary>
        /// Synchronous counterpart to <see cref="SDK.Tenant.ITenantActions.CreateDirectoryAsync(IDirectory, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="directory">The Directory resource to create.</param>
        /// <returns>The created <see cref="IDirectory"/>.</returns>
        IDirectory CreateDirectory(IDirectory directory);

        /// <summary>
        /// Synchronous counterpart to <see cref="SDK.Tenant.ITenantActions.CreateDirectoryAsync(IDirectory, Action{DirectoryCreationOptionsBuilder}, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="directory">The <see cref="IDirectory"/> to create.</param>
        /// <param name="creationOptionsAction">An inline builder for an instance of <see cref="IDirectoryCreationOptions"/>,
        /// which will be used when sending the request.</param>
        /// <returns>The created <see cref="IDirectory"/>.</returns>
        IDirectory CreateDirectory(IDirectory directory, Action<DirectoryCreationOptionsBuilder> creationOptionsAction);

        /// <summary>
        /// Synchronous counterpart to <see cref="SDK.Tenant.ITenantActions.CreateDirectoryAsync(IDirectory, IDirectoryCreationOptions, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="directory">The <see cref="IDirectory"/> to create.</param>
        /// <param name="creationOptions">A <see cref="IDirectoryCreationOptions"/> instance to use when sending the request.</param>
        /// <returns>The created <see cref="IDirectory"/>.</returns>
        IDirectory CreateDirectory(IDirectory directory, IDirectoryCreationOptions creationOptions);

        /// <summary>
        /// Synchronous counterpart to <see cref="SDK.Tenant.ITenantActions.CreateDirectoryAsync(string, string, DirectoryStatus, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="name">The directory name.</param>
        /// <param name="description">The directory description.</param>
        /// <param name="status">The initial directory status.</param>
        /// <returns>The created <see cref="IDirectory"/>.</returns>
        IDirectory CreateDirectory(string name, string description, DirectoryStatus status);

        /// <summary>
        /// Synchronous counterpart to <see cref="SDK.Tenant.ITenantActions.VerifyAccountEmailAsync(string, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="token">The <c>sptoken</c> query parameter value.</param>
        /// <returns>The verified account.</returns>
        IAccount VerifyAccountEmail(string token);
    }
}
