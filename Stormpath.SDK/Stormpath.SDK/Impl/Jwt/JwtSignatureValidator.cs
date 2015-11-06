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
using Stormpath.SDK.Api;

namespace Stormpath.SDK.Impl.Jwt
{
    internal sealed class JwtSignatureValidator
    {
        private readonly IJwtSigner jwtSigner;

        public JwtSignatureValidator(IClientApiKey apiKey)
        {
            if (apiKey == null)
                throw new ArgumentNullException(nameof(apiKey));

            this.jwtSigner = new DefaultJwtSigner(apiKey.GetSecret());
        }

        public bool IsValid(string jwt)
        {
            if (jwt == null)
                throw new ArgumentNullException(nameof(jwt));

            var tokenizer = new JwtTokenizer(jwt);
            var calculatedSignature = this.jwtSigner.CalculateSignature(tokenizer.Base64Header, tokenizer.Base64Payload);

            return tokenizer.Base64Signature.Equals(calculatedSignature, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
