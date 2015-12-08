// <copyright file="DefaultClient.TenantActionsSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Sync;

namespace Stormpath.SDK.Impl.Client
{
    internal sealed partial class DefaultClient
    {
        IApplication ITenantActionsSync.CreateApplication(IApplication application, Action<ApplicationCreationOptionsBuilder> creationOptionsAction)
        {
            this.EnsureTenant();

            return this.tenant.CreateApplication(application, creationOptionsAction);
        }

        IApplication ITenantActionsSync.CreateApplication(IApplication application, IApplicationCreationOptions creationOptions)
        {
            this.EnsureTenant();

            return this.tenant.CreateApplication(application, creationOptions);
        }

        IApplication ITenantActionsSync.CreateApplication(IApplication application)
        {
            this.EnsureTenant();

            return this.tenant.CreateApplication(application);
        }

        IApplication ITenantActionsSync.CreateApplication(string name, bool createDirectory)
        {
            this.EnsureTenant();

            return this.tenant.CreateApplication(name, createDirectory);
        }

        IDirectory ITenantActionsSync.CreateDirectory(IDirectory directory)
        {
            this.EnsureTenant();

            return this.tenant.CreateDirectory(directory);
        }

        IDirectory ITenantActionsSync.CreateDirectory(IDirectory directory, Action<DirectoryCreationOptionsBuilder> creationOptionsAction)
        {
            this.EnsureTenant();

            return this.tenant.CreateDirectory(directory, creationOptionsAction);
        }

        IDirectory ITenantActionsSync.CreateDirectory(IDirectory directory, IDirectoryCreationOptions creationOptions)
        {
            this.EnsureTenant();

            return this.tenant.CreateDirectory(directory, creationOptions);
        }

        IDirectory ITenantActionsSync.CreateDirectory(string name, string description, DirectoryStatus status)
        {
            this.EnsureTenant();

            return this.tenant.CreateDirectory(name, description, status);
        }

        IAccount ITenantActionsSync.VerifyAccountEmail(string token)
        {
            this.EnsureTenant();

            return this.tenant.VerifyAccountEmail(token);
        }
    }
}
