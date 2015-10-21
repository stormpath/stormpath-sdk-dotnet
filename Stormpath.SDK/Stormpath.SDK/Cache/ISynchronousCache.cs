// <copyright file="ISynchronousCache.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Cache
{
    /// <summary>
    /// This interface provides an abstraction (wrapper) API on top of an underlying synchronous cache framework's cache instance.
    /// </summary>
    /// <typeparam name="K">The key type stored in the cache.</typeparam>
    /// <typeparam name="V">The value type stored in the cache.</typeparam>
    public interface ISynchronousCache<K, V> : ICache<K, V>
    {
        /// <summary>
        /// Gets the cached value stored under the specified key.
        /// </summary>
        /// <param name="key">The key that the value was added with.</param>
        /// <returns>The cached object, or <c>null</c> if there is no entry for the specified key.</returns>
        V Get(K key);

        /// <summary>
        /// Adds a cache entry.
        /// </summary>
        /// <param name="key">The key used to identify the object being stored.</param>
        /// <param name="value">The value to be stored in the cache.</param>
        /// <returns>The previous value associated with the given key, or <c>null</c> if there was no previous value.</returns>
        V Put(K key, V value);

        /// <summary>
        /// Removes the cached value stored under the specified key.
        /// </summary>
        /// <param name="key">The key used to identify the object being stored.</param>
        /// <returns>The removed value, or <c>null</c> if there was no value cached.</returns>
        V Remove(K key);
    }
}
