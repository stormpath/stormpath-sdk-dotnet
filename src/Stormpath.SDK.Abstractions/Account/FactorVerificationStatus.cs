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

using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Account
{
    /// <summary>
    /// Represents the various states a <see cref="IFactor">Factor</see> may be in with regards to verification.
    /// </summary>
    public sealed class FactorVerificationStatus : AbstractEnumProperty
    {
        /// <summary>
        /// The factor has been verified.
        /// </summary>
        public static FactorVerificationStatus Verified = new FactorVerificationStatus("VERIFIED");

        /// <summary>
        /// The factor has not been verified.
        /// </summary>
        public static FactorVerificationStatus Unverified = new FactorVerificationStatus("UNVERIFIED");

        public FactorVerificationStatus(string value)
            : base(value)
        {
        }
    }
}