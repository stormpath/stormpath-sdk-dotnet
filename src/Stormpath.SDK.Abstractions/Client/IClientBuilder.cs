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

using System;
using System.Net;
using Stormpath.Configuration.Abstractions;
using Stormpath.SDK.Api;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Http;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Serialization;

namespace Stormpath.SDK.Client
{
    /// <summary>
    /// Represents a builder that can configure and construct <see cref="IClient">Client</see> instances.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Stormpath SDK can be configured in a variety of ways. The methods on <c>IClientBuilder</c> can be used to set configuration,
    /// but configuration options can also be loaded from a variety of sources.
    /// (Further documentation is available at <see href="https://github.com/stormpath/stormpath-sdk-spec/blob/master/specifications/config.md">Stormpath SDK Configuration Spec</see>
    /// <list type="bullet">
    ///     <item>
    ///         <description>Default configuration/fallback</description>
    ///     </item>
    ///     <item>
    ///         <description><c>apiKey.properties</c> file from the <c>~/.stormpath</c> directory</description>
    ///     </item>
    ///     <item>
    ///         <description><c>stormpath.json</c> or <c>stormpath.yaml</c> file from the <c>~/.stormpath</c> directory</description>
    ///     </item>
    ///     <item>
    ///         <description><c>apiKey.properties</c> file from the application directory</description>
    ///     </item>
    ///     <item>
    ///         <description><c>stormpath.json</c> or <c>stormpath.yaml</c> file from the application directory</description>
    ///     </item>
    ///     <item>
    ///         <description>Environment variables prefixed by <c>STORMPATH_</c> (for example, <c>STORMPATH_CLIENT_APIKEY_ID</c>)</description>
    ///     </item>
    ///     <item>
    ///         <description>Configuration provided by the <c>IClientBuilder</c> convenience methods, or <see cref="SetConfiguration(StormpathConfiguration)"/>.</description>
    ///     </item>
    /// </list>
    /// </para>
    /// <para>
    /// This builder provides a number of optional methods that fall back to sensible defaults:
    /// <list type="bullet">
    ///     <item>
    ///         <description>
    ///             If API credentials are not set manually using <see cref="SetApiKeyId(string)"/> or <see cref="SetApiKeyFilePath(string)"/>, the default locations will be searched
    ///             to discover an API Key (see above).
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             If <see cref="SetAuthenticationScheme(Configuration.Abstractions.Model.ClientAuthenticationScheme)"/> is not called, the default scheme
    ///             (<see cref="Configuration.Abstractions.Model.ClientAuthenticationScheme.SAuthc1">SAuthc1</see>) will be used.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             If <see cref="SetCacheProvider(ICacheProvider)"/> is not called, an in-memory cache will be used to improve performance,
    ///             with a default TTL (Time to Live) and TTI (Time to Idle) of 1 hour. Caching can be disabled
    ///             by passing <see langword="null"/> to <see cref="SetCacheProvider(ICacheProvider)"/>.
    ///         </description>  
    ///     </item>
    /// </list>
    /// </para>
    /// <note type="caution">
    /// Note: The default in-memory cache might not be sufficient for an application that runs on multiple servers,
    /// due to cache-coherency issues. If your application runs in multiple instances, consider plugging in a
    /// distributed cache like Redis.
    /// </note>
    /// </remarks>
    /// <example>
    /// <code source="ClientBuilderExamples.cs" region="DefaultClientOptions" lang="C#" title="Create a Client with the default options" />
    /// <code source="ClientBuilderExamples.cs" region="InstanceClientOptions" lang="C#" title="Specify options using StormpathConfiguration" />
    /// <code source="ClientBuilderExamples.cs" region="AnonTypeClientOptions" lang="C#" title="Specify options using an anonymous type" />
    /// </example>
    public interface IClientBuilder : ILoggerConsumer<IClientBuilder>, ISerializerConsumer<IClientBuilder>
    {
        /// <summary>
        /// Sets the <see cref="IClientApiKey">Client API Key</see> to use when making requests.
        /// If this method is not called, the default API Key locations will be checked.
        /// </summary>
        /// <param name="apiKey">The API Key to use.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="apiKey"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="apiKey"/> is not valid.</exception>
        [Obsolete("Set the API Key ID and Secret directly, or use SetApiKeyFilePath, or SetConfiguration")]
        IClientBuilder SetApiKey(IClientApiKey apiKey);

