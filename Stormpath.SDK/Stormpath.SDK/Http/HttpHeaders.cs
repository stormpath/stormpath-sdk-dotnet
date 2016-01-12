// <copyright file="HttpHeaders.cs" company="Stormpath, Inc.">
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Stormpath.SDK.Impl.Extensions;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Http
{
    /// <summary>
    /// Represents a collection of HTTP headers in an HTTP message.
    /// </summary>
    public sealed class HttpHeaders : IEnumerable<KeyValuePair<string, IEnumerable<string>>>
    {
        /// <summary>
        /// A string representation of the Content-Type header indicating x-www-form-urlencoded data.
        /// </summary>
        /// <value>application/x-www-form-urlencoded</value>
        public static readonly string MediaTypeApplicationFormUrlEncoded = "application/x-www-form-urlencoded";

        /// <summary>
        /// The HTTP Accept header.
        /// </summary>
        /// <value>Accept</value>
        public static readonly string HeaderAcceptName = "Accept";

        /// <summary>
        /// The HTTP Authorization header.
        /// </summary>
        /// <value>Accept</value>
        public static readonly string HeaderAuthorizationName = "Authorization";

        /// <summary>
        /// The HTTP Content-Length header.
        /// </summary>
        /// <value>Content-Length</value>
        public static readonly string HeaderContentLengthName = "Content-Length";

        /// <summary>
        /// The HTTP Content-Type header.
        /// </summary>
        /// <value>Content-Type</value>
        public static readonly string HeaderContentTypeName = "Content-Type";

        /// <summary>
        /// The HTTP Host header.
        /// </summary>
        /// <value>Host</value>
        public static readonly string HeaderHostName = "Host";

        /// <summary>
        /// The HTTP Location header.
        /// </summary>
        /// <value>Location</value>
        public static readonly string LocationName = "Location";

        /// <summary>
        /// The HTTP User-Agent header.
        /// </summary>
        /// <value>User-Agent</value>
        public static readonly string HeaderUserAgentName = "User-Agent";

        private readonly IDictionary<string, List<object>> headers;
        private bool readOnly;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpHeaders"/> class.
        /// </summary>
        public HttpHeaders()
        {
            this.headers = new Dictionary<string, List<object>>();
            this.readOnly = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpHeaders"/> class
        /// by copying values from a dictionary.
        /// </summary>
        /// <param name="existing">A collection of headers to copy.</param>
        internal HttpHeaders(Map existing)
        {
            this.headers = existing
                .Select(x => new KeyValuePair<string, List<object>>(
                    x.Key, new List<object>() { x.Value }))
                .ToDictionary();

            this.readOnly = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpHeaders"/> class
        /// by copying values from an existing instance.
        /// </summary>
        /// <param name="existing">An existing <see cref="HttpHeaders"/> instance to copy.</param>
        internal HttpHeaders(HttpHeaders existing)
        {
            this.headers = new Dictionary<string, List<object>>(existing.headers);
            this.readOnly = existing.readOnly;
        }

        /// <summary>
        /// Gets or sets the HTTP Accept header value.
        /// </summary>
        /// <value>Accept string (e.g. application/json).</value>
        public string Accept
        {
            get { return this.GetFirst<string>(HeaderAcceptName); }
            set { this.Add(HeaderAcceptName, value); }
        }

        /// <summary>
        /// Gets or sets the HTTP Authorization header value.
        /// </summary>
        /// <value>Authentication credentials represented by a <see cref="AuthorizationHeaderValue"/> instance.</value>
        public AuthorizationHeaderValue Authorization
        {
            get { return this.GetFirst<AuthorizationHeaderValue>(HeaderAuthorizationName); }
            set { this.Add(HeaderAuthorizationName, value); }
        }

        /// <summary>
        /// Gets or sets the HTTP Content-Length header value.
        /// </summary>
        /// <value>The length of the body in bytes.</value>
        public long? ContentLength
        {
            get { return this.GetFirst<long?>(HeaderContentLengthName); }
            set { this.Add(HeaderContentLengthName, value); }
        }

        /// <summary>
        /// Gets or sets the HTTP Content-Type header value.
        /// </summary>
        /// <value>The MIME type of the body (e.g. application/json).</value>
        public string ContentType
        {
            get { return this.GetFirst<string>(HeaderContentTypeName); }
            set { this.Add(HeaderContentTypeName, value); }
        }

        /// <summary>
        /// Gets or sets the HTTP Host header value.
        /// </summary>
        /// <value>The host name (and optionally, port number).</value>
        public string Host
        {
            get { return this.GetFirst<string>(HeaderHostName); }
            set { this.Add(HeaderHostName, value); }
        }

        /// <summary>
        /// Gets or sets the HTTP Location header value.
        /// </summary>
        /// <value>The location URL.</value>
        public Uri Location
        {
            get
            {
                var location = this.GetFirst<string>(LocationName);
                if (string.IsNullOrEmpty(location))
                {
                    return null;
                }

                return new Uri(location, UriKind.Absolute);
            }

            set
            {
                this.Add(LocationName, value.AbsoluteUri);
            }
        }

        /// <summary>
        /// Gets or sets the HTTP User-Agent header value.
        /// </summary>
        /// <value>The user agent string.</value>
        public string UserAgent
        {
            get { return this.GetFirst<string>(HeaderUserAgentName); }
            set { this.Add(HeaderUserAgentName, value); }
        }

        /// <summary>
        /// Gets the first value (if any) associated with the <paramref name="key"/>.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="key">The header name to retrieve.</param>
        /// <returns>The value for <paramref name="key"/>, or <c>default(T)</c>.</returns>
        public T GetFirst<T>(string key)
        {
            if (!this.headers.ContainsKey(key) || !this.headers[key].Any())
            {
                return default(T);
            }

            return (T)this.headers[key][0];
        }

        /// <summary>
        /// Adds a new header value to the collection.
        /// </summary>
        /// <param name="key">The header name to add a value for.</param>
        /// <param name="value">The header value.</param>
        public void Add(string key, object value)
        {
            if (this.readOnly)
            {
                throw new InvalidOperationException("This headers collection is read-only.");
            }

            if (!this.headers.ContainsKey(key))
            {
                this.headers.Add(key, new List<object>());
            }

            this.headers[key].Add(value);
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
        {
            foreach (var header in this.headers)
            {
                yield return new KeyValuePair<string, IEnumerable<string>>(
                    header.Key,
                    header.Value.Select(x => x.ToString()).ToList());
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
