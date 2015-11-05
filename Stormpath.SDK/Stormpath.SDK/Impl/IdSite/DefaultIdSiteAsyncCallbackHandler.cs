// <copyright file="DefaultIdSiteAsyncCallbackHandler.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Application;
using Stormpath.SDK.Http;
using Stormpath.SDK.IdSite;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Jwt;

namespace Stormpath.SDK.Impl.IdSite
{
    internal sealed class DefaultIdSiteAsyncCallbackHandler : IIdSiteAsyncCallbackHandler
    {
        private readonly IInternalDataStore internalDataStore;
        private readonly IApplication application;
        private readonly string jwtResponse;

        private INonceStore nonceStore;
        private IIdSiteResultAsyncListener resultListener;

        public DefaultIdSiteAsyncCallbackHandler(IInternalDataStore internalDataStore, IApplication application, IHttpRequest httpRequest)
        {
            if (internalDataStore == null)
                throw new ArgumentNullException(nameof(internalDataStore));
            if (application == null)
                throw new ArgumentNullException(nameof(application));
            if (httpRequest == null)
                throw new ArgumentNullException(nameof(httpRequest));

            this.internalDataStore = internalDataStore;
            this.application = application;
            this.jwtResponse = GetJwtResponse(httpRequest);
            //this.nonceStore = new DefaultNonceStore(internalDataStore.GetCacheResolver());
        }

        private static string GetJwtResponse(IHttpRequest request)
        {
            if (request.Method != HttpMethod.Get)
                throw new ApplicationException("Only HTTP GET method is supported.");

            var jwtResponse = request.CanonicalUri.QueryString[IdSiteClaims.JwtResponse];

            //if (string.IsNullOrEmpty(jwtResponse))
            //    throw new InvalidJwtException(InvalidJwtException.JwtRequired);

            return jwtResponse;
        }

        IIdSiteAsyncCallbackHandler IIdSiteAsyncCallbackHandler.SetNonceStore(INonceStore nonceStore)
        {
            if (nonceStore == null)
                throw new ArgumentNullException(nameof(nonceStore));

            this.nonceStore = nonceStore;

            return this;
        }

        IIdSiteAsyncCallbackHandler IIdSiteAsyncCallbackHandler.SetResultListener(IIdSiteResultAsyncListener resultListener)
        {
            this.resultListener = resultListener;

            return this;
        }

        Task<IAccountResult> IIdSiteAsyncCallbackHandler.ProcessRequestAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            //var jsonPayload = JWT.JsonWebToken.DecodeToObject<IDictionary<string, object>>()
        }
    }
}
