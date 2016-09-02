// <copyright file="DefaultClientBuilder_tests.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Api;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;
using Stormpath.SDK.Extensions.Http.RestSharp;
using Stormpath.SDK.Extensions.Serialization;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Impl.Client;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Serialization;
using Stormpath.SDK.Tests.Common.Fakes;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.Configuration.Abstractions;
using Xunit;

namespace Stormpath.SDK.Tests
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
                .SetHttpClient(HttpClients.Create().RestSharpClient())
                .SetSerializer(Serializers.Create().JsonNetSerializer());
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
        public void Passes_authentication_scheme_to_RequestExecutor()
        {
            var client = this.builder
                .SetApiKey(FakeApiKey.Create(valid: true))
                .SetAuthenticationScheme(AuthenticationScheme.Basic)
                .Build();

            // Defaults to SAuthc1
            ((client as DefaultClient).DataStore.RequestExecutor as DefaultRequestExecutor).AuthenticationScheme.ShouldBe(AuthenticationScheme.Basic);
        }

        [Fact]
        public void Passes_base_url_to_DataStore()
        {
            var client = this.builder
                .SetApiKey(FakeApiKey.Create(valid: true))
                .SetBaseUrl("http://foo.bar")
                .Build();

            // Default value
            (client as DefaultClient).DataStore.BaseUrl.ShouldBe("http://foo.bar");
        }

        [Fact]
        public void Passes_api_credentials_to_DataStore()
        {
            var client = this.builder
                .SetApiKey(FakeApiKey.Create(valid: true))
                .Build();

            (client as DefaultClient).DataStore.ApiKey.GetId().ShouldBe("FooId");
            (client as DefaultClient).DataStore.ApiKey.GetSecret().ShouldBe("FooSecret");
        }

        [Fact]
        public void Supports_legacy_api_key()
        {
            var legacyApiKey = ClientApiKeys.Builder()
                .SetId("fooBar")
                .SetSecret("secret123!")
                .Build();

            var client = this.builder
                .SetApiKey(legacyApiKey)
                .Build();

            (client as DefaultClient).DataStore.ApiKey.GetId().ShouldBe("fooBar");
            (client as DefaultClient).DataStore.ApiKey.GetSecret().ShouldBe("secret123!");
        }

        [Fact]
        public void Supports_new_api_key()
        {
            var client = this.builder
                .SetApiKeyId("barFoo")
                .SetApiKeySecret("123secret!")
                .Build();

            (client as DefaultClient).DataStore.ApiKey.GetId().ShouldBe("barFoo");
            (client as DefaultClient).DataStore.ApiKey.GetSecret().ShouldBe("123secret!");
        }

        [Fact]
        public void Supports_legacy_stream_handling_and_name_overrides()
        {
            var propertiesFile = @"
myId = fooBar
mySecret = secret123!";

            using (var stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(propertiesFile)))
            {
                var legacyApiKey = ClientApiKeys.Builder()
                    .SetIdPropertyName("myId")
                    .SetSecretPropertyName("mySecret")
                    .SetInputStream(stream)
                    .Build();

                var client = this.builder
                    .SetApiKey(legacyApiKey)
                    .Build();

                (client as DefaultClient).DataStore.ApiKey.GetId().ShouldBe("fooBar");
                (client as DefaultClient).DataStore.ApiKey.GetSecret().ShouldBe("secret123!");
            }
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
                .SetCacheProvider(SDK.Cache.CacheProviders.Create().DisabledCache())
                .Build();

            client.GetCacheProvider().ShouldBeOfType<NullCacheProvider>();
        }

        [Fact]
        public void Disabling_cacheManager_with_configuration()
        {
            var client = this.builder
                .SetApiKeyId("fake")
                .SetApiKeySecret("fake")
                .SetConfiguration(new
                {
                    client = new
                    {
                        cacheManager = new
                        {
                            enabled = false
                        }
                    }
                })
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
        public void Configuring_cache_with_builder()
        {
            var client = this.builder
                .SetApiKeyId("fake")
                .SetApiKeySecret("fake")
                .SetCacheProvider(CacheProviders.Create()
                    .InMemoryCache()
                    .WithDefaultTimeToIdle(TimeSpan.FromSeconds(600))
                    .WithDefaultTimeToLive(TimeSpan.FromSeconds(900))
                    .WithCache(Caches
                        .ForResource<Account.IAccount>()
                        .WithTimeToIdle(TimeSpan.FromSeconds(5000))
                        .WithTimeToLive(TimeSpan.FromSeconds(6000)))
                    .Build())
                .Build();

            client.GetCacheProvider().ShouldBeOfType<InMemoryCacheProvider>();

            var provider = client.GetCacheProvider() as InMemoryCacheProvider;
            provider.DefaultTimeToIdle.ShouldBe(TimeSpan.FromSeconds(600));
            provider.DefaultTimeToLive.ShouldBe(TimeSpan.FromSeconds(900));

            provider.CacheConfigs.Single().Key.ShouldBe("IAccount");
            provider.CacheConfigs.Single().Value.TimeToIdle.ShouldBe(TimeSpan.FromSeconds(5000));
            provider.CacheConfigs.Single().Value.TimeToLive.ShouldBe(TimeSpan.FromSeconds(6000));
        }

        [Fact]
        public void Configuring_cache_with_configuration()
        {
            var client = this.builder
                .SetApiKeyId("fake")
                .SetApiKeySecret("fake")
                .SetConfiguration(new StormpathConfiguration
                {
                    Client = new ClientConfiguration
                    {
                        CacheManager = new ClientCacheManagerConfiguration
                        {
                            DefaultTti = 600,
                            DefaultTtl = 900,
                            Caches = new Dictionary<string, ClientCacheConfiguration>()
                            {
                                ["account"] = new ClientCacheConfiguration {Tti = 5000, Ttl = 6000}
                            }
                        }
                    }
                })
                .Build();

            client.GetCacheProvider().ShouldBeOfType<InMemoryCacheProvider>();

            var provider = client.GetCacheProvider() as InMemoryCacheProvider;
            provider.DefaultTimeToIdle.ShouldBe(TimeSpan.FromSeconds(600));
            provider.DefaultTimeToLive.ShouldBe(TimeSpan.FromSeconds(900));

            provider.CacheConfigs.Single().Key.ShouldBe("IAccount");
            provider.CacheConfigs.Single().Value.TimeToIdle.ShouldBe(TimeSpan.FromSeconds(5000));
            provider.CacheConfigs.Single().Value.TimeToLive.ShouldBe(TimeSpan.FromSeconds(6000));
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

        [Fact]
        public void Configuration_is_available()
        {
            var client = this.builder
                .SetApiKeyId("barFoo")
                .SetApiKeySecret("123secret!")
                .Build();

            client.Configuration.Client.ApiKey.Id.ShouldBe("barFoo");
            client.Configuration.Client.ApiKey.Secret.ShouldBe("123secret!");
        }
    }
}