        /// <summary>
        /// Sets the Stormpath API Key ID to use when making requests.
        /// </summary>
        /// <remarks>
        /// This can also be set by passing a configuration object to <see cref="SetConfiguration(StormpathConfiguration)"/>, or by a JSON or YAML configuration.
        /// If no API credentials are provided, the default locations will be searched.
        /// </remarks>
        /// <param name="id">The Stormpath API Key ID.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <example>
        /// <code source = "ClientBuilderExamples.cs" region="SetApiCredentialsDirectly" lang="C#" title="Set API credentials directly" />
        /// </example>
        IClientBuilder SetApiKeyId(string id);

        /// <summary>
        /// Sets the Stormpath API Key Secret to use when making requests.
        /// </summary>
        /// <remarks>
        /// This can also be set by passing a configuration object to <see cref="SetConfiguration(StormpathConfiguration)"/>, or by a JSON or YAML configuration.
        /// If no API credentials are provided, the default locations will be searched.
        /// </remarks>
        /// <param name="secret">The Stormpath API Key Secret.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <example>
        /// <code source="ClientBuilderExamples.cs" region="SetApiCredentialsDirectly" lang="C#" title="Set API credentials directly" />
        /// </example>
        IClientBuilder SetApiKeySecret(string secret);

        /// <summary>
        /// Sets the location of an <c>apiKey.properties</c> file to load a Stormpath API Key ID/Secret pair to use when making requests.
        /// </summary>
        /// <remarks>
        /// This can also be set by passing a configuration object to <see cref="SetConfiguration(StormpathConfiguration)"/>, or by a JSON or YAML configuration.
        /// If no API credentials are provided, the default locations will be searched.
        /// </remarks>
        /// <param name="path">The path to <c>apiKey.properties</c>.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <example>
        /// <code source="ClientBuilderExamples.cs" region="SetApiCredentialsFile" lang="C#" title="Load API credentials from a file" />
        /// </example>
        IClientBuilder SetApiKeyFilePath(string path);

        /// <summary>
        /// Sets the configuration options to use for this <see cref="IClient">Client</see>.
        /// </summary>
        /// <param name="configuration">An instance of <see cref="StormpathConfiguration"/>.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <example>
        /// <code source="ClientBuilderExamples.cs" region="InstanceClientOptions" lang="C#" title="Specify options using StormpathConfiguration" />
        /// </example>
        IClientBuilder SetConfiguration(StormpathConfiguration configuration);

        /// <summary>
        /// Sets the configuration options to use for this <see cref="IClient">Client</see>.
        /// </summary>
        /// <param name="configuration">An anonymous type containing configuration options.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <example>
        /// <code source="ClientBuilderExamples.cs" region="AnonTypeClientOptions" lang="C#" title="Specify options using an anonymous type" />
        /// </example>
        IClientBuilder SetConfiguration(object configuration);

        /// <summary>
        /// Sets the authentication scheme to use when making requests.
        /// </summary>
        /// <param name="scheme">The authentication scheme to use.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <seealso cref="AuthenticationScheme.SAuthc1"/>
        [Obsolete("Use SetAuthenticationScheme(ClientAuthenticationScheme)")]
        IClientBuilder SetAuthenticationScheme(AuthenticationScheme scheme);

        /// <summary>
        /// Sets the authentication scheme to use when making requests.
        /// </summary>
        /// <param name="scheme">The authentication scheme to use.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <example>
        /// <code source="ClientBuilderExamples.cs" region="UseBasicAuthentication" lang="C#" title="Use HTTP Basic authentication" />
        /// </example>
        IClientBuilder SetAuthenticationScheme(ClientAuthenticationScheme scheme);

        /// <summary>
        /// Sets the HTTP connection timeout in milliseconds to observe when making requests.
        /// To use the default connection timeout, don't call this method.
        /// </summary>
        /// <param name="timeout">The HTTP connection timeout to use, in milliseconds.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
        IClientBuilder SetConnectionTimeout(int timeout);

        /// <summary>
        /// Sets the base API URL to use when making requests.
        /// The default is almost always correct; you shouldn't ever have to call this method.
        /// </summary>
        /// <param name="baseUrl">The base API URL to use.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <example>
        /// <code source="ClientBuilderExamples.cs" region="SetBaseUrl" lang="C#" />
        /// </example>
        IClientBuilder SetBaseUrl(string baseUrl);

