// <copyright file="IClientApiKey.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Api
{
    /// <summary>
    /// Represents a Stormpath customer's API-specific ID and secret. All Stormpath REST invocations must be authenticated with a <see cref="IClientApiKey"/>.
    /// </summary>
    public interface IClientApiKey
    {
        /// <summary>
        /// Gets the public unique identifier. This can be publicly visible to anyone - it is not considered secure information.
        /// </summary>
        /// <returns>The public unique identifier (API Key ID).</returns>
        string GetId();

        /// <summary>
        /// Gets the raw secret used for API authentication.
        /// <b>NEVER EVER</b> print this value anywhere - logs, files, etc. It is TOP SECRET.
       ///  This should not be publicly visible to anyone other than the person to which the <see cref="IClientApiKey"/> is assigned. It is considered secure information.
        /// </summary>
        /// <returns>The raw secret (API Key Secret) used for authentication.</returns>
        string GetSecret();

        /// <summary>
        /// Checks whether the key/secret pair represented by this <see cref="IClientApiKey"/> is valid.
        /// This is a client-side check for convenience only. It does <b>not</b> make a request to the Stormpath server.
        /// </summary>
        /// <returns><see langword="true"/> if the API Key ID and Secret are not empty; <see langword="false"/> otherwise.</returns>
        bool IsValid();
    }
}
