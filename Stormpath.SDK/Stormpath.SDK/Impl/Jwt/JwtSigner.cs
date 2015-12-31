// <copyright file="JwtSigner.cs" company="Stormpath, Inc.">
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

using System.Text;
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Impl.Jwt
{
    internal sealed class JwtSigner
    {
        private readonly byte[] signingKey;

        public JwtSigner(byte[] signingKey)
        {
            this.signingKey = signingKey;
        }

        public string CalculateSignature(string base64UrlEncodedHeader, string base64UrlEncodedPayload)
        {
            var input = $"{base64UrlEncodedHeader}{DefaultJwt.Separator}{base64UrlEncodedPayload}";
            var hmac = new HmacGenerator(this.signingKey, Encoding.UTF8).ComputeHmac(input);

            return Base64.EncodeUrlSafe(hmac);
        }
    }
}
