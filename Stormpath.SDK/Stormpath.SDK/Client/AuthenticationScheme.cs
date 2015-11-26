// <copyright file="AuthenticationScheme.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Http.Authentication;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Client
{
    /// <summary>
    /// Enumeration that defines the available HTTP authentication schemes to be used when communicating with the Stormpath API server.
    /// The Authentication Scheme setting is helpful in cases where the code is run in a platform where the header information for outgoing
    /// HTTP requests is modified and thus causing communication issues. For example, for Google App Engine you need to set <see cref="Basic"/>
    /// in order for your code to properly communicate with Stormpath API server.
    /// There are currently two authentication schemes available: HTTP Basic Authentication (<see cref="Basic"/>) and Digest Authentication (<see cref="SAuthc1"/>).
    /// </summary>
    public sealed class AuthenticationScheme : StringEnumeration
    {
        /// <summary>
        /// HTTP Basic Authentication
        /// </summary>
        public static AuthenticationScheme Basic = new AuthenticationScheme("BASIC", typeof(BasicRequestAuthenticator));

        /// <summary>
        /// Digest Authentication
        /// </summary>
        public static AuthenticationScheme SAuthc1 = new AuthenticationScheme("SAUTHC1", typeof(SAuthc1RequestAuthenticator));

        private readonly Type authenticatorType;

        private AuthenticationScheme()
        {
        }

        private AuthenticationScheme(string value, Type authenticatorType)
            : base(value)
        {
            this.authenticatorType = authenticatorType;
        }

        /// <summary>
        /// Parses a string to an <see cref="AuthenticationScheme"/>.
        /// </summary>
        /// <param name="scheme">A string containing "basic" or "sauthc1" (matching is case-insensitive).</param>
        /// <returns>The <see cref="AuthenticationScheme"/> with the specified name.</returns>
        /// <exception cref="ApplicationException">No match is found.</exception>
        public static AuthenticationScheme Parse(string scheme)
        {
            switch (scheme.ToUpper())
            {
                case "BASIC": return Basic;
                case "SAUTHC1": return SAuthc1;
                default:
                    throw new ApplicationException($"Could not parse scheme type '{scheme.ToUpper()}'");
            }
        }
    }
}
