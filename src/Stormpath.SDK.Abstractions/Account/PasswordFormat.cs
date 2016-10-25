// <copyright file="PasswordFormat.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Account
{
    /// <summary>
    /// Represents the various formats supported for importing passwords.
    /// </summary>
    public sealed class PasswordFormat : AbstractEnumProperty
    {
        /// <summary>
        /// Modular Crypt Format (a <c>$</c> delimited string).
        /// </summary>
        public static PasswordFormat MCF = new PasswordFormat("MCF");

        /// <summary>
        /// Creates a new <see cref="PasswordFormat"/> instance.
        /// </summary>
        /// <param name="value">The value to use.</param>
        public PasswordFormat(string value)
            : base(value)
        {
        }
    }
}
