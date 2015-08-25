// <copyright file="DirectoryStatus.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Directory
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields must be private", Justification = "Public enumeration elements")]
    public sealed class DirectoryStatus : Enumeration
    {
        /// <summary>
        /// The directory can be used as an account store.
        /// </summary>
        public static DirectoryStatus Enabled = new DirectoryStatus(0, "ENABLED");

        /// <summary>
        /// The directory cannot be used for login.
        /// </summary>
        public static DirectoryStatus Disabled = new DirectoryStatus(1, "DISABLED");

        private static readonly Dictionary<string, DirectoryStatus> LookupMap = new Dictionary<string, DirectoryStatus>()
        {
            { Enabled.DisplayName, Enabled },
            { Disabled.DisplayName, Disabled },
        };

        private DirectoryStatus()
        {
        }

        private DirectoryStatus(int value, string displayName)
            : base(value, displayName)
        {
        }

        public static DirectoryStatus Parse(string status)
        {
            DirectoryStatus found;
            if (!LookupMap.TryGetValue(status.ToUpper(), out found))
                throw new ApplicationException($"Could not parse status value '{status.ToUpper()}'");

            return found;
        }
    }
}