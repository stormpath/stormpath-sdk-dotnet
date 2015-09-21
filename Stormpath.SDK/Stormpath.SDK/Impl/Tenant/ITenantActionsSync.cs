// <copyright file="ITenantActionsSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Application;

namespace Stormpath.SDK.Impl.Tenant
{
    /// <summary>
    /// Represents common tenant actions that can be executed synchronously on a <see cref="ITenant"/> instance
    /// <i>or</i> a <see cref="Client.IClient"/> instance acting on behalf of its current tenant.
    /// </summary>
    internal interface ITenantActionsSync
    {
        /// <summary>
        /// Creates a new <see cref="IApplication"/> resource in the current tenant.
        /// </summary>
        /// <param name="application">The <see cref="IApplication"/> to create.</param>
        /// <param name="creationOptionsAction">An inline builder for an instance of <see cref="ApplicationCreationOptionsBuilder"/>,
        /// which will be used when sending the request.</param>
        /// <returns>The created <see cref="IApplication"/>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the application.</exception>
        IApplication CreateApplication(IApplication application, Action<ApplicationCreationOptionsBuilder> creationOptionsAction);

        /// <summary>
        /// Creates a new <see cref="IApplication"/> resource in the current tenant.
        /// </summary>
        /// <param name="application">The <see cref="IApplication"/> to create.</param>
        /// <param name="creationOptions">An <see cref="IApplicationCreationOptions"/> instance to use when sending the request.</param>
        /// <returns>The created <see cref="IApplication"/>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the application.</exception>
        IApplication CreateApplication(IApplication application, IApplicationCreationOptions creationOptions);

        /// <summary>
        /// Creates a new <see cref="IApplication"/> resource in the current tenant, with the default creation options.
        /// </summary>
        /// <param name="application">The <see cref="IApplication"/> to create.</param>
        /// <returns>The created <see cref="IApplication"/>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the application.</exception>
        IApplication CreateApplication(IApplication application);

        /// <summary>
        /// Creates a new <see cref="IApplication"/> resource in the current tenant.
        /// </summary>
        /// <param name="name">The name of the application.</param>
        /// <param name="createDirectory">Whether a default directory should be created automatically.</param>
        /// <returns>The created <see cref="IApplication"/>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the application.</exception>
        IApplication CreateApplication(string name, bool createDirectory);
    }
}
