// <copyright file="DefaultIdSiteSyncCallbackHandler.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Http;
using Stormpath.SDK.IdSite;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Impl.Jwt;
using Stormpath.SDK.Jwt;
using HandlerShared = Stormpath.SDK.Impl.IdSite.DefaultIdSiteAsyncCallbackHandler;

namespace Stormpath.SDK.Impl.IdSite
{
    internal sealed class DefaultIdSiteSyncCallbackHandler : IIdSiteSyncCallbackHandler
    {
        private readonly IInternalDataStore internalDataStore;
        private readonly string jwtResponse;

        private INonceStore nonceStore;
        private ISynchronousNonceStore syncNonceStore;

        private IIdSiteSyncResultListener resultListener;

        private IIdSiteSyncCallbackHandler AsInterface => this;

        public DefaultIdSiteSyncCallbackHandler(IInternalDataStore internalDataStore, IHttpRequest httpRequest)
        {
            if (internalDataStore == null)
            {
                throw new ArgumentNullException(nameof(internalDataStore));
            }

            if (httpRequest == null)
            {
                throw new ArgumentNullException(nameof(httpRequest));
            }

            this.internalDataStore = internalDataStore;
            this.jwtResponse = HandlerShared.GetJwtResponse(httpRequest);

            this.nonceStore = new DefaultNonceStore(internalDataStore.CacheResolver);
            this.syncNonceStore = this.nonceStore as ISynchronousNonceStore;
        }

        IIdSiteSyncCallbackHandler IIdSiteSyncCallbackHandler.SetNonceStore(INonceStore nonceStore)
        {
            if (nonceStore == null)
            {
                throw new ArgumentNullException(nameof(nonceStore));
            }

            this.nonceStore = nonceStore;

            return this;
        }

        IIdSiteSyncCallbackHandler IIdSiteSyncCallbackHandler.SetResultListener(IIdSiteSyncResultListener resultListener)
        {
            this.resultListener = resultListener;

            return this;
        }

        IIdSiteSyncCallbackHandler IIdSiteSyncCallbackHandler.SetResultListener(
            Action<IAccountResult> onRegistered,
            Action<IAccountResult> onAuthenticated,
            Action<IAccountResult> onLogout)
        {
            return this.AsInterface.SetResultListener(new InlineIdSiteSyncResultListener(onRegistered, onAuthenticated, onLogout));
        }

        IAccountResult IIdSiteSyncCallbackHandler.GetAccountResult()
        {
            var signingKeyBytes = Encoding.UTF8.GetBytes(
                this.internalDataStore.ApiKey.GetSecret());

            IJwtParser parser = new DefaultJwtParser(this.internalDataStore.Serializer);
            var jwt = parser
                .SetSigningKey(signingKeyBytes)
                .Parse(this.jwtResponse);

            HandlerShared.ThrowIfRequiredParametersMissing(jwt.Body);

            string apiKeyFromJwt = null;
            if (HandlerShared.IsError(jwt.Body))
            {
                jwt.Header.TryGetValueAsString(JwtHeaderParameters.KeyId, out apiKeyFromJwt);
            }
            else
            {
                apiKeyFromJwt = (string)jwt.Body.GetClaim(DefaultJwtClaims.Audience);
            }

            HandlerShared.ThrowIfJwtSignatureInvalid(apiKeyFromJwt, this.internalDataStore.ApiKey, jwt);
            HandlerShared.ThrowIfJwtIsExpired(jwt.Body);

            HandlerShared.IfErrorThrowIdSiteException(jwt.Body);

            if (!this.nonceStore.IsAsynchronousSupported || this.syncNonceStore == null)
            {
                throw new Exception("The current nonce store does not support synchronous operations.");
            }

            var responseNonce = (string)jwt.Body.GetClaim(IdSiteClaims.ResponseId);
            this.ThrowIfNonceIsAlreadyUsed(responseNonce);
            this.syncNonceStore.PutNonce(responseNonce);

            HandlerShared.ThrowIfSubjectIsMissing(jwt.Body);

            var accountResult = HandlerShared.CreateAccountResult(jwt.Body, this.internalDataStore);
            var resultStatus = HandlerShared.GetResultStatus(jwt.Body);

            if (this.resultListener != null)
            {
                this.DispatchResponseStatus(resultStatus, accountResult);
            }

            return accountResult;
        }

        private void DispatchResponseStatus(IdSiteResultStatus status, IAccountResult accountResult)
        {
            if (status == IdSiteResultStatus.Registered)
            {
                this.resultListener.OnRegistered(accountResult);
                return;
            }
            else if (status == IdSiteResultStatus.Authenticated)
            {
                this.resultListener.OnAuthenticated(accountResult);
                return;
            }
            else if (status == IdSiteResultStatus.Logout)
            {
                this.resultListener.OnLogout(accountResult);
                return;
            }

            throw new ArgumentException($"Encountered unknown ID Site result status: {status}");
        }

        private void ThrowIfNonceIsAlreadyUsed(string nonce)
        {
            bool alreadyUsed = this.syncNonceStore.ContainsNonce(nonce);
            if (alreadyUsed)
            {
                throw new ExpiredJwtException("This JWT has already been used.");
            }
        }
    }
}
