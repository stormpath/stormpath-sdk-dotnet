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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Cache;
using Map = System.Collections.Generic.IDictionary<string, object>;

#if NET45
using Stormpath.SDK.Impl.Cache.Polyfill.Microsoft.Extensions.Caching.Memory;
#else
using Microsoft.Extensions.Caching.Memory;
#endif

namespace Stormpath.SDK.Impl.Cache
{
    internal sealed class InMemoryCache : ISynchronousCache, IAsynchronousCache
    {
        private readonly string region;
        private readonly TimeSpan? timeToLive;
        private readonly TimeSpan? timeToIdle;

        private IMemoryCache memoryCache;
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

            this.memoryCache = Create();
            this.region = region;
            this.timeToLive = timeToLive;
            this.timeToIdle = timeToIdle;

            this.accessCount = 0;
            this.hitCount = 0;
            this.missCount = 0;
        }

        private static IMemoryCache Create()
            => new MemoryCache(new MemoryCacheOptions());

        private void ThrowIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new Exception($"The object ({this.GetType().Name}) has been disposed.");
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

        public long AccessCount => Interlocked.Read(ref this.accessCount);

        public long HitCount => Interlocked.Read(ref this.hitCount);

        public long MissCount => Interlocked.Read(ref this.missCount);

        public long PutCount => Interlocked.Read(ref this.putCount);

        public void Clear()
        {
            this.memoryCache.Dispose();
            this.memoryCache = Create();
        }

        Map ISynchronousCache.Get(string key)
        {
            this.ThrowIfDisposed();

            Interlocked.Increment(ref this.accessCount);

            var compositeKey = this.CreateCompositeKey(key);

            Map value = null;
            bool found = this.memoryCache.TryGetValue(compositeKey, out value);

            if (found)
            {
                Interlocked.Increment(ref this.hitCount);
            }
            else
            {
                Interlocked.Increment(ref this.missCount);
            }

            return value;
        }

        Map ISynchronousCache.Put(string key, Map value)
        {
            this.ThrowIfDisposed();

            var compositeKey = this.CreateCompositeKey(key);

            var options = new MemoryCacheEntryOptions();

            if (this.timeToLive != null)
            {
                options.SetAbsoluteExpiration(this.timeToLive.Value);
            }

            if (this.timeToIdle != null)
            {
                options.SetSlidingExpiration(this.timeToIdle.Value);
            }

            Interlocked.Increment(ref this.putCount);

            return this.memoryCache.Set<Map>(compositeKey, value, options);
        }

        Map ISynchronousCache.Remove(string key)
        {
            this.ThrowIfDisposed();

            // Get the item, if it exists
            var item = this.AsSyncInterface.Get(key);

            // Actually remove it
            this.memoryCache.Remove(this.CreateCompositeKey(key));

            return item; // or null
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
                    this.memoryCache.Dispose();
                }
            }
        }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }
    }
}
