// <copyright file="CanonicalUri.cs" company="Stormpath, Inc.">
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

using System;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Impl.Http.Support;

namespace Stormpath.SDK.Http
{
    /// <summary>
    /// Represents a URI that has <see cref="ResourcePath"/> and <see cref="QueryString"/> elements.
    /// </summary>
    public sealed class CanonicalUri
    {
        private readonly Uri resourcePath;
        private readonly QueryString query;

        /// <summary>
        /// Initializes a new instance of the <see cref="CanonicalUri"/> class from a fully-qualified URI.
        /// </summary>
        /// <param name="href">The value for <see cref="ResourcePath"/>.</param>
        public CanonicalUri(string href)
        {
            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException(nameof(href));

            if (!UriQualifier.IsFullyQualified(href))
                throw new ArgumentException("URI must be fully-qualified.", nameof(href));

            Uri parsedUri = null;
            if (!Uri.TryCreate(href, UriKind.Absolute, out parsedUri))
                throw new ArgumentException("URI is invalid.", nameof(href));

            this.resourcePath = parsedUri.WithoutQueryAndFragment();
            var queryPart = GetQueryParametersFromHref(href) ?? string.Empty;
            this.query = new QueryString(queryPart);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanonicalUri"/> class
        /// with the specified <see cref="ResourcePath"/> and <see cref="QueryString"/> components.
        /// </summary>
        /// <param name="href">The value for <see cref="ResourcePath"/>.</param>
        /// <param name="queryParams">The value for <see cref="QueryString"/>.</param>
        public CanonicalUri(string href, QueryString queryParams)
            : this(href)
        {
            if (HasQueryParameters(href))
            {
                var queryParamsFromHref = GetQueryParametersFromHref(href);

                // Explicit parameters from href string are not replaced
                queryParams = new QueryString(queryParamsFromHref).Merge(queryParams);
            }

            this.query = queryParams ?? new QueryString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanonicalUri"/> class
        /// with the <see cref="resourcePath"/> overridden by the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="existing">An existing <see cref="CanonicalUri"/> to copy.</param>
        /// <param name="overrideResourcePath">A replacement for <see cref="ResourcePath"/>.</param>
        internal CanonicalUri(CanonicalUri existing, Uri overrideResourcePath)
        {
            this.resourcePath = overrideResourcePath == null
                ? new Uri(existing.ResourcePath.WithoutQueryAndFragment().ToString())
                : overrideResourcePath.WithoutQueryAndFragment();

            this.query = new QueryString(existing.QueryString);
        }

        /// <summary>
        /// Gets the resource URL component.
        /// </summary>
        /// <value>The resource path (absolute URL without query string) of this <see cref="CanonicalUri"/>.</value>
        public Uri ResourcePath => this.resourcePath;

        /// <summary>
        /// Gets whether this <see cref="CanonicalUri"/> has a <see cref="QueryString"/> component.
        /// </summary>
        /// <value><c>true</c> if <see cref="QueryString"/> is not null; <c>false</c> otherwise.</value>
        public bool HasQuery => this.query == null;

        /// <summary>
        /// Gets the query string component.
        /// </summary>
        /// <value>The query string (any values after '?') of this <see cref="CanonicalUri"/>.</value>
        public QueryString QueryString => this.query;

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.query.Any())
                return $"{this.resourcePath}?{this.query}";
            else
                return this.resourcePath.ToString();
        }

        /// <summary>
        /// Converts this <see cref="CanonicalUri"/> to a <see cref="Uri"/>.
        /// </summary>
        /// <returns>A <see cref="Uri"/> with <see cref="UriKind"/> <see cref="UriKind.Absolute"/>.</returns>
        public Uri ToUri() => new Uri(this.ToString(), UriKind.Absolute);

        private static bool HasQueryParameters(string href)
        {
            return href.Contains("?");
        }

        private static string GetQueryParametersFromHref(string href)
        {
            var segments = href.Split(new char[] { '?' }, 2);

            return segments.Length > 1
                ? segments[1]
                : null;
        }
    }
}
