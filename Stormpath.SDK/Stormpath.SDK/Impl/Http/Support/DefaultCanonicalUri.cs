// <copyright file="DefaultCanonicalUri.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using Stormpath.SDK.Impl.Extensions;

namespace Stormpath.SDK.Impl.Http.Support
{
    internal sealed class DefaultCanonicalUri : ICanonicalUri
    {
        private readonly Uri resourcePath;
        private readonly QueryString query;

        public DefaultCanonicalUri(string href, Dictionary<string, string> queryParams)
            : this(href, new QueryString(queryParams))
        {
        }

        public DefaultCanonicalUri(string href, QueryString queryParams)
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

        public DefaultCanonicalUri(string href)
        {
            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException(nameof(href));

            Uri parsedUri = null;
            if (!Uri.TryCreate(href, UriKind.Absolute, out parsedUri))
                throw new ArgumentException("URI is invalid.", nameof(href));

            this.resourcePath = parsedUri.WithoutQueryAndFragment();
            var queryPart = GetQueryParametersFromHref(href) ?? string.Empty;
            this.query = new QueryString(queryPart);
        }

        // Copy-ish constructor
        public DefaultCanonicalUri(ICanonicalUri existing, Uri overrideResourcePath = null)
        {
            this.resourcePath = overrideResourcePath == null
                ? new Uri(existing.ResourcePath.WithoutQueryAndFragment().ToString())
                : overrideResourcePath.WithoutQueryAndFragment();

            this.query = new QueryString(existing.QueryString);
        }

        public Uri ResourcePath => resourcePath;

        public bool HasQuery => query == null;

        public QueryString QueryString => query;

        public override string ToString()
        {
            if (query.Any())
                return $"{resourcePath}?{query}";
            else
                return resourcePath.ToString();
        }

        public Uri ToUri() => new Uri(ToString(), UriKind.Absolute);

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
