// <copyright file="RedisCacheProvider.cs" company="Stormpath, Inc.">
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
using StackExchange.Redis;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Serialization;

namespace Stormpath.SDK.Extensions.Cache.Redis
{
    public class RedisCacheProvider : AbstractCacheProvider
    {
        private readonly IConnectionMultiplexer connection;
        private readonly IJsonSerializer serializer;
        private readonly ILogger logger;

        public RedisCacheProvider(string redisConfiguration, IJsonSerializer serializer, ILogger logger = null)
            : base(syncSupported: true, asyncSupported: true)
        {
            this.connection = ConnectionMultiplexer.Connect(redisConfiguration);
            this.serializer = serializer;
            this.logger = logger;
        }

        protected override IAsynchronousCache<K, V> CreateAsyncCache<K, V>(string name, TimeSpan? ttl, TimeSpan? tti)
        {
            return new RedisAsyncCache<K, V>(this.connection, this.serializer, name, ttl, tti);
        }

        protected override ISynchronousCache<K, V> CreateSyncCache<K, V>(string name, TimeSpan? ttl, TimeSpan? tti)
        {
            return new RedisSyncCache<K, V>(this.connection, this.serializer, name, ttl, tti);
        }
    }
}
