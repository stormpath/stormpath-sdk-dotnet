// <copyright file="User_agent_tests.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;
using Stormpath.SDK.Impl.Client;
using Stormpath.SDK.Serialization;
using Stormpath.SDK.Tests.Common.Fakes;
using Xunit;
using Xunit.Abstractions;

namespace Stormpath.SDK.Tests
{
    public class User_agent_tests
    {
        private readonly ITestOutputHelper output;

        public User_agent_tests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact(Skip = "Refactor DNX stuff")]
        public void Generates_DNX_user_agent()
        {
            //var userAgent = new DnxUserAgentBuilder(
            //        PlatformServices.Default.Runtime,
            //        PlatformServices.Default.Application,
            //        language: string.Empty)
            //    .GetUserAgent();

            //userAgent.ShouldNotContain("unknown");

            //this.output.WriteLine($"UserAgent: {userAgent}");
        }

        [Fact]
        public async Task Csharp_code_creates_csharp_user_agent()
        {
            var fakeHttpClient = new ResourceReturningHttpClient("http://foo.bar", FakeJson.Account);
            var client = Clients.Builder()
                .SetApiKeyId("foo")
                .SetApiKeySecret("bar")
                .SetBaseUrl("http://foo.bar/")
                .SetHttpClient(fakeHttpClient)
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .SetCacheProvider(CacheProviders.Create().DisabledCache())
                .Build();

            var tenant = await client.GetResourceAsync<IAccount>("http://foo.bar/fooAccount");

            var userAgent = fakeHttpClient.Calls.Single().Headers.UserAgent;
            userAgent.Split(' ').Count(x => x.StartsWith("lang")).ShouldBe(1);
            userAgent.ShouldContain("lang/csharp");
        }

        [Fact]
        public void Decorator_prepends_additional_tokens()
        {
            var mockBuilder = new MockUserAgentBuilder("fake/1.0 qux/123");

            IUserAgentBuilder prependingBuilder = new PrependingUserAgentBuilder(mockBuilder, "foo/1.0", "bar/2.0");

            prependingBuilder.GetUserAgent().ShouldBe("foo/1.0 bar/2.0 fake/1.0 qux/123");
        }

        private class MockUserAgentBuilder : IUserAgentBuilder
        {
            private readonly string value;

            public MockUserAgentBuilder(string value)
            {
                this.value = value;
            }

            public string GetUserAgent() => this.value;
        }
    }
}
