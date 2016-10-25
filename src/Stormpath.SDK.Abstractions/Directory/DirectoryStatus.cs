// <copyright file="DirectoryStatus.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Directory
{
    /// <summary>
    /// Represents the status of a <see cref="IDirectory">Directory</see>.
    /// </summary>
    public sealed class DirectoryStatus : AbstractEnumProperty
    {
        /// <summary>
        /// Accounts in this directory may login to applications.
        /// </summary>
        public static DirectoryStatus Enabled = new DirectoryStatus("ENABLED");

        /// <summary>
        /// Accounts in this directory may not login to applications.
        /// </summary>
        public static DirectoryStatus Disabled = new DirectoryStatus("DISABLED");

        /// <summary>
        /// Creates a new <see cref="DirectoryStatus"/> instance.
        /// </summary>
        /// <param name="value">The value to use.</param>
        public DirectoryStatus(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Parses a string to a <see cref="DirectoryStatus">Directory Status</see>.
        /// </summary>
        /// <param name="status">A string containing "enabled" or "disabled" (matching is case-insensitive).</param>
        /// <returns>The <see cref="DirectoryStatus">Directory Status</see> with the specified name.</returns>
        /// <exception cref="Exception">No match is found.</exception>
        [Obsolete("Use constructor")]
        public static DirectoryStatus Parse(string status)
        {
            switch (status.ToUpper())
            {
                case "ENABLED": return Enabled;
                case "DISABLED": return Disabled;
                default:
                    throw new Exception($"Could not parse directory status value '{status.ToUpper()}'");
            }
        }
    }
}