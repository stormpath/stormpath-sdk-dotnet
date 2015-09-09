// <copyright file="AccountStatus.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Account
{
    public sealed class AccountStatus : Enumeration
    {
        /// <summary>
        /// The account is able to log into assigned applications.
        /// </summary>
        public static AccountStatus Enabled = new AccountStatus(0, "ENABLED");

        /// <summary>
        /// The account cannot log into any applications.
        /// </summary>
        public static AccountStatus Disabled = new AccountStatus(1, "DISABLED");

        /// <summary>
        /// The account is disabled and has not verified their email address.
        /// </summary>
        public static AccountStatus Unverified = new AccountStatus(2, "UNVERIFIED");

        private AccountStatus()
        {
        }

        private AccountStatus(int value, string displayName)
            : base(value, displayName)
        {
        }

        public static AccountStatus Parse(string status)
        {
            switch (status.ToUpper())
            {
                case "ENABLED":
                    return Enabled;
                case "DISABLED":
                    return Disabled;
                case "UNVERIFIED":
                    return Unverified;
                default:
                    throw new ApplicationException($"Could not parse account status value '{status.ToUpper()}'");
            }
        }
    }
}