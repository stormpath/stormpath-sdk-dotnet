// <copyright file="DefaultHttpRequest.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Http;

namespace Stormpath.SDK.Impl.Http
{
    internal sealed class DefaultHttpRequest : HttpMessageBase, IHttpRequest
    {
        private readonly HttpMethod method;
        private readonly CanonicalUri canonicalUri;
        private readonly HttpHeaders headers;
        private readonly string body;
        private readonly string bodyContentType;

        // Copy constructor
        public DefaultHttpRequest(IHttpRequest existingRequest, Uri overrideUri = null)
        {
            this.body = existingRequest.Body;
            this.bodyContentType = existingRequest.BodyContentType;
            this.headers = new HttpHeaders(existingRequest.Headers);
            this.method = existingRequest.Method.Clone();
            this.canonicalUri = new CanonicalUri(existingRequest.CanonicalUri, overrideResourcePath: overrideUri);
        }

        public DefaultHttpRequest(HttpMethod method, CanonicalUri canonicalUri)
            : this(method, canonicalUri, null, null, null, null)
        {
        }

        public DefaultHttpRequest(HttpMethod method, CanonicalUri canonicalUri, QueryString queryParams, HttpHeaders headers, string body, string bodyContentType)
        {
            this.method = method;
            this.canonicalUri = canonicalUri;

            bool queryParamsWerePassed = queryParams?.Any() ?? false;
            if (queryParamsWerePassed)
            {
                var mergedQueryString = this.canonicalUri.QueryString.Merge(queryParams);
                this.canonicalUri = new CanonicalUri(this.canonicalUri.ResourcePath.ToString(), mergedQueryString);
            }

            this.headers = headers;
            if (headers == null)
                this.headers = new HttpHeaders();

            this.body = body;
            this.bodyContentType = bodyContentType;
        }

        public override string Body => this.body;

        public override string BodyContentType => this.bodyContentType;

        public override HttpHeaders Headers => this.headers;

        public HttpMethod Method => this.method;

        public CanonicalUri CanonicalUri => this.canonicalUri;
    }
}
