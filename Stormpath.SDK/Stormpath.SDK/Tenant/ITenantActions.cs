// <copyright file="ITenantActions.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Tenant
{
    public interface ITenantActions
    {
        Task<IApplication> CreateApplicationAsync(IApplication application, Action<ApplicationCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken = default(CancellationToken));

        Task<IApplication> CreateApplicationAsync(IApplication application, IApplicationCreationOptions creationOptions, CancellationToken cancellationToken = default(CancellationToken));

        Task<IApplication> CreateApplicationAsync(IApplication application, CancellationToken cancellationToken = default(CancellationToken));

        Task<IApplication> CreateApplicationAsync(string name, bool createDirectory, CancellationToken cancellationToken = default(CancellationToken));

        Task<IDirectory> CreateDirectoryAsync(IDirectory directory, CancellationToken cancellationToken = default(CancellationToken));

        Task<IDirectory> CreateDirectoryAsync(string name, CancellationToken cancellationToken = default(CancellationToken));

        ICollectionResourceQueryable<IAccount> GetAccounts();

        ICollectionResourceQueryable<IApplication> GetApplications();

        ICollectionResourceQueryable<IDirectory> GetDirectories();

        ICollectionResourceQueryable<IGroup> GetGroups();

        Task<IAccount> VerifyAccountEmailAsync(string token, CancellationToken cancellationToken = default(CancellationToken));
    }
}
