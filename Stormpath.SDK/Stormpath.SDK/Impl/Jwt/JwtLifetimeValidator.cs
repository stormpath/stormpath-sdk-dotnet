// <copyright file="JwtLifetimeValidator.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Jwt;

namespace Stormpath.SDK.Impl.Jwt
{
    internal sealed class JwtLifetimeValidator
    {
        private readonly DateTimeOffset now;

        public JwtLifetimeValidator(DateTimeOffset now)
        {
            this.now = now;
        }

        public void Validate(IJwt jwt)
        {
            if (jwt?.Body == null)
            {
                throw new ArgumentNullException(nameof(jwt));
            }

            this.ValidateExpiration(jwt);
            this.ValidateNotBefore(jwt);
        }

        /// <summary>
        /// Ensures the token is not accepted on or after any specified <c>exp</c> time.
        /// </summary>
        /// <see href="https://tools.ietf.org/html/draft-ietf-oauth-json-web-token-30#section-4.1.4"/>
        /// <param name="jwt">The JWT.</param>
        private void ValidateExpiration(IJwt jwt)
        {
            var expiration = jwt.Body.Expiration;
            if (expiration != null && expiration >= this.now)
            {
                var msg = $"JWT expired at {Iso8601.Format(expiration.Value)}. Current time: {Iso8601.Format(this.now)}";
                throw new ExpiredJwtException(msg);
            }
        }

        /// <summary>
        /// Ensures the token is not accepted before any specified <c>nbf</c> time.
        /// </summary>
        /// <see href="https://tools.ietf.org/html/draft-ietf-oauth-json-web-token-30#section-4.1.5"/>
        /// <param name="jwt">The JWT.</param>
        private void ValidateNotBefore(IJwt jwt)
        {
            var notBefore = jwt.Body.NotBefore;
            if (notBefore != null && this.now < notBefore)
            {
                var msg = $"JWT must not be accepted before {Iso8601.Format(notBefore.Value)}. Current time: {Iso8601.Format(this.now)}";
                throw new PrematureJwtException(msg);
            }
        }
    }
}
