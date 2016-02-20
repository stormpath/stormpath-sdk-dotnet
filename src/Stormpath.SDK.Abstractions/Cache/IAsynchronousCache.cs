// <copyright file="IAsynchronousCache.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Cache
{
    /// <summary>
    /// This interface provides access to an underlying asynchronous cache framework's cache instance.
    /// </summary>
    public interface IAsynchronousCache : ICache
    {
        /// <summary>
        /// Gets the cached value stored under the specified key.
        /// </summary>
        /// <param name="key">The key that the value was added with.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The cached object, or <see langword="null"/> if there is no entry for the specified key.</returns>
        Task<Map> GetAsync(string key, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds a cache entry.
        /// </summary>
        /// <param name="key">The key used to identify the object being stored.</param>
        /// <param name="value">The value to be stored in the cache.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The previous value associated with the given key, or <see langword="null"/> if there was no previous value.</returns>
        Task<Map> PutAsync(string key, Map value, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Removes the cached value stored under the specified key.
        /// </summary>
        /// <param name="key">The key used to identify the object being stored.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The removed value, or <see langword="null"/> if there was no value cached.</returns>
        Task<Map> RemoveAsync(string key, CancellationToken cancellationToken = default(CancellationToken));
    }
}
