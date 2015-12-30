// <copyright file="DefaultJwtParser.cs" company="Stormpath, Inc.">
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
using System.Text;
using Stormpath.SDK.Impl.Client;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Jwt;
using Stormpath.SDK.Serialization;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.Jwt
{
    internal sealed class DefaultJwtParser : IJwtParser
    {
        private readonly IJwtClaimsBuilder expectedClaims;
        private readonly IJsonSerializer serializer;

        private byte[] keyBytes;

        public DefaultJwtParser(IJsonSerializer serializer)
        {
            this.expectedClaims = new DefaultJwtClaimsBuilder();
            this.serializer = serializer;
        }

        internal DefaultJwtParser()
            : this(((IJsonSerializerBuilder)new DefaultJsonSerializerBuilder()).Build())
        {
        }

        IJwtParser IJwtParser.RequireAudience(string audience)
        {
            this.expectedClaims.SetAudience(audience);

            return this;
        }

        IJwtParser IJwtParser.RequireClaim(string claimName, object value)
        {
            this.expectedClaims.SetClaim(claimName, value);

            return this;
        }

        IJwtParser IJwtParser.RequireExpiration(DateTimeOffset expiration)
        {
            this.expectedClaims.SetExpiration(expiration);

            return this;
        }

        IJwtParser IJwtParser.RequireId(string id)
        {
            this.expectedClaims.SetId(id);

            return this;
        }

        IJwtParser IJwtParser.RequireIssuedAt(DateTimeOffset issuedAt)
        {
            this.expectedClaims.SetIssuedAt(issuedAt);

            return this;
        }

        IJwtParser IJwtParser.RequireIssuer(string issuer)
        {
            this.expectedClaims.SetIssuer(issuer);

            return this;
        }

        IJwtParser IJwtParser.RequireNotBefore(DateTimeOffset notBefore)
        {
            this.expectedClaims.SetNotBeforeDate(notBefore);

            return this;
        }

        IJwtParser IJwtParser.RequireSubject(string subject)
        {
            this.expectedClaims.SetSubject(subject);

            return this;
        }

        IJwtParser IJwtParser.SetSigningKey(byte[] signingKey)
        {
            this.keyBytes = signingKey;

            return this;
        }

        IJwtParser IJwtParser.SetSigningKey(string base64EncodedSigningKey)
        {
            this.keyBytes = Convert.FromBase64String(base64EncodedSigningKey);

            return this;
        }

        IJwt IJwtParser.Parse(string jwt)
        {
            if (string.IsNullOrEmpty(jwt))
            {
                throw new ArgumentNullException("JWT string cannot be null or empty.");
            }

            var parts = jwt.Split(DefaultJwt.Separator);
            if (parts.Length != 3)
            {
                throw new MalformedJwtException("Token must consist of exactly 3 delimited parts.");
            }

            var base64UrlEncodedHeader = parts[0];
            var base64UrlEncodedPayload = parts[1];
            var base64UrlEncodedDigest = parts[2];

            if (string.IsNullOrEmpty(base64UrlEncodedPayload))
            {
                throw new MalformedJwtException("JWT string is missing a body/payload.");
            }

            Map header = this.serializer.Deserialize(Base64.Decode(base64UrlEncodedHeader, Encoding.UTF8));
            Map payload = this.serializer.Deserialize(Base64.Decode(base64UrlEncodedPayload, Encoding.UTF8));
            IJwtClaims claims = new DefaultJwtClaims(payload);

            // Validate signing algorithm. Currently only HS256 is supported
            string algorithm = null;
            if (!header.TryGetValueAsString(JwtHeaderParameters.Algorithm, out algorithm)
                || !algorithm.Equals("HS256", StringComparison.OrdinalIgnoreCase))
            {
                throw new MalformedJwtException($"JWT uses unsupported signing algorithm '{algorithm}'. Only HS256 is supported.");
            }

            // Validate the signature
            if (this.keyBytes != null)
            {
                var signatureValidator = new JwtSignatureValidator(this.keyBytes);
                if (!signatureValidator.IsValid(
                    base64UrlEncodedHeader,
                    base64UrlEncodedPayload,
                    base64UrlEncodedDigest))
                {
                    throw new JwtSignatureException("JWT signature does not match locally computed signature. JWT validity cannot be asserted and should not be trusted.");
                }
            }

            var jwtInstance = new DefaultJwt(header, claims, base64UrlEncodedHeader, base64UrlEncodedPayload, base64UrlEncodedDigest);

            // Validate expected claims
            this.ValidateExpectedClaims(jwtInstance);

            return jwtInstance;
        }

        private void ValidateExpectedClaims(IJwt jwt)
        {
            var expected = this.expectedClaims.Build().ToDictionary();
            var actual = jwt.Body.ToDictionary();

            foreach (var claim in expected)
            {
                object actualValue = null;
                if (!actual.TryGetValue(claim.Key, out actualValue))
                {
                    throw new MissingClaimException($"The claim '{claim.Key}' was expected, but was not found in the JWT.");
                }

                if (claim.Value != actualValue)
                {
                    throw new InvalidClaimException($"The claim '{claim.Key}' should have a value of '{claim.Value}', but instead was '{actualValue}'.");
                }
            }
        }
    }
}
