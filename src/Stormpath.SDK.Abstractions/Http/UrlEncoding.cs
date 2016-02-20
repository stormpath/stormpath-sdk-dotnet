// <copyright file="RequestHelper.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Http
{
    /// <summary>
    /// Provides methods that encode URL-safe strings.
    /// </summary>
    public static class UrlEncoding
    {
        /// <summary>
        /// Encodes a string using URL-safe encoding rules.
        /// </summary>
        /// <param name="value">The string to encode.</param>
        /// <param name="isPath">Determines whether this string represents a resource path.</param>
        /// <param name="canonicalize">Determines whether Stormpath canonicalization rules should be applied.</param>
        /// <returns>The encoded string.</returns>
        public static string Encode(string value, bool isPath = false, bool canonicalize = false)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var encoded = WebUtility.UrlEncode(value);

            // Perform some custom Stormpath encoding
            if (canonicalize)
            {
                encoded = encoded
                    .Replace("%2B", "+")
                    .Replace("+", "%20")
                    .Replace("*", "%2A")
                    .Replace("%7E", "~")
                    .Replace("(", "%28")
                    .Replace(")", "%29");

                if (isPath)
                {
                    encoded = encoded.Replace("%2F", "/");
                }
            }

            return encoded;
        }
    }
}
