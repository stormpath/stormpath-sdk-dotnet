// <copyright file="RedisAsyncCache.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Serialization;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Extensions.Cache.Redis
{
    internal sealed class RedisAsyncCache<K, V> : IAsynchronousCache<K, V>
    {
        private readonly IConnectionMultiplexer connection;
        private readonly IJsonSerializer serializer;
        private readonly string region;
        private readonly TimeSpan? ttl;
        private readonly TimeSpan? tti;

        public RedisAsyncCache(
            IConnectionMultiplexer connection,
            IJsonSerializer serializer,
            string region,
            TimeSpan? ttl,
            TimeSpan? tti)
        {
            this.connection = connection;
            this.serializer = serializer;
            this.region = region;
            this.ttl = ttl;
            this.tti = tti;
        }

        string ICache<K, V>.Name => this.region;

        TimeSpan? ICache<K, V>.TimeToLive => this.ttl;

        TimeSpan? ICache<K, V>.TimeToIdle => this.tti;

        void IDisposable.Dispose()
        {
        }

#pragma warning disable CS4014 // Use await for async calls
        async Task<V> IAsynchronousCache<K, V>.GetAsync(K key, CancellationToken cancellationToken)
        {
            var db = this.connection.GetDatabase();
            var cacheKey = this.ConstructKey(key);

            cancellationToken.ThrowIfCancellationRequested();

            var transaction = db.CreateTransaction();
            var value = transaction.StringGetAsync(cacheKey);
            transaction.KeyExpireAsync(cacheKey, this.tti);
            await transaction.ExecuteAsync().ConfigureAwait(false);

            if (value.Result.IsNullOrEmpty)
                return default(V);

            var entry = CacheEntry.Parse(value.Result);
            if (this.IsExpired(entry))
            {
                await db.KeyDeleteAsync(cacheKey).ConfigureAwait(false);
                return default(V);
            }

            var map = this.serializer.Deserialize(entry.Data);
            return (V)map;
        }
#pragma warning restore CS4014

        async Task<V> IAsynchronousCache<K, V>.PutAsync(K key, V value, CancellationToken cancellationToken)
        {
            var db = this.connection.GetDatabase();

            var cacheKey = this.ConstructKey(key);
            var cacheData = this.serializer.Serialize((Map)value);

            var entry = new CacheEntry(
                cacheData,
                DateTimeOffset.UtcNow);

            cancellationToken.ThrowIfCancellationRequested();
            await db.StringSetAsync(cacheKey, entry.ToString()).ConfigureAwait(false);
            return value;
        }

#pragma warning disable CS4014 // Use await for async calls
        async Task<V> IAsynchronousCache<K, V>.RemoveAsync(K key, CancellationToken cancellationToken)
        {
            var db = this.connection.GetDatabase();
            var cacheKey = this.ConstructKey(key);

            cancellationToken.ThrowIfCancellationRequested();

            var transaction = db.CreateTransaction();
            var lastValue = transaction.StringGetAsync(cacheKey);
            transaction.KeyDeleteAsync(cacheKey);
            var committed = await transaction.ExecuteAsync().ConfigureAwait(false);

            if (!committed)
                return default(V);

            var entry = CacheEntry.Parse(lastValue.Result);
            var map = this.serializer.Deserialize(entry.Data);
            return (V)map;
        }
#pragma warning restore CS4014

        private string ConstructKey(K key)
        {
            var sanitizedKey = key.ToString().Replace("://", "--");

            return $"{this.region}:{sanitizedKey}";
        }

        private bool IsExpired(CacheEntry entry)
        {
            if (this.ttl == null)
                return false;

            return (DateTimeOffset.UtcNow - entry.CreatedAt) > this.ttl.Value;
        }
    }
}
