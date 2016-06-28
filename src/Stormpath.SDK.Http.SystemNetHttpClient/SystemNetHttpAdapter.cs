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
using System.Threading.Tasks;

namespace Stormpath.SDK.Http.SystemNetHttpClient
{
    /// <summary>
    /// Translates requests and responses to Stormpath SDK interface types.
    /// </summary>
    public class SystemNetHttpAdapter
    {
        private readonly string baseUrl;

        public SystemNetHttpAdapter(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public HttpRequestMessage ToHttpRequest(IHttpRequest stormpathRequest)
        {
            var method = this.ToHttpMethod(stormpathRequest.Method);

            var httpRequest = new HttpRequestMessage(method, stormpathRequest.CanonicalUri.ToUri());

            if (stormpathRequest.HasBody)
            {
                httpRequest.Content = new StringContent(stormpathRequest.Body, Encoding.UTF8);
            }

            this.ToHttpHeaders(stormpathRequest.Headers, httpRequest);

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
                    content.Headers.ContentType?.MediaType,
                    transportError);
            }
        }

        public IHttpResponse ToStormpathErrorResponse(string errorMessage)
        {
            return new SystemNetHttpResponse(
                statusCode: 599,
                responseMessage: errorMessage,
                headers: null,
                body: null,
                contentType: null,
                transportError: true);
        }

        private void ToHttpHeaders(HttpHeaders stormpathHeaders, HttpRequestMessage httpRequest)
        {
            if (stormpathHeaders == null)
            {
                return;
            }

            // Set Content-Type manually.
            // This is necessary because new StringContent(...) will add
            // a Content-Type like "application/json; charset=utf-8", which messes up SAuthc1
            if (httpRequest.Content != null)
            {
                httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue(stormpathHeaders.ContentType);
            }

            var ignoredHeaders = new string[]
            {
                HttpHeaders.HeaderContentTypeName, // handled above
                HttpHeaders.HeaderContentLengthName // handled automatically
            };

            var headersToCopy = stormpathHeaders
                .Where(h => !ignoredHeaders.Contains(h.Key));

            foreach (var header in headersToCopy)
            {
                bool added = httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                if (!added)
                {
                    throw new Exception($"Could not add header with key '{header.Key}' and value '{string.Join(",", header.Value)}'");
                }
            }
        }

        private HttpHeaders ToStormpathHeaders(HttpResponseHeaders responseHeaders, HttpContentHeaders contentHeaders)
        {
            var result = new HttpHeaders();

            foreach (var header in responseHeaders)
            {
                result.Add(header.Key, header.Value.FirstOrDefault());
            }

            foreach (var header in contentHeaders)
            {
                result.Add(header.Key, header.Value.FirstOrDefault());
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
