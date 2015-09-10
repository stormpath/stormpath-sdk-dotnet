// <copyright file="InMemoryCacheManager.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System;
using System.Runtime.Caching;

namespace Stormpath.SDK.Impl.Cache
{
    /// <summary>
    /// A wrapper around <see cref="MemoryCache"/> that allows for both absolute and sliding expirations. (This separates implementation details from <see cref="InMemoryCache{K, V}"/>.)
    /// </summary>
    /// <typeparam name="V">The type of vaules stored in the cache.</typeparam>
    internal sealed class InMemoryCacheManager<V> : IDisposable
        where V : class
    {
        private readonly MemoryCache memoryCache;
        private bool alreadyDisposed = false;

        public InMemoryCacheManager()
        {
            this.memoryCache = new MemoryCache("StormpathSDK");
        }

        private static string CreateAbsoluteTokenKey(string key)
        {
            return $"{key}-absoluteToken";
        }

        public V Get(string key)
        {
            var absoluteTokenKey = CreateAbsoluteTokenKey(key);
            var tokenAndItem = this.memoryCache.GetValues(new string[] { absoluteTokenKey, key });

            bool itemHasNotExpired =
                tokenAndItem != null &&
                (tokenAndItem.ContainsKey(absoluteTokenKey) && tokenAndItem[absoluteTokenKey] != null) &&
                (tokenAndItem.ContainsKey(key) && tokenAndItem[key] != null);

            if (itemHasNotExpired)
                return (V)tokenAndItem[key];
            else
                return null;
        }

        public V Put(string key, V value, DateTimeOffset absoluteExpiration, TimeSpan slidingExpiration)
        {
            var absoluteTokenKey = CreateAbsoluteTokenKey(key);
            bool absoluteTokenInserted = this.memoryCache.Add(absoluteTokenKey, new object(), absoluteExpiration);

            // Create a monitor to link the two items
            var monitor = this.memoryCache.CreateCacheEntryChangeMonitor(new string[] { absoluteTokenKey });

            var mainItemPolicy = new CacheItemPolicy();
            mainItemPolicy.SlidingExpiration = slidingExpiration;
            mainItemPolicy.ChangeMonitors.Add(monitor);
            bool mainItemInserted = this.memoryCache.Add(key, value, mainItemPolicy);

            if (absoluteTokenInserted && mainItemInserted)
                return value;
            else
                return null;
        }

        public V Remove(string key)
        {
            this.memoryCache.Remove(CreateAbsoluteTokenKey(key));
            return (V)this.memoryCache.Remove(key);
        }

        public long Count => this.memoryCache.GetCount() / 2;

        private void Dispose(bool disposing)
        {
            if (!this.alreadyDisposed)
            {
                if (disposing)
                {
                    this.memoryCache.Dispose();
                }

                this.alreadyDisposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }
    }
}
