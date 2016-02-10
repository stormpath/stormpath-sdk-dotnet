// <copyright file="DefaultClientBuilder.cs" company="Stormpath, Inc.">
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
using System.Net;
using Stormpath.SDK.Api;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Logging;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Serialization;

namespace Stormpath.SDK.Impl.Client
{
    internal sealed class DefaultClientBuilder : IClientBuilder
    {
        public static readonly int DefaultConnectionTimeout = 20 * 1000;
        public static readonly string DefaultBaseUrl = "https://api.stormpath.com/v1";
        public static readonly AuthenticationScheme DefaultAuthenticationScheme = AuthenticationScheme.SAuthc1;
        private static readonly TimeSpan DefaultIdentityMapSlidingExpiration = TimeSpan.FromMinutes(10);

        private readonly IClientApiKeyBuilder clientApiKeyBuilder;
        private readonly IUserAgentBuilder userAgentBuilder;

        private ISerializerBuilder serializerBuilder;
        private IHttpClientBuilder httpClientBuilder;
        private IJsonSerializer overrideSerializer;
        private IHttpClient overrideHttpClient;

        private string baseUrl = DefaultBaseUrl;
        private int connectionTimeout = DefaultConnectionTimeout;
        private IWebProxy proxy;
        private ICacheProvider cacheProvider;
        private AuthenticationScheme authenticationScheme = DefaultAuthenticationScheme;
        private IClientApiKey apiKey;
        private ILogger logger;

        public DefaultClientBuilder(IUserAgentBuilder userAgentBuilder)
        {
            this.userAgentBuilder = userAgentBuilder;
            this.clientApiKeyBuilder = ClientApiKeys.Builder();
        }

        internal DefaultClientBuilder(IClientApiKeyBuilder clientApiKeyBuilder, IUserAgentBuilder userAgentBuilder)
            : this(userAgentBuilder)
        {
            this.clientApiKeyBuilder = clientApiKeyBuilder;
        }

        IClientBuilder IClientBuilder.SetApiKey(IClientApiKey apiKey)
        {
            if (apiKey == null)
            {
                throw new ArgumentNullException("API Key cannot be null.");
            }

            if (!apiKey.IsValid())
            {
                throw new ArgumentException("API Key is not valid.");
            }

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
            {
                throw new ArgumentNullException("Base URL cannot be empty.");
            }

            this.baseUrl = baseUrl;

            return this;
        }

        IClientBuilder IClientBuilder.SetConnectionTimeout(int timeout)
        {
            if (timeout < 0)
            {
                throw new ArgumentOutOfRangeException("Timeout cannot be negative.");
            }

            this.connectionTimeout = timeout;

            return this;
        }

        IClientBuilder IClientBuilder.SetProxy(System.Net.IWebProxy proxy)
        {
            this.proxy = proxy;

            return this;
        }

        IClientBuilder ISerializerConsumer<IClientBuilder>.SetSerializer(IJsonSerializer serializer)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            this.overrideSerializer = serializer;

            return this;
        }

        IClientBuilder IClientBuilder.SetSerializer(ISerializerBuilder serializerBuilder)
        {
            if (serializerBuilder == null)
            {
                throw new ArgumentNullException(nameof(serializerBuilder));
            }

            this.serializerBuilder = serializerBuilder;

            return this;
        }

        IClientBuilder IClientBuilder.SetHttpClient(IHttpClient httpClient)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            this.overrideHttpClient = httpClient;

            return this;
        }

        IClientBuilder IClientBuilder.SetHttpClient(IHttpClientBuilder httpClientBuilder)
        {
            if (httpClientBuilder == null)
            {
                throw new ArgumentNullException(nameof(httpClientBuilder));
            }

            this.httpClientBuilder = httpClientBuilder;

            return this;
        }

        IClientBuilder ILoggerConsumer<IClientBuilder>.SetLogger(ILogger logger)
        {
            this.logger = logger;

            return this;
        }

        IClientBuilder IClientBuilder.SetCacheProvider(ICacheProvider cacheProvider)
        {
            if (cacheProvider == null)
            {
                throw new ArgumentNullException(nameof(cacheProvider));
            }

            this.cacheProvider = cacheProvider;

            return this;
        }

        IClient IClientBuilder.Build()
        {
            if (this.apiKey == null)
            {
                if (this.clientApiKeyBuilder != null)
                {
                    this.apiKey = this.clientApiKeyBuilder.Build();
                }
                else
                {
                    throw new Exception("No valid API Key and Secret could be found.");
                }
            }

            var logger = this.logger ?? new NullLogger();

            IJsonSerializer serializer = null;
            if (this.overrideSerializer != null)
            {
                serializer = this.overrideSerializer;
            }
            else
            {
                if (this.serializerBuilder == null)
                {
                    throw new Exception("No serializer plugin specified.");
                }

                serializer = this.serializerBuilder.Build();
            }

            IHttpClient httpClient = null;
            if (this.overrideHttpClient != null)
            {
                httpClient = this.overrideHttpClient;
            }
            else
            {
                if (this.httpClientBuilder == null)
                {
                    throw new Exception("No HTTP plugin specified.");
                }

                this.httpClientBuilder
                    .SetBaseUrl(this.baseUrl)
                    .SetConnectionTimeout(this.connectionTimeout)
                    .SetProxy(this.proxy)
                    .SetLogger(this.logger);

                httpClient = this.httpClientBuilder.Build();
            }

            if (this.cacheProvider == null)
            {
                this.logger.Info("No CacheProvider configured. Defaulting to in-memory CacheProvider with default TTL and TTI of one hour.");

                this.cacheProvider = CacheProviders.Create()
                    .InMemoryCache()
                    .WithDefaultTimeToIdle(TimeSpan.FromHours(1))
                    .WithDefaultTimeToLive(TimeSpan.FromHours(1))
                    .Build();
            }
            else
            {
                var injectableWithSerializer = this.cacheProvider as ISerializerConsumer<ICacheProvider>;
                if (injectableWithSerializer != null)
                {
                    injectableWithSerializer.SetSerializer(serializer);
                }

                var injectableWithLogger = this.cacheProvider as ILoggerConsumer<ICacheProvider>;
                if (injectableWithLogger != null)
                {
                    injectableWithLogger.SetLogger(this.logger);
                }
            }

            return new DefaultClient(
                this.apiKey,
                this.baseUrl,
                this.authenticationScheme,
                this.connectionTimeout,
                this.proxy,
                httpClient,
                serializer,
                this.cacheProvider,
                this.userAgentBuilder,
                logger,
                DefaultIdentityMapSlidingExpiration);
        }
    }
}
