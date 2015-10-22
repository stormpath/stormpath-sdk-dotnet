// <copyright file="DefaultClientBuilder.cs" company="Stormpath, Inc.">
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
using System.Net;
using Stormpath.SDK.Api;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;
using Stormpath.SDK.Http;
using Stormpath.SDK.Serialization;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.Client
{
    internal sealed class DefaultClientBuilder : IClientBuilder
    {
        public static readonly int DefaultConnectionTimeout = 20 * 1000;
        public static readonly string DefaultBaseUrl = "https://api.stormpath.com/v1";
        public static readonly AuthenticationScheme DefaultAuthenticationScheme = AuthenticationScheme.SAuthc1;
        private static readonly TimeSpan DefaultIdentityMapSlidingExpiration = TimeSpan.FromMinutes(10);

        private readonly IClientApiKeyBuilder clientApiKeyBuilder;
        private readonly IJsonSerializerBuilder serializerBuilder;
        private readonly IHttpClientBuilder httpClientBuilder;

        private string baseUrl = DefaultBaseUrl;
        private int connectionTimeout = DefaultConnectionTimeout;
        private IWebProxy proxy;
        private ICacheProvider cacheProvider;
        private AuthenticationScheme authenticationScheme = DefaultAuthenticationScheme;
        private IClientApiKey apiKey;
        private ILogger logger;
        private TimeSpan? identityMapExpiration;

        public DefaultClientBuilder()
        {
            this.serializerBuilder = new DefaultJsonSerializerBuilder();
            this.httpClientBuilder = new DefaultHttpClientBuilder();
            this.clientApiKeyBuilder = ClientApiKeys.Builder();
        }

        internal DefaultClientBuilder(IClientApiKeyBuilder clientApiKeyBuilder)
            : this()
        {
            this.clientApiKeyBuilder = clientApiKeyBuilder;
        }

        IClientBuilder IClientBuilder.SetApiKey(IClientApiKey apiKey)
        {
            if (apiKey == null)
                throw new ArgumentNullException("API Key cannot be null.");
            if (!apiKey.IsValid())
                throw new ArgumentException("API Key is not valid.");

            this.apiKey = apiKey;
            return this;
        }

        IClientBuilder IClientBuilder.SetAuthenticationScheme(AuthenticationScheme scheme)
        {
            this.authenticationScheme = scheme;
            return this;
        }

        IClientBuilder IClientBuilder.SetBaseUrl(string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException("Base URL cannot be empty.");

            this.baseUrl = baseUrl;

            return this;
        }

        IClientBuilder IClientBuilder.SetConnectionTimeout(int timeout)
        {
            if (timeout < 0)
                throw new ArgumentOutOfRangeException("Timeout cannot be negative.");

            this.connectionTimeout = timeout;

            return this;
        }

        IClientBuilder IClientBuilder.SetProxy(System.Net.IWebProxy proxy)
        {
            this.proxy = proxy;

            return this;
        }

        IClientBuilder IClientBuilder.UseJsonSerializer(IJsonSerializer serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            this.serializerBuilder.UseSerializer(serializer);

            return this;
        }

        IClientBuilder IClientBuilder.UseHttpClient(IHttpClient httpClient)
        {
            if (httpClient == null)
                throw new ArgumentNullException(nameof(httpClient));

            this.httpClientBuilder.UseHttpClient(httpClient);

            return this;
        }

        IClientBuilder IClientBuilder.SetLogger(ILogger logger)
        {
            this.logger = logger;

            return this;
        }

        IClientBuilder IClientBuilder.SetIdentityMapExpiration(TimeSpan expiration)
        {
            if (expiration.TotalSeconds < 10 ||
                expiration.TotalHours > 24)
                throw new ArgumentOutOfRangeException($"{nameof(expiration)} must be between 10 seconds and 24 hours.");

            this.identityMapExpiration = expiration;

            return this;
        }

        IClientBuilder IClientBuilder.SetCacheProvider(ICacheProvider cacheProvider)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            if (this.cacheProvider != null)
                throw new ApplicationException("Cache provider already set.");

            this.cacheProvider = cacheProvider;

            return this;
        }

        IClient IClientBuilder.Build()
        {
            if (this.apiKey == null)
            {
                if (this.clientApiKeyBuilder != null)
                    this.apiKey = this.clientApiKeyBuilder.Build();
                else
                    throw new ApplicationException("No valid API Key and Secret could be found.");
            }

            if (this.logger == null)
                this.logger = new NullLogger();

            if (this.cacheProvider == null)
                this.cacheProvider = Caches.NewDisabledCacheProvider();

            this.httpClientBuilder
                .SetBaseUrl(this.baseUrl)
                .SetConnectionTimeout(this.connectionTimeout)
                .SetProxy(this.proxy)
                .SetLogger(this.logger);

            return new DefaultClient(
                this.apiKey,
                this.baseUrl,
                this.authenticationScheme,
                this.connectionTimeout,
                this.proxy,
                this.httpClientBuilder.Build(),
                this.serializerBuilder.Build(),
                this.cacheProvider,
                this.logger,
                this.identityMapExpiration ?? DefaultIdentityMapSlidingExpiration);
        }
    }
}
