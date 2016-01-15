// <copyright file="DefaultJwt.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
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
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.Jwt
{
    internal sealed class DefaultJwt : IJwt
    {
        public static readonly char Separator = '.';

        private readonly Map header;
        private readonly IJwtClaims body;

        private readonly string base64UrlEncodedHeader;
        private readonly string base64UrlEncodedPayload;
        private readonly string base64UrlEncodedDigest;

        public DefaultJwt(Map header, IJwtClaims body, string base64UrlEncodedHeader, string base64UrlEncodedPayload, string base64UrlEncodedDigest)
        {
            this.header = header;
            this.body = body;

            this.base64UrlEncodedHeader = base64UrlEncodedHeader;
            this.base64UrlEncodedPayload = base64UrlEncodedPayload;
            this.base64UrlEncodedDigest = base64UrlEncodedDigest;
        }

        string IJwt.Base64Header => this.base64UrlEncodedHeader;

        string IJwt.Base64Payload => this.base64UrlEncodedPayload;

        string IJwt.Base64Digest => this.base64UrlEncodedDigest;

        Map IJwt.Header => this.header;

        IJwtClaims IJwt.Body => this.body;

        public override string ToString()
            => $"{this.base64UrlEncodedHeader}{Separator}{this.base64UrlEncodedPayload}{Separator}{this.base64UrlEncodedDigest}";
    }
}
