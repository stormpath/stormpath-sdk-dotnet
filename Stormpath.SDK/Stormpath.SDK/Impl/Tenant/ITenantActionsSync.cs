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
    internal interface ITenantActionsSync
    {
        IApplication CreateApplication(IApplication application, Action<ApplicationCreationOptionsBuilder> creationOptionsAction);

        IApplication CreateApplication(IApplication application, IApplicationCreationOptions creationOptions);

        IApplication CreateApplication(IApplication application);

        IApplication CreateApplication(string name, bool createDirectory);

        IDirectory CreateDirectory(IDirectory directory);

        IDirectory CreateDirectory(IDirectory directory, Action<DirectoryCreationOptionsBuilder> creationOptionsAction);

        IDirectory CreateDirectory(IDirectory directory, IDirectoryCreationOptions creationOptions);

        IDirectory CreateDirectory(string name, string description, DirectoryStatus status);

        IAccount VerifyAccountEmail(string token);
    }
}
