// <copyright file="FactorVerificationStatus.cs" company="Stormpath, Inc.">
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
    /// Represents the various states a <see cref="IFactor">Factor</see> may be in with regards to verification.
    /// </summary>
    public sealed class FactorVerificationStatus : StringEnumeration
    {
        // TODO are these all the values?
        /// <summary>
        /// The factor has been verified.
        /// </summary>
        public static FactorVerificationStatus Verified = new FactorVerificationStatus("VERIFIED");

        /// <summary>
        /// The factor has not been verified.
        /// </summary>
        public static FactorVerificationStatus Unverified = new FactorVerificationStatus("UNVERIFIED");

        private FactorVerificationStatus(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Parses a string to an <see cref="FactorVerificationStatus"/>.
        /// </summary>
        /// <param name="status">A string containing "verified" or "unverified" (matching is case-insensitive).</param>
        /// <returns>The <see cref="FactorVerificationStatus"/> with the specified name.</returns>
        /// <exception cref="Exception">No match is found.</exception>
        public static FactorVerificationStatus Parse(string status)
        {
            // TODO don't think I need the switch
            return new FactorVerificationStatus(status);
        }
    }
}