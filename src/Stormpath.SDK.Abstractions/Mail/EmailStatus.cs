// <copyright file="EmailStatus.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Mail
{
    /// <summary>
    /// Represents the status of an email template or workflow.
    /// </summary>
    public sealed class EmailStatus : AbstractEnumProperty
    {
        /// <summary>
        /// Emails will be sent.
        /// </summary>
        public static EmailStatus Enabled = new EmailStatus("ENABLED");

        /// <summary>
        /// Emails will not be sent.
        /// </summary>
        public static EmailStatus Disabled = new EmailStatus("DISABLED");

        /// <summary>
        /// Creates a new <see cref="EmailStatus"/> instance.
        /// </summary>
        /// <param name="value">The value to use.</param>
        public EmailStatus(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Parses a string to an <see cref="EmailStatus">Email Status</see>.
        /// </summary>
        /// <param name="status">A string containing "enabled" or "disabled" (matching is case-insensitive).</param>
        /// <returns>The <see cref="EmailStatus">Email Status</see> with the specified name.</returns>
        /// <exception cref="Exception">No match is found.</exception>
        [Obsolete("Use constructor")]
        public static EmailStatus Parse(string status)
        {
            switch (status.ToUpper())
            {
                case "ENABLED": return Enabled;
                case "DISABLED": return Disabled;
                default:
                    throw new Exception($"Could not parse email status value '{status.ToUpper()}'");
            }
        }
    }
}
