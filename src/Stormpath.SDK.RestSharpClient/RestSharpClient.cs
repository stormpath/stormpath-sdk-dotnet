﻿// <copyright file="RestSharpClient.cs" company="Stormpath, Inc.">
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
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using Stormpath.SDK.Logging;

namespace Stormpath.SDK.Extensions.Http.RestSharp
{
    /// <summary>
    /// RestSharp-based HTTP client for Stormpath.SDK.
    /// </summary>
    public sealed class RestSharpClient : SDK.Http.ISynchronousHttpClient, SDK.Http.IAsynchronousHttpClient
    {
        private readonly RestSharpAdapter adapter;
        private readonly string baseUrl;
        private readonly TimeSpan timeout;
        private readonly IWebProxy proxy;
        private readonly ILogger logger;

        private bool alreadyDisposed = false;

        /// <inheritdoc/>
        string SDK.Http.IHttpClient.BaseUrl => this.baseUrl;

        /// <inheritdoc/>
        int SDK.Http.IHttpClient.ConnectionTimeout => (int)this.timeout.TotalMilliseconds;

        /// <inheritdoc/>
        IWebProxy SDK.Http.IHttpClient.Proxy => this.proxy;

        /// <inheritdoc/>
        bool SDK.Http.IHttpClient.IsSynchronousSupported => true;

        /// <inheritdoc/>
        bool SDK.Http.IHttpClient.IsAsynchronousSupported => true;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestSharpClient"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL of the remote API.</param>
        /// <param name="connectionTimeout">The connection timeout (in milliseconds).</param>
        /// <param name="proxy">The proxy, or <see langword="null"/> to connect directly.</param>
        /// <param name="logger">The logger.</param>
        [Obsolete("Don't create clients directly. Use HttpClients.Create().Default() or Create().RestSharpClient() instead.")]
        public RestSharpClient(string baseUrl, int connectionTimeout, IWebProxy proxy, ILogger logger)
            : this(baseUrl, TimeSpan.FromMilliseconds(connectionTimeout), proxy, logger)
        {
        }

        internal RestSharpClient(string baseUrl, TimeSpan timeout, IWebProxy proxy, ILogger logger)
        {
            this.adapter = new RestSharpAdapter();
            this.baseUrl = baseUrl;
            this.timeout = timeout;
            this.proxy = proxy;
            this.logger = logger;
        }

        private IRestClient CreateClientForRequest(SDK.Http.IHttpRequest request)
        {
            var client = new RestClient();

            // Configure default settings
            client.BaseUrl = new Uri(this.baseUrl, UriKind.Absolute);
            client.DefaultParameters.Clear();
            client.Encoding = Encoding.UTF8;
            client.FollowRedirects = false;
            client.Timeout = (this as SDK.Http.IHttpClient).ConnectionTimeout;
            client.UserAgent = request.Headers?.UserAgent;

            if (this.proxy != null)
            {
                client.Proxy = this.proxy;
            }

            return client;
        }

        private static bool IsValidBaseUrl(IRestClient client, SDK.Http.IHttpRequest request)
        {
            return request.CanonicalUri
                .ToString()
                .Contains(client.BaseUrl.ToString());
        }

        /// <inheritdoc/>
        SDK.Http.IHttpResponse SDK.Http.ISynchronousHttpClient.Execute(SDK.Http.IHttpRequest request)
        {
            var client = this.CreateClientForRequest(request);
            if (!IsValidBaseUrl(client, request))
            {
                throw new ApplicationException($"Request URI '{request.CanonicalUri.ToString()}' does not match client base URI '{client.BaseUrl}");
            }

            var restRequest = this.adapter.ToRestRequest(this.baseUrl, request);
            var response = client.Execute(restRequest);

            return this.adapter.ToHttpResponse(response);
        }

        /// <inheritdoc/>
        async Task<SDK.Http.IHttpResponse> SDK.Http.IAsynchronousHttpClient.ExecuteAsync(SDK.Http.IHttpRequest request, CancellationToken cancellationToken)
        {
            var client = this.CreateClientForRequest(request);
            if (!IsValidBaseUrl(client, request))
            {
                throw new ApplicationException($"Request URI '{request.CanonicalUri.ToString()}' does not match client base URI '{client.BaseUrl}");
            }

            var restRequest = this.adapter.ToRestRequest(this.baseUrl, request);
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

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
        }
    }
}
