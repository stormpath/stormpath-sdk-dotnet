// <copyright file="RestSharpClient.cs" company="Stormpath, Inc.">
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Extensions.Http
{
    public sealed class RestSharpClient : SDK.Http.ISynchronousHttpClient, SDK.Http.IAsynchronousHttpClient
    {
        private readonly RestSharpAdapter adapter;
        private readonly int connectionTimeout;
        private readonly ILogger logger;

        private bool alreadyDisposed = false;

        bool SDK.Http.IHttpClient.IsSynchronousSupported => false; // TODO

        bool SDK.Http.IHttpClient.IsAsynchronousSupported => false; // TODO

        public RestSharpClient()
            : this(0, null)
        {
        }

        public RestSharpClient(int connectionTimeout, ILogger logger)
        {
            this.adapter = new RestSharpAdapter();
            this.connectionTimeout = connectionTimeout;
            this.logger = logger;
        }

        private RestSharp.IRestClient GetClient()
        {
            var client = new RestSharp.RestClient();

            // Configure default settings
            client.Encoding = Encoding.UTF8;
            client.FollowRedirects = false;
            client.Timeout = this.connectionTimeout;

            return client;
        }

        SDK.Http.IHttpResponse SDK.Http.ISynchronousHttpClient.Execute(SDK.Http.IHttpRequest request)
        {
            var client = this.GetClient();
            var restRequest = this.adapter.ToRestRequest(request);

            var response = client.Execute(restRequest);

            return this.adapter.ToHttpResponse(response);
        }

        async Task<SDK.Http.IHttpResponse> SDK.Http.IAsynchronousHttpClient.ExecuteAsync(SDK.Http.IHttpRequest request, CancellationToken cancellationToken)
        {
            var client = this.GetClient();
            var restRequest = this.adapter.ToRestRequest(request);

            var response = await client.ExecuteTaskAsync(restRequest, cancellationToken).ConfigureAwait(false);

            return this.adapter.ToHttpResponse(response);
        }

        private void Dispose(bool disposing)
        {
            if (!this.alreadyDisposed)
            {
                if (disposing)
                {
                    // Currently there's nothing to dispose
                    // noop.
                }

                this.alreadyDisposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }
    }
}
