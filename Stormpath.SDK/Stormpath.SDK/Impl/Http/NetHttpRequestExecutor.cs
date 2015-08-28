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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:DoNotUseRegions", Justification = "Reviewed.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements must appear in the correct order", Justification = "Reviewed.")]
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
            requestAuthenticator = requestAuthenticatorFactory.Create(authenticationScheme);
            httpAdapter = new NetHttpAdapter();
            client = BuildClient(connectionTimeout);
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

        AuthenticationScheme IRequestExecutor.AuthenticationScheme => authScheme;

        int IRequestExecutor.ConnectionTimeout => connectionTimeout;

        async Task<IHttpResponse> IRequestExecutor.ExecuteAsync(IHttpRequest request, CancellationToken cancellationToken)
        {
            Uri currentUri = request.CanonicalUri.ToUri();

            while (true)
            {
                var currentRequest = new DefaultHttpRequest(request, overrideUri: currentUri);
                requestAuthenticator.Authenticate(currentRequest, apiKey);

                var requestMessage = httpAdapter.ToHttpRequestMessage(currentRequest);
                var response = await client.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
                if (IsRedirect(response))
                {
                    currentUri = response.Headers.Location;
                    continue;
                }

                var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var headers = httpAdapter.ToHttpHeaders(response.Headers);
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

        #region IDisposable implementation

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    client.Dispose();
                }

                disposed = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
