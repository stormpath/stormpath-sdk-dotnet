// <copyright file="ChallengeStatus.cs" company="Stormpath, Inc.">
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
    /// Represents the various states a <see cref="IChallenge">Challenge</see> may be in.
    /// </summary>
    public sealed class ChallengeStatus : AbstractEnumProperty
    {
        /// <summary>
        /// The challenge has been created.
        /// </summary>
        public static ChallengeStatus Created = new ChallengeStatus("CREATED");

        /// <summary>
        /// The challenge has been created.
        /// </summary>
        public static ChallengeStatus Waiting = new ChallengeStatus("WAITING");

        /// <summary>
        /// The challenge was successful.
        /// </summary>
        public static ChallengeStatus Success = new ChallengeStatus("SUCCESS");

        /// <summary>
        /// The challenge was not successful.
        /// </summary>
        public static ChallengeStatus Failed = new ChallengeStatus("FAILED");

        /// <summary>
        /// The challenge was denied.
        /// </summary>
        public static ChallengeStatus Denied = new ChallengeStatus("DENIED");

        /// <summary>
        /// The challenge was cancelled.
        /// </summary>
        public static ChallengeStatus Cancelled = new ChallengeStatus("CANCELLED");

        /// <summary>
        /// The challenge has expired.
        /// </summary>
        public static ChallengeStatus Expired = new ChallengeStatus("EXPIRED");

        /// <summary>
        /// The challenge was not successful because of an error.
        /// </summary>
        public static ChallengeStatus Error = new ChallengeStatus("ERROR");

        /// <summary>
        /// The challenge could not be delivered.
        /// </summary>
        public static ChallengeStatus Undelivered = new ChallengeStatus("UNDELIVERED");

        public ChallengeStatus(string value)
            : base(value)
        {
        }
    }
}