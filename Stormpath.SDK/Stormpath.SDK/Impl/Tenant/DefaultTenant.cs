// <copyright file="DefaultTenant.cs" company="Stormpath, Inc.">
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
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Tenant
{
    internal sealed class DefaultTenant : ITenant
    {
        string IResource.Href
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ITenant.Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application)
        {
            throw new NotImplementedException();
        }

        Task<IDirectory> ITenantActions.CreateDirectoryAsync(IDirectory directory)
        {
            throw new NotImplementedException();
        }

        Task<IAccountList> ITenantActions.GetAccountsAsync()
        {
            throw new NotImplementedException();
        }

        IApplicationAsyncList ITenantActions.GetApplications()
        {
            throw new NotImplementedException();
        }

        Task<IApplicationAsyncList> ITenantActions.GetApplicationsAsync()
        {
            throw new NotImplementedException();
        }

        Task<IDirectoryList> ITenantActions.GetDirectoriesAsync()
        {
            throw new NotImplementedException();
        }

        Task<IGroupList> ITenantActions.GetGroupsAsync()
        {
            throw new NotImplementedException();
        }

        Task<IAccount> ITenantActions.VerifyAccountEmailAsync()
        {
            throw new NotImplementedException();
        }
    }
}
