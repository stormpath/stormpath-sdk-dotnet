// <copyright file="IPlatform.cs" company="Stormpath, Inc.">
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

#if NET45
namespace Stormpath.SDK.Impl.Introspection
{
    /// <summary>
    /// Represents the local machine environment.
    /// </summary>
    internal interface IPlatform
    {
        /// <summary>
        /// Gets the .NET Framework version.
        /// </summary>
        /// <value>The .NET Framework version.</value>
        string FrameworkVersion { get; }

        /// <summary>
        /// Gets a value indicating whether the current platform is *nix.
        /// </summary>
        /// <value><see langword="true"/> if the current platform is *nix; <see langword="false"/> otherwise.</value>
        bool IsPlatformUnix { get; }

        /// <summary>
        /// Gets a value indicating whether the current environment is running on the Mono interpreter.
        /// </summary>
        /// <value><see langword="true"/> if the current environment is running on the Mono interpreter; <see langword="false"/> otherwise.</value>
        bool IsRunningOnMono { get; }

        /// <summary>
        /// Gets the Mono runtime version, if applicable.
        /// </summary>
        /// <value>The Mono runtime version, or <see langword="null"/> if the current environment is not running on the Mono interpreter.</value>
        string MonoRuntimeVersion { get; }

        /// <summary>
        /// Gets the operating system name.
        /// </summary>
        /// <value>The operating system name.</value>
        string OsName { get; }

        /// <summary>
        /// Gets the operating system version.
        /// </summary>
        /// <value>The operating system version.</value>
        string OsVersion { get; }
    }
}
#endif