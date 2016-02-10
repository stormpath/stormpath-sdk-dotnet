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
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Stormpath.SDK.Logging;

namespace Stormpath.SDK.Impl.IdentityMap
{
    internal class MemoryCacheIdentityMap<TItem> : IIdentityMap<TItem>, IDisposable
        where TItem : class
    {
        private readonly ILogger logger;
        private readonly IMemoryCache itemCache;
        private readonly TimeSpan slidingExpiration;
        private long lifetimeItemsAdded;
        private bool isDisposed = false; // To detect redundant calls

        private object @lock;

        public MemoryCacheIdentityMap(TimeSpan slidingExpiration, ILogger logger)
        {
            this.itemCache = new MemoryCache(new MemoryCacheOptions());
            this.slidingExpiration = slidingExpiration;
            this.logger = logger;
        }

        private static string CreateCacheKey(string key)
            => $"idmap-{key}";

        public long LifetimeItemsAdded => this.lifetimeItemsAdded;

        public TItem GetOrAdd(string key, Func<TItem> itemFactory, bool storeInfinitely)
        {
            var cacheKey = CreateCacheKey(key);

            bool added = false;
            Lazy<TItem> existingItem = null;
            Lazy<TItem> addedItem = new Lazy<TItem>(() => itemFactory());

            lock (@lock)
            {   
                if (!this.itemCache.TryGetValue(cacheKey, out existingItem))
                {
                    var options = new MemoryCacheEntryOptions();

                    if (storeInfinitely)
                    {
                        options.SetPriority(CacheItemPriority.NeverRemove);
                    }
                    else
                    {
                        options.SetSlidingExpiration(this.slidingExpiration);
                    }

                    this.itemCache.Set(cacheKey, addedItem, options);

                    added = true;
                }
            }

            if (added)
            {
                Interlocked.Increment(ref this.lifetimeItemsAdded);
                this.logger.Trace($"Added item to identity map with key '{key}'. (Lifetime items: {this.lifetimeItemsAdded})");
            }
            else
            {
                this.logger.Trace($"Retrieved item from identity map with key '{key}'. (Lifetime items: {this.lifetimeItemsAdded})");
            }

            return added
                ? addedItem.Value
                : existingItem.Value;
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
