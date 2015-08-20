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
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Api;
using Stormpath.SDK.Client;
using Stormpath.SDK.Impl.Http.Authentication;

namespace Stormpath.SDK.Impl.Http
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:DoNotUseRegions", Justification = "Reviewed.")]
    internal sealed class NetHttpRequestExecutor : IRequestExecutor
    {
        private readonly IClientApiKey apiKey;
        private readonly AuthenticationScheme authScheme;
        private readonly int connectionTimeout;

        private readonly IRequestAuthenticator requestAuthenticator;
        private HttpClient client;

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

            SetupClient();
        }

        AuthenticationScheme IRequestExecutor.AuthenticationScheme => authScheme;

        int IRequestExecutor.ConnectionTimeout => connectionTimeout;

        private void SetupClient()
        {
            var clientSettings = new HttpClientHandler()
            {
                AllowAutoRedirect = false,

                // Proxy...
            };

            client = new HttpClient(clientSettings);
            client.Timeout = TimeSpan.FromMilliseconds(connectionTimeout);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", UserAgentBuilder.GetUserAgent());
        }

        async Task<string> IRequestExecutor.GetAsync(Uri href, CancellationToken cancellationToken)
        {
            Uri currentUri = href;

            while (true)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, currentUri);
                requestAuthenticator.Authenticate(request, apiKey);
                var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (IsRedirect(response))
                {
                    currentUri = response.Headers.Location;
                    continue;
                }

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    throw new RequestException($"Unable to execute HTTP request. Message: '{content}'");
                }
            }
        }

        string IRequestExecutor.GetSync(Uri href)
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
