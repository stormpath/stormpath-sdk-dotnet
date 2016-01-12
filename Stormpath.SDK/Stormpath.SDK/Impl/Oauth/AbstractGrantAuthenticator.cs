// <copyright file="AbstractGrantAuthenticator.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Application;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Oauth;

namespace Stormpath.SDK.Impl.Oauth
{
    internal abstract class AbstractGrantAuthenticator<T>
        where T : IOauthGrantRequest
    {
        protected static readonly string OauthTokenPath = "/oauth/token";

        protected readonly IApplication application;
        protected readonly IInternalDataStore internalDataStore;

        public AbstractGrantAuthenticator(IApplication application, IInternalDataStore internalDataStore)
        {
            this.application = application;
            this.internalDataStore = internalDataStore;
        }

        protected IInternalAsyncDataStore InternalAsyncDataStore
            => this.internalDataStore as IInternalAsyncDataStore;

        protected IInternalSyncDataStore InternalSyncDataStore
            => this.internalDataStore as IInternalSyncDataStore;

        protected void ThrowIfInvalid(T authenticationRequest)
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

        protected HttpHeaders GetHeaderWithMediaType()
        {
            var headers = new HttpHeaders();
            headers.ContentType = HttpHeaders.MediaTypeApplicationFormUrlEncoded;
            return headers;
        }
    }
}
