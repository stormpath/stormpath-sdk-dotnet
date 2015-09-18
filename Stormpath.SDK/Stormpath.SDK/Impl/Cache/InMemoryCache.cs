// <copyright file="InMemoryCache.cs" company="Stormpath, Inc.">
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Cache;

namespace Stormpath.SDK.Impl.Cache
{
    internal sealed class InMemoryCache<K, V> : ISynchronousCache<K, V>, IAsynchronousCache<K, V>, IDisposable
        where V : class
    {
        private readonly string region;
        private readonly TimeSpan? timeToLive;
        private readonly TimeSpan? timeToIdle;

        private InMemoryCacheManager<V> cacheManager;
        private bool alreadyDisposed = false;

        private long accessCount;
        private long hitCount;
        private long missCount;

        public InMemoryCache(string region)
            : this(region, timeToLive: null, timeToIdle: null)
        {
        }

        public InMemoryCache(string region, TimeSpan? timeToLive, TimeSpan? timeToIdle)
        {
            if (string.IsNullOrEmpty(region))
                throw new ArgumentNullException(nameof(region));
            if (timeToLive.HasValue && timeToLive.Value.TotalMilliseconds <= 0)
                throw new ArgumentException("TTL duration must be greater than zero.", nameof(timeToLive));
            if (timeToIdle.HasValue && timeToIdle.Value.TotalMilliseconds <= 0)
                throw new ArgumentException("TTI duration must be greater than zero.", nameof(timeToIdle));

            this.cacheManager = new InMemoryCacheManager<V>();
            this.region = region;
            this.timeToLive = timeToLive;
            this.timeToIdle = timeToIdle;

            this.accessCount = 0;
            this.hitCount = 0;
            this.missCount = 0;
        }

        private string CreateCompositeKey(K key)
        {
            return $"{this.region}-{key}";
        }

        private ISynchronousCache<K, V> AsSyncInterface => this;

        string ICache<K, V>.Name => this.region;

        /// <summary>
        /// Gets the number of items stored in all regions of the cache.
        /// </summary>
        /// <value>The total number of items stored in all regions of the cache.</value>
        public long TotalSize => this.cacheManager.Count;

        public TimeSpan? TimeToLive => this.timeToLive;

        public TimeSpan? TimeToIdle => this.timeToIdle;

        public long AccessCount => Interlocked.Read(ref this.accessCount);

        public long HitCount => Interlocked.Read(ref this.hitCount);

        public long MissCount => Interlocked.Read(ref this.missCount);

        public void Clear()
        {
            this.cacheManager.Dispose();
            this.cacheManager = new InMemoryCacheManager<V>();
        }

        V ISynchronousCache<K, V>.Get(K key)
        {
            Interlocked.Increment(ref this.accessCount);

            var compositeKey = this.CreateCompositeKey(key);
            V value = this.cacheManager.Get(compositeKey);

            if (value == null)
                Interlocked.Increment(ref this.missCount);
            else
                Interlocked.Increment(ref this.hitCount);

            return value;
        }

        V ISynchronousCache<K, V>.Put(K key, V value)
        {
            var compositeKey = this.CreateCompositeKey(key);
            var absoluteExpiration = this.timeToLive.HasValue
                ? DateTimeOffset.Now.Add(this.timeToLive.Value)
                : ObjectCache.InfiniteAbsoluteExpiration;
            var slidingExpiration = this.timeToIdle.HasValue
                ? this.timeToIdle.Value
                : ObjectCache.NoSlidingExpiration;

            return this.cacheManager.Put(compositeKey, value, absoluteExpiration, slidingExpiration);
        }

        V ISynchronousCache<K, V>.Remove(K key)
        {
            Interlocked.Increment(ref this.accessCount);

            var compositeKey = this.CreateCompositeKey(key);
            var value = this.cacheManager.Remove(compositeKey);

            if (value == null)
                Interlocked.Increment(ref this.missCount);
            else
                Interlocked.Increment(ref this.hitCount);

            return value;
        }

        async Task<V> IAsynchronousCache<K, V>.GetAsync(K key, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Yield();
            return this.AsSyncInterface.Get(key);
        }

        async Task<V> IAsynchronousCache<K, V>.PutAsync(K key, V value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Yield();
            return this.AsSyncInterface.Put(key, value);
        }

        async Task<V> IAsynchronousCache<K, V>.RemoveAsync(K key, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Yield();
            return this.AsSyncInterface.Remove(key);
        }

        public double GetHitRatio()
        {
            double accessCount = this.AccessCount;
            if (accessCount == 0)
                return 0;

            double hitCount = this.HitCount;
            return hitCount / accessCount;
        }

        public override string ToString()
        {
            return new StringBuilder()
                .AppendLine($"Region: {this.region}")
                .AppendLine($"AccessCount: {this.AccessCount}")
                .AppendLine($"HitCount: {this.HitCount}")
                .AppendLine($"MissCount: {this.MissCount}")
                .AppendLine($"HitRatio: {this.GetHitRatio()}")
                .ToString();
        }

        private void Dispose(bool disposing)
        {
            if (!this.alreadyDisposed)
            {
                if (disposing)
                {
                    this.cacheManager.Dispose();
                }

                this.alreadyDisposed = true;
            }
        }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }
    }
}
