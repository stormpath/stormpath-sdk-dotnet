// <copyright file="DefaultEmailVerificationRequest.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Impl.Account
{
    internal sealed class DefaultEmailVerificationRequest : AbstractResource, IEmailVerificationRequest, IEmailVerificationRequestSync
    {
        private static readonly string LoginPropertyName = "login";
        private static readonly string AccountStorePropertyName = "accountStore";

        public DefaultEmailVerificationRequest(ResourceData data)
            : base(data)
        {
        }

        string IEmailVerificationRequest.Login
            => this.GetProperty<string>(LoginPropertyName);

        public IEmailVerificationRequest SetLogin(string usernameOrEmail)
        {
            this.SetProperty(LoginPropertyName, usernameOrEmail);
            return this;
        }

        public IEmailVerificationRequest SetAccountStore(IAccountStore accountStore)
        {
            if (string.IsNullOrEmpty(accountStore?.Href))
            {
                throw new ArgumentNullException(accountStore.Href);
            }

            this.SetLinkProperty(AccountStorePropertyName, accountStore.Href);
            return this;
        }

        Task<IAccountStore> IEmailVerificationRequest.GetAccountStoreAsync(CancellationToken cancellationToken)
        {
            var accountStoreHref = this.GetLinkProperty(AccountStorePropertyName)?.Href;
            if (string.IsNullOrEmpty(accountStoreHref))
            {
                return null;
            }

            return this.GetInternalAsyncDataStore().GetResourceAsync<IAccountStore>(accountStoreHref);
        }

        IAccountStore IEmailVerificationRequestSync.GetAccountStore()
        {
            var accountStoreHref = this.GetLinkProperty(AccountStorePropertyName)?.Href;
            if (string.IsNullOrEmpty(accountStoreHref))
            {
                return null;
            }

            return this.GetInternalSyncDataStore().GetResource<IAccountStore>(accountStoreHref);
        }
    }
}
