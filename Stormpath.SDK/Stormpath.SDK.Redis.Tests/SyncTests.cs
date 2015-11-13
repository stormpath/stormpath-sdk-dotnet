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
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Application;
using Stormpath.SDK.Tests.Common;
using Stormpath.SDK.Tests.Common.Fakes;
using Stormpath.SDK.Sync;
using Xunit;
using System.Threading;

namespace Stormpath.SDK.Extensions.Cache.Redis.Tests
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
            this.CreateClient(ttl: null, tti: null);
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
            this.fakeHttpClient.CallCount.ShouldBe(1);
        }

        [DebugOnlyFact]
        public void Resource_expired_by_TTL()
        {
            this.CreateClient(ttl: TimeSpan.FromSeconds(1), tti: null);
            this.fakeHttpClient.SetupGet("/applications/foobarApplication", 200, FakeJson.Application);

            var application = this.client.GetResource<IApplication>("https://api.stormpath.com/v1/applications/foobarApplication");
            this.client.GetResource<IApplication>(application.Href);
            this.client.GetResource<IApplication>(application.Href);
            this.fakeHttpClient.CallCount.ShouldBe(1);

            Thread.Sleep(1000);

            this.client.GetResource<IApplication>(application.Href);
            this.client.GetResource<IApplication>(application.Href);
            this.fakeHttpClient.CallCount.ShouldBe(2);
        }

        [DebugOnlyFact]
        public void Resource_expired_by_TTI()
        {
            this.CreateClient(ttl: null, tti: TimeSpan.FromSeconds(1));
            this.fakeHttpClient.SetupGet("/applications/foobarApplication", 200, FakeJson.Application);

            var application = this.client.GetResource<IApplication>("https://api.stormpath.com/v1/applications/foobarApplication");
            this.client.GetResource<IApplication>(application.Href);
            this.client.GetResource<IApplication>(application.Href);
            this.fakeHttpClient.CallCount.ShouldBe(1);

            Thread.Sleep(1000);

            this.client.GetResource<IApplication>(application.Href);
            this.client.GetResource<IApplication>(application.Href);
            this.fakeHttpClient.CallCount.ShouldBe(2);
        }
    }
}
