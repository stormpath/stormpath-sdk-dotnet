// <copyright file="JwtTokenizer.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Jwt;

namespace Stormpath.SDK.Impl.Jwt
{
    internal sealed class JwtTokenizer
    {
        private readonly string[] tokens;

        public JwtTokenizer(string jwt)
        {
            this.tokens = jwt.Split(JwtStrings.Separator);

            if (this.tokens.Length != 3)
                throw InvalidJwtException.InvalidValue;
        }

        public string Base64Header => this.tokens[0];

        public string Base64Payload => this.tokens[1];

        public string Base64Signature => this.tokens[2];
    }
}
