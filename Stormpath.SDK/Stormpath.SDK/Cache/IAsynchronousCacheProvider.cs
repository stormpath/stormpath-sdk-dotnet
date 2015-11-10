// <copyright file="IAsynchronousCacheProvider.cs" company="Stormpath, Inc.">
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
    /// Provides and maintains the lifecycles of <see cref="IAsynchronousCache{K, V}"/> instances.
    /// </summary>
    public interface IAsynchronousCacheProvider : ICacheProvider
    {
        /// <summary>
        /// Acquires the cache with the specified <code>name</code>. If a cache does not yet exist with that name,
        /// a new one will be created with that name and returned.
        /// </summary>
        /// <param name="name">The name of the cache to acquire.</param>
        /// <typeparam name="K">The key type of the cache.</typeparam>
        /// <typeparam name="V">The value type of the cache.</typeparam>
        /// <returns>The cache with the given name.</returns>
        IAsynchronousCache<K, V> GetAsyncCache<K, V>(string name);
    }
}
