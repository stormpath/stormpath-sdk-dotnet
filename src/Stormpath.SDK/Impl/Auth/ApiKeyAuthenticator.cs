// <copyright file="ApiKeyAuthenticator.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Api;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Sync;

namespace Stormpath.SDK.Impl.Auth
{
    internal sealed class ApiKeyAuthenticator
    {
        private readonly IInternalAsyncDataStore asyncDataStore;
        private readonly IInternalSyncDataStore syncDataStore;

        public ApiKeyAuthenticator(IInternalDataStore dataStore)
        {
            this.asyncDataStore = dataStore as IInternalAsyncDataStore;
            this.syncDataStore = dataStore as IInternalSyncDataStore;
        }

        public async Task<IAuthenticationResult> AuthenticateAsync(IApplication application, IAuthenticationRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var id = request.Principals;
            var secret = request.Credentials;

            var foundApiKey = await application.GetApiKeyAsync(id, opt => opt.Expand(e => e.GetAccount()), cancellationToken).ConfigureAwait(false);

            ThrowIfApiKeyInvalid(foundApiKey, secret);

            var account = await foundApiKey.GetAccountAsync(cancellationToken).ConfigureAwait(false);

            ThrowIfAccountInvalid(account);

            return this.BuildResult(account);
        }

        public IAuthenticationResult Authenticate(IApplication application, IAuthenticationRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var id = request.Principals;
            var secret = request.Credentials;

            var foundApiKey = application.GetApiKey(id, opt => opt.Expand(e => e.GetAccount()));

            ThrowIfApiKeyInvalid(foundApiKey, secret);

            var account = foundApiKey.GetAccount();

            ThrowIfAccountInvalid(account);

            return this.BuildResult(account);
        }

        private static void ThrowIfApiKeyInvalid(IApiKey foundApiKey, string expectedSecret)
        {
            if (foundApiKey == null || !foundApiKey.Secret.Equals(expectedSecret, StringComparison.Ordinal))
            {
                throw new IncorrectCredentialsException();
            }

            if (foundApiKey.Status == ApiKeyStatus.Disabled)
            {
                throw new DisabledApiKeyException();
            }
        }

        private static void ThrowIfAccountInvalid(IAccount account)
        {
            if (account.Status != AccountStatus.Enabled)
            {
                throw new DisabledAccountException(account.Status);
            }
        }

        private IAuthenticationResult BuildResult(IAccount account)
        {
            var properties = new Dictionary<string, object>()
            {
                ["account"] = new LinkProperty(account.Href)
            };

            return this.asyncDataStore.InstantiateWithData<IAuthenticationResult>(properties);
        }
    }
}
