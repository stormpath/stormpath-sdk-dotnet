// <copyright file="DefaultPasswordGrantAuthenticator.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Application;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Oauth;

namespace Stormpath.SDK.Impl.Oauth
{
    internal sealed class DefaultPasswordGrantAuthenticator : IPasswordGrantAuthenticator
    {
        private static readonly string OauthTokenPath = "/oauth/token";

        private readonly IApplication application;
        private readonly IInternalDataStore internalDataStore;

        public DefaultPasswordGrantAuthenticator(IApplication application, IInternalDataStore internalDataStore)
        {
            this.application = application;
            this.internalDataStore = internalDataStore;
        }

        private IInternalAsyncDataStore InternalAsyncDataStore
            => this.internalDataStore as IInternalAsyncDataStore;

        async Task<IOauthGrantAuthenticationResult> IOauthAuthenticator<IPasswordGrantRequest, IOauthGrantAuthenticationResult>
            .AuthenticateAsync(IPasswordGrantRequest authenticationRequest, CancellationToken cancellationToken)
        {
            if (this.application == null)
            {
                throw new ApplicationException($"{nameof(this.application)} cannot be null.");
            }

            if (authenticationRequest == null)
            {
                throw new ApplicationException($"{nameof(authenticationRequest)} cannot be null.");
            }

            var createGrantAttempt = this.internalDataStore.Instantiate<IGrantAuthenticationAttempt>();
            createGrantAttempt.SetLogin(authenticationRequest.Login);
            createGrantAttempt.SetPassword(authenticationRequest.Password);
            createGrantAttempt.SetGrantType(authenticationRequest.GrantType);

            if (!string.IsNullOrEmpty(authenticationRequest.AccountStoreHref))
            {
                createGrantAttempt.SetAccountStore(authenticationRequest.AccountStoreHref);
            }

            var headers = new HttpHeaders();
            headers.ContentType = HttpHeaders.MediaTypeApplicationFormUrlEncoded;

            var grantResult = await this.InternalAsyncDataStore.CreateAsync<IGrantAuthenticationAttempt, IGrantAuthenticationToken>(
                $"{this.application.Href}{OauthTokenPath}",
                createGrantAttempt,
                headers,
                cancellationToken).ConfigureAwait(false);

            return grantResult;
        }
    }
}
