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
using System.IO;
using System.Net;
using Stormpath.Configuration;
using Stormpath.Configuration.Abstractions;
using Stormpath.Configuration.Abstractions.Model;
using Stormpath.SDK.Api;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Logging;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Serialization;
using Stormpath.SDK.Shared.Extensions;

namespace Stormpath.SDK.Impl.Client
{
    internal sealed class DefaultClientBuilder : IClientBuilder
    {
        private static readonly TimeSpan DefaultIdentityMapSlidingExpiration = TimeSpan.FromMinutes(10);

        private readonly IUserAgentBuilder userAgentBuilder;

        private ISerializerBuilder serializerBuilder;
        private IHttpClientBuilder httpClientBuilder;
        private IJsonSerializer overrideSerializer;
        private IHttpClient overrideHttpClient;
        private ICacheProvider cacheProvider;
        private ILogger logger;

        // Set if the user supplies a configuration to use
        private StormpathConfiguration useConfiguration = null;
        private object useConfigurationAnonymous = null;

        // These are for backwards compatibility and will be removed at 1.0
        [Obsolete]
        private string useApiKeyId = Default.Configuration.Client.ApiKey.Id;
        private string useApiKeySecret = Default.Configuration.Client.ApiKey.Secret;
        private string useApiKeyFileName = Default.Configuration.Client.ApiKey.File;
        private ClientAuthenticationScheme useAuthenticationScheme = Default.Configuration.Client.AuthenticationScheme.Value;
        private string useBaseUrl = Default.Configuration.Client.BaseUrl;
        private int useConnectionTimeout = Default.Configuration.Client.ConnectionTimeout.Value;
        private ClientProxyConfiguration useProxy = Default.Configuration.Client.Proxy;

        public DefaultClientBuilder(IUserAgentBuilder userAgentBuilder)
        {
            this.userAgentBuilder = userAgentBuilder;
        }

        IClientBuilder IClientBuilder.SetApiKeyId(string id)
        {
            this.useApiKeyId = id;
            return this;
        }

        IClientBuilder IClientBuilder.SetApiKeySecret(string secret)
        {
            this.useApiKeySecret = secret;
            return this;
        }

        IClientBuilder IClientBuilder.SetApiKeyFilePath(string path)
        {
            this.useApiKeyFileName = path;
            return this;
        }

        IClientBuilder IClientBuilder.SetConfiguration(StormpathConfiguration configuration)
        {
            this.useConfiguration = configuration;
            return this;
        }

        IClientBuilder IClientBuilder.SetConfiguration(object configuration)
        {
            this.useConfigurationAnonymous = configuration;
            return this;
        }

        IClientBuilder IClientBuilder.SetApiKey(IClientApiKey apiKey)
        {
            if (!apiKey.IsValid())
            {
                throw new ArgumentException("API Key is not valid.");
            }

            if (apiKey == null)
            {
                return this;
            }

            var asShim = apiKey as ShimClientApiKey;
            if (asShim != null)
            {
                HandleApiKeyCompatibility(asShim);
                return this;
            }

            this.useApiKeyId = apiKey.GetId();
            this.useApiKeySecret = apiKey.GetSecret();

            return this;
        }

        [Obsolete("Remove for 1.0")]
        private void HandleApiKeyCompatibility(ShimClientApiKey shim)
        {
            var contents = string.Empty;

            bool isPropertyNameOverridden =
                !string.IsNullOrEmpty(shim.AdditionalSettings.IdPropertyName)
                || !string.IsNullOrEmpty(shim.AdditionalSettings.SecretPropertyName);

            if (shim.AdditionalSettings.InputStream != null)
            {
                using (var reader = new StreamReader(shim.AdditionalSettings.InputStream))
                {
                    contents = reader.ReadToEnd();
                }
            }
            else if (!string.IsNullOrEmpty(shim.Configuration.File) && isPropertyNameOverridden)
            {
                contents = File.ReadAllText(shim.Configuration.File);
            }

            if (string.IsNullOrEmpty(contents))
            {
                this.useApiKeyId = shim.Configuration.Id;
                this.useApiKeySecret = shim.Configuration.Secret;
                this.useApiKeyFileName = shim.Configuration.File;
                return; // done
            }

            var idPropertyName = shim.AdditionalSettings.IdPropertyName?.Nullable() ?? "id";
            var secretPropertyName = shim.AdditionalSettings.SecretPropertyName?.Nullable() ?? "secret";

            var properties = new Properties(contents);

            this.useApiKeyId = properties.GetProperty(idPropertyName);
            this.useApiKeySecret = properties.GetProperty(secretPropertyName);
            this.useApiKeyFileName = string.Empty;
        }

