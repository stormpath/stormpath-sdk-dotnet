// <copyright file="HttpRequests.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
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

using System.Collections.Generic;
using Stormpath.SDK.Impl.Http;

namespace Stormpath.SDK.Http
{
    /// <summary>
    /// Utility factory class for creating <see cref="IHttpRequest"/> instances.
    /// </summary>
    public static class HttpRequests
    {
        /// <summary>
        /// Creates a new instance of <see cref="IHttpRequest"/> based on the provided attributes.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="uri">The URI of the request.</param>
        /// <returns>An <see cref="IHttpRequest"/> instance that represents this request.</returns>
        public static IHttpRequest Build(HttpMethod method, string uri)
        {
            return new DefaultHttpRequest(
                method, new CanonicalUri(uri), null, null, null, null);
        }

        /// <summary>
        /// Creates a new instance of <see cref="IHttpRequest"/> based on the provided attributes.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="uri">The URI of the request.</param>
        /// <param name="headers">The request headers, if any.</param>
        /// <param name="body">The body content, if any.</param>
        /// <param name="bodyContentType">The body content type.</param>
        /// <returns>An <see cref="IHttpRequest"/> instance that represents this request.</returns>
        public static IHttpRequest Build(HttpMethod method, string uri, IDictionary<string, object> headers, string body, string bodyContentType)
        {
            return new DefaultHttpRequest(
                method,
                new CanonicalUri(uri),
                null,
                new HttpHeaders(headers),
                body,
                bodyContentType);
        }
    }
}
