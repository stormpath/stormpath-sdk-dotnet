// <copyright file="ApiKeyStatus.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Api
{
    /// <summary>
    /// Represents the status (usability) of an <see cref="IApiKey">API Key</see> instance.
    /// </summary>
    public sealed class ApiKeyStatus : StringEnumeration
    {
        /// <summary>
        /// An enabled API Key may be used for API Authentication.
        /// </summary>
        public static ApiKeyStatus Enabled = new ApiKeyStatus("ENABLED");

        /// <summary>
        /// A disabled API Key may not be used for API Authentication.
        /// </summary>
        public static ApiKeyStatus Disabled = new ApiKeyStatus("DISABLED");

        private ApiKeyStatus(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Parses a string to an <see cref="ApiKeyStatus">API Key Status</see>.
        /// </summary>
        /// <param name="status">A string containing "enabled" or "disabled" (matching is case-insensitive).</param>
        /// <returns>The <see cref="ApiKeyStatus">Application Status</see> with the specified name.</returns>
        public static ApiKeyStatus Parse(string status)
        {
            switch (status.ToUpper())
            {
                case "ENABLED": return Enabled;
                case "DISABLED": return Disabled;
                default:
                    throw new Exception($"Could not parse API Key status value '{status.ToUpper()}'");
            }
        }
    }
}
