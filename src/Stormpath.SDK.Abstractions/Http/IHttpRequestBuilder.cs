// <copyright file="IHttpRequestBuilder.cs" company="Stormpath, Inc.">
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
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Http
{
    /// <summary>
    /// A Builder design pattern used to construct <see cref="IHttpRequest"/> instances, which represent abstract HTTP requests.
    /// </summary>
    public interface IHttpRequestBuilder
    {
        /// <summary>
        /// Sets the HTTP method used for this request.
        /// </summary>
        /// <param name="methodName">The HTTP method name (case-insensitive).</param>
        /// <returns>This instance for method chaining.</returns>
        IHttpRequestBuilder WithMethod(string methodName);

        /// <summary>
        /// Sets the HTTP method used for this request.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <returns>This instance for method chaining.</returns>
        IHttpRequestBuilder WithMethod(HttpMethod method);

        /// <summary>
        /// Sets the URI used for this request.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>This instance for method chaining.</returns>
        IHttpRequestBuilder WithUri(string uri);

        /// <summary>
        /// /// Sets the URI used for this request.
        /// </summary>
        /// <param name="uri">The URI</param>
        /// <returns>This instance for method chaining.</returns>
        IHttpRequestBuilder WithUri(Uri uri);

        /// <summary>
        /// Sets the HTTP header collection used for this request.
        /// </summary>
        /// <param name="headers">The HTTP headers.</param>
        /// <returns>This instance for method chaining.</returns>
        IHttpRequestBuilder WithHeaders(Map headers);

        /// <summary>
        /// Sets the body used for this request.
        /// </summary>
        /// <param name="body">The request body.</param>
        /// <returns>This instance for method chaining.</returns>
        IHttpRequestBuilder WithBody(string body);

        /// <summary>
        /// Sets the content type of the request body.
        /// </summary>
        /// <param name="bodyContentType">The request body content type.</param>
        /// <returns>This instance for method chaining.</returns>
        IHttpRequestBuilder WithBodyContentType(string bodyContentType);

        /// <summary>
        /// Creates a new <see cref="IHttpRequest"/> instance based on the builder's current configuration state.
        /// </summary>
        /// <returns>A new <see cref="IHttpRequest"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The HTTP method or URI was not set.</exception>
        IHttpRequest Build();
    }
}
