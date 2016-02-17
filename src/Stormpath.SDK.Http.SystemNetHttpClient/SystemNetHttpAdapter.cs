// <copyright file="RestSharpClient.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Stormpath.SDK.Http.SystemNetHttpClient
{
    public class SystemNetHttpAdapter
    {
        private readonly string baseUrl;

        public SystemNetHttpAdapter(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public HttpRequestMessage ToHttpRequest(IHttpRequest stormpathRequest)
        {
            var resourcePath = stormpathRequest.CanonicalUri.ToString().Replace(baseUrl, string.Empty);
            var method = this.ToHttpMethod(stormpathRequest.Method);

            var httpRequest = new HttpRequestMessage(method, resourcePath);
            this.ToHttpHeaders(stormpathRequest.Headers, httpRequest);

            if (stormpathRequest.HasBody)
            {
                httpRequest.Content = new StringContent(stormpathRequest.Body, Encoding.UTF8, stormpathRequest.BodyContentType);
            }

            return httpRequest;
        }

        public async Task<IHttpResponse> ToStormpathResponseAsync(HttpResponseMessage httpResponse)
        {
            bool transportError = false;

            using (var content = httpResponse.Content)
            {
                var stringContent = await content.ReadAsStringAsync().ConfigureAwait(false);

                var headers = this.ToStormpathHeaders(httpResponse.Headers, content.Headers);

                return new SystemNetHttpResponse(
                    (int)httpResponse.StatusCode,
                    httpResponse.ReasonPhrase,
                    headers,
                    stringContent,
                    content.Headers.ContentType.MediaType,
                    transportError);
            }
        }

        private void ToHttpHeaders(HttpHeaders stormpathHeaders, HttpRequestMessage httpRequest)
        {
            if (stormpathHeaders == null)
            {
                return;
            }

            foreach (var header in stormpathHeaders)
            {
                httpRequest.Headers.Add(header.Key, header.Value);
            }
        }

        private HttpHeaders ToStormpathHeaders(HttpResponseHeaders responseHeaders, HttpContentHeaders contentHeaders)
        {
            var result = new HttpHeaders();

            foreach (var header in responseHeaders.Concat(contentHeaders))
            {
                result.Add(header.Key, header.Value.ToList());
            }

            return result;
        }

        private System.Net.Http.HttpMethod ToHttpMethod(HttpMethod httpMethod)
        {
            if (httpMethod == HttpMethod.Delete)
            {
                return System.Net.Http.HttpMethod.Delete;
            }

            if (httpMethod == HttpMethod.Get)
            {
                return System.Net.Http.HttpMethod.Get;
            }

            if (httpMethod == HttpMethod.Head)
            {
                return System.Net.Http.HttpMethod.Head;
            }

            if (httpMethod == HttpMethod.Options)
            {
                return System.Net.Http.HttpMethod.Options;
            }

            if (httpMethod == HttpMethod.Post)
            {
                return System.Net.Http.HttpMethod.Post;
            }

            if (httpMethod == HttpMethod.Put)
            {
                return System.Net.Http.HttpMethod.Put;
            }

            throw new ArgumentException($"Unknown method type {httpMethod}", nameof(httpMethod));
        }

    }
}
