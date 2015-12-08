// <copyright file="DefaultAccountStore.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.AccountStore
{
    internal sealed class DefaultAccountStore : AbstractInstanceResource, IAccountStore, IAccountStoreSync
    {
        public DefaultAccountStore(ResourceData data)
            : base(data)
        {
        }

        IAsyncQueryable<IAccount> IAccountStore.GetAccounts()
        {
            throw new ApplicationException("Access this resource through the IDirectory or IGroup interface to enumerate accounts.");
        }

        public new ITenant GetTenant()
        {
            throw new ApplicationException("Access this resource through the IDirectory or IGroup interface to get the current tenant.");
        }

        public new Task<ITenant> GetTenantAsync(CancellationToken cancellationToken)
        {
            throw new ApplicationException("Access this resource through the IDirectory or IGroup interface to get the current tenant.");
        }
    }
}
