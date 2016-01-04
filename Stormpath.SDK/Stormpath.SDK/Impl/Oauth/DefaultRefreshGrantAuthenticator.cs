// <copyright file="DefaultRefreshGrantAuthenticator.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Oauth;

namespace Stormpath.SDK.Impl.Oauth
{
    internal sealed class DefaultRefreshGrantAuthenticator :
        AbstractGrantAuthenticator<IRefreshGrantRequest>,
        IRefreshGrantAuthenticator,
        IRefreshGrantAuthenticatorSync
    {
        public DefaultRefreshGrantAuthenticator(IApplication application, IInternalDataStore internalDataStore)
            : base(application, internalDataStore)
        {
        }

        async Task<IOauthGrantAuthenticationResult> IOauthAuthenticator<IRefreshGrantRequest, IOauthGrantAuthenticationResult>
            .AuthenticateAsync(IRefreshGrantRequest authenticationRequest, CancellationToken cancellationToken)
        {
            this.ThrowIfInvalid(authenticationRequest);

            var refreshGrantAttempt = this.BuildGrantAttempt(authenticationRequest);
            var headers = this.GetHeaderWithMediaType();

            return await this.InternalAsyncDataStore.CreateAsync<IRefreshGrantAuthenticationAttempt, IGrantAuthenticationToken>(
                $"{this.application.Href}{OauthTokenPath}",
                refreshGrantAttempt,
                headers,
                cancellationToken).ConfigureAwait(false);
        }

        IOauthGrantAuthenticationResult IOauthAuthenticatorSync<IRefreshGrantRequest, IOauthGrantAuthenticationResult>
            .Authenticate(IRefreshGrantRequest authenticationRequest)
        {
            this.ThrowIfInvalid(authenticationRequest);

            var refreshGrantAttempt = this.BuildGrantAttempt(authenticationRequest);
            var headers = this.GetHeaderWithMediaType();

            return this.InternalSyncDataStore.Create<IRefreshGrantAuthenticationAttempt, IGrantAuthenticationToken>(
                $"{this.application.Href}{OauthTokenPath}",
                refreshGrantAttempt,
                headers);
        }

        private IRefreshGrantAuthenticationAttempt BuildGrantAttempt(IRefreshGrantRequest authenticationRequest)
        {
            var refreshGrantRequest = this.internalDataStore.Instantiate<IRefreshGrantAuthenticationAttempt>();
            refreshGrantRequest.SetRefreshToken(authenticationRequest.RefreshToken);

            return refreshGrantRequest;
        }
    }
}
