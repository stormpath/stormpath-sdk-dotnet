// <copyright file="DefaultIdSiteUrlBuilder.cs" company="Stormpath, Inc.">
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
using System.Text;
using Stormpath.SDK.IdSite;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Jwt;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Jwt;

namespace Stormpath.SDK.Impl.IdSite
{
    internal sealed class DefaultIdSiteUrlBuilder : IIdSiteUrlBuilder
    {
        private static string logoutSuffix = "/logout";

        private readonly IInternalDataStore internalDataStore;
        private readonly string ssoEndpoint;
        private readonly string applicationHref;

        private readonly IIdSiteJtiProvider jtiProvider;
        private readonly IClock clock;

        private bool logout = false;
        private string callbackUri = string.Empty;
        private Dictionary<string, object> claims = new Dictionary<string, object>();

        public DefaultIdSiteUrlBuilder(IInternalDataStore internalDataStore, string applicationHref, IIdSiteJtiProvider jtiProvider, IClock clock)
        {
            if (internalDataStore == null)
            {
                throw new ArgumentNullException(nameof(internalDataStore));
            }

            if (string.IsNullOrEmpty(applicationHref))
            {
                throw new ArgumentNullException(nameof(applicationHref));
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
            this.jtiProvider = jtiProvider;
            this.clock = clock;
            this.applicationHref = applicationHref;
            this.ssoEndpoint = GetBaseUrl(applicationHref) + "/sso";
        }

        /// <summary>
        /// Trim off anything after the first single slash in a URL.
        /// </summary>
        /// <remarks>
        /// For example, http://foo.com/test becomes http://foo.com
        /// </remarks>
        /// <param name="href">The URL to trim.</param>
        /// <returns><paramref name="href"/> up to the first single slash.</returns>
        private static string GetBaseUrl(string href)
        {
            var baseUrl = string.Empty;

            try
            {
                var doubleSlashIndex = href.IndexOf("//");
                var singleSlashIndex = href.IndexOf("/", doubleSlashIndex + "//".Length);
                singleSlashIndex = singleSlashIndex == -1
                    ? href.Length
                    : singleSlashIndex;

                baseUrl = href.Substring(0, singleSlashIndex);
            }
            catch (Exception e)
            {
                throw new Exception("ID Site base URL could not be constructed.", e);
            }

            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new Exception("ID Site base URL could not be constructed.");
            }

            return baseUrl;
        }

        IIdSiteUrlBuilder IIdSiteUrlBuilder.ForLogout()
        {
            this.logout = true;
            return this;
        }

        IIdSiteUrlBuilder IIdSiteUrlBuilder.SetCallbackUri(string callbackUri)
        {
            this.callbackUri = callbackUri;
            return this;
        }

        IIdSiteUrlBuilder IIdSiteUrlBuilder.SetCallbackUri(Uri callbackUri)
        {
            this.callbackUri = callbackUri.ToString();
            return this;
        }

        IIdSiteUrlBuilder IIdSiteUrlBuilder.SetPath(string path)
        {
            this.claims.Add(IdSiteClaims.Path, path);
            return this;
        }

        IIdSiteUrlBuilder IIdSiteUrlBuilder.SetOrganizationNameKey(string organizationNameKey)
        {
            this.claims.Add(IdSiteClaims.OrganizationNameKey, organizationNameKey);
            return this;
        }

        IIdSiteUrlBuilder IIdSiteUrlBuilder.SetUseSubdomain(bool useSubdomain)
        {
            this.claims.Add(IdSiteClaims.UseSubdomain, useSubdomain);
            return this;
        }

        IIdSiteUrlBuilder IIdSiteUrlBuilder.SetShowOrganizationField(bool showOrganizationField)
        {
            this.claims.Add(IdSiteClaims.ShowOrganizationField, showOrganizationField);
            return this;
        }

        IIdSiteUrlBuilder IIdSiteUrlBuilder.SetState(string state)
        {
            this.claims.Add(IdSiteClaims.State, state);
            return this;
        }

        IIdSiteUrlBuilder IIdSiteUrlBuilder.SetSpToken(string sptoken)
        {
            this.claims.Add(IdSiteClaims.SpToken, sptoken);
            return this;
        }

        IIdSiteUrlBuilder IIdSiteUrlBuilder.SetClaim(string key, object value)
        {
            this.claims.Add(key, value);
            return this;
        }

        string IIdSiteUrlBuilder.Build()
        {
            if (string.IsNullOrEmpty(this.callbackUri))
            {
                throw new Exception($"{nameof(this.callbackUri)} cannot be null or empty.");
            }

            var jti = this.jtiProvider.NewJti();
            var apiKey = this.internalDataStore.ApiKey;

            IJwtBuilder jwtBuilder = new DefaultJwtBuilder(this.internalDataStore.Serializer);
            jwtBuilder
                .SetId(jti)
                .SetIssuedAt(this.clock.Now)
                .SetIssuer(apiKey.GetId())
                .SetSubject(this.applicationHref)
                .SetClaim(IdSiteClaims.RedirectUri, this.callbackUri)
                .SignWith(apiKey.GetSecret(), Encoding.UTF8);

            foreach (var claim in this.claims)
            {
                jwtBuilder.SetClaim(claim.Key, claim.Value);
            }

            string jwt = jwtBuilder.Build()
                .ToString();

            var urlBuilder = new StringBuilder(this.ssoEndpoint);

            if (this.logout)
            {
                urlBuilder.Append(logoutSuffix);
            }

            urlBuilder.Append($"?{IdSiteClaims.JwtRequest}={jwt}");

            return urlBuilder.ToString();
        }
    }
}
