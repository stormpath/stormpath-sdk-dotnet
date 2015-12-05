// <copyright file="IProviderAccountAccess.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Provider;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Provider
{
    /// <summary>
    /// Represents a request for an Account given a specified Provider.
    /// </summary>
    internal interface IProviderAccountAccess : IResource
    {
        /// <summary>
        /// Gets the Provider-specific <see cref="IProviderData"/> containing the information required to execute an access attempt.
        /// </summary>
        /// <returns>The Provider-specific <see cref="IProviderData"/> containing the information required to execute an access attempt.</returns>
        IProviderData GetProviderData();

        /// <summary>
        /// Sets the Provider-specific <see cref="IProviderData"/> containing the information required to execute an access attempt.
        /// </summary>
        /// <param name="providerData">The Provider-specific <see cref="IProviderData"/>.</param>
        void SetProviderData(IProviderData providerData);
    }
}
