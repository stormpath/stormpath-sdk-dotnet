// <copyright file="HttpHeaders.cs" company="Stormpath, Inc.">
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stormpath.SDK.Http
{
    // TODO this class needs a lot of TLC
    public sealed class HttpHeaders : IEnumerable<KeyValuePair<string, IEnumerable<string>>>
    {
        private static readonly string HeaderAcceptName = "Accept";
        private static readonly string HeaderAuthorizationName = "Authorization";
        private static readonly string HeaderContentLengthName = "Content-Length";
        private static readonly string HeaderContentTypeName = "Content-Type";
        private static readonly string HeaderHostName = "Host";
        private static readonly string LocationName = "Location";
        private static readonly string HeaderUserAgentName = "User-Agent";

        private readonly Dictionary<string, List<object>> headers;
        private bool readOnly;

        public HttpHeaders()
        {
            this.headers = new Dictionary<string, List<object>>();
            this.readOnly = false;
        }

        // Copy constructor
        public HttpHeaders(HttpHeaders existing)
        {
            this.headers = new Dictionary<string, List<object>>(existing.headers);
            this.readOnly = existing.readOnly;
        }

        // TODO MediaTypeQuery
        public string Accept
        {
            get { return GetFirst<string>(HeaderAcceptName); }
            set { this.Add(HeaderAcceptName, value); }
        }

        public AuthorizationHeaderValue Authorization
        {
            get { return GetFirst<AuthorizationHeaderValue>(HeaderAuthorizationName); }
            set { this.Add(HeaderAuthorizationName, value); }
        }

        public long? ContentLength
        {
            get { return GetFirst<long?>(HeaderContentLengthName); }
            set { this.Add(HeaderContentLengthName, value); }
        }

        // TODO MediaTypeQuery
        public string ContentType
        {
            get { return GetFirst<string>(HeaderContentTypeName); }
            set { this.Add(HeaderContentTypeName, value); }
        }

        public string Host
        {
            get { return GetFirst<string>(HeaderHostName); }
            set { this.Add(HeaderHostName, value); }
        }

        public Uri Location
        {
            get
            {
                var location = GetFirst<string>(LocationName);
                if (string.IsNullOrEmpty(location))
                    return null;

                return new Uri(location, UriKind.Absolute);
            }

            set
            {
                this.Add(LocationName, value.AbsoluteUri);
            }
        }

        public string UserAgent
        {
            get { return GetFirst<string>(HeaderUserAgentName); }
            set { this.Add(HeaderUserAgentName, value); }
        }

        public T GetFirst<T>(string key)
        {
            if (!this.headers.ContainsKey(key) || !this.headers[key].Any())
                return default(T);

            return (T)this.headers[key][0];
        }

        public void Add(string key, object value)
        {
            if (this.readOnly)
                throw new InvalidOperationException("This headers collection is read-only.");

            if (!this.headers.ContainsKey(key))
                this.headers.Add(key, new List<object>());

            this.headers[key].Add(value);
        }

        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
        {
            foreach (var header in this.headers)
            {
                yield return new KeyValuePair<string, IEnumerable<string>>(
                    header.Key,
                    header.Value.Select(x => x.ToString()).ToList());
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
