// <copyright file="AbstractHttpResponse.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Http
{
    /// <summary>
    /// Base class for HTTP responses returned from HTTP client plugins.
    /// </summary>
    public abstract class AbstractHttpResponse : IHttpResponse
    {
        /// <summary>
        /// Creates a new instance of <see cref="AbstractHttpResponse"/>.
        /// </summary>
        /// <param name="statusCode">The HTTP response status code.</param>
        /// <param name="responseMessage">The HTTP response message.</param>
        /// <param name="headers">The HTTP headers.</param>
        /// <param name="body">The response body.</param>
        /// <param name="contentType">The response body Content-Type.</param>
        /// <param name="transportError">Determines whether the request failed due to a network or transport error.</param>
        public AbstractHttpResponse(
            int statusCode,
            string responseMessage,
            HttpHeaders headers,
            string body,
            string contentType,
            bool transportError)
        {
            this.StatusCode = statusCode;
            this.ResponsePhrase = responseMessage;
            this.Headers = headers ?? new HttpHeaders();
            this.Body = body;
            this.BodyContentType = contentType;
            this.TransportError = transportError;
        }

        /// <inheritdoc/>
        public string Body { get; private set; }

        /// <inheritdoc/>
        public string BodyContentType { get; private set; }

        /// <inheritdoc/>
        public bool HasBody => !string.IsNullOrEmpty(this.Body);

        /// <inheritdoc/>
        public HttpHeaders Headers { get; private set; }

        /// <inheritdoc/>
        public string ResponsePhrase { get; private set; }

        /// <inheritdoc/>
        public int StatusCode { get; private set; }

        /// <inheritdoc/>
        public bool TransportError { get; private set; }
    }
}
