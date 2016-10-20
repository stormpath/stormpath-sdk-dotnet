// <copyright file="PhoneVerificationStatus.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Account
{
    /// <summary>
    /// Represents the various states an <see cref="IAccount">Account</see>'s phone may be in with regards to verification.
    /// </summary>
    public sealed class PhoneVerificationStatus : AbstractEnumProperty
    {
        /// <summary>
        /// The phone has been verified.
        /// </summary>
        public static PhoneVerificationStatus Verified = new PhoneVerificationStatus("VERIFIED");

        /// <summary>
        /// The phone has not been verified.
        /// </summary>
        public static PhoneVerificationStatus Unverified = new PhoneVerificationStatus("UNVERIFIED");

        /// <summary>
        /// Creates a new <see cref="PhoneVerificationStatus"/> instance.
        /// </summary>
        /// <param name="value">The value to use.</param>
        public PhoneVerificationStatus(string value)
            : base(value)
        {
        }
    }
}