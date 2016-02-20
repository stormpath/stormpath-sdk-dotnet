// <copyright file="DefaultSamlAccountResult.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Saml;

namespace Stormpath.SDK.Impl.Saml
{
    internal class DefaultSamlAccountResult : AbstractResource, ISamlAccountResult, ISamlAccountResultSync
    {
        public static readonly string StatePropertyName = "state";
        public static readonly string NewAccountPropertyName = "isNewAccount";
        public static readonly string AccountPropertyName = "account";
        public static readonly string StatusPropertyName = "status";

        public DefaultSamlAccountResult(ResourceData data)
            : base(data)
        {
        }

        internal IEmbeddedProperty Account => this.GetLinkProperty(AccountPropertyName);

        bool ISamlAccountResult.IsNewAccount => this.GetBoolProperty(NewAccountPropertyName);

        string ISamlAccountResult.State => this.GetStringProperty(StatePropertyName);

        SamlResultStatus ISamlAccountResult.Status => this.GetProperty<SamlResultStatus>(StatusPropertyName);

        Task<IAccount> ISamlAccountResult.GetAccountAsync(CancellationToken cancellationToken)
        {
            this.ThrowIfAccountNotPresent();

            return this.GetInternalAsyncDataStore().GetResourceAsync<IAccount>(this.Account.Href, cancellationToken);
        }

        IAccount ISamlAccountResultSync.GetAccount()
        {
            this.ThrowIfAccountNotPresent();

            return this.GetInternalSyncDataStore().GetResource<IAccount>(this.Account.Href);
        }

        private void ThrowIfAccountNotPresent()
        {
            if (!this.ContainsProperty(AccountPropertyName))
            {
                throw new Exception("The account is not present.");
            }
        }
    }
}
