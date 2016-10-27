// <copyright file="AsyncTests.cs" company="Stormpath, Inc.">
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
using System.Threading.Tasks;
using FluentAssertions;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Group;
using Stormpath.SDK.Tests.Common;
using Stormpath.SDK.Tests.Common.Fakes;
using Xunit;

namespace Stormpath.SDK.Cache.Redis.Tests
{
    [Collection(nameof(RedisTestCollection))]
    public class AsyncTests : RedisTestBase
    {
        public AsyncTests(RedisTestFixture fixture)
            : base(fixture)
        {
        }

        [DebugOnlyFact]
        public async Task Resource_is_cached_indefinitely()
        {
            var cacheProvider = CacheProviders.Create().RedisCache()
                .WithRedisConnection(this.fixture.Connection)
                .Build();

            this.CreateClient(cacheProvider);
            this.fakeHttpClient.SetupGet("/accounts/foobarAccount", 200, FakeJson.Account);

            // Prime the cache
            var account = await this.client.GetResourceAsync<IAccount>("https://api.stormpath.com/v1/accounts/foobarAccount");

            // All cache hits
            await this.client.GetResourceAsync<IAccount>(account.Href);
            await this.client.GetResourceAsync<IAccount>(account.Href);
            await this.client.GetResourceAsync<IAccount>(account.Href);

            var db = this.fixture.Connection.GetDatabase();
            var key = TestHelper.CreateKey(account);
            var cached = await db.StringGetWithExpiryAsync(key);

            cached.Expiry.Should().Be(null); // No TTI
            cached.Value.ToString().Should().NotBeNullOrEmpty();
            this.fakeHttpClient.Calls.Count.Should().Be(1);
        }

        [DebugOnlyFact]
        public async Task Resource_expired_by_TTL()
        {
            var cacheProvider = CacheProviders.Create().RedisCache()
                .WithRedisConnection(this.fixture.Connection)
                .WithDefaultTimeToLive(TimeSpan.FromSeconds(1))
                .Build();

            this.CreateClient(cacheProvider);
            this.fakeHttpClient.SetupGet("/accounts/foobarAccount", 200, FakeJson.Account);

            var account = await this.client.GetResourceAsync<IAccount>("https://api.stormpath.com/v1/accounts/foobarAccount");
            await this.client.GetResourceAsync<IAccount>(account.Href);
            await this.client.GetResourceAsync<IAccount>(account.Href);
            this.fakeHttpClient.Calls.Count.Should().Be(1);

            await Task.Delay(1500);

            await this.client.GetResourceAsync<IAccount>(account.Href);
            await this.client.GetResourceAsync<IAccount>(account.Href);
            this.fakeHttpClient.Calls.Count.Should().Be(2);
        }

        [DebugOnlyFact]
        public async Task Resource_expired_by_TTI()
        {
            var cacheProvider = CacheProviders.Create().RedisCache()
                .WithRedisConnection(this.fixture.Connection)
                .WithDefaultTimeToIdle(TimeSpan.FromSeconds(1))
                .Build();

            this.CreateClient(cacheProvider);
            this.fakeHttpClient.SetupGet("/accounts/foobarAccount", 200, FakeJson.Account);

            var account = await this.client.GetResourceAsync<IAccount>("https://api.stormpath.com/v1/accounts/foobarAccount");
            await this.client.GetResourceAsync<IAccount>(account.Href);
            await this.client.GetResourceAsync<IAccount>(account.Href);
            this.fakeHttpClient.Calls.Count.Should().Be(1);

            await Task.Delay(1500);

            await this.client.GetResourceAsync<IAccount>(account.Href);
            await this.client.GetResourceAsync<IAccount>(account.Href);
            this.fakeHttpClient.Calls.Count.Should().Be(2);
        }

        [DebugOnlyFact]
        public async Task Resource_with_custom_configuration_expired_by_TTL()
        {
            // Make the default TTL 10 minutes, but IAccounts expire in 1 second
            var cacheProvider = CacheProviders.Create().RedisCache()
                .WithRedisConnection(this.fixture.Connection)
                .WithDefaultTimeToLive(TimeSpan.FromMinutes(10))
                .WithCache(Caches.ForResource<IAccount>()
                    .WithTimeToLive(TimeSpan.FromSeconds(1)))
                .Build();

            this.CreateClient(cacheProvider);
            this.fakeHttpClient.SetupGet("/groups/group1", 200, FakeJson.Group);
            this.fakeHttpClient.SetupGet("/accounts/foobarAccount", 200, FakeJson.Account);

            var account = await this.client.GetResourceAsync<IAccount>("https://api.stormpath.com/v1/accounts/foobarAccount");
            var group = await this.client.GetResourceAsync<IGroup>("https://api.stormpath.com/v1/groups/group1");
            this.fakeHttpClient.Calls.Count.Should().Be(2);

            await Task.Delay(1500);

            await this.client.GetResourceAsync<IAccount>(account.Href);
            this.fakeHttpClient.Calls.Count.Should().Be(3);

            await this.client.GetResourceAsync<IGroup>(group.Href);
            this.fakeHttpClient.Calls.Count.Should().Be(3);
        }

