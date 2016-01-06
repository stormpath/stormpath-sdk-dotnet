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

        internal DefaultJwtParser()
            : this(Serializers.Create().Default().Build()) //todo awful
        {
        }

        public DefaultJwtParser(IJsonSerializer serializer)
        {
            this.expectedClaims = new DefaultJwtClaimsBuilder();
            this.serializer = serializer;
        }

        IJwtParser IJwtParser.RequireClaim(string claimName, object value)
        {
            this.expectedClaims.SetClaim(claimName, value);

            return this;
        }

        IJwtParser IJwtParser.RequireAudience(string audience)
        {
            this.expectedClaims.SetAudience(audience);

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

        IJwtParser IJwtParser.SetSigningKey(string signingKeyString, Encoding encoding)
        {
            this.keyBytes = encoding.GetBytes(signingKeyString);

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

            Map header = null;
            Map payload = null;
            try
            {
                header = this.serializer.Deserialize(Base64.Decode(base64UrlEncodedHeader, Encoding.UTF8));
            }
            catch (Exception e)
            {
                throw new MalformedJwtException("Could not decode JWT header.", e);
            }

            try
            {
                payload = this.serializer.Deserialize(Base64.Decode(base64UrlEncodedPayload, Encoding.UTF8));
            }
            catch (Exception e)
            {
                throw new MalformedJwtException("Could not decode JWT payload.", e);
            }

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

            IJwt jwtInstance = new DefaultJwt(header, claims, base64UrlEncodedHeader, base64UrlEncodedPayload, base64UrlEncodedDigest);

            // Validate lifetime (exp, iat, nbf claims)
            var lifetimeValidator = new JwtLifetimeValidator(DateTimeOffset.Now);
            lifetimeValidator.Validate(jwtInstance.Body);

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

                // Special handling of integer comparisons. This fails for .Equals() because
                // one value may be type Int64 and the other type Int32 (for example).
                if (claim.Value.GetType() == typeof(long)
                    || claim.Value.GetType() == typeof(int)
                    || claim.Value.GetType() == typeof(short))
                {
                    long unboxedExpectedValue = default(long);
                    long unboxedActualValue = default(long);

                    try
                    {
                        unboxedExpectedValue = Convert.ToInt64(claim.Value);
                        unboxedActualValue = Convert.ToInt64(actualValue);
                    }
                    catch (Exception e)
                    {
                        throw new InvalidJwtException($"Could not decode the claim '{claim.Key}'.", e);
                    }

                    if (unboxedExpectedValue != unboxedActualValue)
                    {
                        throw new MismatchedClaimException($"The claim '{claim.Key}' should have a value of '{claim.Value}', but instead was '{actualValue}'.");
                    }

                    continue;
                }

                if (!claim.Value.Equals(actualValue))
                {
                    throw new MismatchedClaimException($"The claim '{claim.Key}' should have a value of '{claim.Value}', but instead was '{actualValue}'.");
                }
            }
        }
    }
}
