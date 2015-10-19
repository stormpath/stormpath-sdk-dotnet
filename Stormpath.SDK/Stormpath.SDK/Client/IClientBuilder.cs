// <copyright file="IClientBuilder.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Http;
using Stormpath.SDK.Serialization;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Client
{
    /// <summary>
    /// A Builder design pattern used to construct <see cref="IClient"/> instances.
    /// </summary>
    /// <example>
    /// Create a client with a specified API Key:
    /// <code>
    ///     IClient client = Clients.Builder()
    ///         .SetApiKey(apiKey)
    ///         .Build();
    /// </code>
    /// </example>
    public interface IClientBuilder
    {
        /// <summary>
        /// Sets the <see cref="IClientApiKey"/> to use when making requests.
        /// If this method is not called, the default API Key locations will be checked.
        /// </summary>
        /// <param name="apiKey">The API Key to use.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="apiKey"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="apiKey"/> is not valid.</exception>
        IClientBuilder SetApiKey(IClientApiKey apiKey);

        /// <summary>
        /// Sets the authentication scheme to use when making requests.
        /// To use the default authentication scheme (<see cref="AuthenticationScheme.SAuthc1"/>), don't call this method.
        /// </summary>
        /// <param name="scheme">The authentication scheme to use.</param>
        /// <returns>This instance for method chaining.</returns>
        IClientBuilder SetAuthenticationScheme(AuthenticationScheme scheme);

        /// <summary>
        /// Sets the HTTP connection timeout to observe when making requests.
        /// To use the default connection timeout (20 seconds), don't call this method.
        /// </summary>
        /// <param name="timeout">The HTTP connection timeout to use, in milliseconds.</param>
        /// <returns>This instance for method chaining.</returns>
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
        /// To connect without a proxy server, don't call this method, or call with <paramref name="proxy"/> set to <c>null</c>.
        /// </summary>
        /// <param name="proxy">The proxy server to use.</param>
        /// <returns>This instance for method chaining.</returns>
        IClientBuilder SetProxy(IWebProxy proxy);

        /// <summary>
        /// Sets the JSON serializer to use when serializing and deserializing request data.
        /// Don't call this method unless you want to use a different serializer than the default.
        /// </summary>
        /// <param name="serializer">A valid <see cref="IJsonSerializer"/> instance.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="serializer"/> is null.</exception>
        IClientBuilder UseJsonSerializer(IJsonSerializer serializer);

        /// <summary>
        /// Sets the HTTP client to use when making requests.
        /// Don't call this method unless you want to use a different HTTP client than the default.
        /// </summary>
        /// <param name="httpClient">A valid <see cref="IHttpClient"/> instance.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="httpClient"/> is null.</exception>
        IClientBuilder UseHttpClient(IHttpClient httpClient);

        /// <summary>
        /// Sets an optional logger to send trace and debug messages to.
        /// </summary>
        /// <param name="logger">A logger instance for capturing trace output; pass <c>null</c> to disable logging.</param>
        /// <returns>This instance for method chaining.</returns>
        IClientBuilder SetLogger(ILogger logger);

        /// <summary>
        /// Advanced use; only change this if you know what you are doing. Sets the internal identity map expiration time.
        /// </summary>
        /// <param name="expiration">Identity map expiration timeout.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="expiration"/> is less than 10 seconds or more than 24 hours.</exception>
        IClientBuilder SetIdentityMapExpiration(TimeSpan expiration);

        /// <summary>
        /// Constructs a new <see cref="IClient"/> instance based on the builder's current configuration state.
        /// </summary>
        /// <returns>A new <see cref="IClient"/> instance.</returns>
        /// <exception cref="System.ApplicationException">No valid API Key ID and Secret could be found.</exception>
        IClient Build();
    }
}
