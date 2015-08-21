// <copyright file="ApplicationStatus.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Application
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields must be private", Justification = "Public enumeration elements")]
    public sealed class ApplicationStatus : Enumeration
    {
        /// <summary>
        /// Accounts can log into this application.
        /// </summary>
        public static ApplicationStatus Enabled = new ApplicationStatus(0, "ENABLED");

        /// <summary>
        /// Accounts are prevented from logging into this application.
        /// </summary>
        public static ApplicationStatus Disabled = new ApplicationStatus(1, "DISABLED");

        private static readonly Dictionary<string, ApplicationStatus> LookupMap = new Dictionary<string, ApplicationStatus>()
        {
            { Enabled.DisplayName, Enabled },
            { Disabled.DisplayName, Disabled },
        };

        private ApplicationStatus()
        {
        }

        private ApplicationStatus(int value, string displayName)
            : base(value, displayName)
        {
        }

        public static ApplicationStatus Parse(string status)
        {
            ApplicationStatus found;
            if (!LookupMap.TryGetValue(status.ToUpper(), out found))
                throw new ApplicationException($"Could not parse status value '{status.ToUpper()}'");

            return found;
        }
    }
}