// <copyright file="DefaultIdSiteUrlBuilder.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System;
using System.Text;
using Stormpath.SDK.IdSite;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Jwt;
using Stormpath.SDK.Jwt;

namespace Stormpath.SDK.Impl.IdSite
{
    internal sealed class DefaultIdSiteUrlBuilder : IIdSiteUrlBuilder
    {
        private static string logoutSuffix = "/logout";

        private readonly IInternalDataStore internalDataStore;
        private readonly string ssoEndpoint;
        private readonly string applicationHref;

        private string callbackUri;
        private string state;
        private string path;
        private bool logout = false;
        private string organizationNameKey;
        private bool? useSubdomain;
        private bool? showOrganizationField;

        public DefaultIdSiteUrlBuilder(IInternalDataStore internalDataStore, string applicationHref)
        {
            if (internalDataStore == null)
                throw new ArgumentNullException(nameof(internalDataStore));
            if (string.IsNullOrEmpty(applicationHref))
                throw new ArgumentNullException(nameof(applicationHref));

            this.internalDataStore = internalDataStore;
            this.applicationHref = applicationHref;
            this.ssoEndpoint = GetBaseUrl(applicationHref) + "/sso";
        }

        /// <summary>
        /// Trim off anything after the first single slash in a URL.
        /// <para>Example: http://foo.com/test becomes http://foo.com</para>
        /// </summary>
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
                throw new ApplicationException("ID Site base URL could not be constructed.", e);
            }

            if (string.IsNullOrEmpty(baseUrl))
                throw new ApplicationException("ID Site base URL could not be constructed.");

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
            this.path = path;
            return this;
        }

        IIdSiteUrlBuilder IIdSiteUrlBuilder.SetOrganizationNameKey(string organizationNameKey)
        {
            this.organizationNameKey = organizationNameKey;
            return this;
        }

        IIdSiteUrlBuilder IIdSiteUrlBuilder.SetUseSubdomain(bool useSubdomain)
        {
            this.useSubdomain = useSubdomain;
            return this;
        }

        IIdSiteUrlBuilder IIdSiteUrlBuilder.SetShowOrganizationField(bool showOrganizationField)
        {
            this.showOrganizationField = showOrganizationField;
            return this;
        }

        IIdSiteUrlBuilder IIdSiteUrlBuilder.SetState(string state)
        {
            this.state = state;
            return this;
        }

        string IIdSiteUrlBuilder.Build()
        {
            if (string.IsNullOrEmpty(this.callbackUri))
                throw new ApplicationException($"{nameof(this.callbackUri)} cannot be null or empty.");

            var jti = Guid.NewGuid().ToString();
            var now = DateTimeOffset.UtcNow;
            var apiKey = this.internalDataStore.ApiKey;

            var jwtBuilder = Jwts.NewClaimsBuilder()
                .SetId(jti)
                .SetIssuedAt(DateTimeOffset.Now)
                .SetIssuer(apiKey.GetId())
                .SetSubject(this.applicationHref)
                .SetClaim(IdSiteClaims.RedirectUri, this.callbackUri);

            if (!string.IsNullOrEmpty(this.path))
                jwtBuilder.SetClaim(IdSiteClaims.Path, this.path);

            if (!string.IsNullOrEmpty(this.state))
                jwtBuilder.SetClaim(IdSiteClaims.State, this.state);

            if (!string.IsNullOrEmpty(this.organizationNameKey))
                jwtBuilder.SetClaim(IdSiteClaims.OrganizationNameKey, this.organizationNameKey);

            if (this.useSubdomain.HasValue)
                jwtBuilder.SetClaim(IdSiteClaims.UseSubdomain, this.useSubdomain.Value);

            if (this.showOrganizationField.HasValue)
                jwtBuilder.SetClaim(IdSiteClaims.ShowOrganizationField, this.showOrganizationField.Value);

            string jwt = JWT.JsonWebToken.Encode(jwtBuilder.Build().ToDictionary(), apiKey.GetSecret(), JWT.JwtHashAlgorithm.HS256);

            var urlBuilder = new StringBuilder(this.ssoEndpoint);

            if (this.logout)
                urlBuilder.Append(logoutSuffix);

            urlBuilder.Append($"?{IdSiteClaims.JwtRequest}={jwt}");

            return urlBuilder.ToString();
        }
    }
}
