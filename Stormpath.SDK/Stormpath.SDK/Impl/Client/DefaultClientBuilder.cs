// <copyright file="DefaultClientBuilder.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

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

        private readonly IJsonSerializerBuilder serializerBuilder;
        private readonly IHttpClientBuilder httpClientBuilder;
        private readonly ICacheProviderBuilder cacheProviderBuilder;

        private string baseUrl = DefaultBaseUrl;
        private int connectionTimeout = DefaultConnectionTimeout;
        private IWebProxy proxy;
        private AuthenticationScheme authenticationScheme = DefaultAuthenticationScheme;
        private IClientApiKey apiKey;
        private ILogger logger;

        public DefaultClientBuilder()
        {
            this.serializerBuilder = new DefaultJsonSerializerBuilder();
            this.cacheProviderBuilder = new DefaultCacheProviderBuilder();
            this.httpClientBuilder = new DefaultHttpClientBuilder();
        }

        IClientBuilder IClientBuilder.SetApiKey(IClientApiKey apiKey)
        {
            if (apiKey == null)
                throw new ArgumentNullException("API Key cannot be null.");

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
            this.serializerBuilder.UseSerializer(serializer);

            return this;
        }

        IClientBuilder IClientBuilder.UseHttpClient(IHttpClient httpClient)
        {
            this.httpClientBuilder.UseHttpClient(httpClient);

            return this;
        }

        IClientBuilder IClientBuilder.SetLogger(ILogger logger)
        {
            this.logger = logger;

            return this;
        }

        internal IClientBuilder SetCache(bool cacheEnabled)
        {
            this.cacheProviderBuilder.UseCache(cacheEnabled);

            return this;
        }

        internal IClientBuilder SetCache(ICacheProvider cacheProvider)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            this.cacheProviderBuilder.UseCache(true);
            this.cacheProviderBuilder.UseProvider(cacheProvider);

            return this;
        }

        IClient IClientBuilder.Build()
        {
            if (this.apiKey == null || !this.apiKey.IsValid())
                throw new ArgumentException("API Key is not valid or has not been set. Use ClientApiKeys.Builder() to construct one.");

            if (this.logger == null)
                this.logger = new NullLogger();

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
                this.cacheProviderBuilder.Build(),
                this.logger);
        }
    }
}
