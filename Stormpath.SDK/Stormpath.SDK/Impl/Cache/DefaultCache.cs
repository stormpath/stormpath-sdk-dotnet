// <copyright file="DefaultCache.cs" company="Stormpath, Inc.">
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
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Impl.Extensions;

namespace Stormpath.SDK.Impl.Cache
{
    internal sealed class DefaultCache<K, V> : ISynchronousCache<K, V>, IAsynchronousCache<K, V>
    {
        private readonly ConcurrentDictionary<K, DefaultCacheEntry<V>> backingMap;
        private readonly TimeSpan? timeToLive;
        private readonly TimeSpan? timeToIdle;
        private readonly string name;

        private long accessCount;
        private long hitCount;
        private long missCount;

        public DefaultCache(string name)
            : this(name, new ConcurrentDictionary<K, DefaultCacheEntry<V>>(), null, null)
        {
        }

        public DefaultCache(string name, TimeSpan? timeToLive = null, TimeSpan? timeToIdle = null)
            : this(name, new ConcurrentDictionary<K, DefaultCacheEntry<V>>(), timeToLive, timeToIdle)
        {
        }

        internal DefaultCache(string name, ConcurrentDictionary<K, DefaultCacheEntry<V>> backingMap, TimeSpan? timeToLive, TimeSpan? timeToIdle)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (backingMap == null)
                throw new ArgumentNullException(nameof(backingMap));
            if (timeToLive.HasValue && timeToLive.Value.TotalMilliseconds <= 0)
                throw new ArgumentException("TTL duration must be greater than zero.", nameof(timeToLive));
            if (timeToIdle.HasValue && timeToIdle.Value.TotalMilliseconds <= 0)
                throw new ArgumentException("TTI duration must be greater than zero.", nameof(timeToIdle));

            this.name = name;
            this.backingMap = backingMap;
            this.timeToLive = timeToLive;
            this.timeToIdle = timeToIdle;

            this.accessCount = 0;
            this.hitCount = 0;
            this.missCount = 0;
        }

        internal ISynchronousCache<K, V> ThisSync => this;

        string ICache<K, V>.Name => this.name;

        V ISynchronousCache<K, V>.Get(K key)
        {
            Interlocked.Increment(ref this.accessCount);

            DefaultCacheEntry<V> entry = null;
            if (!this.backingMap.TryGetValue(key, out entry))
            {
                Interlocked.Increment(ref this.missCount);
                return default(V);
            }

            var ttl = this.timeToLive;
            var tti = this.timeToIdle;
            var nowMillis = DateTime.UtcNow.ToUnixTimestamp();

            if (ttl.HasValue)
            {
                var sinceCreation = TimeSpan.FromMilliseconds(nowMillis - entry.GetCreationTimeMilliseconds());
                if (sinceCreation > ttl)
                {
                    DefaultCacheEntry<V> ignored = null;
                    this.backingMap.TryRemove(key, out ignored);

                    Interlocked.Increment(ref this.missCount); // expired TTL = miss
                    return default(V);
                }
            }

            if (tti.HasValue)
            {
                var sinceLastAccess = TimeSpan.FromMilliseconds(nowMillis - entry.GetLastAccessTimeMilliseconds());
                if (sinceLastAccess > tti)
                {
                    DefaultCacheEntry<V> ignored = null;
                    this.backingMap.TryRemove(key, out ignored); // expired TTI = miss

                    Interlocked.Increment(ref this.missCount);
                    return default(V);
                }
            }

            entry.SetLastAccessTimeMilliseconds(nowMillis);
            Interlocked.Increment(ref this.hitCount);

            return entry.GetValue();
        }

        V ISynchronousCache<K, V>.Put(K key, V value)
        {
            var newEntry = new DefaultCacheEntry<V>(value);

            DefaultCacheEntry<V> previous = null;
            this.backingMap.TryRemove(key, out previous);

            this.backingMap.TryAdd(key, newEntry);

            if (previous != null)
                return previous.GetValue();

            return default(V);
        }

        V ISynchronousCache<K, V>.Remove(K key)
        {
            Interlocked.Increment(ref this.accessCount);

            DefaultCacheEntry<V> previous = null;
            if (this.backingMap.TryRemove(key, out previous))
            {
                Interlocked.Increment(ref this.hitCount);
                return previous.GetValue();
            }
            else
            {
                Interlocked.Increment(ref this.missCount);
                return default(V);
            }
        }

        async Task<V> IAsynchronousCache<K, V>.GetAsync(K key, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Yield();
            return this.ThisSync.Get(key);
        }

        async Task<V> IAsynchronousCache<K, V>.PutAsync(K key, V value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Yield();
            return this.ThisSync.Put(key, value);
        }

        async Task<V> IAsynchronousCache<K, V>.RemoveAsync(K key, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Yield();
            return this.ThisSync.Remove(key);
        }

        public TimeSpan? TimeToLive => this.timeToLive;

        public TimeSpan? TimeToIdle => this.timeToIdle;

        public long AccessCount => Interlocked.Read(ref this.accessCount);

        public long HitCount => Interlocked.Read(ref this.hitCount);

        public long MissCount => Interlocked.Read(ref this.missCount);

        public double GetHitRatio()
        {
            double accessCount = this.AccessCount;
            if (accessCount == 0)
                return 0;

            double hitCount = this.HitCount;
            return hitCount / accessCount;
        }

        public int Size => this.backingMap.Count;

        public void Clear()
        {
            this.backingMap.Clear();
        }

        public override string ToString()
        {
            return new StringBuilder()
                .AppendLine($"Name: {this.name}")
                .AppendLine($"AccessCount: {this.AccessCount}")
                .AppendLine($"HitCount: {this.HitCount}")
                .AppendLine($"MissCount: {this.MissCount}")
                .AppendLine($"HitRatio: {this.GetHitRatio()}")
                .ToString();
        }
    }
}
