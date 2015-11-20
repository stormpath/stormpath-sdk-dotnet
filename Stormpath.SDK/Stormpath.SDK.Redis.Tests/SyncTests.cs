// <copyright file="SyncTests.cs" company="Stormpath, Inc.">
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
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Common;
using Stormpath.SDK.Tests.Common.Fakes;
using Xunit;

namespace Stormpath.SDK.Cache.Redis.Tests
{
    [Collection(nameof(RedisTestCollection))]
    public class SyncTests : RedisTestBase
    {
        public SyncTests(RedisTestFixture fixture)
            : base(fixture)
        {
        }

        [DebugOnlyFact]
        public void Resource_is_cached_indefinitely()
        {
            var cacheProvider = RedisCaches.NewRedisCacheProvider()
                .WithRedisConnection(this.fixture.Connection)
                .Build();

            this.CreateClient(cacheProvider);
            this.fakeHttpClient.SetupGet("/applications/foobarApplication", 200, FakeJson.Application);

            // Prime the cache
            var application = this.client.GetResource<IApplication>("https://api.stormpath.com/v1/applications/foobarApplication");

            // All cache hits
            this.client.GetResource<IApplication>(application.Href);
            this.client.GetResource<IApplication>(application.Href);
            this.client.GetResource<IApplication>(application.Href);

            var db = this.fixture.Connection.GetDatabase();
            var key = TestHelper.CreateKey(application);
            var cached = db.StringGetWithExpiry(key);

            cached.Expiry.ShouldBeNull(); // No TTI
            cached.Value.ToString().ShouldNotBeNullOrEmpty();
            this.fakeHttpClient.Calls.Count.ShouldBe(1);
        }

        [DebugOnlyFact]
        public void Resource_expired_by_TTL()
        {
            var cacheProvider = RedisCaches.NewRedisCacheProvider()
                .WithRedisConnection(this.fixture.Connection)
                .WithDefaultTimeToLive(TimeSpan.FromSeconds(1))
                .Build();

            this.CreateClient(cacheProvider);
            this.fakeHttpClient.SetupGet("/applications/foobarApplication", 200, FakeJson.Application);

            var application = this.client.GetResource<IApplication>("https://api.stormpath.com/v1/applications/foobarApplication");
            this.client.GetResource<IApplication>(application.Href);
            this.client.GetResource<IApplication>(application.Href);
            this.fakeHttpClient.Calls.Count.ShouldBe(1);

            Thread.Sleep(1500);

            this.client.GetResource<IApplication>(application.Href);
            this.client.GetResource<IApplication>(application.Href);
            this.fakeHttpClient.Calls.Count.ShouldBe(2);
        }

        [DebugOnlyFact]
        public void Resource_expired_by_TTI()
        {
            var cacheProvider = RedisCaches.NewRedisCacheProvider()
                .WithRedisConnection(this.fixture.Connection)
                .WithDefaultTimeToIdle(TimeSpan.FromSeconds(1))
                .Build();

            this.CreateClient(cacheProvider);
            this.fakeHttpClient.SetupGet("/applications/foobarApplication", 200, FakeJson.Application);

            var application = this.client.GetResource<IApplication>("https://api.stormpath.com/v1/applications/foobarApplication");
            this.client.GetResource<IApplication>(application.Href);
            this.client.GetResource<IApplication>(application.Href);
            this.fakeHttpClient.Calls.Count.ShouldBe(1);

            Thread.Sleep(1500);

            this.client.GetResource<IApplication>(application.Href);
            this.client.GetResource<IApplication>(application.Href);
            this.fakeHttpClient.Calls.Count.ShouldBe(2);
        }

        [DebugOnlyFact]
        public void Resource_with_custom_configuration_expired_by_TTL()
        {
            // Make the default TTL 10 minutes, but IAccounts expire in 1 second
            var cacheProvider = RedisCaches.NewRedisCacheProvider()
                .WithRedisConnection(this.fixture.Connection)
                .WithDefaultTimeToLive(TimeSpan.FromMinutes(10))
                .WithCache(Caches.ForResource<IAccount>()
                    .WithTimeToLive(TimeSpan.FromSeconds(1)))
                .Build();

            this.CreateClient(cacheProvider);
            this.fakeHttpClient.SetupGet("/applications/foobarApplication", 200, FakeJson.Application);
            this.fakeHttpClient.SetupGet("/accounts/foobarAccount", 200, FakeJson.Account);

            var application = this.client.GetResource<IApplication>("https://api.stormpath.com/v1/applications/foobarApplication");
            var account = this.client.GetResource<IAccount>("https://api.stormpath.com/v1/accounts/foobarAccount");
            this.fakeHttpClient.Calls.Count.ShouldBe(2);

            Thread.Sleep(1500);

            this.client.GetResourceAsync<IAccount>(account.Href);
            this.fakeHttpClient.Calls.Count.ShouldBe(3);

            this.client.GetResource<IApplication>(application.Href);
            this.fakeHttpClient.Calls.Count.ShouldBe(3);
        }

        [DebugOnlyFact]
        public void Resource_with_custom_configuration_expired_by_TTI()
        {
            // Make the default TTL 10 minutes, but IAccounts expire in 1 second
            var cacheProvider = RedisCaches.NewRedisCacheProvider()
                .WithRedisConnection(this.fixture.Connection)
                .WithDefaultTimeToIdle(TimeSpan.FromMinutes(10))
                .WithCache(Caches.ForResource<IAccount>()
                    .WithTimeToIdle(TimeSpan.FromSeconds(1)))
                .Build();

            this.CreateClient(cacheProvider);
            this.fakeHttpClient.SetupGet("/applications/foobarApplication", 200, FakeJson.Application);
            this.fakeHttpClient.SetupGet("/accounts/foobarAccount", 200, FakeJson.Account);

            var application = this.client.GetResource<IApplication>("https://api.stormpath.com/v1/applications/foobarApplication");
            var account = this.client.GetResource<IAccount>("https://api.stormpath.com/v1/accounts/foobarAccount");
            this.fakeHttpClient.Calls.Count.ShouldBe(2);

            Thread.Sleep(1500);

            this.client.GetResource<IAccount>(account.Href);
            this.fakeHttpClient.Calls.Count.ShouldBe(3);

            this.client.GetResource<IApplication>(application.Href);
            this.fakeHttpClient.Calls.Count.ShouldBe(3);
        }
    }
}
