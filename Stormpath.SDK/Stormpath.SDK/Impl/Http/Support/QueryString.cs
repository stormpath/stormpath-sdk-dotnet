// <copyright file="QueryString.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Impl.Http.Support
{
    internal class QueryString : ImmutableValueObject<QueryString>
    {
        private static Func<QueryString, QueryString, bool> compareFunction = new Func<QueryString, QueryString, bool>(
            (a, b) => a.queryStringItems.SequenceEqual(b.queryStringItems));

        private readonly Dictionary<string, string> queryStringItems;

        public QueryString()
            : base(compareFunction)
        {
            queryStringItems = null;
        }

        public QueryString(Uri uri)
            : this(uri.Query)
        {
        }

        public QueryString(string queryString)
            : this()
        {
            queryStringItems = Parse(queryString);
        }

        public QueryString(Dictionary<string, string> queryParams)
            : this()
        {
            if (queryParams != null)
                queryStringItems = ToSortedDictionary(queryParams);
        }

        // Copy constructor
        public QueryString(QueryString existing)
            : this(existing.queryStringItems)
        {
        }

        public bool Any()
        {
            return queryStringItems?.Any() ?? false;
        }

        public string ToString(bool canonical)
        {
            bool isEmpty = queryStringItems == null || queryStringItems.Count == 0;
            if (isEmpty)
                return string.Empty;

            var items = queryStringItems.Select(x =>
            {
                var key = RequestHelper.UrlEncode(x.Key, false, canonical);
                var value = RequestHelper.UrlEncode(x.Value, false, canonical);
                return $"{key}={value}";
            });

            return string.Join("&", items);
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public QueryString Merge(QueryString replacementParams)
        {
            if (replacementParams == null)
                return this;

            if (!replacementParams.queryStringItems.Any())
                return this;

            var mergedItems = new List<KeyValuePair<string, string>>(this.queryStringItems);
            foreach (var param in replacementParams.queryStringItems)
            {
                // The values in the actual queryString are explicit and take precedence over any values passed in
                bool duplicateExists = this.queryStringItems.ContainsKey(param.Key);
                if (!duplicateExists)
                {
                    mergedItems.Add(param);
                }
            }

            return new QueryString(ToSortedDictionary(mergedItems));
        }

        private static Dictionary<string, string> Parse(string queryString)
        {
            var resultItems = new Dictionary<string, string>();

            if (queryString.Contains("?"))
                queryString = queryString.Split('?')?[1];

            if (string.IsNullOrEmpty(queryString))
                return resultItems; // empty

            foreach (var token in queryString.Split('&'))
            {
                var pair = token.Split('=');

                if (pair == null)
                    resultItems.Add(token, null);

                var key = pair[0];
                var value = string.Empty;
                if (pair.Length > 1 && !string.IsNullOrEmpty(pair[1]))
                    value = pair[1];
                resultItems.Add(key, value);
            }

            return ToSortedDictionary(resultItems);
        }

        private static Dictionary<string, string> ToSortedDictionary(IEnumerable<KeyValuePair<string, string>> queryParams)
        {
            return queryParams
                .OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}
