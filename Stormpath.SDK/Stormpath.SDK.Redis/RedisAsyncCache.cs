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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;
using Stormpath.SDK.Cache;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Extensions.Cache.Redis
{
    internal sealed class RedisAsyncCache : IAsynchronousCache<string, Map>
    {
        private readonly IConnectionMultiplexer connection;
        private readonly string region;
        private readonly TimeSpan? ttl;
        private readonly TimeSpan? tti;

        public RedisAsyncCache(
            IConnectionMultiplexer connection,
            string region,
            TimeSpan? ttl,
            TimeSpan? tti)
        {
            this.connection = connection;
            this.region = region;
            this.ttl = ttl;
            this.tti = tti;
        }

        string ICache<string, Map>.Name => this.region;

        TimeSpan? ICache<string, Map>.TimeToLive => this.ttl;

        TimeSpan? ICache<string, Map>.TimeToIdle => this.tti;

        void IDisposable.Dispose() { }

        Task<Map> IAsynchronousCache<string, Map>.GetAsync(string key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<Map> IAsynchronousCache<string, Map>.PutAsync(string key, Map value, CancellationToken cancellationToken)
        {
            var db = this.connection.GetDatabase();

            var regionKey = this.ConstructKey(key);
            //var cacheValue = new HashEntry[]
            //{
            //    new HashEntry("data", (object)value),
            //    new HashEntry("ttl", ttl),
            //    new HashEntry("tti", tti)
            //};

            await db.HashSetAsync(regionKey, null /*todo cacheValue*/).ConfigureAwait(false);
            return value;
        }

        Task<Map> IAsynchronousCache<string, Map>.RemoveAsync(string key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private string ConstructKey(string key)
            => $"{this.region}:{key}";
    }
}
