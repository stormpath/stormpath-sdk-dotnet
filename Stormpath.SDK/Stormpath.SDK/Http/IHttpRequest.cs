// <copyright file="IHttpRequest.cs" company="Stormpath, Inc.">
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
    /// Represents an HTTP request message.
    /// </summary>
    public interface IHttpRequest : IHttpMessage
    {
        /// <summary>
        /// Gets the HTTP method used in this request.
        /// </summary>
        /// <value>An HTTP verb (e.g. GET, POST, PUT).</value>
        HttpMethod Method { get; }

        /// <summary>
        /// Gets the target URL of this request.
        /// </summary>
        /// <value>The target URL represented by a <see cref="CanonicalUri"/> instance.</value>
        CanonicalUri CanonicalUri { get; }
    }
}
