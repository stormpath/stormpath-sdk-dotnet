// <copyright file="SystemNetHttpClient.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Logging;

namespace Stormpath.SDK.Http.SystemNetHttpClient
{
    public sealed class SystemNetHttpClient : ISynchronousHttpClient, IAsynchronousHttpClient
    {
        private readonly SystemNetHttpAdapter adapter;
        private readonly string baseUrl;
        private readonly int connectionTimeout;
        private readonly IWebProxy proxy;
        private readonly ILogger logger;

        private bool alreadyDisposed = false;

        internal SystemNetHttpClient(string baseUrl, int connectionTimeout, IWebProxy proxy, ILogger logger)
        {
            this.adapter = new SystemNetHttpAdapter(baseUrl);
            this.baseUrl = baseUrl;
            this.connectionTimeout = connectionTimeout;
            this.proxy = proxy;
            this.logger = logger;
        }

        /// <inheritdoc/>
        string IHttpClient.BaseUrl => this.baseUrl;

        /// <inheritdoc/>
        int IHttpClient.ConnectionTimeout => this.connectionTimeout;

        /// <inheritdoc/>
        bool IHttpClient.IsAsynchronousSupported => true;

        /// <inheritdoc/>
        bool IHttpClient.IsSynchronousSupported => true;

        /// <inheritdoc/>
        IWebProxy IHttpClient.Proxy => this.proxy;

        /// <inheritdoc/>
        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }

        /// <inheritdoc/>
        IHttpResponse ISynchronousHttpClient.Execute(IHttpRequest request)
        {
            var httpRequest = this.adapter.ToHttpRequest(request);

            using (var client = this.CreateClientForRequest(request))
            using (var response = client.SendAsync(httpRequest, HttpCompletionOption.ResponseContentRead).Result)
            {
                return this.adapter.ToStormpathResponseAsync(response).Result;
            }
        }

        /// <inheritdoc/>
        async Task<IHttpResponse> IAsynchronousHttpClient.ExecuteAsync(IHttpRequest request, CancellationToken cancellationToken)
        {
            var httpRequest = this.adapter.ToHttpRequest(request);

            try
            {
                using (var client = this.CreateClientForRequest(request))
                using (var response = await client.SendAsync(httpRequest, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false))
                {
                    return await this.adapter.ToStormpathResponseAsync(response).ConfigureAwait(false);
                }
            }
            catch (TaskCanceledException tcx) when (tcx.CancellationToken != cancellationToken)
            {
                return this.adapter.ToStormpathErrorResponse("Timed out");
            }
            catch (Exception ex)
            {
                return this.adapter.ToStormpathErrorResponse(ex.Message);
            }
        }

        private HttpClient CreateClientForRequest(IHttpRequest request)
        {
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
            };

            if (this.proxy != null)
            {
                handler.Proxy = this.proxy;
                handler.UseProxy = true;
            }

            var client = new HttpClient(handler);

            // Configure default settings
            //client.BaseAddress = new Uri(this.baseUrl, UriKind.Absolute);
            client.Timeout = TimeSpan.FromMilliseconds(this.connectionTimeout);

            return client;
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
    }
}
