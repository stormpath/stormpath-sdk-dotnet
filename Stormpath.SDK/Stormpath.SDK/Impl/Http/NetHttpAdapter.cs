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
using System.Collections.Generic;

namespace Stormpath.SDK.Impl.Http
{
    internal sealed class NetHttpAdapter
    {
        private static Dictionary<Support.HttpMethod, System.Net.Http.HttpMethod> httpMethodLookup = new Dictionary<Support.HttpMethod, System.Net.Http.HttpMethod>()
        {
            { Support.HttpMethod.Get, System.Net.Http.HttpMethod.Get },
            { Support.HttpMethod.Head, System.Net.Http.HttpMethod.Head },
            { Support.HttpMethod.Post, System.Net.Http.HttpMethod.Post },
            { Support.HttpMethod.Put, System.Net.Http.HttpMethod.Put },
            { Support.HttpMethod.Delete, System.Net.Http.HttpMethod.Delete },
            { Support.HttpMethod.Options, System.Net.Http.HttpMethod.Options },
            { Support.HttpMethod.Trace, System.Net.Http.HttpMethod.Trace },
        };

        public System.Net.Http.HttpRequestMessage ToHttpRequestMessage(IHttpRequest request)
        {
            System.Net.Http.HttpMethod httpMethod = null;
            if (!httpMethodLookup.TryGetValue(request.Method, out httpMethod))
                throw new ArgumentException($"Unknown method type {request.Method.DisplayName}", nameof(request));

            var httpRequestMessage = new System.Net.Http.HttpRequestMessage(httpMethod, request.CanonicalUri.ToString());
            CopyHttpHeaders(request.Headers, httpRequestMessage.Headers);

            return httpRequestMessage;
        }

        public void CopyHttpHeaders(HttpHeaders source, System.Net.Http.Headers.HttpHeaders destination)
        {
            foreach (var header in source)
            {
                if (!destination.TryAddWithoutValidation(header.Key, header.Value))
                    throw new ArgumentException($"Header '{header.Key}' or contained values '{string.Join("','", header.Value)}' is not valid", nameof(source));
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
    }
}
