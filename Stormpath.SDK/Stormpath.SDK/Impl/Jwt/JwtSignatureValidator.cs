// <copyright file="JwtSignatureValidator.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Jwt;

namespace Stormpath.SDK.Impl.Jwt
{
    /// <summary>
    /// Validates a JWT signature.
    /// </summary>
    internal sealed class JwtSignatureValidator
    {
        private readonly byte[] signingKey;

        public JwtSignatureValidator(byte[] signingKey)
        {
            this.signingKey = signingKey;
        }

        public bool IsValid(IJwt jwt)
            => this.IsValid(jwt.Base64Header, jwt.Base64Payload, jwt.Base64Digest);

        public bool IsValid(string base64UrlEncodedHeader, string base64UrlEncodedPayload, string base64UrlEncodedDigest)
        {
            if (string.IsNullOrEmpty(base64UrlEncodedHeader))
            {
                throw new ArgumentNullException(nameof(base64UrlEncodedHeader));
            }

            if (string.IsNullOrEmpty(base64UrlEncodedPayload))
            {
                throw new ArgumentNullException(nameof(base64UrlEncodedPayload));
            }

            if (string.IsNullOrEmpty(base64UrlEncodedDigest))
            {
                throw new ArgumentNullException(nameof(base64UrlEncodedDigest));
            }

            var signer = new JwtSigner(this.signingKey);
            var calculatedSignature = signer.CalculateSignature(base64UrlEncodedHeader, base64UrlEncodedPayload);
            return base64UrlEncodedDigest.Equals(calculatedSignature, StringComparison.Ordinal);
        }
    }
}
