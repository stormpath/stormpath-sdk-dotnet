// <copyright file="InMemoryCacheManager.cs" company="Stormpath, Inc.">
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

using System;
using System.Runtime.Caching;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.Cache
{
    /// <summary>
    /// A wrapper around <see cref="MemoryCache"/> that allows for both absolute and sliding expirations. (This separates implementation details from <see cref="InMemoryCache"/>.)
    /// </summary>
    internal sealed class InMemoryCacheManager : IDisposable
    {
        private readonly MemoryCache memoryCache;
        private bool isDisposed = false;

        public InMemoryCacheManager()
        {
            this.memoryCache = new MemoryCache("StormpathSDK");
        }

        private static string CreateAbsoluteTokenKey(string key)
        {
            return $"{key}-absoluteToken";
        }

        private void ThrowIfDisposed()
        {
            if (this.isDisposed)
                throw new ApplicationException($"The object ({this.GetType().Name}) has been disposed.");
        }

        public Map Get(string key)
        {
            this.ThrowIfDisposed();

            var absoluteTokenKey = CreateAbsoluteTokenKey(key);
            var tokenAndItem = this.memoryCache.GetValues(new string[] { absoluteTokenKey, key });

            bool itemHasNotExpired =
                tokenAndItem != null &&
                (tokenAndItem.ContainsKey(absoluteTokenKey) && tokenAndItem[absoluteTokenKey] != null) &&
                (tokenAndItem.ContainsKey(key) && tokenAndItem[key] != null);

            if (itemHasNotExpired)
                return tokenAndItem[key] as Map;
            else
                return null;
        }

        public Map Put(string key, Map value, DateTimeOffset absoluteExpiration, TimeSpan slidingExpiration)
        {
            this.ThrowIfDisposed();

            var absoluteTokenKey = CreateAbsoluteTokenKey(key);
            this.memoryCache.Set(absoluteTokenKey, new object(), absoluteExpiration);

            // Create a monitor to link the two items
            var monitor = this.memoryCache.CreateCacheEntryChangeMonitor(new string[] { absoluteTokenKey });

            var mainItemPolicy = new CacheItemPolicy();
            mainItemPolicy.SlidingExpiration = slidingExpiration;
            mainItemPolicy.ChangeMonitors.Add(monitor);
            this.memoryCache.Set(key, value, mainItemPolicy);

            return value;
        }

        public Map Remove(string key)
        {
            this.ThrowIfDisposed();

            this.memoryCache.Remove(CreateAbsoluteTokenKey(key));
            return this.memoryCache.Remove(key) as Map;
        }

        public long Count
        {
            get
            {
                this.ThrowIfDisposed();

                return this.memoryCache.GetCount() / 2;
            }
        }

        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                this.isDisposed = true;

                if (disposing)
                {
                    this.memoryCache.Dispose();
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }
    }
}
