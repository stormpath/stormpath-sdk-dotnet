// <copyright file="RedisTestBase.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Client;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Tests.Common.Fakes;

namespace Stormpath.SDK.Extensions.Cache.Redis.Tests
{
    public abstract class RedisTestBase
    {
        private static readonly string BaseUrl = "https://api.stormpath.com/v1";
        protected readonly RedisTestFixture fixture;

        protected FakeHttpClient fakeHttpClient;
        protected IClient client;
        protected ILogger logger;

        public RedisTestBase(RedisTestFixture fixture)
        {
            this.fixture = fixture;

            fixture.FlushDatabase();
        }

        protected void CreateClient(TimeSpan? ttl, TimeSpan? tti)
        {
            var redisCacheProvider = new RedisCacheProvider(this.fixture.Connection);

            if (ttl != null)
                redisCacheProvider.SetDefaultTimeToLive(ttl.Value);
            if (tti != null)
                redisCacheProvider.SetDefaultTimeToIdle(tti.Value);

            this.logger = new InMemoryLogger();
            this.fakeHttpClient = new FakeHttpClient(BaseUrl);

            this.client = Clients.Builder()
                .UseHttpClient(this.fakeHttpClient)
                .SetCacheProvider(redisCacheProvider)
                .SetLogger(this.logger)
                .Build();
        }
    }
}
