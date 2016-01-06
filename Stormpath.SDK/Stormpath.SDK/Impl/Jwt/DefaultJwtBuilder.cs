// <copyright file="DefaultJwtBuilder.cs" company="Stormpath, Inc.">
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
using System.Text;
using Stormpath.SDK.Impl.Client;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Jwt;
using Stormpath.SDK.Serialization;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.Jwt
{
    internal sealed class DefaultJwtBuilder : IJwtBuilder
    {
        private readonly IJwtClaimsBuilder claimsBuilder;
        private readonly IJsonSerializer serializer;
        private readonly string algorithm = "HS256";

        private Map header;
        private byte[] signingKey;

        internal DefaultJwtBuilder()
            : this(Serializers.Create().Default().Build()) //todo fix this, because it's awful
        {
        }

        public DefaultJwtBuilder(IJsonSerializer serializer)
        {
            this.claimsBuilder = new DefaultJwtClaimsBuilder();
            this.serializer = serializer;
        }

        private string Base64UrlEncode(Map map, Encoding encoding)
        {
            var json = this.serializer.Serialize(map);
            var encoded = Base64.EncodeUrlSafe(json, encoding);

            return encoded;
        }

        IJwtBuilder IJwtBuilder.SetHeader(Map header)
        {
            this.header = header;
            return this;
        }

        IJwtBuilder IClaimsMutator<IJwtBuilder>.SetAudience(string aud)
        {
            this.claimsBuilder.SetAudience(aud);
            return this;
        }

        IJwtBuilder IClaimsMutator<IJwtBuilder>.SetClaim(string claimName, object value)
        {
            this.claimsBuilder.SetClaim(claimName, value);
            return this;
        }

        IJwtBuilder IJwtBuilder.SetClaims(IJwtClaims claims)
        {
            (this.claimsBuilder as DefaultJwtClaimsBuilder).ReplaceAll(claims.ToDictionary());
            return this;
        }

        IJwtBuilder IJwtBuilder.SetClaims(Map claims)
        {
            (this.claimsBuilder as DefaultJwtClaimsBuilder).ReplaceAll(claims);
            return this;
        }

        IJwtBuilder IClaimsMutator<IJwtBuilder>.SetExpiration(DateTimeOffset? exp)
        {
            this.claimsBuilder.SetExpiration(exp);
            return this;
        }

        IJwtBuilder IClaimsMutator<IJwtBuilder>.SetId(string jti)
        {
            this.claimsBuilder.SetId(jti);
            return this;
        }

        IJwtBuilder IClaimsMutator<IJwtBuilder>.SetIssuedAt(DateTimeOffset? iat)
        {
            this.claimsBuilder.SetIssuedAt(iat);
            return this;
        }

        IJwtBuilder IClaimsMutator<IJwtBuilder>.SetIssuer(string iss)
        {
            this.claimsBuilder.SetIssuer(iss);
            return this;
        }

        IJwtBuilder IClaimsMutator<IJwtBuilder>.SetNotBeforeDate(DateTimeOffset? nbf)
        {
            this.claimsBuilder.SetNotBeforeDate(nbf);
            return this;
        }

        IJwtBuilder IClaimsMutator<IJwtBuilder>.SetSubject(string sub)
        {
            this.claimsBuilder.SetSubject(sub);
            return this;
        }

        IJwtBuilder IJwtBuilder.SignWith(string secretKeyString, Encoding encoding)
        {
            if (string.IsNullOrEmpty(secretKeyString))
            {
                throw new ArgumentNullException(nameof(secretKeyString));
            }

            this.signingKey = encoding.GetBytes(secretKeyString);
            return this;
        }

        IJwtBuilder IJwtBuilder.SignWith(byte[] secretKey)
        {
            if (secretKey == null)
            {
                throw new ArgumentNullException(nameof(secretKey));
            }

            this.signingKey = secretKey;
            return this;
        }

        IJwt IJwtBuilder.Build()
        {
            var jwtParts = this.Compact();

            return new DefaultJwt(
                this.header,
                this.claimsBuilder.Build(),
                jwtParts.Item1,
                jwtParts.Item2,
                jwtParts.Item3);
        }

        private Tuple<string, string, string> Compact()
        {
            if (this.header == null)
            {
                this.header = new Dictionary<string, object>();
            }

            this.header[JwtHeaderParameters.Type] = "JWT";
            this.header[JwtHeaderParameters.Algorithm] = this.algorithm;
            var base64UrlEncodedHeader = this.Base64UrlEncode(this.header, Encoding.UTF8);

            var claims = this.claimsBuilder
                .Build()
                .ToDictionary();
            var base64UrlEncodedPayload = this.Base64UrlEncode(claims, Encoding.UTF8);

            string base64UrlEncodedDigest = null;
            if (this.signingKey != null)
            {
                var signer = new JwtSigner(this.signingKey);
                base64UrlEncodedDigest = signer.CalculateSignature(base64UrlEncodedHeader, base64UrlEncodedPayload);
            }

            return new Tuple<string, string, string>(
                base64UrlEncodedHeader,
                base64UrlEncodedPayload,
                base64UrlEncodedDigest);
        }
    }
}
