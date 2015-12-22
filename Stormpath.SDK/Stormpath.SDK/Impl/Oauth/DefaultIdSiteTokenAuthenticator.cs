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
        IIdSiteTokenAuthenticator,
        IIdSiteTokenAuthenticatorSync
    {
        private static readonly string OauthTokenPath = "/oauth/token";

        private readonly IApplication application;
        private readonly IInternalDataStore internalDataStore;

        public DefaultIdSiteTokenAuthenticator(IApplication application, IInternalDataStore internalDataStore)
        {
            this.application = application;
            this.internalDataStore = internalDataStore;
        }

        private IInternalAsyncDataStore InternalAsyncDataStore
            => this.internalDataStore as IInternalAsyncDataStore;

        private IInternalSyncDataStore InternalSyncDataStore
            => this.internalDataStore as IInternalSyncDataStore;

        Task<IOauthGrantAuthenticationResult> IOauthAuthenticator<IIdSiteTokenAuthenticationRequest, IOauthGrantAuthenticationResult>
            .AuthenticateAsync(IIdSiteTokenAuthenticationRequest authenticationRequest, CancellationToken cancellationToken)
        {
            this.ThrowIfInvalid(authenticationRequest);

            throw new NotImplementedException();
        }

        IOauthGrantAuthenticationResult IOauthAuthenticatorSync<IIdSiteTokenAuthenticationRequest, IOauthGrantAuthenticationResult>
            .Authenticate(IIdSiteTokenAuthenticationRequest authenticationRequest)
        {
            this.ThrowIfInvalid(authenticationRequest);

            throw new NotImplementedException();
        }

        private void ThrowIfInvalid(IIdSiteTokenAuthenticationRequest authenticationRequest)
        {
            if (this.application == null)
            {
                throw new ApplicationException($"{nameof(this.application)} cannot be null.");
            }

            if (authenticationRequest == null)
            {
                throw new ApplicationException($"{nameof(authenticationRequest)} cannot be null.");
            }
        }

        private static HttpHeaders GetHeaders()
        {
            var headers = new HttpHeaders();
            headers.ContentType = HttpHeaders.MediaTypeApplicationFormUrlEncoded;
            return headers;
        }
    }
}
