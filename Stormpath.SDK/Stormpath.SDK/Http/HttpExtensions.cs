// <copyright file="HttpExtensions.cs" company="Stormpath, Inc.">
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
    public static class HttpExtensions
    {
        /// <summary>
        /// Determines whether an HTTP response resulted in a client-side (HTTP 4xx) error.
        /// </summary>
        /// <param name="response">The HTTP response to check.</param>
        /// <returns>True if the response represents a client-side (HTTP 4xx) error.</returns>
        public static bool IsClientError(this IHttpResponse response)
        {
            return response.StatusCode >= 400 && response.StatusCode < 500;
        }

        /// <summary>
        /// Determines whether an HTTP response resulted in a server-side (HTTP 5xx) error.
        /// </summary>
        /// <param name="response">The HTTP response to check.</param>
        /// <returns>True if the response represents a server-side (HTTP 5xx) error.</returns>
        public static bool IsServerError(this IHttpResponse response)
        {
            return response.StatusCode >= 500 && response.StatusCode < 600;
        }
    }
}