        /// <summary>
        /// Sets the proxy server to use when making requests.
        /// To connect without a proxy server, don't call this method, or call with <paramref name="proxy"/> set to <see langword="null"/>.
        /// </summary>
        /// <param name="proxy">The proxy server to use.</param>
        /// <returns>This instance for method chaining.</returns>
        [Obsolete("Use the SetProxy(ClientProxyConfiguration) method")]
        IClientBuilder SetProxy(IWebProxy proxy);

        /// <summary>
        /// Sets the proxy server to use when making requests.
        /// To connect without a proxy server, don't call this method, or call with <paramref name="proxyConfiguration"/> set to <see langword="null"/>.
        /// </summary>
        /// <param name="proxyConfiguration">The configuration server to use.</param>
        /// <returns>This instance for method chaining.</returns>
        IClientBuilder SetProxy(ClientProxyConfiguration proxyConfiguration);

        /// <summary>
        /// Sets the cache provider that should be used to cache Stormpath resources, reducing round-trips
        /// to the Stormpath API server and enhancing application performance.
        /// </summary>
        /// <remarks>
        /// Use the <c>CacheProviders</c> object construct a cache provider.
        /// <para>
        /// Passing <see langword="null"/> to this method will disable caching; this is equivalent to passing <see cref="ICacheProviderFactory.DisabledCache"/>.
        /// </para>
        /// </remarks>
        /// <param name="cacheProvider">The cache provider to use, or <see langword="null"/> to disable caching.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <seealso cref="ICacheProviderFactory"/>
        /// <example>
        /// <code source="ClientBuilderExamples.cs" region="DisableCaching" lang="C#" title="Disable caching" />
        /// <code source="ClientBuilderExamples.cs" region="InMemoryCacheWithOptions" lang="C#" title="Use default (in-memory) cache with options" />
        /// </example>
        IClientBuilder SetCacheProvider(ICacheProvider cacheProvider);

        /// <summary>
        /// Sets the HTTP client to use when making requests.
        /// </summary>
        /// <remarks>
        /// Use the <c>HttpClients</c> object construct an HTTP client.
        /// <para>
        /// <see cref="SetHttpClient(IHttpClientBuilder)"/> is preferred, unless you have manually instantiated a class supporting <see cref="IHttpClient"/>.
        /// </para>
        /// </remarks>
        /// <param name="httpClient">The HTTP client to use.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="httpClient"/> is null.</exception>
        /// <seealso cref="IHttpClientFactory"/>
        /// <example>
        /// <code source="ClientBuilderExamples.cs" region="DefaultClientOptions" lang="C#" title="Create a Client with the default options" />
        /// </example>
        IClientBuilder SetHttpClient(IHttpClient httpClient);

        /// <summary>
        /// Sets the HTTP client to use when making requests.
        /// </summary>
        /// <remarks>
        /// Use the <c>HttpClients</c> object construct an HTTP client.
        /// </remarks>
        /// <param name="httpClientBuilder">The HTTP client to use.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="httpClientBuilder"/> is null.</exception>
        /// <seealso cref="IHttpClientFactory"/>
        /// <example>
        /// <code source="ClientBuilderExamples.cs" region="DefaultClientOptions" lang="C#" title="Create a Client with the default options" />
        /// </example>
        IClientBuilder SetHttpClient(IHttpClientBuilder httpClientBuilder);

        /// <summary>
        /// Sets the serializer to use when making requests.
        /// </summary>
        /// <remarks>
        /// Use the <c>Serializers</c> object construct a serializer.
        /// </remarks>
        /// <param name="serializerBuilder">The serializer to use.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serializerBuilder"/> is null.</exception>
        /// <seealso cref="ISerializerFactory"/>
        /// <example>
        /// <code source="ClientBuilderExamples.cs" region="DefaultClientOptions" lang="C#" title="Create a Client with the default options" />
        /// </example>
        IClientBuilder SetSerializer(ISerializerBuilder serializerBuilder);

        /// <summary>
        /// Constructs a new <see cref="IClient">Client</see> instance based on the builder's current configuration state.
        /// </summary>
        /// <returns>A new <see cref="IClient">Client</see> instance.</returns>
        /// <exception cref="Exception">No valid API Key ID and Secret could be found.</exception>
        /// <example>
        /// <code source="ClientBuilderExamples.cs" region="DefaultClientOptions" lang="C#" title="Create a Client with the default options" />
        /// </example>
        IClient Build();
    }
}
