// <copyright file="JwtValidator.cs" company="Stormpath, Inc.">
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
    internal sealed class JwtValidator
    {
        private readonly JwtSignatureValidator signatureValidator;

        public JwtValidator(byte[] signingKey)
        {
            if (signingKey == null)
            {
                throw new ArgumentNullException(nameof(signingKey));
            }

            this.signatureValidator = new JwtSignatureValidator(signingKey);
        }

        public bool IsValid(IJwt jwt)
        {
            if (jwt == null)
            {
                throw new ArgumentNullException(nameof(jwt));
            }

            if (!this.signatureValidator.IsValid(jwt))
            {
                return false;
            }

            // During parsing, the JWT is validated for expiration/lifetime, signature, and tampering
            throw new NotImplementedException();
        }
    }
}
