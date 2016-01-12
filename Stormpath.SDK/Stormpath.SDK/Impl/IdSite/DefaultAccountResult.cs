// <copyright file="DefaultAccountResult.cs" company="Stormpath, Inc.">
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

using System;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.IdSite;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.IdSite
{
    internal class DefaultAccountResult : AbstractResource, IAccountResult, IAccountResultSync
    {
        public static readonly string StatePropertyName = "state";
        public static readonly string NewAccountPropertyName = "isNewAccount";
        public static readonly string AccountPropertyName = "account";
        public static readonly string StatusPropertyName = "status";

        public DefaultAccountResult(ResourceData data)
            : base(data)
        {
        }

        internal IEmbeddedProperty Account => this.GetLinkProperty(AccountPropertyName);

        bool IAccountResult.IsNewAccount => this.GetBoolProperty(NewAccountPropertyName);

        string IAccountResult.State => this.GetStringProperty(StatePropertyName);

        IdSiteResultStatus IAccountResult.Status => this.GetProperty<IdSiteResultStatus>(StatusPropertyName);

        Task<IAccount> IAccountResult.GetAccountAsync(CancellationToken cancellationToken)
        {
            this.ThrowIfAccountNotPresent();

            return this.GetInternalAsyncDataStore().GetResourceAsync<IAccount>(this.Account.Href, cancellationToken);
        }

        IAccount IAccountResultSync.GetAccount()
        {
            this.ThrowIfAccountNotPresent();

            return this.GetInternalSyncDataStore().GetResource<IAccount>(this.Account.Href);
        }

        private void ThrowIfAccountNotPresent()
        {
            if (!this.ContainsProperty(AccountPropertyName))
            {
                throw new ApplicationException("The account is not present.");
            }
        }
    }
}
