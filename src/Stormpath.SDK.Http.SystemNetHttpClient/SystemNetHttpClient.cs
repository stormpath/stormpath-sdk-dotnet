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
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Logging;

namespace Stormpath.SDK.Http.SystemNetHttpClient
{
    public sealed class SystemNetHttpClient : ISynchronousHttpClient, IAsynchronousHttpClient
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl;
        private readonly TimeSpan _timeout;
        private readonly IWebProxy _proxy;
        private readonly ILogger _logger;

        private bool _alreadyDisposed;

        [Obsolete("Use ctor with TimeSpan")]
        internal SystemNetHttpClient(string baseUrl, int connectionTimeout, IWebProxy proxy, ILogger logger)
            : this(baseUrl, TimeSpan.FromMilliseconds(connectionTimeout), proxy, logger)
        {
        }

        internal SystemNetHttpClient(string baseUrl, TimeSpan timeout, IWebProxy proxy, ILogger logger)
        {
            _baseUrl = baseUrl;
            _timeout = timeout;
            _proxy = proxy;
            _logger = logger;

            // Create the HttpClient. The instance is reused, per the
            // best practices recommendations for HttpClient.
            // See http://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
            _client = CreateClient();
        }

        private HttpClient CreateClient()
        {
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,

            };

            if (_proxy != null)
            {
                handler.Proxy = _proxy;
                handler.UseProxy = true;
            }

            var client = new HttpClient(handler)
            {
                Timeout = _timeout
            };

            return client;
        }

        /// <inheritdoc/>
        string IHttpClient.BaseUrl => _baseUrl;

        /// <inheritdoc/>
        int IHttpClient.ConnectionTimeout => (int)_timeout.TotalMilliseconds;

        /// <inheritdoc/>
        bool IHttpClient.IsAsynchronousSupported => true;

        /// <inheritdoc/>
        bool IHttpClient.IsSynchronousSupported => true;

        /// <inheritdoc/>
        IWebProxy IHttpClient.Proxy => _proxy;

        /// <inheritdoc/>
        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        /// <inheritdoc/>
        IHttpResponse ISynchronousHttpClient.Execute(IHttpRequest request)
        {
            if (_alreadyDisposed)
            {
                throw new ObjectDisposedException(nameof(SystemNetHttpClient));
            }

            var adapter = new SystemNetHttpAdapter(_baseUrl);
            var httpRequest = adapter.ToHttpRequest(request);

            using (var response = _client.SendAsync(httpRequest, HttpCompletionOption.ResponseContentRead).Result)
            {
                return adapter.ToStormpathResponseAsync(response).Result;
            }
        }

        /// <inheritdoc/>
        async Task<IHttpResponse> IAsynchronousHttpClient.ExecuteAsync(IHttpRequest request, CancellationToken cancellationToken)
        {
            if (_alreadyDisposed)
            {
                throw new ObjectDisposedException(nameof(SystemNetHttpClient));
            }

            var adapter = new SystemNetHttpAdapter(_baseUrl);
            var httpRequest = adapter.ToHttpRequest(request);

            try
            {
                using (var response = await _client.SendAsync(httpRequest, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false))
                {
                    return await adapter.ToStormpathResponseAsync(response).ConfigureAwait(false);
                }
            }
            catch (TaskCanceledException tcx) when (tcx.CancellationToken != cancellationToken)
            {
                return adapter.ToStormpathErrorResponse("Timed out");
            }
            catch (Exception ex)
            {
                return adapter.ToStormpathErrorResponse(ex.Message);
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_alreadyDisposed)
            {
                if (disposing)
                {
                    _client.Dispose();
                }

                _alreadyDisposed = true;
            }
        }
    }
}
