// <copyright file="AbstractHttpClientBuilder.cs" company="Stormpath, Inc.">
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
using System.Reflection;
using Stormpath.SDK.Shared.Extensions;
using Stormpath.SDK.Logging;

namespace Stormpath.SDK.Http
{
    /// <summary>
    /// Builder class capable of constructing <see cref="IHttpClient"/> instances using the specified properties.
    /// </summary>
    /// <remarks>
    /// The constructed type <b>must</b> have a public or private constructor that accepts these parameters in this order:
    /// baseUrl (<see cref="string">string</see>), connectionTimeout (<see cref="int">int</see>), proxy (<see cref="IWebProxy">IWebProxy</see> or <see langword="null"/>), logger (<see cref="ILogger">ILogger</see> or <see langword="null"/>)
    /// </remarks>
    /// <typeparam name="T">The concrete type being constructed.</typeparam>
    public sealed class AbstractHttpClientBuilder<T> : IHttpClientBuilder
        where T : IHttpClient
    {
        private readonly ConstructorInfo targetConstructor;

        private string baseUrl;
        private TimeSpan connectionTimeout;
        private IWebProxy proxy;
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractHttpClientBuilder{T}"/> class.
        /// </summary>
        public AbstractHttpClientBuilder()
            : this(typeof(T))
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="AbstractHttpClientBuilder{T}"/> for the given <paramref name="targetType"/>.
        /// </summary>
        /// <remarks>
        /// <paramref name="targetType"/> must have a constructor that accepts these parameters (in order): <c>string, int, IWebProxy, ILogger</c>
        /// </remarks>
        /// <param name="targetType">The target plugin type to build.</param>
        public AbstractHttpClientBuilder(Type targetType)
        {
            this.targetConstructor = targetType?.GetTypeInfo()
                .GetConstructor(parameterTypes: new Type[] { typeof(string), typeof(TimeSpan), typeof(IWebProxy), typeof(ILogger) });

            if (this.targetConstructor == null)
            {
                throw new NotSupportedException(
                    $"The HTTP client '{typeof(T)?.Name}' does not have a supported constructor. Instantiate this HTTP client directly instead of using IHttpClientBuilder.");
            }
        }

        /// <inheritdoc/>
        IHttpClientBuilder IHttpClientBuilder.SetBaseUrl(string baseUrl)
        {
            this.baseUrl = baseUrl;
            return this;
        }

        /// <inheritdoc/>
        IHttpClientBuilder IHttpClientBuilder.SetConnectionTimeout(int connectionTimeout)
        {
            this.connectionTimeout = TimeSpan.FromMilliseconds(connectionTimeout);
            return this;
        }

        /// <inheritdoc/>
        IHttpClientBuilder IHttpClientBuilder.SetConnectionTimeout(TimeSpan timeout)
        {
            connectionTimeout = timeout;
            return this;
        }

        /// <inheritdoc/>
        IHttpClientBuilder IHttpClientBuilder.SetProxy(IWebProxy proxy)
        {
            this.proxy = proxy;
            return this;
        }

        /// <inheritdoc/>
        IHttpClientBuilder IHttpClientBuilder.SetProxy(Configuration.Abstractions.Immutable.ClientProxyConfiguration clientProxyConfiguration)
        {
            bool proxyConfigurationMissing = string.IsNullOrEmpty(clientProxyConfiguration?.Host);

            if (proxyConfigurationMissing)
            {
                return this;
            }

#if NET45 || NET451
            var proxy = new WebProxy(clientProxyConfiguration.Host, clientProxyConfiguration.Port.Value);
            proxy.Credentials = new NetworkCredential(clientProxyConfiguration.Username, clientProxyConfiguration.Password);

            this.proxy = proxy;
            return this;
#else
            throw new NotImplementedException("Proxy support is not yet available for this platform.");
#endif
        }

        /// <inheritdoc/>
        IHttpClientBuilder ILoggerConsumer<IHttpClientBuilder>.SetLogger(ILogger logger)
        {
            if (logger != null)
            {
                this.logger = logger;
            }

            return this;
        }

        /// <inheritdoc/>
        IHttpClient IHttpClientBuilder.Build()
        {
            var parameters = new object[] { this.baseUrl, this.connectionTimeout, this.proxy, this.logger };

            IHttpClient instance = null;

            try
            {
                instance = (IHttpClient)this.targetConstructor.Invoke(parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Unable to build the HTTP client {typeof(T).Name}; see the inner exception for details. Try instantiating the HTTP client directly instead of using IHttpClientBuilder.",
                    ex);
            }

            if (instance == null)
            {
                throw new Exception($"Unable to build the HTTP client {typeof(T).Name}. No exception was thrown, but the result was null. Try instantiating the HTTP client directly instead of using IHttpClientBuilder.");
            }

            return instance;
        }
    }
}
