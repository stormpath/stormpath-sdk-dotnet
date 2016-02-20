// <copyright file="RedisSyncCache.cs" company="Stormpath, Inc.">
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
using StackExchange.Redis;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Serialization;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Cache.Redis
{
    internal sealed class RedisSyncCache : ISynchronousCache
    {
        private readonly IConnectionMultiplexer connection;
        private readonly IJsonSerializer serializer;
        private readonly ILogger logger;
        private readonly string region;
        private readonly TimeSpan? ttl;
        private readonly TimeSpan? tti;

        public RedisSyncCache(
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

        Map ISynchronousCache.Get(string key)
        {
            try
            {
                var db = this.connection.GetDatabase();
                var cacheKey = this.ConstructKey(key);

                var transaction = db.CreateTransaction();
                var value = transaction.StringGetAsync(cacheKey);
                transaction.KeyExpireAsync(cacheKey, this.tti);
                transaction.Execute();

                if (value.Result.IsNullOrEmpty)
                {
                    return null;
                }

                var entry = CacheEntry.Parse(value.Result);
                if (this.IsExpired(entry))
                {
                    this.logger.Trace($"Entry {cacheKey} was expired (TTL), purging", "RedisSyncCache.Get");
                    db.KeyDelete(cacheKey);
                    return null;
                }

                var map = this.serializer.Deserialize(entry.Data);
                return map;
            }
            catch (Exception e)
            {
                this.logger.Error(e, "Error while getting cached value.", "RedisSyncCache.Get");
                return null;
            }
        }

        Map ISynchronousCache.Put(string key, Map value)
        {
            try
            {
                var db = this.connection.GetDatabase();

                var cacheKey = this.ConstructKey(key);
                var cacheData = this.serializer.Serialize(value);

                var entry = new CacheEntry(
                    cacheData,
                    DateTimeOffset.UtcNow);

                db.StringSet(cacheKey, entry.ToString(), this.tti);
            }
            catch (Exception e)
            {
                this.logger.Error(e, "Error while storing value in cache.", "RedisSyncCache.Put");
            }

            return value;
        }

#pragma warning disable CS4014 // Use await for async calls
        Map ISynchronousCache.Remove(string key)
        {
            try
            {
                var db = this.connection.GetDatabase();
                var cacheKey = this.ConstructKey(key);

                var transaction = db.CreateTransaction();
                var lastValue = transaction.StringGetAsync(cacheKey);
                transaction.KeyDeleteAsync(cacheKey);
                var committed = transaction.Execute();

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
                this.logger.Error(e, "Error while deleting value from cache.", "RedisSyncCache.Remove");
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
