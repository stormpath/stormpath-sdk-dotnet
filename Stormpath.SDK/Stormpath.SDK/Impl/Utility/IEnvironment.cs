// <copyright file="IEnvironment.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Impl.Utility
{
    /// <summary>
    /// An abstraction over System.Environment.
    /// </summary>
    internal interface IEnvironment
    {
        /// <summary>
        /// An abstraction over <see cref="System.Environment.ExpandEnvironmentVariables(string)"/>.
        /// </summary>
        /// <param name="name">The environment variable name to expand.</param>
        /// <returns>The expanded environment variable string.</returns>
        string ExpandEnvironmentVariables(string name);

        /// <summary>
        /// An abstraction over <see cref="System.Environment.GetEnvironmentVariable(string)"/>.
        /// </summary>
        /// <param name="variable">The environment variable name.</param>
        /// <returns>The environment variable value.</returns>
        string GetEnvironmentVariable(string variable);
    }
}
