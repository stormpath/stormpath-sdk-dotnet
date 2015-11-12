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

        async Task<V> IAsynchronousCache<K, V>.GetAsync(K key, CancellationToken cancellationToken)
        {
            var db = this.connection.GetDatabase();
            var cacheKey = this.ConstructKey(key);

            cancellationToken.ThrowIfCancellationRequested();
            var value = await db.HashGetAllAsync(cacheKey).ConfigureAwait(false);
            var storedAt = DateTimeOffset.Parse(value[1].Value);
            var accessedAt = DateTimeOffset.Parse(value[2].Value);
            var itemTtl = DeserializeTimeSpan(value[3].Value);
            var itemTti = DeserializeTimeSpan(value[4].Value);

            if (IsExpired(storedAt, accessedAt, itemTtl, itemTti))
            {
                await db.KeyDeleteAsync(cacheKey).ConfigureAwait(false);
                return default(V);
            }

            await db.HashSetAsync(cacheKey, "accessed", DateTimeOffset.UtcNow.ToString(), When.Exists).ConfigureAwait(false);

            var cacheData = value[0].Value;
            var map = this.serializer.Deserialize(cacheData);
            return (V)map;
        }

        async Task<V> IAsynchronousCache<K, V>.PutAsync(K key, V value, CancellationToken cancellationToken)
        {
            var db = this.connection.GetDatabase();

            var cacheKey = this.ConstructKey(key);
            var cacheData = this.serializer.Serialize((Map)value);

            var cacheValue = new HashEntry[]
            {
                new HashEntry("data", cacheData),
                new HashEntry("stored", DateTimeOffset.UtcNow.ToString()),
                new HashEntry("accessed", DateTimeOffset.UtcNow.ToString()),
                new HashEntry("ttl", SerializeTimeSpan(this.ttl)),
                new HashEntry("tti", SerializeTimeSpan(this.tti))
            };

            cancellationToken.ThrowIfCancellationRequested();
            await db.HashSetAsync(cacheKey, cacheValue).ConfigureAwait(false);
            return value;
        }

        async Task<V> IAsynchronousCache<K, V>.RemoveAsync(K key, CancellationToken cancellationToken)
        {
            var db = this.connection.GetDatabase();
            var cacheKey = this.ConstructKey(key);

            cancellationToken.ThrowIfCancellationRequested();
            var transaction = db.CreateTransaction();
            var lastValue = transaction.HashGetAllAsync(cacheKey);
            var deleteResult = transaction.KeyDeleteAsync(cacheKey);
            var committed = await transaction.ExecuteAsync().ConfigureAwait(false);

            if (!committed)
                return default(V);

            var cacheData = lastValue.Result[0].Value;
            var map = this.serializer.Deserialize(cacheData);
            return (V)map;
        }

        private string ConstructKey(K key)
            => $"{this.region}:{key.ToString()}";

        private static bool IsExpired(DateTimeOffset storedAt, DateTimeOffset accessedAt, TimeSpan? timeToLive, TimeSpan? timeToIdle)
        {
            var now = DateTimeOffset.UtcNow;

            if (timeToIdle != null && (now - accessedAt > timeToIdle))
                return false;

            if (timeToLive != null && (now - storedAt) > timeToLive)
                return false;

            return true;
        }

        private static RedisValue SerializeTimeSpan(TimeSpan? timeSpan)
        {
            return timeSpan == null
                ? RedisValue.Null
                : (long)timeSpan.Value.TotalMilliseconds;
        }

        private static TimeSpan? DeserializeTimeSpan(RedisValue value)
        {
            long millis;

            if (value.IsNull || !value.TryParse(out millis))
                return null;

            return TimeSpan.FromMilliseconds(millis);
        }
    }
}
