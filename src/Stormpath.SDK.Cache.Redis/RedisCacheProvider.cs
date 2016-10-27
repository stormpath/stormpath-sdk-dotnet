// <copyright file="RedisCacheProvider.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Cache.Redis
{
    internal class RedisCacheProvider : AbstractCacheProvider, ILoggerConsumer<RedisCacheProvider>
    {
        private IConnectionMultiplexer connection;
        private ILogger logger;

        public RedisCacheProvider()
            : base(syncSupported: true, asyncSupported: true)
        {
        }

        public RedisCacheProvider SetLogger(ILogger logger)
        {
            this.logger = logger;
            return this;
        }

        public RedisCacheProvider SetRedisConnectionMultiplexer(IConnectionMultiplexer connection)
        {
            this.connection = connection;
            return this;
        }

        private void ThrowIfNotConfigured()
        {
            if (this.connection == null)
            {
                throw new Exception("No connection present. Set up the cache provider with NewRedisCacheProvider first.");
            }
        }

        protected override IAsynchronousCache CreateAsyncCache(string name, TimeSpan? ttl, TimeSpan? tti)
        {
            this.ThrowIfNotConfigured();

            return new RedisAsyncCache(this.connection, this.logger, name, ttl, tti);
        }

        protected override ISynchronousCache CreateSyncCache(string name, TimeSpan? ttl, TimeSpan? tti)
        {
            this.ThrowIfNotConfigured();

            return new RedisSyncCache(this.connection, this.logger, name, ttl, tti);
        }
    }
}
