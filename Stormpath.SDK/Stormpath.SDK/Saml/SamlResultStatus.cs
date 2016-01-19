// <copyright file="IdSiteResultStatus.cs" company="Stormpath, Inc.">
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

using System;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Saml
{
    /// <summary>
    /// Represents the possible results of a SAML invocation.
    /// </summary>
    public sealed class SamlResultStatus : StringEnumeration
    {
        /// <summary>
        /// A successful account authentication via SAML.
        /// </summary>
        public static SamlResultStatus Authenticated = new SamlResultStatus("AUTHENTICATED");

        /// <summary>
        /// A logout via SAML.
        /// </summary>
        public static SamlResultStatus Logout = new SamlResultStatus("LOGOUT");

        private SamlResultStatus(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Parses a string to an <see cref="SamlResultStatus"/>.
        /// </summary>
        /// <param name="status">A string containing "authenticated" or "logout" (matching is case-insensitive).</param>
        /// <returns>The <see cref="SamlResultStatus"/> with the specified name.</returns>
        /// <exception cref="ApplicationException">No match is found.</exception>
        public static SamlResultStatus Parse(string status)
        {
            status = status.ToUpper();

            switch (status)
            {
                case "AUTHENTICATED":
                    return Authenticated;
                case "LOGOUT":
                    return Logout;
                default:
                    throw new ApplicationException($"Could not parse status value '{status}'");
            }
        }
    }
}
