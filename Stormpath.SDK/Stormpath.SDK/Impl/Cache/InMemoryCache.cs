// <copyright file="InMemoryCache.cs" company="Stormpath, Inc.">
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Cache;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.Cache
{
    internal sealed class InMemoryCache : ISynchronousCache, IAsynchronousCache
    {
        private readonly string region;
        private readonly TimeSpan? timeToLive;
        private readonly TimeSpan? timeToIdle;

        private InMemoryCacheManager cacheManager;
        private bool isDisposed = false;

        private long accessCount;
        private long hitCount;
        private long missCount;
        private long putCount;

        public InMemoryCache(string region)
            : this(region, timeToLive: null, timeToIdle: null)
        {
        }

        public InMemoryCache(string region, TimeSpan? timeToLive, TimeSpan? timeToIdle)
        {
            if (string.IsNullOrEmpty(region))
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (timeToLive.HasValue && timeToLive.Value.TotalMilliseconds <= 0)
            {
                throw new ArgumentException("TTL duration must be greater than zero.", nameof(timeToLive));
            }

            if (timeToIdle.HasValue && timeToIdle.Value.TotalMilliseconds <= 0)
            {
                throw new ArgumentException("TTI duration must be greater than zero.", nameof(timeToIdle));
            }

            this.cacheManager = new InMemoryCacheManager();
            this.region = region;
            this.timeToLive = timeToLive;
            this.timeToIdle = timeToIdle;

            this.accessCount = 0;
            this.hitCount = 0;
            this.missCount = 0;
        }

        private void ThrowIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new ApplicationException($"The object ({this.GetType().Name}) has been disposed.");
            }
        }

        private string CreateCompositeKey(string key)
        {
            return $"{this.region}-{key}";
        }

        private ISynchronousCache AsSyncInterface => this;

        string ICache.Name => this.region;

        TimeSpan? ICache.TimeToLive => this.timeToLive;

        TimeSpan? ICache.TimeToIdle => this.timeToIdle;

        /// <summary>
        /// Gets the number of items stored in all regions of the cache.
        /// </summary>
        /// <value>The total number of items stored in all regions of the cache.</value>
        public long TotalSize
        {
            get
            {
                this.ThrowIfDisposed();

                return this.cacheManager.Count;
            }
        }

        public long AccessCount => Interlocked.Read(ref this.accessCount);

        public long HitCount => Interlocked.Read(ref this.hitCount);

        public long MissCount => Interlocked.Read(ref this.missCount);

        public long PutCount => Interlocked.Read(ref this.putCount);

        public void Clear()
        {
            this.cacheManager.Dispose();
            this.cacheManager = new InMemoryCacheManager();
        }

        Map ISynchronousCache.Get(string key)
        {
            this.ThrowIfDisposed();

            Interlocked.Increment(ref this.accessCount);

            var compositeKey = this.CreateCompositeKey(key);
            Map value = this.cacheManager.Get(compositeKey);

            if (value == null)
            {
                Interlocked.Increment(ref this.missCount);
            }
            else
            {
                Interlocked.Increment(ref this.hitCount);
            }

            return value;
        }

        Map ISynchronousCache.Put(string key, Map value)
        {
            this.ThrowIfDisposed();

            var compositeKey = this.CreateCompositeKey(key);
            var absoluteExpiration = this.timeToLive.HasValue
                ? DateTimeOffset.Now.Add(this.timeToLive.Value)
                : ObjectCache.InfiniteAbsoluteExpiration;
            var slidingExpiration = this.timeToIdle.HasValue
                ? this.timeToIdle.Value
                : ObjectCache.NoSlidingExpiration;

            Interlocked.Increment(ref this.putCount);

            return this.cacheManager.Put(compositeKey, value, absoluteExpiration, slidingExpiration);
        }

        Map ISynchronousCache.Remove(string key)
        {
            this.ThrowIfDisposed();

            Interlocked.Increment(ref this.accessCount);

            var compositeKey = this.CreateCompositeKey(key);
            var value = this.cacheManager.Remove(compositeKey);

            if (value == null)
            {
                Interlocked.Increment(ref this.missCount);
            }
            else
            {
                Interlocked.Increment(ref this.hitCount);
            }

            return value;
        }

        Task<Map> IAsynchronousCache.GetAsync(string key, CancellationToken cancellationToken)
        {
            this.ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(this.AsSyncInterface.Get(key));
        }

        Task<Map> IAsynchronousCache.PutAsync(string key, Map value, CancellationToken cancellationToken)
        {
            this.ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(this.AsSyncInterface.Put(key, value));
        }

        Task<Map> IAsynchronousCache.RemoveAsync(string key, CancellationToken cancellationToken)
        {
            this.ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(this.AsSyncInterface.Remove(key));
        }

        public double GetHitRatio()
        {
            double accessCount = this.AccessCount;
            if (accessCount == 0)
            {
                return 0;
            }

            double hitCount = this.HitCount;
            return hitCount / accessCount;
        }

        public override string ToString()
        {
            this.ThrowIfDisposed();

            return new StringBuilder()
                .Append("{")
                .Append($@" ""region"": ""{this.region}"",")
                .Append($@" ""accessCount"": {this.AccessCount},")
                .Append($@" ""putCount"": {this.PutCount},")
                .Append($@" ""hitCount"": {this.HitCount},")
                .Append($@" ""missCount"": {this.MissCount},")
                .Append($@" ""hitRatio"": {this.GetHitRatio()}")
                .Append(" }")
                .ToString();
        }

        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                this.isDisposed = true;

                if (disposing)
                {
                    this.cacheManager.Dispose();
                }
            }
        }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }
    }
}
