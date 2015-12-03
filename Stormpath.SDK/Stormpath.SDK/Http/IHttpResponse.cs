// <copyright file="IHttpResponse.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Http
{
    /// <summary>
    /// Represents an HTTP response message.
    /// </summary>
    public interface IHttpResponse : IHttpMessage
    {
        /// <summary>
        /// Gets the HTTP status code returned from the server.
        /// </summary>
        /// <value>The HTTP status code.</value>
        int StatusCode { get; }

        /// <summary>
        /// Gets a value indicating whether the request failed due to a transport (non-HTTP) error.
        /// </summary>
        /// <value><c>true</c> for requests that failed due to transport problems (e.g. connection timeout, DNS resolution errors).</value>
        bool TransportError { get; }

        /// <summary>
        /// Gets the HTTP response string returned by the server.
        /// </summary>
        /// <value>The HTTP response string (e.g. "OK").</value>
        string ResponsePhrase { get; }
    }
}