// <copyright file="DefaultHttpRequestBuilder.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Http;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.Http
{
    internal sealed class DefaultHttpRequestBuilder : IHttpRequestBuilder
    {
        private HttpMethod method;
        private CanonicalUri uri;
        private Map headers;
        private string body;
        private string bodyContentType;

        IHttpRequestBuilder IHttpRequestBuilder.WithBody(string body)
        {
            this.body = body;
            return this;
        }

        IHttpRequestBuilder IHttpRequestBuilder.WithBodyContentType(string bodyContentType)
        {
            this.bodyContentType = bodyContentType;
            return this;
        }

        IHttpRequestBuilder IHttpRequestBuilder.WithHeaders(Map headers)
        {
            this.headers = headers;
            return this;
        }

        IHttpRequestBuilder IHttpRequestBuilder.WithMethod(HttpMethod method)
        {
            this.method = method;
            return this;
        }

        IHttpRequestBuilder IHttpRequestBuilder.WithMethod(string methodName)
        {
            this.method = HttpMethod.Parse(methodName);
            return this;
        }

        IHttpRequestBuilder IHttpRequestBuilder.WithUri(Uri uri)
        {
            this.uri = new CanonicalUri(uri.ToString());
            return this;
        }

        IHttpRequestBuilder IHttpRequestBuilder.WithUri(string uri)
        {
            this.uri = new CanonicalUri(uri);
            return this;
        }

        IHttpRequest IHttpRequestBuilder.Build()
        {
            this.ThrowIfNotEnoughData();

            var httpHeaders = this.headers == null
                ? null
                : new HttpHeaders(this.headers);

            return new DefaultHttpRequest(this.method, this.uri, null, httpHeaders, this.body, this.bodyContentType);
        }

        private void ThrowIfNotEnoughData()
        {
            if (this.method == null || this.uri == null)
            {
                throw new ArgumentNullException("The HTTP method and URI must be specified when building an HTTP request descriptor.");
            }
        }
    }
}
