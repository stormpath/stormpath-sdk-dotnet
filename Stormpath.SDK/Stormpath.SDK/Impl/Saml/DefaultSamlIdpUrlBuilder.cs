// <copyright file="DefaultSamlIdpUrlBuilder.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.IdSite;
using Stormpath.SDK.Impl.Jwt;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Jwt;
using Stormpath.SDK.Saml;

namespace Stormpath.SDK.Impl.Saml
{
    internal sealed class DefaultSamlIdpUrlBuilder : ISamlIdpUrlBuilder
    {
        private static string logoutSuffix = "/logout";

        private readonly IInternalDataStore internalDataStore;
        private readonly string applicationHref;
        private readonly string ssoEndpoint;
        private readonly IJwtBuilder jwtBuilder;
        private readonly IIdSiteJtiProvider jtiProvider;
        private readonly IClock clock;

        private string callbackUri;
        private string state;
        private string path;
        private string organizationNameKey;
        private string stormpathSpToken;

        public DefaultSamlIdpUrlBuilder(
            IInternalDataStore internalDataStore,
            string applicationHref,
            string samlProviderEndpoint,
            IIdSiteJtiProvider jtiProvider,
            IClock clock)
        {
            if (internalDataStore == null)
            {
                throw new ArgumentNullException(nameof(internalDataStore));
            }

            if (string.IsNullOrEmpty(applicationHref))
            {
                throw new ArgumentNullException(nameof(applicationHref));
            }

            if (string.IsNullOrEmpty(samlProviderEndpoint))
            {
                throw new ArgumentNullException(nameof(samlProviderEndpoint));
            }

            if (jtiProvider == null)
            {
                throw new ArgumentNullException(nameof(jtiProvider));
            }

            if (clock == null)
            {
                throw new ArgumentNullException(nameof(clock));
            }

            this.internalDataStore = internalDataStore;
            this.applicationHref = applicationHref;
            this.ssoEndpoint = samlProviderEndpoint;
            this.jwtBuilder = new DefaultJwtBuilder(this.internalDataStore.Serializer);
            this.jtiProvider = jtiProvider;
            this.clock = clock;
        }

        ISamlIdpUrlBuilder ISamlIdpUrlBuilder.SetCallbackUri(Uri callbackUri)
        {
            this.callbackUri = callbackUri?.ToString();
            return this;
        }

        ISamlIdpUrlBuilder ISamlIdpUrlBuilder.SetCallbackUri(string callbackUri)
        {
            this.callbackUri = callbackUri;
            return this;
        }

        ISamlIdpUrlBuilder ISamlIdpUrlBuilder.SetState(string state)
        {
            this.state = state;
            return this;
        }

        ISamlIdpUrlBuilder ISamlIdpUrlBuilder.SetPath(string path)
        {
            this.path = path;
            return this;
        }

        ISamlIdpUrlBuilder ISamlIdpUrlBuilder.SetOrganizationNameKey(string organizationNameKey)
        {
            this.organizationNameKey = organizationNameKey;
            return this;
        }

        ISamlIdpUrlBuilder ISamlIdpUrlBuilder.SetSpToken(string spToken)
        {
            this.stormpathSpToken = spToken;
            return this;
        }

        ISamlIdpUrlBuilder ISamlIdpUrlBuilder.AddProperty(string name, object value)
        {
            this.jwtBuilder.SetClaim(name, value);
            return this;
        }

        string ISamlIdpUrlBuilder.Build()
        {
            if (string.IsNullOrEmpty(this.callbackUri))
            {
                throw new ApplicationException("CallbackUri cannot be null or empty.");
            }

            var jti = this.jtiProvider.NewJti();
            var apiKey = this.internalDataStore.ApiKey;

            this.jwtBuilder
                .SetId(jti)
                .SetIssuedAt(this.clock.Now)
                .SetIssuer(apiKey.GetId())
                .SetSubject(this.applicationHref)
                .SetClaim(IdSiteClaims.RedirectUri, this.callbackUri);

            if (!string.IsNullOrEmpty(this.path))
            {
                this.jwtBuilder.SetClaim(IdSiteClaims.Path, this.path);
            }

            if (!string.IsNullOrEmpty(this.state))
            {
                this.jwtBuilder.SetClaim(IdSiteClaims.State, this.state);
            }

            if (!string.IsNullOrEmpty(this.organizationNameKey))
            {
                this.jwtBuilder.SetClaim(IdSiteClaims.OrganizationNameKey, this.organizationNameKey);
            }

            if (!string.IsNullOrEmpty(this.stormpathSpToken))
            {
                this.jwtBuilder.SetClaim(IdSiteClaims.SpToken, this.stormpathSpToken);
            }

            var jwt = this.jwtBuilder
                .SetHeaderParameter(JwtHeaderParameters.KeyId, apiKey.GetId())
                .SignWith(apiKey.GetSecret(), Encoding.UTF8)
                .Build()
                .ToString();

            var urlBuilder = new StringBuilder(this.ssoEndpoint)
                .Append("?")
                .Append(IdSiteClaims.AccessToken)
                .Append("=")
                .Append(jwt);

            return urlBuilder.ToString();
        }
    }
}
