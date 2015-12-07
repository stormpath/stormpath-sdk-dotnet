// <copyright file="JsonWebToken.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Serialization;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.Jwt
{
    internal sealed class JsonWebToken
    {
        public static readonly char Separator = '.';

        public string Base64Header { get; private set; }

        public string Base64Payload { get; private set; }

        public string Base64Signature { get; private set; }

        public Map Header { get; private set; }

        public Map Payload { get; private set; }

        private JsonWebToken()
        {
        }

        public override string ToString()
        {
            return string.Join(Separator.ToString(), new string[] { this.Base64Header, this.Base64Payload, this.Base64Signature });
        }

        public static JsonWebToken Encode(Map payload, string key, IJsonSerializer jsonSerializer)
        {
            var header = new Dictionary<string, object>()
            {
                ["typ"] = "JWT",
                ["alg"] = "HS256"
            };

            var headerBase64 = Base64.EncodeUrlSafe(jsonSerializer.Serialize(header), Encoding.UTF8);
            var payloadBase64 = Base64.EncodeUrlSafe(jsonSerializer.Serialize(payload), Encoding.UTF8);

            var stringToSign = string.Join(Separator.ToString(), new string[] { headerBase64, payloadBase64 });

            byte[] signature = new HmacGenerator(key, Encoding.UTF8).ComputeHmac(stringToSign);
            var signatureBase64 = Base64.EncodeUrlSafe(signature);

            return new JsonWebToken()
            {
                Base64Header = headerBase64,
                Base64Payload = payloadBase64,
                Base64Signature = signatureBase64,
                Header = header,
                Payload = payload
            };
        }

        public static JsonWebToken Decode(string jwt, IJsonSerializer jsonSerializer)
        {
            var parts = jwt.Split(Separator);
            if (parts.Length != 3)
            {
                throw new ArgumentException("Token must consist of 3 delimited parts.");
            }

            var headerJson = Base64.Decode(parts[0], Encoding.UTF8);
            var payloadJson = Base64.Decode(parts[1], Encoding.UTF8);
            var signature = Base64.Decode(parts[2], Encoding.UTF8);

            return new JsonWebToken()
            {
                Base64Header = parts[0],
                Base64Payload = parts[1],
                Base64Signature = parts[2],
                Header = jsonSerializer.Deserialize(headerJson),
                Payload = jsonSerializer.Deserialize(payloadJson),
            };
       }
    }
}