        IClientBuilder IClientBuilder.SetAuthenticationScheme(AuthenticationScheme scheme)
        {
            if (scheme == AuthenticationScheme.Basic)
            {
                this.useAuthenticationScheme = ClientAuthenticationScheme.Basic;
            }
            else if (scheme == AuthenticationScheme.SAuthc1)
            {
                this.useAuthenticationScheme = ClientAuthenticationScheme.SAuthc1;
            }
            else
            {
                throw new Exception($"Authentication scheme {scheme} is not supported.");
            }

            return this;
        }

        IClientBuilder IClientBuilder.SetAuthenticationScheme(ClientAuthenticationScheme scheme)
        {
            this.useAuthenticationScheme = scheme;
            return this;
        }

        IClientBuilder IClientBuilder.SetBaseUrl(string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentNullException("Base URL cannot be empty.");
            }

            this.useBaseUrl = baseUrl;

            return this;
        }

        IClientBuilder IClientBuilder.SetConnectionTimeout(int timeout)
        {
            if (timeout < 0)
            {
                throw new ArgumentOutOfRangeException("Timeout cannot be negative.");
            }

            this.useConnectionTimeout = timeout;

            return this;
        }

        IClientBuilder IClientBuilder.SetProxy(IWebProxy proxy)
        {
            var exampleDestination = new Uri(Default.Configuration.Client.BaseUrl);

            var proxyUri = proxy.GetProxy(exampleDestination);

            string host = proxyUri.Host;
            int port = proxyUri.Port;
            string username = null;
            string password = null;

            var proxyCredentials = proxy.Credentials.GetCredential(exampleDestination, "Basic");
            if (proxyCredentials != null)
            {
                username = proxyCredentials.UserName;
                username = proxyCredentials.Password;
            }

            this.useProxy = new ClientProxyConfiguration(port, host, username, password);

            return this;
        }

        IClientBuilder IClientBuilder.SetProxy(ClientProxyConfiguration proxyConfiguration)
        {
            this.useProxy = proxyConfiguration ?? Default.Configuration.Client.Proxy;
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
            this.cacheProvider = cacheProvider == null
                ? this.cacheProvider = CacheProviders.Create().DisabledCache()
                : cacheProvider;

            return this;
        }

        [Obsolete]
        private StormpathConfiguration CreateSuppliedConfiguration()
        {
            return new StormpathConfiguration(
                new ClientConfiguration(
                    apiKey: new ClientApiKeyConfiguration(this.useApiKeyFileName, this.useApiKeyId, this.useApiKeySecret),
                    baseUrl: this.useBaseUrl,
                    connectionTimeout: this.useConnectionTimeout,
                    authenticationScheme: this.useAuthenticationScheme,
                    proxy: this.useProxy));
        }

        IClient IClientBuilder.Build()
        {
            var logger = this.logger ?? new NullLogger();

            // Construct and validate the Stormpath configuration
            // ConfigurationLoader will throw if an API Key and Secret cannot be found
            var suppliedConfiguration = this.useConfiguration
                ?? this.useConfigurationAnonymous
                ?? CreateSuppliedConfiguration();

            var finalConfiguration = ConfigurationLoader.Load(suppliedConfiguration, logger: logger);

            ThrowForInvalidConfiguration(finalConfiguration);

            IJsonSerializer serializer = null;
            if (this.overrideSerializer != null)
            {
                serializer = this.overrideSerializer;
            }
            else
            {
                if (this.serializerBuilder == null)
                {
                    this.logger.Info("No serializer plugin specified, using default.");
                    this.serializerBuilder = Serializers.Create().AutoDetect();
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
                    this.logger.Info("No HTTP client plugin specified, using default.");
                    this.httpClientBuilder = HttpClients.Create().AutoDetect();
                }

                this.httpClientBuilder
                    .SetBaseUrl(finalConfiguration.Client.BaseUrl)
                    .SetConnectionTimeout(finalConfiguration.Client.ConnectionTimeout.Value)
                    .SetProxy(finalConfiguration.Client.Proxy)
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
                finalConfiguration,
                httpClient,
                serializer,
                this.cacheProvider,
                this.userAgentBuilder,
                logger,
                DefaultIdentityMapSlidingExpiration);
        }

        private void ThrowForInvalidConfiguration(StormpathConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.Client.BaseUrl))
            {
                throw new ArgumentNullException("Base URL cannot be empty.");
            }

            if (configuration.Client.ConnectionTimeout == null || configuration.Client.ConnectionTimeout.Value < 0)
            {
                throw new ArgumentException("Timeout cannot be negative.");
            }
        }
    }
}
