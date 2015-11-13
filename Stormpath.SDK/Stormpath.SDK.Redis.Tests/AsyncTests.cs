// <copyright file="AsyncTests.cs" company="Stormpath, Inc.">
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
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Application;
using Stormpath.SDK.Client;
using Stormpath.SDK.Extensions.Serialization;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Tests.Common;
using Stormpath.SDK.Tests.Common.Fakes;
using Xunit;

namespace Stormpath.SDK.Extensions.Cache.Redis.Tests
{
    [Collection(nameof(RedisTestCollection))]
    public class AsyncTests
    {
        private static readonly string BaseUrl = "https://api.stormpath.com/v1";

        private readonly RedisTestFixture fixture;
        private FakeHttpClient fakeHttpClient;
        private IClient client;

        public AsyncTests(RedisTestFixture fixture)
        {
            this.fixture = fixture;
        }

        private void CreateClient(TimeSpan? ttl, TimeSpan? tti)
        {
            var redisCacheProvider = new RedisCacheProvider(this.fixture.Connection, new JsonNetSerializer());

            if (ttl != null)
                redisCacheProvider.SetDefaultTimeToLive(ttl.Value);
            if (tti != null)
                redisCacheProvider.SetDefaultTimeToIdle(tti.Value);

            var logger = new InMemoryLogger();
            this.fakeHttpClient = new FakeHttpClient(BaseUrl);

            this.client = Clients.Builder()
                .UseHttpClient(this.fakeHttpClient)
                .SetCacheProvider(redisCacheProvider)
                .SetLogger(logger)
                .Build();
        }

        [DebugOnlyFact]
        public async Task Resource_is_cached()
        {
            this.CreateClient(ttl: null, tti: null);
            this.fakeHttpClient.SetupGet("/applications/foobarApplication", 200, FakeJson.Application);

            var application = await this.client.GetResourceAsync<IApplication>("https://api.stormpath.com/v1/applications/foobarApplication");

            var db = this.fixture.Connection.GetDatabase();
            var key = TestHelper.CreateKey(application);
            var cached = await db.StringGetAsync(key);
            cached.ToString().ShouldNotBeNullOrEmpty();
        }
    }
}
