// <copyright file="ISynchronousNonceStore.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.IdSite
{
    /// <inheritdoc/>
    /// <seealso cref="IIdSiteAsyncCallbackHandler.SetNonceStore(INonceStore)"/>
    public interface ISynchronousNonceStore : INonceStore
    {
        /// <summary>
        /// Determines whether this <see cref="INonceStore"/> contains the given nonce.
        /// </summary>
        /// <param name="nonce">The nonce to check.</param>
        /// <returns><c>true</c> if the specified nonce is present in this <see cref="INonceStore"/>, <c>false</c> otherwise.</returns>
        bool ContainsNonce(string nonce);

        /// <summary>
        /// Adds the specified nonce to the store.
        /// </summary>
        /// <param name="nonce">The nonce to add.</param>
        /// <seealso cref="ContainsNonce(string)"/>
        void PutNonce(string nonce);
    }
}
