// <copyright file="AuthorizationHeaderValue.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Http
{
    public sealed class AuthorizationHeaderValue : ImmutableValueObject<AuthorizationHeaderValue>
    {
        private readonly string scheme;
        private readonly string parameter;

        public AuthorizationHeaderValue(string scheme, string parameter)
        {
            this.scheme = scheme;
            this.parameter = parameter;
        }

        public AuthorizationHeaderValue(string schemeAndParameter)
        {
            var segments = schemeAndParameter.Split(' ');
            if (segments.Length != 2)
                throw new ArgumentException("Invalid Authorization header format.", nameof(schemeAndParameter));

            this.scheme = segments[0];
            this.parameter = segments[1];
        }

        public string Scheme => this.scheme;

        public string Parameter => this.parameter;

        public override string ToString()
        {
            return $"{this.scheme} {this.parameter}";
        }
    }
}
