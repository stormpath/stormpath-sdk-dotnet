// <copyright file="BasicAuthenticator.cs" company="Stormpath, Inc.">
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Shared.Extensions;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Auth
{
    internal sealed class BasicAuthenticator
    {
        private readonly IInternalDataStore dataStore;
        private readonly IInternalAsyncDataStore dataStoreAsync;
        private readonly IInternalSyncDataStore dataStoreSync;

        public BasicAuthenticator(IInternalDataStore dataStore)
        {
            this.dataStore = dataStore;
            this.dataStoreAsync = dataStore as IInternalAsyncDataStore;
            this.dataStoreSync = dataStore as IInternalSyncDataStore;

            if (this.dataStore == null ||
                this.dataStoreSync == null)
            {
                throw new ArgumentNullException("Internal data store could not be initialized.");
            }
        }

        private static void Validate(string parentHref, IAuthenticationRequest request)
        {
            if (string.IsNullOrEmpty(parentHref))
            {
                throw new ArgumentNullException(nameof(parentHref));
            }

            if (!(request is UsernamePasswordRequest))
            {
                throw new ArgumentException("Only UsernamePasswordRequest instances are supported by this by this authenticator.");
            }
        }

        private IBasicLoginAttempt BuildRequest(string parentHref, IAuthenticationRequest request)
        {
            var username = request.Principals.Nullable() ?? string.Empty;
            var password = request.Credentials.Nullable() ?? string.Empty;
            var value = $"{username}:{password}";
            value = Base64.Encode(value, Encoding.UTF8);

            var attempt = this.dataStore.Instantiate<IBasicLoginAttempt>();
            attempt.SetType("basic");
            attempt.SetValue(value);

            if (request.AccountStore != null)
            {
                attempt.SetAccountStore(request.AccountStore);
            }

            var supportsNameKey = request as IHasOrganizationNameKey;
            if (supportsNameKey != null && !string.IsNullOrEmpty(supportsNameKey.OrganizationNameKey))
            {
                attempt.SetAccountStore(supportsNameKey.OrganizationNameKey);
            }

            return attempt;
        }

        public Task<IAuthenticationResult> AuthenticateAsync(string parentHref, IAuthenticationRequest request, IRetrievalOptions<IAuthenticationResult> options, CancellationToken cancellationToken)
        {
            Validate(parentHref, request);

            var attempt = this.BuildRequest(parentHref, request);
            var href = $"{parentHref}/loginAttempts";

            return this.dataStoreAsync.CreateAsync<IBasicLoginAttempt, IAuthenticationResult>(href, attempt, options, null, cancellationToken);
        }

        public IAuthenticationResult Authenticate(string parentHref, IAuthenticationRequest request, IRetrievalOptions<IAuthenticationResult> options)
        {
            Validate(parentHref, request);

            var attempt = this.BuildRequest(parentHref, request);
            var href = $"{parentHref}/loginAttempts";

            return this.dataStoreSync.Create<IBasicLoginAttempt, IAuthenticationResult>(href, attempt, options, null);
        }
    }
}
