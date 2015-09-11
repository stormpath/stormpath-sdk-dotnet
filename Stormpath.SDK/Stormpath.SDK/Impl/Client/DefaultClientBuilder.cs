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
using Stormpath.SDK.Api;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;
using Stormpath.SDK.Serialization;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.Client
{
    internal sealed class DefaultClientBuilder : IClientBuilder
    {
        private readonly IJsonSerializerBuilder serializerBuilder;
        private readonly IRequestExecutorBuilder requestExecutorBuilder;
        private readonly ICacheProviderBuilder cacheProviderBuilder;

        private string baseUrl = DefaultClient.DefaultBaseUrl;
        private int connectionTimeout = DefaultClient.DefaultConnectionTimeout;
        private AuthenticationScheme authenticationScheme;
        private IClientApiKey apiKey;
        private ILogger logger;

        public DefaultClientBuilder()
        {
            this.serializerBuilder = new DefaultJsonSerializerBuilder();
            this.cacheProviderBuilder = new DefaultCacheProviderBuilder();
            this.requestExecutorBuilder = new DefaultRequestExecutorBuilder();
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

        IClientBuilder IClientBuilder.SetJsonSerializer(IJsonSerializer serializer)
        {
            this.serializerBuilder.UseSerializer(serializer);

            return this;
        }

        IClientBuilder IClientBuilder.SetHttpClient(Type httpClientType)
        {
            this.requestExecutorBuilder.SetRequestExecutorType(httpClientType);

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

            return new DefaultClient(
                this.apiKey,
                this.baseUrl,
                this.authenticationScheme,
                this.connectionTimeout,
                this.requestExecutorBuilder,
                this.cacheProviderBuilder,
                this.serializerBuilder,
                this.logger);
        }
    }
}
