// <copyright file="OauthRequests.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.Oauth;

namespace Stormpath.SDK.Oauth
{
    /// <summary>
    /// Static entry point for working with OAuth 2.0 requests.
    /// </summary>
    public static class OauthRequests
    {
        /// <summary>
        /// Start a new Password Grant request.
        /// </summary>
        /// <returns>
        /// A new <see cref="IPasswordGrantRequestBuilder"/> instance, used to construct <see cref="IPasswordGrantRequest">Password Grant requests</see>.
        /// </returns>
        public static IPasswordGrantRequestBuilder NewPasswordGrantRequest()
            => new DefaultPasswordGrantRequestBuilder();

        /// <summary>
        /// Start a new JSON Web Token Authentication request.
        /// </summary>
        /// <returns>
        /// A new <see cref="IJwtAuthenticationRequestBuilder"/> instance, used to construct <see cref="IJwtAuthenticationRequest">JWT Authentication requests</see>.
        /// </returns>
        public static IJwtAuthenticationRequestBuilder NewJwtAuthenticationRequest()
            => new DefaultJwtAuthenticationRequestBuilder();

        /// <summary>
        /// Start a new ID Site Token Authentication request.
        /// </summary>
        /// <returns>
        /// A new <see cref="IIdSiteTokenAuthenticationRequestBuilder"/> instance, used to construct <see cref="IIdSiteTokenAuthenticationRequest">ID Site Token Authentication requests</see>.
        /// </returns>
        public static IIdSiteTokenAuthenticationRequestBuilder NewIdSiteTokenAuthenticationRequest()
            => new DefaultIdSiteTokenAuthenticationRequestBuilder();

        /// <summary>
        /// Start a new Refresh Grant request.
        /// </summary>
        /// <returns>
        /// A new <see cref="IRefreshGrantRequestBuilder"/> instance, used to construct <see cref="IRefreshGrantRequest">Refresh Grant requests</see>.
        /// </returns>
        public static IRefreshGrantRequestBuilder NewRefreshGrantRequest()
            => new DefaultRefreshGrantRequestBuilder();
    }
}
