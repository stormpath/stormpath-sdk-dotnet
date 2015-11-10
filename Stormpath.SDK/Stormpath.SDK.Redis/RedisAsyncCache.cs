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

namespace Stormpath.SDK.Redis
{
    internal sealed class RedisAsyncCache<K, V> : IAsynchronousCache<K, V>
    {
        private readonly IConnectionMultiplexer connection;
        private readonly string region;

        public RedisAsyncCache(
            IConnectionMultiplexer connection,
            string region)
        {
            this.connection = connection;
            this.region = region;

            var db = connection.GetDatabase(); // todo
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
