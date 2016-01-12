// <copyright file="IClientBuilder.cs" company="Stormpath, Inc.">
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

using System.Net;
using Stormpath.SDK.Api;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Http;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Serialization;

namespace Stormpath.SDK.Client
{
    /// <summary>
    /// Represents a builder that can construct <see cref="IClient"/> instances.
    /// </summary>
    /// <remarks>
    /// This builder uses a number of optional methods that fall back to sensible defaults.
    /// <para>
    /// If <see cref="SetApiKey(IClientApiKey)"/> is not called, the default locations will be searched
    /// to discover an API Key. See the documentation on <see cref="IClientApiKeyBuilder"/> for details.
    /// </para>
    /// <para>
    /// If <see cref="SetAuthenticationScheme(AuthenticationScheme)"/> is not called, the default scheme
    /// (<see cref="AuthenticationScheme.SAuthc1">SAuthc1</see>) will be used.
    /// </para>
    /// <para>
    /// Unless <see cref="SetHttpClient(IHttpClient)"/> or <see cref="SetHttpClient(IHttpClientBuilder)"/> is called,
    /// the <see cref="IHttpClientFactory.AutoDetect()">default</see> HTTP client will be used.
    /// </para>
    /// <para>
    /// Unless <see cref="ISerializerConsumer{T}.SetSerializer(IJsonSerializer)"/> or <see cref="SetSerializer(ISerializerBuilder)"/> is called,
    /// the <see cref="ISerializerFactory.AutoDetect()">default</see> serializer will be used.
    /// </para>
    /// <para>
    /// Unless <see cref="SetCacheProvider(ICacheProvider)"/> is called, an in-memory cache will be used to improve performance,
    /// with a default TTL (Time to Live) and TTI (Time to Idle) of 1 hour. Caching can be disabled
    /// by passing <see cref="Caches.NewDisabledCacheProvider"/> to <see cref="SetCacheProvider(ICacheProvider)"/>.
    /// </para>
    /// <para>
    /// Note: The default in-memory cache might not be sufficient for an application that runs on multiple servers,
    /// due to cache-coherency issues. If your application runs in multiple instances, consider plugging in a
    /// distributed cache like Redis.
    /// </para>
    /// </remarks>
    /// <example>
    /// Create a client with a specified API Key and default HTTP client, serializer, and caching options:
    /// <code>
    /// IClient client = Clients.Builder()
    ///     .SetApiKey(apiKey)
    ///     .Build();
    /// </code>
    /// </example>
    public interface IClientBuilder : ILoggerConsumer<IClientBuilder>, ISerializerConsumer<IClientBuilder>
    {
        /// <summary>
        /// Sets the <see cref="IClientApiKey"/> to use when making requests.
        /// If this method is not called, the default API Key locations will be checked.
        /// </summary>
        /// <param name="apiKey">The API Key to use.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="apiKey"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="apiKey"/> is not valid.</exception>
        /// <seealso cref="ClientApiKeys.Builder(ILogger)"/>
        IClientBuilder SetApiKey(IClientApiKey apiKey);

        /// <summary>
        /// Sets the authentication scheme to use when making requests.
        /// </summary>
        /// <param name="scheme">The authentication scheme to use.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <seealso cref="AuthenticationScheme.SAuthc1"/>
        IClientBuilder SetAuthenticationScheme(AuthenticationScheme scheme);

        /// <summary>
        /// Sets the HTTP connection timeout to observe when making requests.
        /// To use the default connection timeout (20 seconds), don't call this method.
        /// </summary>
        /// <param name="timeout">The HTTP connection timeout to use, in milliseconds.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
        IClientBuilder SetConnectionTimeout(int timeout);

        /// <summary>
        /// Sets the base API URL to use when making requests.
        /// The default is almost always correct; you shouldn't ever have to call this method.
        /// </summary>
        /// <param name="baseUrl">The base API URL to use.</param>
        /// <returns>This instance for method chaining.</returns>
        IClientBuilder SetBaseUrl(string baseUrl);

        /// <summary>
        /// Sets the proxy server to use when making requests.
        /// To connect without a proxy server, don't call this method, or call with <paramref name="proxy"/> set to <see langword="null"/>.
        /// </summary>
        /// <param name="proxy">The proxy server to use.</param>
        /// <returns>This instance for method chaining.</returns>
        IClientBuilder SetProxy(IWebProxy proxy);

        /// <summary>
        /// Sets the cache provider that should be used to cache Stormpath resources, reducing round-trips
        /// to the Stormpath API server and enhancing application performance.
        /// </summary>
        /// <param name="cacheProvider">The cache provider to use.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="cacheProvider"/> is null.</exception>
        /// <seealso cref="Caches.NewDisabledCacheProvider()"/>
        IClientBuilder SetCacheProvider(ICacheProvider cacheProvider);

        /// <summary>
        /// Sets the HTTP client to use when making requests.
        /// </summary>
        /// <remarks><see cref="SetHttpClient(IHttpClientBuilder)"/> is preferred, unless you have manually instantiated a class supporting <see cref="IHttpClient"/>.</remarks>
        /// <param name="httpClient">The HTTP client to use.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="httpClient"/> is null.</exception>
        /// <seealso cref="HttpClients.Create()"/>
        /// <seealso cref="IHttpClientFactory.AutoDetect()"/>
        IClientBuilder SetHttpClient(IHttpClient httpClient);

        /// <summary>
        /// Sets the HTTP client to use when making requests.
        /// </summary>
        /// <param name="httpClientBuilder">The HTTP client to use.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="httpClientBuilder"/> is null.</exception>
        /// <seealso cref="HttpClients.Create()"/>
        /// <seealso cref="IHttpClientFactory"/>
        IClientBuilder SetHttpClient(IHttpClientBuilder httpClientBuilder);

        /// <summary>
        /// Sets the serializer to use when making requests.
        /// </summary>
        /// <param name="serializerBuilder">The serializer to use.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="serializerBuilder"/> is null.</exception>
        /// <seealso cref="Serializers.Create()"/>
        /// <seealso cref="ISerializerFactory"/>
        IClientBuilder SetSerializer(ISerializerBuilder serializerBuilder);

        /// <summary>
        /// Constructs a new <see cref="IClient"/> instance based on the builder's current configuration state.
        /// </summary>
        /// <returns>A new <see cref="IClient"/> instance.</returns>
        /// <exception cref="System.ApplicationException">No valid API Key ID and Secret could be found.</exception>
        IClient Build();
    }
}
