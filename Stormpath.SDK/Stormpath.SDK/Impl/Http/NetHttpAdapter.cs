// <copyright file="NetHttpAdapter.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Impl.Http
{
    internal sealed class NetHttpAdapter
    {
        public System.Net.Http.HttpRequestMessage ToHttpRequestMessage(IHttpRequest request)
        {
            System.Net.Http.HttpMethod httpMethod = null;
            if (!TryConvertHttpMethod(request.Method, out httpMethod))
                throw new ArgumentException($"Unknown method type {request.Method.DisplayName}", nameof(request));

            var httpRequestMessage = new System.Net.Http.HttpRequestMessage(httpMethod, request.CanonicalUri.ToString());

            if (request.HasBody)
                httpRequestMessage.Content = new System.Net.Http.StringContent(request.Body, System.Text.Encoding.UTF8, request.BodyContentType);

            this.CopyHeadersToRequestMessage(request.Headers, httpRequestMessage);

            return httpRequestMessage;
        }

        public void CopyHeadersToRequestMessage(HttpHeaders source, System.Net.Http.HttpRequestMessage destination)
        {
            foreach (var header in source)
            {
                if (!destination.Headers.TryAddWithoutValidation(header.Key, header.Value))
                    throw new ArgumentException($"Header '{header.Key}' or contained value(s) '{string.Join("','", header.Value)}' is not valid", nameof(source));
            }
        }

        public HttpHeaders ToHttpHeaders(System.Net.Http.Headers.HttpHeaders httpClientHeaders)
        {
            var result = new HttpHeaders();

            foreach (var header in httpClientHeaders)
            {
                var name = header.Key;
                foreach (var value in header.Value)
                {
                    result.Add(name, value);
                }
            }

            return result;
        }

        private static bool TryConvertHttpMethod(Support.HttpMethod abstractMethod, out System.Net.Http.HttpMethod netHttpMethod)
        {
            bool found = true;
            switch (abstractMethod.DisplayName.ToUpper())
            {
                case "GET":
                    netHttpMethod = System.Net.Http.HttpMethod.Get;
                    break;
                case "HEAD":
                    netHttpMethod = System.Net.Http.HttpMethod.Head;
                    break;
                case "POST":
                    netHttpMethod = System.Net.Http.HttpMethod.Post;
                    break;
                case "PUT":
                    netHttpMethod = System.Net.Http.HttpMethod.Put;
                    break;
                case "DELETE":
                    netHttpMethod = System.Net.Http.HttpMethod.Delete;
                    break;
                case "OPTIONS":
                    netHttpMethod = System.Net.Http.HttpMethod.Options;
                    break;
                case "TRACE":
                    netHttpMethod = System.Net.Http.HttpMethod.Trace;
                    break;
                default:
                    netHttpMethod = null;
                    found = false;
                    break;
            }

            return found;
        }
    }
}
