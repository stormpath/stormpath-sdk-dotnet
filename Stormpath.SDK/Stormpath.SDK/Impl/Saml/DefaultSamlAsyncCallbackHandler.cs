// <copyright file="DefaultSamlAsyncCallbackHandler.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Api;
using Stormpath.SDK.Http;
using Stormpath.SDK.IdSite;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Impl.IdSite;
using Stormpath.SDK.Impl.Jwt;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Jwt;
using Stormpath.SDK.Saml;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.Saml
{
    internal sealed class DefaultSamlAsyncCallbackHandler : ISamlAsyncCallbackHandler
    {
        private readonly IInternalDataStore internalDataStore;
        private readonly string jwtResponse;

        private INonceStore nonceStore;
        private IAsynchronousNonceStore asyncNonceStore;

        private ISamlAsyncResultListener resultListener;

        private ISamlAsyncCallbackHandler AsInterface => this;

        public DefaultSamlAsyncCallbackHandler(IInternalDataStore internalDataStore, IHttpRequest httpRequest)
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

        internal static string GetJwtResponse(IHttpRequest request)
        {
            if (request.Method != HttpMethod.Get)
            {
                throw new Exception("Only HTTP GET method is supported.");
            }

            var jwtResponse = request.CanonicalUri.QueryString[IdSiteClaims.JwtResponse];

            if (string.IsNullOrEmpty(jwtResponse))
            {
                throw new InvalidJwtException("JWT parameter is required.");
            }

            return jwtResponse;
        }

        ISamlAsyncCallbackHandler ISamlAsyncCallbackHandler.SetNonceStore(INonceStore nonceStore)
        {
            if (nonceStore == null)
            {
                throw new ArgumentNullException(nameof(nonceStore));
            }

            this.nonceStore = nonceStore;

            return this;
        }

        ISamlAsyncCallbackHandler ISamlAsyncCallbackHandler.SetResultListener(ISamlAsyncResultListener resultListener)
        {
            this.resultListener = resultListener;

            return this;
        }

        ISamlAsyncCallbackHandler ISamlAsyncCallbackHandler.SetResultListener(
            Func<ISamlAccountResult, CancellationToken, Task> onAuthenticated,
            Func<ISamlAccountResult, CancellationToken, Task> onLogout)
        {
            return this.AsInterface.SetResultListener(new InlineSamlAsyncResultListener(onAuthenticated, onLogout));
        }

        async Task<ISamlAccountResult> ISamlAsyncCallbackHandler.GetAccountResultAsync(CancellationToken cancellationToken)
        {
            var signingKeyBytes = Encoding.UTF8.GetBytes(
                this.internalDataStore.ApiKey.GetSecret());

            IJwtParser parser = new DefaultJwtParser(this.internalDataStore.Serializer);
            var jwt = parser
                .SetSigningKey(signingKeyBytes)
                .Parse(this.jwtResponse);

            ThrowIfRequiredParametersMissing(jwt.Body);

            string apiKeyFromJwt = null;
            if (IsError(jwt.Body))
            {
                jwt.Header.TryGetValueAsString(JwtHeaderParameters.KeyId, out apiKeyFromJwt);
            }
            else
            {
                apiKeyFromJwt = (string)jwt.Body.GetClaim(DefaultJwtClaims.Audience);
            }

            ThrowIfJwtSignatureInvalid(apiKeyFromJwt, this.internalDataStore.ApiKey, jwt);
            ThrowIfJwtIsExpired(jwt.Body);

            IfErrorThrowSamlException(jwt.Body);

            if (!this.nonceStore.IsAsynchronousSupported || this.asyncNonceStore == null)
            {
                throw new Exception("The current nonce store does not support asynchronous operations.");
            }

            var responseNonce = (string)jwt.Body.GetClaim(IdSiteClaims.ResponseId);
            await this.ThrowIfNonceIsAlreadyUsedAsync(responseNonce, cancellationToken).ConfigureAwait(false);
            await this.asyncNonceStore.PutNonceAsync(responseNonce, cancellationToken).ConfigureAwait(false);

            ThrowIfSubjectIsMissing(jwt.Body);

            var accountResult = CreateAccountResult(jwt.Body, this.internalDataStore);
            var resultStatus = GetResultStatus(jwt.Body);

            if (this.resultListener != null)
            {
                await this.DispatchResponseStatusAsync(resultStatus, accountResult, cancellationToken).ConfigureAwait(false);
            }

            return accountResult;
        }

        private Task DispatchResponseStatusAsync(SamlResultStatus status, ISamlAccountResult accountResult, CancellationToken cancellationToken)
        {
            if (status == SamlResultStatus.Authenticated)
            {
                return this.resultListener.OnAuthenticatedAsync(accountResult, cancellationToken);
            }
            else if (status == SamlResultStatus.Logout)
            {
                return this.resultListener.OnLogoutAsync(accountResult, cancellationToken);
            }

            throw new ArgumentException($"Encountered unknown SAML result status: {status}");
        }

        internal static bool IsError(IJwtClaims claims)
        {
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            return claims.GetClaim(IdSiteClaims.Error) != null;
        }

        internal static string GetAccountHref(IJwtClaims claims)
            => claims.GetClaim(DefaultJwtClaims.Subject)?.ToString();

        internal static void ThrowIfRequiredParametersMissing(IJwtClaims claims)
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

            bool isError = IsError(claims);

            bool valid = requiredKeys?.All(x => claims.ContainsClaim(x)) ?? false;
            if (!isError && !valid)
            {
                throw new MissingClaimException("Required response parameter is missing.");
            }
        }

        internal static void ThrowIfJwtSignatureInvalid(string jwtApiKey, IClientApiKey clientApiKey, IJwt jwt)
        {
            if (!clientApiKey.GetId().Equals(jwtApiKey, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new JwtSignatureException("The client used to sign the response is different than the one used in this DataStore.");
            }

            var signingKey = Encoding.UTF8.GetBytes(clientApiKey.GetSecret());
            if (!new JwtSignatureValidator(signingKey).IsValid(jwt))
            {
                throw new JwtSignatureException("The JWT signature is invalid.");
            }
        }

        internal static void ThrowIfJwtIsExpired(IJwtClaims claims)
        {
            var validator = new JwtLifetimeValidator(DateTimeOffset.Now);

            validator.Validate(claims);
        }

        internal static void IfErrorThrowSamlException(IJwtClaims claims)
        {
            if (!IsError(claims))
            {
                return;
            }

            var errorData = claims.GetClaim(IdSiteClaims.Error) as Map;
            if (errorData == null)
            {
                throw new Exception("Error parsing SAML error response.");
            }

            object codeRaw;
            int code;
            if (!errorData.TryGetValue("code", out codeRaw) ||
                !int.TryParse(codeRaw.ToString(), out code))
            {
                throw new SamlException(new Error.DefaultError(errorData));
            }

            throw new SamlException(new Error.DefaultError(errorData));
        }

        private async Task ThrowIfNonceIsAlreadyUsedAsync(string nonce, CancellationToken cancellationToken)
        {
            bool alreadyUsed = await this.asyncNonceStore.ContainsNonceAsync(nonce, cancellationToken).ConfigureAwait(false);
            if (alreadyUsed)
            {
                throw new ExpiredJwtException("This JWT has already been used.");
            }
        }

        internal static void ThrowIfSubjectIsMissing(IJwtClaims claims)
        {
            var sub = GetAccountHref(claims);
            bool subMissing = string.IsNullOrEmpty(sub);
            var resultStatus = GetResultStatus(claims);

            // The 'sub' claim (accountHref) can be null if calling /sso/logout when the subject is already logged out,
            // but this is only legal during the logout scenario, so assert:
            if (subMissing && resultStatus != SamlResultStatus.Logout)
            {
                throw new MissingClaimException("Required response parameter is missing.");
            }
        }

        internal static ISamlAccountResult CreateAccountResult(IJwtClaims claims, IInternalDataStore dataStore)
        {
            var state = claims.GetClaim(IdSiteClaims.State);

            bool isNewAccount = (bool)claims.GetClaim(IdSiteClaims.IsNewSubject);
            var resultStatus = GetResultStatus(claims);

            var properties = new Dictionary<string, object>()
            {
                [DefaultAccountResult.NewAccountPropertyName] = isNewAccount,
                [DefaultAccountResult.StatePropertyName] = state,
                [DefaultAccountResult.StatusPropertyName] = resultStatus,
            };

            var accountHref = GetAccountHref(claims);
            if (!string.IsNullOrEmpty(accountHref))
            {
                properties[DefaultAccountResult.AccountPropertyName] = new LinkProperty(accountHref);
            }

            return dataStore.InstantiateWithData<ISamlAccountResult>(properties);
        }

        internal static SamlResultStatus GetResultStatus(IJwtClaims claims)
            => SamlResultStatus.Parse((string)claims.GetClaim(IdSiteClaims.Status));
    }
}
