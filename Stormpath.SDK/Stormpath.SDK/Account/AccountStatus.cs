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
using System.Collections.Generic;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Account
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields must be private", Justification = "Public enumeration elements")]
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

        private static readonly Dictionary<string, AccountStatus> LookupMap = new Dictionary<string, AccountStatus>()
        {
            { Enabled.DisplayName, Enabled },
            { Disabled.DisplayName, Disabled },
            { Unverified.DisplayName, Unverified }
        };

        private AccountStatus()
        {
        }

        private AccountStatus(int value, string displayName)
            : base(value, displayName)
        {
        }

        public static explicit operator AccountStatus(string status)
        {
            throw new NotImplementedException();
        }

        public static AccountStatus Parse(string status)
        {
            AccountStatus found;
            if (!LookupMap.TryGetValue(status.ToUpper(), out found))
                throw new ApplicationException($"Could not parse status value '{status.ToUpper()}'");

            return found;
        }
    }
}