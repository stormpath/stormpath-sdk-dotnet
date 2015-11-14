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
    public class RedisCacheProvider : AbstractCacheProvider, ISerializerConsumer<RedisCacheProvider>, ILoggerConsumer<RedisCacheProvider>
    {
        private readonly IConnectionMultiplexer connection;
        private IJsonSerializer serializer;
        private ILogger logger;

        public RedisCacheProvider(string redisConfiguration)
            : this(ConnectionMultiplexer.Connect(redisConfiguration))
        {
        }

        public RedisCacheProvider(IConnectionMultiplexer redisConnection)
            : base(syncSupported: true, asyncSupported: true)
        {
            this.connection = redisConnection;
        }

        public RedisCacheProvider SetSerializer(IJsonSerializer serializer)
        {
            if (serializer != null)
                this.serializer = serializer;
            return this;
        }

        public RedisCacheProvider SetLogger(ILogger logger)
        {
            if (logger != null)
                this.logger = logger;
            return this;
        }

        protected override IAsynchronousCache CreateAsyncCache(string name, TimeSpan? ttl, TimeSpan? tti)
        {
            return new RedisAsyncCache(this.connection, this.serializer, this.logger, name, ttl, tti);
        }

        protected override ISynchronousCache CreateSyncCache(string name, TimeSpan? ttl, TimeSpan? tti)
        {
            return new RedisSyncCache(this.connection, this.serializer, this.logger, name, ttl, tti);
        }
    }
}
