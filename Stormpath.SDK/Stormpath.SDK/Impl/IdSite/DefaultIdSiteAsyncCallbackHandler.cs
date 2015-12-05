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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Api;
using Stormpath.SDK.Http;
using Stormpath.SDK.IdSite;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Impl.Jwt;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Jwt;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.IdSite
{
    internal sealed class DefaultIdSiteAsyncCallbackHandler : IIdSiteAsyncCallbackHandler
    {
        private readonly IInternalDataStore internalDataStore;
        private readonly string jwtResponse;

        private INonceStore nonceStore;
        private IAsynchronousNonceStore asyncNonceStore;

        private IIdSiteAsyncResultListener resultListener;

        private IIdSiteAsyncCallbackHandler AsInterface => this;

        public DefaultIdSiteAsyncCallbackHandler(IInternalDataStore internalDataStore, IHttpRequest httpRequest)
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
            this.jwtResponse = GetJwtResponse(httpRequest);

            this.nonceStore = new DefaultNonceStore(internalDataStore.CacheResolver);
            this.asyncNonceStore = this.nonceStore as IAsynchronousNonceStore;
        }

        private static string GetJwtResponse(IHttpRequest request)
        {
            if (request.Method != HttpMethod.Get)
            {
                throw new ApplicationException("Only HTTP GET method is supported.");
            }

            var jwtResponse = request.CanonicalUri.QueryString[IdSiteClaims.JwtResponse];

            if (string.IsNullOrEmpty(jwtResponse))
            {
                throw InvalidJwtException.JwtRequired;
            }

            return jwtResponse;
        }

        IIdSiteAsyncCallbackHandler IIdSiteAsyncCallbackHandler.SetNonceStore(INonceStore nonceStore)
        {
            if (nonceStore == null)
            {
                throw new ArgumentNullException(nameof(nonceStore));
            }

            this.nonceStore = nonceStore;

            return this;
        }

        IIdSiteAsyncCallbackHandler IIdSiteAsyncCallbackHandler.SetResultListener(IIdSiteAsyncResultListener resultListener)
        {
            this.resultListener = resultListener;

            return this;
        }

        IIdSiteAsyncCallbackHandler IIdSiteAsyncCallbackHandler.SetResultListener(
            Func<IAccountResult, CancellationToken, Task> onRegistered,
            Func<IAccountResult, CancellationToken, Task> onAuthenticated,
            Func<IAccountResult, CancellationToken, Task> onLogout)
        {
            return this.AsInterface.SetResultListener(new InlineIdSiteAsyncResultListener(onRegistered, onAuthenticated, onLogout));
        }

        async Task<IAccountResult> IIdSiteAsyncCallbackHandler.GetAccountResultAsync(CancellationToken cancellationToken)
        {
            var dataStoreApiKey = this.internalDataStore.ApiKey;

            var jwt = JsonWebToken.Decode(this.jwtResponse, this.internalDataStore.Serializer);

            ThrowIfRequiredParametersMissing(jwt.Payload);

            string apiKeyFromJwt = null;
            if (IsError(jwt.Payload))
            {
                jwt.Header.TryGetValueAsString(JwtHeaderParameters.KeyId, out apiKeyFromJwt);
            }
            else
            {
                jwt.Payload.TryGetValueAsString(DefaultJwtClaims.Audience, out apiKeyFromJwt);
            }

            ThrowIfJwtSignatureInvalid(apiKeyFromJwt, dataStoreApiKey, jwt);
            ThrowIfJwtIsExpired(jwt.Payload);

            IfErrorThrowIdSiteException(jwt.Payload);

            if (!this.nonceStore.IsAsynchronousSupported || this.asyncNonceStore == null)
            {
                throw new ApplicationException("The current nonce store does not support asynchronous operations.");
            }

            var responseNonce = (string)jwt.Payload[IdSiteClaims.ResponseId];
            await this.ThrowIfNonceIsAlreadyUsedAsync(responseNonce, cancellationToken).ConfigureAwait(false);
            await this.asyncNonceStore.PutNonceAsync(responseNonce, cancellationToken).ConfigureAwait(false);

            ThrowIfSubjectIsMissing(jwt.Payload);

            var accountResult = CreateAccountResult(jwt.Payload, this.internalDataStore);
            var resultStatus = GetResultStatus(jwt.Payload);

            if (this.resultListener != null)
            {
                await this.DispatchResponseStatusAsync(resultStatus, accountResult, cancellationToken).ConfigureAwait(false);
            }

            return accountResult;
        }

        private Task DispatchResponseStatusAsync(IdSiteResultStatus status, IAccountResult accountResult, CancellationToken cancellationToken)
        {
            if (status == IdSiteResultStatus.Registered)
            {
                return this.resultListener.OnRegisteredAsync(accountResult, cancellationToken);
            }
            else if (status == IdSiteResultStatus.Authenticated)
            {
                return this.resultListener.OnAuthenticatedAsync(accountResult, cancellationToken);
            }
            else if (status == IdSiteResultStatus.Logout)
            {
                return this.resultListener.OnLogoutAsync(accountResult, cancellationToken);
            }

            throw new ArgumentException($"Encountered unknown ID Site result status: {status}");
        }

        internal static bool IsError(Map payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            object error = null;

            return payload.TryGetValue(IdSiteClaims.Error, out error)
                && error != null;
        }

        internal static string GetAccountHref(Map payload)
        {
            string accountHref = null;
            payload.TryGetValueAsString(DefaultJwtClaims.Subject, out accountHref);

            return accountHref;
        }

        internal static void ThrowIfRequiredParametersMissing(Map payload)
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

            bool isError = IsError(payload);
            bool valid = requiredKeys?.All(x => payload.ContainsKey(x)) ?? false;
            if (!isError && !valid)
            {
                throw InvalidJwtException.ResponseMissingParameter;
            }
        }

        internal static void ThrowIfJwtSignatureInvalid(string jwtApiKey, IClientApiKey clientApiKey, JsonWebToken jwt)
        {
            if (!clientApiKey.GetId().Equals(jwtApiKey, StringComparison.InvariantCultureIgnoreCase))
            {
                throw InvalidJwtException.ResponseInvalidApiKeyId;
            }

            if (!new JwtSignatureValidator(clientApiKey).IsValid(jwt))
            {
                throw InvalidJwtException.SignatureError;
            }
        }

        internal static void ThrowIfJwtIsExpired(Map payload)
        {
            var expiration = Convert.ToInt64(payload[DefaultJwtClaims.Expiration]);
            var now = UnixDate.ToLong(DateTimeOffset.Now);

            if (now > expiration)
            {
                throw InvalidJwtException.Expired;
            }
        }

        internal static void IfErrorThrowIdSiteException(Map payload)
        {
            if (!IsError(payload))
            {
                return;
            }

            var errorData = payload[IdSiteClaims.Error] as Map;
            if (errorData == null)
            {
                throw new ApplicationException("Error parsing ID Site error response.");
            }

            object codeRaw;
            int code;
            if (!errorData.TryGetValue("code", out codeRaw) ||
                !int.TryParse(codeRaw.ToString(), out code))
            {
                throw new ApplicationException($"Error type is unrecognized: '{codeRaw ?? "<null>"}'");
            }

            if (code == 10011
                || code == 10012
                || code == 11001
                || code == 11002
                || code == 11003)
            {
                throw new InvalidIdSiteTokenException(new Error.DefaultError(errorData));
            }

            if (code == 12001)
            {
                throw new IdSiteSessionTimeoutException(new Error.DefaultError(errorData));
            }

            // Default/fallback
            throw new IdSiteRuntimeException(new Error.DefaultError(errorData));
        }

        private async Task ThrowIfNonceIsAlreadyUsedAsync(string nonce, CancellationToken cancellationToken)
        {
            bool alreadyUsed = await this.asyncNonceStore.ContainsNonceAsync(nonce, cancellationToken).ConfigureAwait(false);
            if (alreadyUsed)
            {
                throw InvalidJwtException.AlreadyUsed;
            }
        }

        internal static void ThrowIfSubjectIsMissing(Map payload)
        {
            var sub = GetAccountHref(payload);
            bool subMissing = string.IsNullOrEmpty(sub);
            var resultStatus = GetResultStatus(payload);

            // The 'sub' claim (accountHref) can be null if calling /sso/logout when the subject is already logged out,
            // but this is only legal during the logout scenario, so assert:
            if (subMissing && resultStatus != IdSiteResultStatus.Logout)
            {
                throw InvalidJwtException.ResponseMissingParameter;
            }
        }

        internal static IAccountResult CreateAccountResult(Map payload, IInternalDataStore dataStore)
        {
            object state = null;
            payload.TryGetValue(IdSiteClaims.State, out state);

            bool isNewAccount = (bool)payload[IdSiteClaims.IsNewSubject];
            var resultStatus = GetResultStatus(payload);

            var properties = new Dictionary<string, object>()
            {
                [DefaultAccountResult.NewAccountPropertyName] = isNewAccount,
                [DefaultAccountResult.StatePropertyName] = state,
                [DefaultAccountResult.StatusPropertyName] = resultStatus,
            };

            var accountHref = GetAccountHref(payload);
            if (!string.IsNullOrEmpty(accountHref))
            {
                properties[DefaultAccountResult.AccountPropertyName] = new LinkProperty(accountHref);
            }

            return dataStore.InstantiateWithData<IAccountResult>(properties);
        }

        internal static IdSiteResultStatus GetResultStatus(Map payload)
            => IdSiteResultStatus.Parse((string)payload[IdSiteClaims.Status]);
    }
}
