// <copyright file="OrganizationStatus.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Organization
{
    /// <summary>
    /// Represents the states an <see cref="IOrganization">Organization</see> may be in.
    /// </summary>
    public sealed class OrganizationStatus : StringEnumeration
    {
        /// <summary>
        /// Enabled Organizations can be used as <see cref="AccountStore.IAccountStore">Account Stores</see> for <see cref="Application.IApplication">Applications</see>.
        /// </summary>
        public static OrganizationStatus Enabled = new OrganizationStatus("ENABLED");

        /// <summary>
        /// Enabled Organizations cannot be used as <see cref="AccountStore.IAccountStore">Account Stores</see> for <see cref="Application.IApplication">Applications</see>.
        /// </summary>
        public static OrganizationStatus Disabled = new OrganizationStatus("DISABLED");

        private OrganizationStatus(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Parses a string to an <see cref="OrganizationStatus">Organization Status</see>.
        /// </summary>
        /// <param name="status">A string containing "enabled" or "disabled" (matching is case-insensitive).</param>
        /// <returns>The <see cref="OrganizationStatus">Organization Status</see> with the specified name.</returns>
        public static OrganizationStatus Parse(string status)
        {
            switch (status.ToUpper())
            {
                case "ENABLED": return Enabled;
                case "DISABLED": return Disabled;
                default:
                    throw new ApplicationException($"Could not parse organization status value '{status.ToUpper()}'");
            }
        }
    }
}