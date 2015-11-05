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
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Impl.Jwt;
using Stormpath.SDK.Jwt;

namespace Stormpath.SDK.Impl.IdSite
{
    internal sealed class DefaultIdSiteAsyncCallbackHandler : IIdSiteAsyncCallbackHandler
    {
        private readonly IInternalDataStore internalDataStore;
        //private readonly IApplication application;
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
            //this.application = application;
            this.jwtResponse = GetJwtResponse(httpRequest);
            this.nonceStore = new DefaultNonceStore(internalDataStore.CacheResolver);
        }

        private static string GetJwtResponse(IHttpRequest request)
        {
            if (request.Method != HttpMethod.Get)
                throw new ApplicationException("Only HTTP GET method is supported.");

            var jwtResponse = request.CanonicalUri.QueryString[IdSiteClaims.JwtResponse];

            if (string.IsNullOrEmpty(jwtResponse))
                throw InvalidJwtException.JwtRequired;

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
            try
            {
                var dataStoreApiKey = this.internalDataStore.ApiKey.GetId();

                var jsonPayload = JWT.JsonWebToken.DecodeToObject<IDictionary<string, object>>(
                    this.jwtResponse, dataStoreApiKey, false);

                ThrowIfRequiredParametersMissing(jsonPayload);

                string apiKeyFromJwt = null;
                if (IsError(jsonPayload))
                    jsonPayload.TryGetValueAsString(DefaultJwtClaims.Id, out apiKeyFromJwt);
                else
                    jsonPayload.TryGetValueAsString(DefaultJwtClaims.Audience, out apiKeyFromJwt);

                ThrowIfJwtSignatureInvalid(apiKeyFromJwt, dataStoreApiKey, jsonPayload);
                //ThrowIfJwtIsExpired(jsonPayload);
            }
            catch (JWT.SignatureVerificationException vex)
            {
                // throw as correct SDK exception
                throw;
            }

            throw new NotImplementedException();
        }

        private static void ThrowIfRequiredParametersMissing(IDictionary<string, object> payload)
        {
            var requiredKeys = new string[]
            {
                DefaultJwtClaims.Id,
                DefaultJwtClaims.Audience,
                DefaultJwtClaims.Expiration,
                DefaultJwtClaims.Issuer,
                IdSiteClaims.ResponseId,
                IdSiteClaims.Status,
                IdSiteClaims.IsNewSubject,
            };

            bool valid = requiredKeys?.All(x => payload.ContainsKey(x)) ?? false;
            if (!valid)
                throw InvalidJwtException.ResponseMissingParameter;
        }

        private static void ThrowIfJwtSignatureInvalid(string jwtApiKey, string expectedApiKey, IDictionary<string, object> payload)
        {
            if (!expectedApiKey.Equals(jwtApiKey, StringComparison.InvariantCultureIgnoreCase))
                throw InvalidJwtException.ResponseInvalidApiKeyId;


        }

        private static bool IsError(IDictionary<string, object> payload)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            object error = null;

            return payload.TryGetValue(IdSiteClaims.Error, out error)
                && error != null;
        }
    }
}
