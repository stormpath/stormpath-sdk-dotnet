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
using Stormpath.SDK.Client;
using Stormpath.SDK.Extensions.Serialization;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Tests.Common;
using Xunit;

namespace Stormpath.SDK.Extensions.Cache.Redis.Tests
{
    [Collection(nameof(RedisTestCollection))]
    public class AsyncTests
    {
        private readonly RedisTestFixture fixture;
        private readonly IClient client;

        public AsyncTests(RedisTestFixture fixture)
        {
            this.fixture = fixture;

            var redisCacheProvider = new RedisCacheProvider(fixture.Connection, new JsonNetSerializer());
            redisCacheProvider.SetDefaultTimeToIdle(TimeSpan.FromSeconds(30));
            redisCacheProvider.SetDefaultTimeToLive(TimeSpan.FromSeconds(60));

            var logger = new InMemoryLogger();

            this.client = Clients.Builder()
                .SetCacheProvider(redisCacheProvider)
                .SetLogger(logger)
                .Build();
        }

        [DebugOnlyFact]
        public async Task Resource_is_cached()
        {
            var tenant = await this.client.GetCurrentTenantAsync();
            var application = await tenant.GetApplications().Where(x => x.Name == "My Application").SingleAsync();

            var db = this.fixture.Connection.GetDatabase();
            var key = TestHelper.CreateKey(application);

            var cached = await db.StringGetAsync(key);
            cached.ToString().ShouldNotBeNullOrEmpty();
        }
    }
}
