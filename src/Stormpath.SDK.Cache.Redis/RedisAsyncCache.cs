// <copyright file="RedisAsyncCache.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Redis;
using Stormpath.SDK.Serialization;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Cache.Redis
{
    internal sealed class RedisAsyncCache : IAsynchronousCache
    {
        private readonly IConnectionMultiplexer connection;
        private readonly IJsonSerializer serializer;
        private readonly ILogger logger;
        private readonly string region;
        private readonly TimeSpan? ttl;
        private readonly TimeSpan? tti;

        public RedisAsyncCache(
            IConnectionMultiplexer connection,
            IJsonSerializer serializer,
            ILogger logger,
            string region,
            TimeSpan? ttl,
            TimeSpan? tti)
        {
            this.connection = connection;
            this.serializer = serializer;
            this.logger = logger;

            this.region = region;
            this.ttl = ttl;
            this.tti = tti;
        }

        string ICache.Name => this.region;

        TimeSpan? ICache.TimeToLive => this.ttl;

        TimeSpan? ICache.TimeToIdle => this.tti;

        void IDisposable.Dispose()
        {
        }

#pragma warning disable CS4014 // Use await for async calls
        async Task<Map> IAsynchronousCache.GetAsync(string key, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var db = this.connection.GetDatabase();
                var cacheKey = this.ConstructKey(key);

                var transaction = db.CreateTransaction();
                var value = transaction.StringGetAsync(cacheKey);
                transaction.KeyExpireAsync(cacheKey, this.tti);
                await transaction.ExecuteAsync().ConfigureAwait(false);

                if (value.Result.IsNullOrEmpty)
                {
                    return null;
                }

                var entry = CacheEntry.Parse(value.Result);
                if (this.IsExpired(entry))
                {
                    this.logger.Trace($"Entry {cacheKey} was expired (TTL), purging", "RedisAsyncCache.GetAsync");
                    await db.KeyDeleteAsync(cacheKey).ConfigureAwait(false);
                    return null;
                }

                var map = JsonConvert.DeserializeObject<IDictionary<string, object>>(entry.Data, Constants.SerializerSettings);
                return map;
            }
            catch (Exception e)
            {
                this.logger.Error(e, "Error while getting cached value.", "RedisAsyncCache.GetAsync");
                return null;
            }
        }
#pragma warning restore CS4014

        async Task<Map> IAsynchronousCache.PutAsync(string key, Map value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var db = this.connection.GetDatabase();

                var cacheKey = this.ConstructKey(key);
                var cacheData = JsonConvert.SerializeObject(value, Constants.SerializerSettings);

                var entry = new CacheEntry(
                    cacheData,
                    DateTimeOffset.UtcNow);

                await db.StringSetAsync(cacheKey, entry.ToString(), this.tti).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                this.logger.Error(e, "Error while storing value in cache.", "RedisAsyncCache.PutAsync");
            }

            return value;
        }

#pragma warning disable CS4014 // Use await for async calls
        async Task<Map> IAsynchronousCache.RemoveAsync(string key, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var db = this.connection.GetDatabase();
                var cacheKey = this.ConstructKey(key);

                var transaction = db.CreateTransaction();
                var lastValue = transaction.StringGetAsync(cacheKey);
                transaction.KeyDeleteAsync(cacheKey);
                var committed = await transaction.ExecuteAsync().ConfigureAwait(false);

                if (!committed)
                {
                    return null;
                }

                var entry = CacheEntry.Parse(lastValue.Result);
                var map = this.serializer.Deserialize(entry.Data);
                return map;
            }
            catch (Exception e)
            {
                this.logger.Error(e, "Error while deleting value from cache.", "RedisAsyncCache.RemoveAsync");
                return null;
            }
        }
#pragma warning restore CS4014

        private string ConstructKey(string key)
        {
            var sanitizedKey = key.ToString().Replace("://", "--");

            return $"{this.region}:{sanitizedKey}";
        }

        private bool IsExpired(CacheEntry entry)
        {
            if (this.ttl == null)
            {
                return false;
            }

            return (DateTimeOffset.UtcNow - entry.CreatedAt) > this.ttl.Value;
        }
    }
}
