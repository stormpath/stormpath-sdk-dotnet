// <copyright file="IHttpMessage.cs" company="Stormpath, Inc.">
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
    /// Represents an HTTP request or response message.
    /// </summary>
    public interface IHttpMessage
    {
        /// <summary>
        /// Gets the HTTP header collection associated with this message.
        /// </summary>
        /// <value>HTTP message headers.</value>
        HttpHeaders Headers { get; }

        /// <summary>
        /// Gets whether this HTTP message contains a body.
        /// </summary>
        /// <value><c>true</c> if <see cref="Body"/> is not null or empty.</value>
        bool HasBody { get; }

        /// <summary>
        /// Gets the message body, if any.
        /// </summary>
        /// <value>The HTTP message body. Can be null.</value>
        string Body { get; }

        /// <summary>
        /// Gets the message body content type, if any.
        /// </summary>
        /// <value>The HTTP message body content type (e.g. application/json). Null if <see cref="Body"/> is null or empty.</value>
        string BodyContentType { get; }
    }
}
