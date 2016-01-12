// <copyright file="DefaultTenant.Collections.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Organization;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Tenant
{
    internal sealed partial class DefaultTenant
    {
        IAsyncQueryable<IApplication> ITenantActions.GetApplications()
            => new CollectionResourceQueryable<IApplication>(this.Applications.Href, this.GetInternalAsyncDataStore());

        IAsyncQueryable<IDirectory> ITenantActions.GetDirectories()
            => new CollectionResourceQueryable<IDirectory>(this.Directories.Href, this.GetInternalAsyncDataStore());

        IAsyncQueryable<IAccount> ITenantActions.GetAccounts()
            => new CollectionResourceQueryable<IAccount>(this.Accounts.Href, this.GetInternalAsyncDataStore());

        IAsyncQueryable<IGroup> ITenantActions.GetGroups()
            => new CollectionResourceQueryable<IGroup>(this.Groups.Href, this.GetInternalAsyncDataStore());

        IAsyncQueryable<IOrganization> ITenantActions.GetOrganizations()
            => new CollectionResourceQueryable<IOrganization>(this.Organizations.Href, this.GetInternalAsyncDataStore());
    }
}
