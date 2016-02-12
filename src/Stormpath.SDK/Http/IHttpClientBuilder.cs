// <copyright file="IHttpClientBuilder.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Logging;

namespace Stormpath.SDK.Http
{
    /// <summary>
    /// Builder for <see cref="IHttpClient"/> instances.
    /// </summary>
    public interface IHttpClientBuilder : ILoggerConsumer<IHttpClientBuilder>
    {
        /// <summary>
        /// Sets the remote base URL.
        /// </summary>
        /// <param name="baseUrl">The base URL.</param>
        /// <returns>This instance for method chaining.</returns>
        IHttpClientBuilder SetBaseUrl(string baseUrl);

        /// <summary>
        /// Sets the connection timeout.
        /// </summary>
        /// <param name="connectionTimeout">The timeout in milliseconds.</param>
        /// <returns>This instance for method chaining.</returns>
        IHttpClientBuilder SetConnectionTimeout(int connectionTimeout);

        /// <summary>
        /// Sets the connection proxy.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>This instance for method chaining.</returns>
        IHttpClientBuilder SetProxy(IWebProxy proxy);

        /// <summary>
        /// Builds a new <see cref="IHttpClient"/> instance from the current builder state.
        /// </summary>
        /// <returns>A new <see cref="IHttpClient"/> instance.</returns>
        IHttpClient Build();
    }
}
