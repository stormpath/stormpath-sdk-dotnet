// <copyright file="ISdk.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
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

using System.Collections.Generic;

namespace Stormpath.SDK.Impl.Introspection
{
    /// <summary>
    /// Represents the Stormpath .NET SDK version and any installed integration versions.
    /// </summary>
    internal interface ISdk
    {
        /// <summary>
        /// Gets a list of the currently-active Stormpath integrations.
        /// </summary>
        /// <value>The currently-active Stormpath integrations.</value>
        IReadOnlyList<Integration> InstalledIntegrations { get; }

        /// <summary>
        /// Gets the Stormpath .NET SDK library version.
        /// </summary>
        /// <value>The Stormpath .NET SDK library version.</value>
        string Version { get; }
    }
}