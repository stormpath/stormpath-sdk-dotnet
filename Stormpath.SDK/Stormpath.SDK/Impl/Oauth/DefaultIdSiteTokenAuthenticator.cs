// <copyright file="DefaultIdSiteTokenAuthenticator.cs" company="Stormpath, Inc.">
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
    internal sealed class DefaultIdSiteTokenAuthenticator :
        AbstractGrantAuthenticator<IIdSiteTokenAuthenticationRequest>,
        IIdSiteTokenAuthenticator,
        IIdSiteTokenAuthenticatorSync
    {
        public DefaultIdSiteTokenAuthenticator(IApplication application, IInternalDataStore internalDataStore)
            : base(application, internalDataStore)
        {
        }

        async Task<IOauthGrantAuthenticationResult> IOauthAuthenticator<IIdSiteTokenAuthenticationRequest, IOauthGrantAuthenticationResult>
            .AuthenticateAsync(IIdSiteTokenAuthenticationRequest authenticationRequest, CancellationToken cancellationToken)
        {
            this.ThrowIfInvalid(authenticationRequest);

            var idsiteExchangeAttempt = this.BuildExchangeAttempt(authenticationRequest);
            var headers = this.GetHeaderWithMediaType();

            return await this.InternalAsyncDataStore.CreateAsync<IIdSiteTokenAuthenticationAttempt, IGrantAuthenticationToken>(
                $"{this.application.Href}{OauthTokenPath}",
                idsiteExchangeAttempt,
                headers,
                cancellationToken).ConfigureAwait(false);
        }

        IOauthGrantAuthenticationResult IOauthAuthenticatorSync<IIdSiteTokenAuthenticationRequest, IOauthGrantAuthenticationResult>
            .Authenticate(IIdSiteTokenAuthenticationRequest authenticationRequest)
        {
            this.ThrowIfInvalid(authenticationRequest);

            var idsiteExchangeAttempt = this.BuildExchangeAttempt(authenticationRequest);
            var headers = this.GetHeaderWithMediaType();

            return this.InternalSyncDataStore.Create<IIdSiteTokenAuthenticationAttempt, IGrantAuthenticationToken>(
                $"{this.application.Href}{OauthTokenPath}",
                idsiteExchangeAttempt,
                headers);
        }

        private IIdSiteTokenAuthenticationAttempt BuildExchangeAttempt(IIdSiteTokenAuthenticationRequest authenticationRequest)
        {
            var exchangeAttempt = this.internalDataStore.Instantiate<IIdSiteTokenAuthenticationAttempt>();
            exchangeAttempt.SetGrantType(authenticationRequest.GrantType);
            exchangeAttempt.SetToken(authenticationRequest.Jwt);

            return exchangeAttempt;
        }
    }
}
