using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;
using Stormpath.SDK.Cache;

namespace Stormpath.SDK.Redis
{
    internal sealed class RedisAsyncCache<K, V> : IAsynchronousCache<K, V>
    {
        private readonly IConnectionMultiplexer connection;

        public RedisAsyncCache(IConnectionMultiplexer connection,
            string region,
            CancellationToken cancellationToken)
        {
            var db = connection.GetDatabase();
        }

        string ICache<K, V>.Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        TimeSpan? ICache<K, V>.TimeToIdle
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        TimeSpan? ICache<K, V>.TimeToLive
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        Task<V> IAsynchronousCache<K, V>.GetAsync(K key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<V> IAsynchronousCache<K, V>.PutAsync(K key, V value, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<V> IAsynchronousCache<K, V>.RemoveAsync(K key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
