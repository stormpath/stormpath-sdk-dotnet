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
using Shouldly;
using Stormpath.SDK.Api;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;
using Stormpath.SDK.Http;
using Stormpath.SDK.Serialization;

namespace Stormpath.SDK.Tests.Common.Integration
{
    public static class TestClients
    {
        private static readonly Lazy<string> ApiBaseUrl = new Lazy<string>(() =>
        {
            var fromEnvironment = Environment.GetEnvironmentVariable("STORMPATH_IT_API_URL");

            return string.IsNullOrEmpty(fromEnvironment)
                ? "https://api.stormpath.com/v1"
                : fromEnvironment;
        });

        public static readonly Lazy<IClient> Basic = new Lazy<IClient>(() =>
        {
            return Clients.Builder()
                .SetApiKey(GetApiKey())
                .SetHttpClient(HttpClients.Create().RestSharpClient()
                    .SetBaseUrl(ApiBaseUrl.Value)
                    .Build())
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .SetAuthenticationScheme(AuthenticationScheme.Basic)
                .SetBaseUrl(ApiBaseUrl.Value)
                .SetLogger(StaticLogger.Instance)
                .SetCacheProvider(CacheProviders.Create().DisabledCache())
                .Build();
        });

        public static readonly Lazy<IClient> SAuthc1 = new Lazy<IClient>(() =>
        {
            return Clients.Builder()
                .SetApiKey(GetApiKey())
                .SetHttpClient(HttpClients.Create().RestSharpClient()
                    .SetBaseUrl(ApiBaseUrl.Value)
                    .Build())
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .SetAuthenticationScheme(AuthenticationScheme.SAuthc1)
                .SetBaseUrl(ApiBaseUrl.Value)
                .SetLogger(StaticLogger.Instance)
                .SetCacheProvider(CacheProviders.Create().DisabledCache())
                .Build();
        });

        public static readonly Lazy<IClient> SAuthc1Caching = new Lazy<IClient>(() =>
        {
            return Clients.Builder()
                .SetApiKey(GetApiKey())
                .SetHttpClient(HttpClients.Create().RestSharpClient()
                    .SetBaseUrl(ApiBaseUrl.Value)
                    .Build())
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .SetAuthenticationScheme(AuthenticationScheme.SAuthc1)
                .SetBaseUrl(ApiBaseUrl.Value)
                .SetLogger(StaticLogger.Instance)
                .SetCacheProvider(CacheProviders.Create().InMemoryCache()
                    .WithDefaultTimeToIdle(TimeSpan.FromMinutes(10))
                    .WithDefaultTimeToLive(TimeSpan.FromMinutes(10))
                    .Build())
                .Build();
        });

        /// <summary>
        /// Return a list of clients available for parameter-driven tests.
        /// </summary>
        /// <returns>A list of testing clients.</returns>
        public static IEnumerable<object[]> GetClients()
        {
            yield return new object[] { new TestClientProvider(nameof(Basic)) };
            yield return new object[] { new TestClientProvider(nameof(SAuthc1)) };
            yield return new object[] { new TestClientProvider(nameof(SAuthc1Caching)) };
        }

        public static IClient GetSAuthc1Client()
            => SAuthc1Caching.Value;

        public static IClientApiKey GetApiKey()
        {
            // Expect that API keys are in environment variables. (works with travis-ci)
            var apiKey = ClientApiKeys.Builder().Build();
            apiKey.IsValid().ShouldBe(true, "These integration tests look for a valid API Key and Secret in your local environment variables.");
            return apiKey;
        }
    }
}
