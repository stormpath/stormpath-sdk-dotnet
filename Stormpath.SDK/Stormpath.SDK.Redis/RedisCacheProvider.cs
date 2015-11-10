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

namespace Stormpath.SDK.Extensions.Cache.Redis
{
    public class RedisCacheProvider : AbstractCacheProvider
    {
        private readonly IConnectionMultiplexer redisConnection;

        public RedisCacheProvider(string redisConfiguration)
            : base(syncSupported: true, asyncSupported: true)
        {
            this.redisConnection = ConnectionMultiplexer.Connect(redisConfiguration);
        }

        protected override IAsynchronousCache<K, V> CreateAsyncCache<K, V>(string name, TimeSpan? ttl, TimeSpan? tti)
        {
            throw new NotImplementedException();
        }

        protected override ISynchronousCache<K, V> CreateSyncCache<K, V>(string name, TimeSpan? ttl, TimeSpan? tti)
        {
            throw new NotImplementedException();
        }
    }
}
