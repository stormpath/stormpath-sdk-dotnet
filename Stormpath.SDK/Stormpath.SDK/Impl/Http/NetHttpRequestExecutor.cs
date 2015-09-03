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
using Stormpath.SDK.Impl.Http.Support;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.Http
{
    internal sealed class NetHttpRequestExecutor : IRequestExecutor
    {
        private static readonly int MaxBackoffMilliseconds = 20 * 1000;
        private static readonly int DefaultMaxRetries = 4;

        private readonly ILogger logger;

        private readonly IClientApiKey apiKey;
        private readonly AuthenticationScheme authScheme;
        private readonly int connectionTimeout;

        private readonly IRequestAuthenticator requestAuthenticator;
        private readonly NetHttpAdapter httpAdapter;
        private readonly HttpClient client;

        private readonly IBackoffStrategy defaultBackoffStrategy;
        private readonly IBackoffStrategy throttlingBackoffStrategy;
        private readonly int maxRetriesPerRequest = DefaultMaxRetries;

        private bool disposed = false; // To detect redundant calls

        public NetHttpRequestExecutor(IClientApiKey apiKey, AuthenticationScheme authenticationScheme, int connectionTimeout, ILogger logger)
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

            this.defaultBackoffStrategy = new DefaultBackoffStrategy(MaxBackoffMilliseconds);
            this.throttlingBackoffStrategy = new ThrottlingBackoffStrategy(MaxBackoffMilliseconds);

            this.logger = logger;
        }

        private static HttpClient BuildClient(int connectionTimeout)
        {
            var clientSettings = new HttpClientHandler()
            {
                AllowAutoRedirect = false,

                // TODO Proxy...
            };

            var client = new HttpClient(clientSettings);
            client.Timeout = TimeSpan.FromMilliseconds(connectionTimeout);

            return client;
        }

        AuthenticationScheme IRequestExecutor.AuthenticationScheme => this.authScheme;

        int IRequestExecutor.ConnectionTimeout => this.connectionTimeout;

        async Task<IHttpResponse> IRequestExecutor.ExecuteAsync(IHttpRequest request, CancellationToken cancellationToken)
        {
            var retryCount = 0;
            bool throttling = false;

            Uri currentUri = request.CanonicalUri.ToUri();

            while (true)
            {
                var currentRequest = new DefaultHttpRequest(request, overrideUri: currentUri);

                // Sign and build request
                this.requestAuthenticator.Authenticate(currentRequest, this.apiKey);
                var requestMessage = this.httpAdapter.ToHttpRequestMessage(currentRequest);

                try
                {
                    if (retryCount > 0)
                        await this.PauseAsync(retryCount, throttling, cancellationToken).ConfigureAwait(false);

                    retryCount++;

                    var response = await this.client.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
                    if (this.IsRedirect(response))
                    {
                        currentUri = response.Headers.Location;
                        this.logger.Trace($"Redirected to {currentUri}", "NetHttpRequestExecutor.ExecuteAsync");

                        continue;
                    }

                    var statusCode = (int)response.StatusCode;
                    if (statusCode == 429)
                    {
                        throttling = true;
                        this.logger.Warn($"Got HTTP 429, throttling retry request", "NetHttpRequestExecutor.ExecuteAsync");

                        continue; // retry request
                    }

                    if ((statusCode == 503 || statusCode == 504) && retryCount <= this.maxRetriesPerRequest)
                    {
                        this.logger.Warn($"Got HTTP {statusCode}, retrying", "NetHttpRequestExecutor.ExecuteAsync");

                        continue; // retry request
                    }

                    var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var headers = this.httpAdapter.ToHttpHeaders(response.Headers);
                    var returnedResponse = new DefaultHttpResponse(statusCode, response.ReasonPhrase, headers, body, response.Content?.Headers?.ContentType?.MediaType);
                    return returnedResponse;
                }
                catch (Exception ex)
                {
                    if (this.ShouldRetryForError(requestMessage, ex, cancellationToken, retryCount))
                    {
                        this.logger.Warn(ex, "Error during request, retrying", "NetHttpRequestExecutor.ExecuteAsync");
                    }
                    else
                    {
                        throw new RequestException("Unable to execute HTTP request.", ex);
                    }
                }
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

        private Task PauseAsync(int retryCount, bool isThrottling, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var delayMilliseconds = 0;

            if (isThrottling)
                delayMilliseconds = this.throttlingBackoffStrategy.GetDelayMilliseconds(retryCount);
            else
                delayMilliseconds = this.defaultBackoffStrategy.GetDelayMilliseconds(retryCount);
            this.logger.Trace($"Pausing for {delayMilliseconds}", "NetHttpRequestExecutor.PauseAsync");

            return Task.Delay(delayMilliseconds);
        }

        private bool ShouldRetryForError(HttpRequestMessage request, Exception exception, CancellationToken cancellationToken, int currentRetries)
        {
            if (currentRetries > this.maxRetriesPerRequest)
                return false;

            var asTaskCanceledException = exception as TaskCanceledException;
            if (asTaskCanceledException != null)
            {
                bool wasCanceledByUser = asTaskCanceledException.CancellationToken == cancellationToken;
                return !wasCanceledByUser;
            }

            var webException = exception.InnerException as WebException;
            if (webException != null)
            {
                if (webException.Status == WebExceptionStatus.ConnectFailure ||
                    webException.Status == WebExceptionStatus.ConnectionClosed ||
                    webException.Status == WebExceptionStatus.RequestCanceled ||
                    webException.Status == WebExceptionStatus.Timeout)
                    return true;
            }

            return false;
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
