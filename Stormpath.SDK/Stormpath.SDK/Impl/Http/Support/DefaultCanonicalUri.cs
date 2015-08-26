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
using System.Linq;

namespace Stormpath.SDK.Impl.Http.Support
{
    internal sealed class DefaultCanonicalUri : ICanonicalUri
    {
        private readonly Uri uri;
        private readonly QueryString query;

        public DefaultCanonicalUri(string absoluteUri, QueryString query)
        {
            if (string.IsNullOrEmpty(absoluteUri))
                throw new ArgumentNullException(nameof(absoluteUri));

            if (!Uri.TryCreate(absoluteUri, UriKind.Absolute, out uri))
                throw new ArgumentException("URI is invalid.", nameof(absoluteUri));

            this.query = query;
        }

        public DefaultCanonicalUri(string href, Dictionary<string, string> queryParams)
        {
            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException(nameof(href));

            Uri parsedUri = null;
            if (!Uri.TryCreate(href, UriKind.Absolute, out parsedUri))
                throw new ArgumentException("URI is invalid.", nameof(href));

            var finalHref = $"{parsedUri.Scheme}://{parsedUri.Host}{parsedUri.AbsolutePath}"; // chop off query and fragment
            var finalQuery = new QueryString(queryParams);

            bool hrefIncludesQueryParams = !string.IsNullOrEmpty(parsedUri.Query);
            if (hrefIncludesQueryParams)
            {
                var queryParamsFromHref = parsedUri.Query;
                finalQuery.Merge(new QueryString(queryParamsFromHref));
            }

            this.uri = new Uri(finalHref, UriKind.Absolute);
            this.query = finalQuery;
        }

        string ICanonicalUri.AbsoluteUri => ToString();

        bool ICanonicalUri.HasQuery => query == null;

        QueryString ICanonicalUri.QueryString => query;

        public override string ToString() => $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}";
    }
}
