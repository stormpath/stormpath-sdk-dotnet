// <copyright file="DefaultJwtSigner.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Impl.Jwt
{
    internal sealed class DefaultJwtSigner : IJwtSigner
    {
        private static readonly string Base64UrlSafeJwtSignHeader
            = Base64.EncodeUrlSafe("{\"alg\":\"HS256\",\"typ\":\"JWT\"}", Encoding.UTF8);

        private readonly byte[] signingKey;

        public DefaultJwtSigner(string signingKey)
        {
            this.signingKey = Encoding.UTF8.GetBytes(signingKey);
        }

        public string Sign(string jsonPayload)
        {
            if (string.IsNullOrEmpty(jsonPayload))
                throw new ArgumentNullException(nameof(jsonPayload));

            var base64UrlSafeJsonPayload = Base64.EncodeUrlSafe(jsonPayload, Encoding.UTF8);
            var signature = this.CalculateSignature(Base64UrlSafeJwtSignHeader, base64UrlSafeJsonPayload);

            return new StringBuilder()
                .Append(Base64UrlSafeJwtSignHeader)
                .Append(JsonWebToken.Separator)
                .Append(base64UrlSafeJsonPayload)
                .Append(JsonWebToken.Separator)
                .Append(signature)
                .ToString();
        }

        public string CalculateSignature(string base64Header, string base64JsonPayload)
        {
            var input = $"{base64Header}{JsonWebToken.Separator}{base64JsonPayload}";
            var hmac = new HmacGenerator(this.signingKey, Encoding.UTF8).ComputeHmac(input);

            return Base64.EncodeUrlSafe(hmac);
        }
    }
}
