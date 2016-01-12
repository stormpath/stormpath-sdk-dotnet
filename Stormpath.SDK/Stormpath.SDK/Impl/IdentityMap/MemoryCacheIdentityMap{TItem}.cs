// <copyright file="MemoryCacheIdentityMap{TItem}.cs" company="Stormpath, Inc.">
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

using System;
using System.Runtime.Caching;
using System.Threading;
using Stormpath.SDK.Logging;

namespace Stormpath.SDK.Impl.IdentityMap
{
    internal class MemoryCacheIdentityMap<TItem> : IIdentityMap<TItem>, IDisposable
        where TItem : class
    {
        private readonly ILogger logger;
        private readonly MemoryCache itemCache;
        private readonly TimeSpan slidingExpiration;
        private long lifetimeItemsAdded;
        private bool isDisposed = false; // To detect redundant calls

        public MemoryCacheIdentityMap(TimeSpan slidingExpiration, ILogger logger)
        {
            this.itemCache = new MemoryCache("StormpathSDKIdentityMap");
            this.slidingExpiration = slidingExpiration;
            this.logger = logger;
        }

        private static string CreateCacheKey(string key)
            => $"idmap-{key}";

        public long LifetimeItemsAdded => this.lifetimeItemsAdded;

        public TItem GetOrAdd(string key, Func<TItem> itemFactory, bool storeInfinitely)
        {
            var lazyItem = new Lazy<TItem>(() => itemFactory());
            var policy = new CacheItemPolicy()
            {
                SlidingExpiration = storeInfinitely
                    ? ObjectCache.NoSlidingExpiration
                    : this.slidingExpiration
            };

            var existing = this.itemCache.AddOrGetExisting(CreateCacheKey(key), lazyItem, policy);

            bool added = existing == null;
            if (added)
            {
                Interlocked.Increment(ref this.lifetimeItemsAdded);
                this.logger.Trace($"Added item to identity map with key '{key}'. (Lifetime items: {this.lifetimeItemsAdded})");
            }
            else
            {
                this.logger.Trace($"Retrieved item from identity map with key '{key}'. (Lifetime items: {this.lifetimeItemsAdded})");
            }

            return existing == null
                ? lazyItem.Value
                : (existing as Lazy<TItem>).Value;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                this.isDisposed = true;

                if (disposing)
                {
                    this.itemCache.Dispose();
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }
    }
}
