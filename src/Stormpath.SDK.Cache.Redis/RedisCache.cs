using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Cache.Redis;
using Stormpath.SDK.Logging;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Redis
{
    internal sealed class RedisCache : IAsynchronousCache, ISynchronousCache
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly ILogger _logger;

        public RedisCache(
            IConnectionMultiplexer connection,
            ILogger logger,
            string region,
            TimeSpan? ttl,
            TimeSpan? tti)
        {
            _connection = connection;
            _logger = logger;

            Name = region;
            TimeToLive = ttl;
            TimeToIdle = tti;
        }

        public string Name { get; }

        public TimeSpan? TimeToLive { get; }

        public TimeSpan? TimeToIdle { get; }

        void IDisposable.Dispose()
        {
        }

#pragma warning disable CS4014 // Use await for async calls
        public async Task<Map> GetAsync(string key, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var db = _connection.GetDatabase();
                var cacheKey = ConstructKey(key);

                var transaction = db.CreateTransaction();
                var value = transaction.StringGetAsync(cacheKey);
                transaction.KeyExpireAsync(cacheKey, TimeToIdle);
                await transaction.ExecuteAsync().ConfigureAwait(false);

                if (value.Result.IsNullOrEmpty)
                {
                    return null;
                }

                var entry = CacheEntry.Parse(value.Result);
                if (IsExpired(entry))
                {
                    _logger.Trace($"Entry {cacheKey} was expired (TTL), purging", "RedisAsyncCache.GetAsync");
                    await db.KeyDeleteAsync(cacheKey).ConfigureAwait(false);
                    return null;
                }

                var map = JsonConvert.DeserializeObject<Map>(entry.Data, Constants.SerializerSettings);
                return map;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while getting cached value.", "RedisAsyncCache.GetAsync");
                return null;
            }
        }
#pragma warning restore CS4014

        public async Task<Map> PutAsync(string key, Map value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var db = _connection.GetDatabase();

                var cacheKey = this.ConstructKey(key);
                var cacheData = JsonConvert.SerializeObject(value, Constants.SerializerSettings);

                var entry = new CacheEntry(
                    cacheData,
                    DateTimeOffset.UtcNow);

                await db.StringSetAsync(cacheKey, entry.ToString(), TimeToIdle).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while storing value in cache.", "RedisAsyncCache.PutAsync");
            }

            return value;
        }

#pragma warning disable CS4014 // Use await for async calls
        public async Task<Map> RemoveAsync(string key, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var db = _connection.GetDatabase();
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
                var map = JsonConvert.DeserializeObject<Map>(entry.Data, Constants.SerializerSettings);
                return map;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while deleting value from cache.", "RedisAsyncCache.RemoveAsync");
                return null;
            }
        }
#pragma warning restore CS4014

        public Map Get(string key)
        {
            try
            {
                var db = _connection.GetDatabase();
                var cacheKey = this.ConstructKey(key);

                var transaction = db.CreateTransaction();
                var value = transaction.StringGetAsync(cacheKey);
                transaction.KeyExpireAsync(cacheKey, TimeToIdle);
                transaction.Execute();

                if (value.Result.IsNullOrEmpty)
                {
                    return null;
                }

                var entry = CacheEntry.Parse(value.Result);
                if (this.IsExpired(entry))
                {
                    _logger.Trace($"Entry {cacheKey} was expired (TTL), purging", "RedisSyncCache.Get");
                    db.KeyDelete(cacheKey);
                    return null;
                }

                var map = JsonConvert.DeserializeObject<Map>(entry.Data, Constants.SerializerSettings);
                return map;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while getting cached value.", "RedisSyncCache.Get");
                return null;
            }
        }

        public Map Put(string key, Map value)
        {
            try
            {
                var db = _connection.GetDatabase();

                var cacheKey = this.ConstructKey(key);
                var cacheData = JsonConvert.SerializeObject(value, Constants.SerializerSettings);

                var entry = new CacheEntry(
                    cacheData,
                    DateTimeOffset.UtcNow);

                db.StringSet(cacheKey, entry.ToString(), TimeToIdle);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while storing value in cache.", "RedisSyncCache.Put");
            }

            return value;
        }

        public Map Remove(string key)
        {
            try
            {
                var db = _connection.GetDatabase();
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
                var map = JsonConvert.DeserializeObject<Map>(entry.Data, Constants.SerializerSettings);
                return map;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while deleting value from cache.", "RedisSyncCache.Remove");
                return null;
            }
        }

        private string ConstructKey(string key)
        {
            var sanitizedKey = key.ToString().Replace("://", "--");

            return $"{Name}:{sanitizedKey}";
        }

        private bool IsExpired(CacheEntry entry)
        {
            if (TimeToLive == null)
            {
                return false;
            }

            return (DateTimeOffset.UtcNow - entry.CreatedAt) > TimeToLive.Value;
        }
    }
}
