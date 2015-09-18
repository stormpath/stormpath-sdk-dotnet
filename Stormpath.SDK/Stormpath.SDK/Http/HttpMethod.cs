// <copyright file="HttpMethod.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Http
{
    /// <summary>
    /// Enumeration of HTTP methods defined in the HTTP/1.1 specification.
    /// </summary>
    public sealed class HttpMethod : Enumeration
    {
        /// <summary>
        /// An HTTP GET.
        /// </summary>
        public static HttpMethod Get = new HttpMethod(0, "GET");

        /// <summary>
        /// An HTTP HEAD.
        /// </summary>
        public static HttpMethod Head = new HttpMethod(10, "HEAD");

        /// <summary>
        /// An HTTP POST.
        /// </summary>
        public static HttpMethod Post = new HttpMethod(20, "POST");

        /// <summary>
        /// An HTTP PUT.
        /// </summary>
        public static HttpMethod Put = new HttpMethod(30, "PUT");

        /// <summary>
        /// An HTTP PATCH.
        /// </summary>
        public static HttpMethod Patch = new HttpMethod(40, "PATCH");

        /// <summary>
        /// An HTTP DELETE.
        /// </summary>
        public static HttpMethod Delete = new HttpMethod(50, "DELETE");

        /// <summary>
        /// A HTTP OPTIONS.
        /// </summary>
        public static HttpMethod Options = new HttpMethod(60, "OPTIONS");

        /// <summary>
        /// An HTTP TRACE.
        /// </summary>
        public static HttpMethod Trace = new HttpMethod(70, "TRACE");

        /// <summary>
        /// An HTTP Connect.
        /// </summary>
        public static HttpMethod Connect = new HttpMethod(80, "CONNECT");

        private HttpMethod()
        {
        }

        private HttpMethod(int value, string displayName)
            : base(value, displayName)
        {
        }

        /// <summary>
        /// Creates a deep copy of an HttpMethod instance.
        /// </summary>
        /// <returns>A new instance representing the same <see cref="HttpMethod"/> action.</returns>
        public HttpMethod Clone()
        {
            return Parse(this.DisplayName);
        }

        /// <summary>
        /// Parses a string to an <see cref="HttpMethod"/>.
        /// </summary>
        /// <param name="method">A string containing the name of the HTTP method (matching is case-insensitive).</param>
        /// <returns>The <see cref="HttpMethod"/> with the specified name.</returns>
        /// <exception cref="ApplicationException">No match is found.</exception>
        public static HttpMethod Parse(string method)
        {
            switch (method.ToUpper())
            {
                case "GET": return Get;
                case "HEAD": return Head;
                case "POST": return Post;
                case "PUT": return Put;
                case "PATCH": return Patch;
                case "DELETE": return Delete;
                case "OPTIONS": return Options;
                case "TRACE": return Trace;
                case "CONNECT": return Connect;
                default:
                    throw new ApplicationException($"Could not parse HTTP method value '{method.ToUpper()}'");
            }
        }
    }
}
