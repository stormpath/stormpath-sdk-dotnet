// <copyright file="IProviderAccountRequest.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Provider
{
    /// <summary>
    /// Represents an attempt to get or create a Provider-based <see cref="Account.IAccount"/> record in Stormpath.
    /// <para>
    /// NOTE: A Provider-specific <see cref="Directory.IDirectory"/>
    /// must previously exist in Stormpath and it must also
    /// be an Enabled Account Store within the Application.
    /// </para>
    /// </summary>
    public interface IProviderAccountRequest
    {
        /// <summary>
        /// Gets the <see cref="IProviderData"/> Resource containing the data required to access to the account.
        /// </summary>
        /// <returns>The <see cref="IProviderData"/> Resource containing the data required to access to the account.</returns>
        IProviderData GetProviderData();
    }
}
