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
using Stormpath.SDK.Impl.Http.Authentication;
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Client
{
    public sealed class AuthenticationScheme : Enumeration
    {
#pragma warning disable SA1401 // Fields must be private
        public static AuthenticationScheme Basic = new AuthenticationScheme(0, "BASIC", typeof(BasicRequestAuthenticator));
        public static AuthenticationScheme SAuthc1 = new AuthenticationScheme(1, "SAUTHC1", typeof(SAuthc1RequestAuthenticator));
#pragma warning restore SA1401 // Fields must be private

        private readonly Type authenticatorType;

        private AuthenticationScheme()
        {
        }

        private AuthenticationScheme(int value, string displayName, Type authenticatorType) : base(value, displayName)
        {
            this.authenticatorType = authenticatorType;
        }
    }
}
