// <copyright file="GroupStatus.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Group
{
    public sealed class GroupStatus : Enumeration
    {
        /// <summary>
        /// The group can authenticate against an application.
        /// </summary>
        public static GroupStatus Enabled = new GroupStatus(0, "ENABLED");

        /// <summary>
        /// The group cannot authentication against an application.
        /// </summary>
        public static GroupStatus Disabled = new GroupStatus(1, "DISABLED");

        private GroupStatus()
        {
        }

        private GroupStatus(int value, string displayName)
            : base(value, displayName)
        {
        }

        public static GroupStatus Parse(string status)
        {
            switch (status.ToUpper())
            {
                case "ENABLED": return Enabled;
                case "DISABLED": return Disabled;
                default:
                    throw new ApplicationException($"Could not parse group status value '{status.ToUpper()}'");
            }
        }
    }
}