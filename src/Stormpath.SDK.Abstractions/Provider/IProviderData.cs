// <copyright file="IProviderData.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Provider
{
    /// <summary>
    /// Resource containing Provider-specific data for the <see cref="Account.IAccount">Account</see>. For example, for Google, it
    /// contains <c>refreshToken</c> and <c>accessToken</c>.
    /// </summary>
    public interface IProviderData : IResource, IAuditable
    {
        /// <summary>
        /// Gets the Stormpath ID of the Provider (e.g. "facebook" or "google").
        /// </summary>
        /// <value>The Stormpath ID of the Provider</value>
        string ProviderId { get; }
    }
}
