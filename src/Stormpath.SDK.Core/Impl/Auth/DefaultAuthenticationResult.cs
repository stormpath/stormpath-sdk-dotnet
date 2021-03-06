﻿// <copyright file="DefaultAuthenticationResult.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Auth
{
    internal sealed class DefaultAuthenticationResult : AbstractResource, IAuthenticationResult, IAuthenticationResultSync
    {
        private static readonly string AccountPropertyName = "account";

        public DefaultAuthenticationResult(ResourceData data)
            : base(data)
        {
        }

        internal IEmbeddedProperty Account => this.GetLinkProperty(AccountPropertyName);

        bool IAuthenticationResult.Success
            => !string.IsNullOrEmpty(this?.Account?.Href);

        string IAuthenticationResult.AccountHref
            => this?.Account?.Href;

        Task<IAccount> IAuthenticationResult.GetAccountAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<IAccount>(this.Account?.Href, cancellationToken);

        IAccount IAuthenticationResultSync.GetAccount()
            => this.GetInternalSyncDataStore().GetResource<IAccount>(this.Account?.Href);
    }
}
