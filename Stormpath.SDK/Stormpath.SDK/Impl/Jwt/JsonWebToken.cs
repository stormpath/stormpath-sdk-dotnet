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
using System.Text;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Serialization;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.Jwt
{
    internal sealed class JsonWebToken
    {
        public static readonly char Separator = '.';

        private readonly IJsonSerializer jsonSerializer;

        private readonly string headerJson;
        private readonly string payloadJson;
        private readonly string signature;

        private readonly Lazy<Map> header;
        private readonly Lazy<Map> payload;

        public string Base64Header { get; private set; }

        public string Base64Payload { get; private set; }

        public string Base64Signature { get; private set; }

        public Map Header => this.jsonSerializer.Deserialize(this.headerJson);

        public Map Payload => this.jsonSerializer.Deserialize(this.payloadJson);

        public string Signature => this.signature;

        public JsonWebToken(string jwt, IJsonSerializer jsonSerializer)
        {
            var parts = jwt.Split(Separator);
            if (parts.Length != 3)
                throw new ArgumentException("Token must consist of 3 delimeted parts.");

            this.Base64Header = parts[0];
            this.Base64Payload = parts[1];
            this.Base64Signature = parts[2];

            this.headerJson = Base64.Decode(this.Base64Header, Encoding.UTF8);
            this.payloadJson = Base64.Decode(this.Base64Payload, Encoding.UTF8);
            this.signature = Base64.Decode(this.Base64Signature, Encoding.UTF8);

            this.jsonSerializer = jsonSerializer;

            this.header = new Lazy<Map>(() => this.jsonSerializer.Deserialize(this.headerJson));
            this.payload = new Lazy<Map>(() => this.jsonSerializer.Deserialize(this.payloadJson));
        }
    }
}
