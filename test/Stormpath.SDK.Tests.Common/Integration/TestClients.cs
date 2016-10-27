// <copyright file="TestClients.cs" company="Stormpath, Inc.">
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
using Stormpath.Configuration.Abstractions.Immutable;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;
using Stormpath.SDK.Http;
using Stormpath.SDK.Serialization;

namespace Stormpath.SDK.Tests.Common.Integration
{
    public static class TestClients
    {
        public static readonly Lazy<IClient> Basic = new Lazy<IClient>(() =>
        {
            return Clients.Builder()
                .SetHttpClient(HttpClients.Create().SystemNetHttpClient())
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .SetAuthenticationScheme(Configuration.Abstractions.ClientAuthenticationScheme.Basic)
                .SetLogger(StaticLogger.Instance)
                .SetCacheProvider(CacheProviders.Create().DisabledCache())
                .Build();
        });

        public static readonly Lazy<IClient> SAuthc1 = new Lazy<IClient>(() =>
        {
            return Clients.Builder()
                .SetHttpClient(HttpClients.Create().SystemNetHttpClient())
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .SetAuthenticationScheme(Configuration.Abstractions.ClientAuthenticationScheme.SAuthc1)
                .SetLogger(StaticLogger.Instance)
                .SetCacheProvider(CacheProviders.Create().DisabledCache())
                .Build();
        });

        public static readonly Lazy<IClient> SAuthc1Caching = new Lazy<IClient>(() =>
        {
            return Clients.Builder()
                .SetHttpClient(HttpClients.Create().SystemNetHttpClient())
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .SetAuthenticationScheme(Configuration.Abstractions.ClientAuthenticationScheme.SAuthc1)
                .SetLogger(StaticLogger.Instance)
                .SetCacheProvider(CacheProviders.Create().InMemoryCache()
                    .WithDefaultTimeToIdle(TimeSpan.FromMinutes(10))
                    .WithDefaultTimeToLive(TimeSpan.FromMinutes(10))
                    .Build())
                .Build();
        });

        public static readonly Lazy<IClient> SAuthc1RedisCaching = new Lazy<IClient>(() =>
        {
            return Clients.Builder()
                .SetHttpClient(HttpClients.Create().SystemNetHttpClient())
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .SetAuthenticationScheme(Configuration.Abstractions.ClientAuthenticationScheme.SAuthc1)
                .SetLogger(StaticLogger.Instance)
                .SetCacheProvider(CacheProviders.Create().RedisCache()
                    .WithRedisConnection("localhost:6379")
                    .WithDefaultTimeToIdle(TimeSpan.FromMinutes(10))
                    .WithDefaultTimeToLive(TimeSpan.FromMinutes(10))
                    .Build())
                .Build();
        });

        private static Lazy<StormpathConfiguration> lazyConfiguration =
            new Lazy<StormpathConfiguration>(() => Configuration.ConfigurationLoader.Initialize().Load());

        public static StormpathConfiguration CurrentConfiguration => lazyConfiguration.Value;

        /// <summary>
        /// Return a list of clients available for parameter-driven tests.
        /// </summary>
        /// <returns>A list of testing clients.</returns>
        public static IEnumerable<object[]> GetClients()
        {
            // Always return the default client - SAuthc1 with caching disabled
            yield return new object[] { new TestClientProvider(nameof(SAuthc1)) };

            // Allow the STORMPATH_IT_QUICK env var to skip the remaining clients
            bool isFastMode = false;
            bool.TryParse(Environment.GetEnvironmentVariable("STORMPATH_IT_QUICK"), out isFastMode);
            if (isFastMode) yield break;

            yield return new object[] { new TestClientProvider(nameof(Basic)) };
            yield return new object[] { new TestClientProvider(nameof(SAuthc1Caching)) };

            // Optionally support SAutch1 with the Redis cache adapter
            bool enableRedisTests;
            bool.TryParse(Environment.GetEnvironmentVariable("STORMPATH_IT_REDIS"), out enableRedisTests);
            if (enableRedisTests) yield return new object[] { new TestClientProvider(nameof(SAuthc1RedisCaching)) };
        }

        public static IClient GetSAuthc1Client()
            => SAuthc1Caching.Value;
    }
}
