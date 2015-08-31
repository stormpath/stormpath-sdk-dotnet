// <copyright file="AuthenticationScheme.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using Stormpath.SDK.Impl.Http.Authentication;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Client
{
    public sealed class AuthenticationScheme : Enumeration
    {
        public static AuthenticationScheme Basic = new AuthenticationScheme(0, "BASIC", typeof(BasicRequestAuthenticator));
        public static AuthenticationScheme SAuthc1 = new AuthenticationScheme(1, "SAUTHC1", typeof(SAuthc1RequestAuthenticator));

        private readonly Type authenticatorType;

        private static readonly Dictionary<string, AuthenticationScheme> LookupMap = new Dictionary<string, AuthenticationScheme>()
        {
            { Basic.DisplayName, Basic },
            { SAuthc1.DisplayName, SAuthc1 },
        };

        private AuthenticationScheme()
        {
        }

        private AuthenticationScheme(int value, string displayName, Type authenticatorType) : base(value, displayName)
        {
            this.authenticatorType = authenticatorType;
        }

        public static AuthenticationScheme Parse(string status)
        {
            AuthenticationScheme found;
            if (!LookupMap.TryGetValue(status.ToUpper(), out found))
                throw new ApplicationException($"Could not parse status value '{status.ToUpper()}'");

            return found;
        }
    }
}
