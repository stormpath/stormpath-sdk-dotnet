// <copyright file="DefaultClientBuilder_tests.cs" company="Stormpath, Inc.">
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
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Api;
using Stormpath.SDK.Client;
using Stormpath.SDK.Extensions.Http;
using Stormpath.SDK.Extensions.Serialization;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Impl.Client;
using Stormpath.SDK.Tests.Common.Fakes;
using Stormpath.SDK.Tests.Fakes;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class DefaultClientBuilder_tests
    {
        private IClientBuilder builder;

        public DefaultClientBuilder_tests()
        {
            this.builder = new DefaultClientBuilder(new FakeUserAgentBuilder());

            // Providing these means the tests won't try to do a dynamic assembly lookup
            // which tends to screw up parallel-running tests
            this.builder
                .SetHttpClient(new RestSharpClient("https://api.stormpath.com/v1", 20000, null, null))
                .SetSerializer(new JsonNetSerializer());
        }

        [Fact]
        public void Throws_for_missing_API_key()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var client = this.builder
                .SetApiKey(null)
                .Build();
            });
        }

        [Fact]
        public void Throws_for_invalid_API_key()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                this.builder
                    .SetApiKey(FakeApiKey.Create(valid: false))
                    .Build();
            });
        }

        [Fact]
        public void Looks_for_default_ClientApiKey_if_none_specified()
        {
            var fakeKey = FakeApiKey.Create(valid: true);
            var fakeClientApiKeyBuilder = Substitute.For<IClientApiKeyBuilder>();
            fakeClientApiKeyBuilder.Build().Returns(fakeKey);
            IClientBuilder builder = new DefaultClientBuilder(fakeClientApiKeyBuilder, new FakeUserAgentBuilder());

            var client = builder.Build();
            (client as DefaultClient).ApiKey.ShouldBe(fakeKey);
        }

        [Fact]
        public void AuthenticationScheme_is_optional()
        {
            var client = this.builder
                .SetApiKey(FakeApiKey.Create(valid: true))
                .Build();

            // Defaults to SAuthc1
            (client as DefaultClient).AuthenticationScheme.ShouldBe(AuthenticationScheme.SAuthc1);
        }

        [Fact]
        public void Passing_custom_HttpClient()
        {
            var fakeHttpClient = Substitute.For<Http.IHttpClient>();

            var client = this.builder
                .SetApiKey(FakeApiKey.Create(valid: true))
                .SetHttpClient(fakeHttpClient)
                .Build();

            (client as DefaultClient).HttpClient.ShouldBe(fakeHttpClient);
            (client as DefaultClient).Serializer.ShouldBeOfType<JsonNetSerializer>();
        }

        [Fact]
        public void Passing_custom_JsonSerializer()
        {
            var fakeSerializer = Substitute.For<Serialization.IJsonSerializer>();

            var client = this.builder
                .SetApiKey(FakeApiKey.Create(valid: true))
                .SetSerializer(fakeSerializer)
                .Build();

            (client as DefaultClient).HttpClient.ShouldBeOfType<RestSharpClient>();
            (client as DefaultClient).Serializer.ShouldBe(fakeSerializer);
        }

        [Fact]
        public void BaseUrl_is_optional()
        {
            var client = this.builder
                .SetApiKey(FakeApiKey.Create(valid: true))
                .Build();

            // Default value
            (client as DefaultClient).BaseUrl.ShouldBe("https://api.stormpath.com/v1");
        }

        [Fact]
        public void Default_cache_is_InMemoryCache()
        {
            var client = this.builder
                .SetApiKey(FakeApiKey.Create(valid: true))
                .Build();

            // Default value
            client.GetCacheProvider().ShouldBeOfType<InMemoryCacheProvider>();
            (client.GetCacheProvider() as InMemoryCacheProvider).DefaultTimeToIdle.ShouldBe(TimeSpan.FromHours(1));
            (client.GetCacheProvider() as InMemoryCacheProvider).DefaultTimeToLive.ShouldBe(TimeSpan.FromHours(1));
        }

        [Fact]
        public void Passing_disabled_cache()
        {
            var client = this.builder
                .SetApiKey(FakeApiKey.Create(valid: true))
                .SetCacheProvider(SDK.Cache.Caches.NewDisabledCacheProvider())
                .Build();

            client.GetCacheProvider().ShouldBeOfType<NullCacheProvider>();
        }

        [Fact]
        public void Passing_custom_cache()
        {
            var fakeCache = Substitute.For<SDK.Cache.ICacheProvider>();

            var client = this.builder
                .SetApiKey(FakeApiKey.Create(valid: true))
                .SetCacheProvider(fakeCache)
                .Build();

            client.GetCacheProvider().ShouldBe(fakeCache);
        }

        [Fact]
        public void ConnectionTimeout_is_optional()
        {
            var client = this.builder
                .SetApiKey(FakeApiKey.Create(valid: true))
                .Build();

            // Default value
            (client as DefaultClient).ConnectionTimeout.ShouldBe(20 * 1000);
        }

        [Fact]
        public void Throws_when_BaseUrl_is_empty()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var client = this.builder
                .SetAuthenticationScheme(AuthenticationScheme.SAuthc1)
                .SetBaseUrl(string.Empty)
                .SetConnectionTimeout(10)
                .Build();
            });
        }

        [Fact]
        public void Throws_when_ConnectionTimeout_is_negative()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var client = this.builder
                    .SetAuthenticationScheme(AuthenticationScheme.SAuthc1)
                    .SetBaseUrl("foobar")
                    .SetConnectionTimeout(-1)
                    .Build();
            });
        }
    }
}
