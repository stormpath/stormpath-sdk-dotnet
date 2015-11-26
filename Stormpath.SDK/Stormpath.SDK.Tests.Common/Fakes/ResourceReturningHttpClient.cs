// <copyright file="ResourceReturningHttpClient.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using Stormpath.SDK.Http;

namespace Stormpath.SDK.Tests.Common.Fakes
{
    public sealed class ResourceReturningHttpClient : AbstractMockHttpClient
    {
        private static readonly List<HttpMethod> SupportedMethods =
            new List<HttpMethod>() { HttpMethod.Delete, HttpMethod.Get, HttpMethod.Post, HttpMethod.Put };

        private readonly string defaultResponse;
        private readonly IDictionary<string, IHttpResponse> responses;

        public ResourceReturningHttpClient(string baseUrl, string defaultResponse = null)
            : base(baseUrl)
        {
            this.defaultResponse = defaultResponse;
            this.responses = new Dictionary<string, IHttpResponse>();
        }

        public ResourceReturningHttpClient SetupGet(string resource, int responseCode, string responseBody, string responseBodyContentType = null)
        {
            var key = CreateKey(HttpMethod.Get, MergeWithBaseUrl(this.baseUrl, resource));
            this.responses[key] = new FakeHttpResponse(responseCode, responseBody, responseBodyContentType);

            return this;
        }

        protected override bool IsSupported(HttpMethod method)
            => SupportedMethods.Contains(method);

        protected override IHttpResponse GetResponse(IHttpRequest request)
        {
            var key = CreateKey(request.Method, request.CanonicalUri.ToString());
            IHttpResponse response = null;

            if (this.responses.TryGetValue(key, out response))
                return response;

            return this.DefaultResponse();
        }

        private IHttpResponse DefaultResponse()
        {
            if (string.IsNullOrEmpty(this.defaultResponse))
                throw new ArgumentException("No default response set up in FakeHttpClient.");

            return new FakeHttpResponse(200, this.defaultResponse);
        }

        private static string CreateKey(HttpMethod method, string url)
            => $"{method} {url}";
    }
}
