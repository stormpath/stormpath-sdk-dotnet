// <copyright file="NetHttpRequestExecutor.cs" company="Stormpath, Inc.">
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
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Api;
using Stormpath.SDK.Client;
using Stormpath.SDK.Impl.Http.Authentication;

namespace Stormpath.SDK.Impl.Http
{
    internal sealed class NetHttpRequestExecutor : IRequestExecutor
    {
        private readonly IClientApiKey apiKey;
        private readonly AuthenticationScheme authScheme;
        private readonly int connectionTimeout;
        private readonly IRequestAuthenticator requestAuthenticator;
        private readonly NetHttpAdapter httpAdapter;
        private readonly HttpClient client;

        private bool disposed = false; // To detect redundant calls

        public NetHttpRequestExecutor(IClientApiKey apiKey, AuthenticationScheme authenticationScheme, int connectionTimeout)
        {
            if (!apiKey.IsValid())
                throw new ApplicationException("API Key is invalid.");

            this.apiKey = apiKey;
            this.authScheme = authenticationScheme;
            this.connectionTimeout = connectionTimeout;

            IRequestAuthenticatorFactory requestAuthenticatorFactory = new DefaultRequestAuthenticatorFactory();
            this.requestAuthenticator = requestAuthenticatorFactory.Create(authenticationScheme);
            this.httpAdapter = new NetHttpAdapter();
            this.client = BuildClient(connectionTimeout);
        }

        private static HttpClient BuildClient(int connectionTimeout)
        {
            var clientSettings = new HttpClientHandler()
            {
                AllowAutoRedirect = false,

                // Proxy...
            };

            var client = new HttpClient(clientSettings);
            client.Timeout = TimeSpan.FromMilliseconds(connectionTimeout);

            return client;
        }

        AuthenticationScheme IRequestExecutor.AuthenticationScheme => this.authScheme;

        int IRequestExecutor.ConnectionTimeout => this.connectionTimeout;

        async Task<IHttpResponse> IRequestExecutor.ExecuteAsync(IHttpRequest request, CancellationToken cancellationToken)
        {
            Uri currentUri = request.CanonicalUri.ToUri();

            while (true)
            {
                var currentRequest = new DefaultHttpRequest(request, overrideUri: currentUri);
                this.requestAuthenticator.Authenticate(currentRequest, this.apiKey);

                var requestMessage = this.httpAdapter.ToHttpRequestMessage(currentRequest);
                var response = await this.client.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
                if (this.IsRedirect(response))
                {
                    currentUri = response.Headers.Location;
                    continue;
                }

                var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var headers = this.httpAdapter.ToHttpHeaders(response.Headers);
                var returnedResponse = new DefaultHttpResponse((int)response.StatusCode, headers, body, response.Content?.Headers?.ContentType?.MediaType);
                return returnedResponse;
            }
        }

        IHttpResponse IRequestExecutor.ExecuteSync(IHttpRequest request)
        {
            throw new NotImplementedException();
        }

        private bool IsRedirect(HttpResponseMessage response)
        {
            bool moved =
                response.StatusCode == HttpStatusCode.MovedPermanently || // 301
                response.StatusCode == HttpStatusCode.Redirect || // 302
                response.StatusCode == HttpStatusCode.TemporaryRedirect; // 307
            bool hasNewLocation = !string.IsNullOrEmpty(response.Headers.Location?.AbsoluteUri);

            return moved && hasNewLocation;
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.client.Dispose();
                }

                this.disposed = true;
            }
        }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }
    }
}
