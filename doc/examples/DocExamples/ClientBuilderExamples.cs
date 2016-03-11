// <copyright file="CacheProviderExamples.cs" company="Stormpath, Inc.">
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
using Stormpath.Configuration.Abstractions;
using Stormpath.Configuration.Abstractions.Model;
using Stormpath.SDK.Account;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;
using Stormpath.SDK.Http;
using Stormpath.SDK.Serialization;

namespace DocExamples
{
    public class ClientBuilderExamples
    {
        private readonly IClientBuilder clientBuilder = null;

        public void DefaultClientOptions()
        {
            #region DefaultClientOptions
            // Default locations will be searched for API credentials
            // Default (in-memory) cache will be used
            var client = clientBuilder
                .SetHttpClient(HttpClients.Create().RestSharpClient())
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .Build();
            #endregion
        }

        public void InstanceClientOptions()
        {
            #region InstanceClientOptions
            // SetConfiguration takes an instance of StormpathConfiguration. Null values fall back to the defaults.
            // These options can also be specified in a stormpath.json or stormpath.yaml file.
            // See: https://github.com/stormpath/stormpath-sdk-spec/blob/master/specifications/config.md
            var client = clientBuilder
                .SetConfiguration(new StormpathConfiguration(
                    client: new ClientConfiguration(
                        apiKey: new ClientApiKeyConfiguration(file: "my_apiKey.properties"),
                        cacheManager: null,
                        authenticationScheme: ClientAuthenticationScheme.Basic)))
                .SetHttpClient(HttpClients.Create().RestSharpClient())
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .Build();
            #endregion
        }

        public void AnonTypeClientOptions()
        {
            #region AnonTypeClientOptions
            // Passing an anonymous type with keys matching the property names of StormpathConfiguration.
            // Omitted values fall back to the defaults.
            // These options can also be specified in a stormpath.json or stormpath.yaml file.
            // See: https://github.com/stormpath/stormpath-sdk-spec/blob/master/specifications/config.md
            var client = clientBuilder
                .SetConfiguration(new
                {
                    client = new
                    {
                        apiKey = new
                        {
                            file = "my_apiKey.properties"
                        },
                        authenticationScheme = "Basic"
                    }
                })
                .SetHttpClient(HttpClients.Create().RestSharpClient())
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .Build();
            #endregion
        }

        public void SetApiCredentialsDirectly()
        {
            #region SetApiCredentialsDirectly
            // Don't hardcode credentials in production!
            // Use SetApiKeyFilePath, or use environment variables.
            clientBuilder
                .SetApiKeyId("myApiKeyId")
                .SetApiKeySecret("mySuperSecretApiKeySecret");
            #endregion
        }

        public void SetApiCredentialsFile()
        {
            #region SetApiCredentialsFile
            // Specify a path relative to the application base directory:
            clientBuilder.SetApiKeyFilePath("my_apiKey.properties");

            // The method will also expand the token '~' to the current user's home path:
            clientBuilder.SetApiKeyFilePath("~\\.stormpath\\my_apiKey.properties");

            // If SetApiKeyFilePath
            #endregion
        }

        public void DisableCaching()
        {
            #region DisableCaching
            // To disable caching, pass null:
            clientBuilder.SetCacheProvider(null);

            // Or, use DisabledCache():
            clientBuilder.SetCacheProvider(CacheProviders.Create().DisabledCache());
            #endregion
        }

        public void InMemoryCacheWithOptions()
        {
            #region InMemoryCacheWithOptions
            clientBuilder.SetCacheProvider(
                CacheProviders.Create().InMemoryCache()
                    // Default TTI is 60 minutes
                    .WithDefaultTimeToIdle(TimeSpan.FromMinutes(60))

                    // Default TTL is 60 minutes
                    .WithDefaultTimeToLive(TimeSpan.FromMinutes(60))

                    // TTL and TTI can be overridden on a per-resource basis
                    .WithCache(Caches.ForResource<IAccount>()
                        .WithTimeToIdle(TimeSpan.FromMinutes(30))
                        .WithTimeToLive(TimeSpan.FromMinutes(30)))
                    .Build());
            #endregion
        }

        public void UseBasicAuthentication()
        {
            #region UseBasicAuthentication
            clientBuilder.SetAuthenticationScheme(ClientAuthenticationScheme.Basic);
            #endregion
        }

        public void SetBaseUrl()
        {
            #region SetBaseUrl
            // Use a different base URL than the default (https://api.stormpath.com/v1).
            // This isn't necessary unless you are running an Enterprise or Private deployment of Stormpath:
            clientBuilder.SetBaseUrl("https://enterprise.stormpath.io/v1");
            #endregion
        }
    }
}