        [DebugOnlyFact]
        public async Task Resource_with_custom_configuration_expired_by_TTI()
        {
            // Make the default TTL 10 minutes, but IAccounts expire in 1 second
            var cacheProvider = CacheProviders.Create().RedisCache()
                .WithRedisConnection(this.fixture.Connection)
                .WithDefaultTimeToIdle(TimeSpan.FromMinutes(10))
                .WithCache(Caches.ForResource<IAccount>()
                    .WithTimeToIdle(TimeSpan.FromSeconds(1)))
                .Build();

            this.CreateClient(cacheProvider);
            this.fakeHttpClient.SetupGet("/groups/group1", 200, FakeJson.Group);
            this.fakeHttpClient.SetupGet("/accounts/foobarAccount", 200, FakeJson.Account);

            var account = await this.client.GetResourceAsync<IAccount>("https://api.stormpath.com/v1/accounts/foobarAccount");
            var group = await this.client.GetResourceAsync<IGroup>("https://api.stormpath.com/v1/groups/group1");
            this.fakeHttpClient.Calls.Count.Should().Be(2);

            await Task.Delay(1500);

            await this.client.GetResourceAsync<IAccount>(account.Href);
            this.fakeHttpClient.Calls.Count.Should().Be(3);

            await this.client.GetResourceAsync<IGroup>(group.Href);
            this.fakeHttpClient.Calls.Count.Should().Be(3);
        }

        [DebugOnlyFact]
        public async Task Date_properties_are_cached_correctly()
        {
            var cacheProvider = CacheProviders.Create().RedisCache()
                .WithRedisConnection(fixture.Connection)
                .Build();

            CreateClient(cacheProvider);
            fakeHttpClient.SetupGet("https://api.stormpath.com/v1/applications/foobarApplication", 200, FakeJson.Application);

            // Prime the cache
            await client.GetApplicationAsync("https://api.stormpath.com/v1/applications/foobarApplication");

            // Retrieve it from the cache
            var app = await client.GetApplicationAsync("https://api.stormpath.com/v1/applications/foobarApplication");
            app.CreatedAt.Year.Should().Be(2015);
            app.CreatedAt.Month.Should().Be(7);
            app.CreatedAt.Day.Should().Be(21);
            app.CreatedAt.Hour.Should().Be(23);
            app.CreatedAt.Minute.Should().Be(50);
            app.CreatedAt.Second.Should().Be(49);
            app.CreatedAt.Millisecond.Should().Be(563);
        }

        [DebugOnlyFact]
        public async Task Status_properties_are_cached_correctly()
        {
            var cacheProvider = CacheProviders.Create().RedisCache()
                .WithRedisConnection(fixture.Connection)
                .Build();

            CreateClient(cacheProvider);
            fakeHttpClient.SetupGet("https://api.stormpath.com/v1/applications/foobarApplication", 200, FakeJson.Application);

            // Prime the cache
            var app = await client.GetApplicationAsync("https://api.stormpath.com/v1/applications/foobarApplication");
            app.Status.Should().Be(ApplicationStatus.Enabled);

            // Retrieve it from the cache
            var app2 = await client.GetApplicationAsync("https://api.stormpath.com/v1/applications/foobarApplication");
            app2.Status.Should().Be(ApplicationStatus.Enabled);
        }

        [DebugOnlyFact]
        public async Task Link_properties_are_cached_correctly()
        {
            var cacheProvider = CacheProviders.Create().RedisCache()
                .WithRedisConnection(fixture.Connection)
                .Build();

            CreateClient(cacheProvider);
            fakeHttpClient.SetupGet("https://api.stormpath.com/v1/applications/foobarApplication", 200, FakeJson.Application);
            fakeHttpClient.SetupGet("https://api.stormpath.com/v1/applications/foobarApplication/accounts?limit=100", 200, FakeJson.AccountList);

            // Prime the cache
            await client.GetApplicationAsync("https://api.stormpath.com/v1/applications/foobarApplication");

            // Retrieve it from the cache
            var app = await client.GetApplicationAsync("https://api.stormpath.com/v1/applications/foobarApplication");
            var accounts = await app.GetAccounts().ToArrayAsync();
            accounts.Length.Should().Be(6);
        }

        [DebugOnlyFact]
        public async Task Collection_resource_access_works()
        {
            var cacheProvider = CacheProviders.Create().RedisCache()
                .WithRedisConnection(fixture.Connection)
                .Build();

            CreateClient(cacheProvider);
            fakeHttpClient.SetupGet("/tenants/current", 200, FakeJson.Tenant);
            fakeHttpClient.SetupGet("https://api.stormpath.com/v1/tenants/foo-bar/applications?limit=100", 200, FakeJson.ApplicationList);

            // Get the collection
            var applications = await client.GetApplications().ToArrayAsync();
            applications.Length.Should().Be(2);

            // Try getting an item from the cache
            var app1 = await client.GetApplicationAsync(applications[0].Href);
            app1.Href.Should().NotBeNullOrEmpty();
        }
    }
}
