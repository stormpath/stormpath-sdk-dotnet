// <copyright file="IAsynchronousNonceStore.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;

namespace Stormpath.SDK.IdSite
{
    public interface IAsynchronousNonceStore : INonceStore
    {
        /// <summary>
        /// Determines whether this <see cref="INonceStore"/> contains the given nonce.
        /// </summary>
        /// <param name="nonce">The nonce to check.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is <c>true</c> if the specified nonce is present in this <see cref="INonceStore"/>, <c>false</c> otherwise.</returns>
        Task<bool> ContainsNonceAsync(string nonce, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds the specified nonce to the store.
        /// </summary>
        /// <param name="nonce">The nonce to add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <seealso cref="ContainsNonceAsync(string)"/>
        /// <returns>A Task indicating the completion of the method.</returns>
        Task PutNonceAsync(string nonce, CancellationToken cancellationToken = default(CancellationToken));
    }
}
