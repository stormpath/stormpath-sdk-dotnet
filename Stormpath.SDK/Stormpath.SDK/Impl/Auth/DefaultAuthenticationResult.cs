// <copyright file="DefaultAuthenticationResult.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Auth
{
    internal sealed class DefaultAuthenticationResult : AbstractResource, IAuthenticationResult, IAuthenticationResultSync
    {
        private static readonly string AccountPropertyName = "account";

        public DefaultAuthenticationResult(IInternalDataStore dataStore)
            : base(dataStore)
        {
        }

        public DefaultAuthenticationResult(IInternalDataStore dataStore, IDictionary<string, object> properties)
            : base(dataStore, properties)
        {
        }

        internal LinkProperty Account => this.GetLinkProperty(AccountPropertyName);

        Task<IAccount> IAuthenticationResult.GetAccountAsync(CancellationToken cancellationToken)
            => this.GetInternalDataStore().GetResourceAsync<IAccount>(this.Account.Href, cancellationToken);

        IAccount IAuthenticationResultSync.GetAccount()
            => this.GetInternalDataStoreSync().GetResource<IAccount>(this.Account.Href);
    }
}
