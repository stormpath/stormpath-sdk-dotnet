// <copyright file="UriQualifier.cs" company="Stormpath, Inc.">
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
using System.Text;

namespace Stormpath.SDK.Http
{
    /// <summary>
    /// Provides methods that ensure resource paths are fully-qualified.
    /// </summary>
    public sealed class UriQualifier
    {
        private readonly string baseUri;

        /// <summary>
        /// Creates a new <see cref="UriQualifier"/> given the specified <paramref name="baseUri"/>.
        /// </summary>
        /// <param name="baseUri"></param>
        public UriQualifier(string baseUri)
        {
            if (string.IsNullOrEmpty(baseUri))
            {
                throw new ArgumentNullException("baseUrl");
            }

            this.baseUri = baseUri;
        }

        /// <summary>
        /// Returns a fully-qualified URI for a given resource <paramref name="href"/>.
        /// </summary>
        /// <param name="href">The resource path.</param>
        /// <returns><paramref name="href"/> if it is already fully-qualified; otherwise, <paramref name="href"/> with <c>baseUri</c> prepended.</returns>
        public string EnsureFullyQualified(string href)
        {
            if (string.IsNullOrEmpty(href))
            {
                return href;
            }

            if (IsFullyQualified(href))
            {
                return href;
            }

            var fullyQualified = new StringBuilder(this.baseUri);
            if (!href.StartsWith("/"))
            {
                fullyQualified.Append("/");
            }

            fullyQualified.Append(href);
            return fullyQualified.ToString();
        }

        /// <summary>
        /// Determines whether a resource <paramref name="href"/> is fully-qualified.
        /// </summary>
        /// <param name="href">The resource path.</param>
        /// <returns><see langword="true"/> if <paramref name="href"/> is fully-qualified; otherwise, <see langword="false"/>.</returns>
        public static bool IsFullyQualified(string href)
        {
            bool tooShort = string.IsNullOrEmpty(href) || href.Length < 5;
            if (tooShort)
            {
                return false;
            }

            bool startsWithHttp = href.Substring(0, 4).Equals("http", StringComparison.OrdinalIgnoreCase);
            return startsWithHttp;
        }
    }
}
