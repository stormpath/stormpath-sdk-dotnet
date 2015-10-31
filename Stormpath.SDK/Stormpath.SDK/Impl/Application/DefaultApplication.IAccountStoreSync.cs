// <copyright file="DefaultApplication.IAccountStoreSync.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Sync;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed partial class DefaultApplication
    {
        IAccountStore IApplicationSync.GetDefaultAccountStore()
        {
            if (this.DefaultAccountStoreMapping.Href == null)
                return null;

            var accountStoreMapping = this.GetInternalAsyncDataStore().GetResource<IAccountStoreMapping>(this.DefaultAccountStoreMapping.Href);
            if (accountStoreMapping == null)
                return null;

            return accountStoreMapping.GetAccountStore();
        }

        IAccountStore IApplicationSync.GetDefaultGroupStore()
        {
            if (this.DefaultGroupStoreMapping.Href == null)
                return null;

            var groupStoreMapping = this.GetInternalSyncDataStore().GetResource<IAccountStoreMapping>(this.DefaultAccountStoreMapping.Href);

            return groupStoreMapping == null
                ? null
                : groupStoreMapping.GetAccountStore();
        }
    }
}
