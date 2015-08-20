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
    internal class QueryString
    {
        private readonly IList<KeyValuePair<string, string>> queryStringItems;

        public QueryString()
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

        private static List<KeyValuePair<string, string>> Parse(string queryString)
        {
            var resultItems = new List<KeyValuePair<string, string>>();

            if (queryString.Contains("?"))
                queryString = queryString.Split('?')?[1];

            if (string.IsNullOrEmpty(queryString))
                return resultItems; // empty

            foreach (var token in queryString.Split('&'))
            {
                var pair = token.Split('=');

                if (pair == null)
                    resultItems.Add(new KeyValuePair<string, string>(token, null));

                var key = pair[0];
                var value = string.Empty;
                if (pair.Length > 1 && !string.IsNullOrEmpty(pair[1]))
                    value = pair[1];
                resultItems.Add(new KeyValuePair<string, string>(key, value));
            }

            resultItems = resultItems
                .OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
                .ToList();
            return resultItems;
        }
    }
}
