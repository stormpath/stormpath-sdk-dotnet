// <copyright file="IHttpClient.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Http
{
    /// <summary>
    /// A wrapper interface for HTTP client plugins.
    /// </summary>
    public interface IHttpClient : IDisposable
    {
        /// <summary>
        /// Gets the base URL that is used to make requests.
        /// </summary>
        /// <value>The base request URL.</value>
        string BaseUrl { get; }

        /// <summary>
        /// Gets the connection timeout (in milliseconds) that is observed when making requests.
        /// </summary>
        /// <value>The connection timeout value in milliseconds.</value>
        int ConnectionTimeout { get; }

        /// <summary>
        /// Gets the proxy server that is used when making requests.
        /// </summary>
        /// <value>The proxy server.</value>
        IWebProxy Proxy { get; }

        /// <summary>
        /// Gets a value indicating whether this client instance is capable of making synchronous requests.
        /// </summary>
        /// <value>Should return <see langword="true"/> for clients implementing <see cref="ISynchronousHttpClient"/>.</value>
        bool IsSynchronousSupported { get; }

        /// <summary>
        /// Gets a value indicating whether this client instance is capable of making asynchronous requests.
        /// </summary>
        /// <value>Should return <see langword="true"/> for clients implementing <see cref="IAsynchronousHttpClient"/>.</value>
        bool IsAsynchronousSupported { get; }
    }
}
