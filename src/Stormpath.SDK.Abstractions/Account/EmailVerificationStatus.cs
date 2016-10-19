// <copyright file="EmailVerificationStatus.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Account
{
    /// <summary>
    /// Represents the various states an <see cref="IAccount">Account</see>'s email address may be in with regards to verification.
    /// </summary>
    public sealed class EmailVerificationStatus : StringEnumeration
    {
        /// <summary>
        /// The account's email has been verified.
        /// </summary>
        public static EmailVerificationStatus Verified = new EmailVerificationStatus("VERIFIED");

        /// <summary>
        /// The account's email has not been verified.
        /// </summary>
        public static EmailVerificationStatus Unverified = new EmailVerificationStatus("UNVERIFIED");

        /// <summary>
        /// The account's email status is not known.
        /// </summary>
        public static EmailVerificationStatus Unknown = new EmailVerificationStatus("UNKNOWN");

        private EmailVerificationStatus(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Parses a string to an <see cref="EmailVerificationStatus"/>.
        /// </summary>
        /// <param name="status">A string containing "verified", "unverified", or "unknown" (matching is case-insensitive).</param>
        /// <returns>The <see cref="EmailVerificationStatus"/> with the specified name.</returns>
        /// <exception cref="Exception">No match is found.</exception>
        public static EmailVerificationStatus Parse(string status)
        {
            switch (status.ToUpper())
            {
                case "VERIFIED":
                    return Verified;
                case "UNVERIFIED":
                    return Unverified;
                case "UNKNOWN":
                    return Unknown;
                default:
                    throw new Exception($"Could not parse email verification status value '{status.ToUpper()}'");
            }
        }
    }
}