// <copyright file="DefaultRequestExecutor.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Api;
using Stormpath.SDK.Client;
using Stormpath.SDK.Http;
using Stormpath.SDK.Shared.Extensions;
using Stormpath.SDK.Impl.Http.Authentication;
using Stormpath.SDK.Impl.Http.Support;
using Stormpath.SDK.Logging;

namespace Stormpath.SDK.Impl.Http
{
    internal sealed class DefaultRequestExecutor : IRequestExecutor
    {
        private const int MovedPermanently = 301;
        private const int Redirect = 302;
        private const int TemporaryRedirect = 307;
        private const int Conflict = 409;
        private const int TooManyRequests = 429;
        private const int ServerUnavailable = 503;
        private const int NoGatewayResponse = 504;

        private static readonly int MaxBackoffMilliseconds = 20 * 1000;
        private static readonly int DefaultMaxRetries = 4;
        private static readonly Task CompletedTask = Task.FromResult(false);

        private readonly IHttpClient httpClient;
        private readonly ISynchronousHttpClient syncHttpClient;
        private readonly IAsynchronousHttpClient asyncHttpClient;

        private readonly IClientApiKey apiKey;
        private readonly AuthenticationScheme authenticationScheme;
        private readonly ILogger logger;

        private readonly IRequestAuthenticator requestAuthenticator;
        private readonly IBackoffStrategy defaultBackoffStrategy;
        private readonly IBackoffStrategy throttlingBackoffStrategy;
        private readonly int maxAttemptsPerRequest = DefaultMaxRetries + 1;

        private bool alreadyDisposed = false;

        public DefaultRequestExecutor(
            IHttpClient httpClient,
            IClientApiKey apiKey,
            AuthenticationScheme authenticationScheme,
            ILogger logger)
            : this(httpClient, apiKey, authenticationScheme, logger, new DefaultBackoffStrategy(MaxBackoffMilliseconds), new ThrottlingBackoffStrategy(MaxBackoffMilliseconds))
        {
        }

        public DefaultRequestExecutor(
            IHttpClient httpClient,
            IClientApiKey apiKey,
            AuthenticationScheme authenticationScheme,
            ILogger logger,
            IBackoffStrategy defaultBackoffStrategy,
            IBackoffStrategy throttlingBackoffStrategy)
        {
            if (!apiKey.IsValid())
            {
                throw new Exception("API Key is invalid.");
            }

            this.httpClient = httpClient;
            this.syncHttpClient = httpClient as ISynchronousHttpClient;
            this.asyncHttpClient = httpClient as IAsynchronousHttpClient;

            this.apiKey = apiKey;
            this.authenticationScheme = authenticationScheme;

            IRequestAuthenticatorFactory requestAuthenticatorFactory = new DefaultRequestAuthenticatorFactory();
            this.requestAuthenticator = requestAuthenticatorFactory.Create(authenticationScheme);

            this.logger = logger;
            this.defaultBackoffStrategy = defaultBackoffStrategy;
            this.throttlingBackoffStrategy = throttlingBackoffStrategy;
        }

        IClientApiKey IRequestExecutor.ApiKey => this.apiKey;

        Task<IHttpResponse> IRequestExecutor.ExecuteAsync(IHttpRequest request, CancellationToken cancellationToken)
        {
            if (!this.httpClient.IsAsynchronousSupported || this.asyncHttpClient == null)
            {
                throw new Exception("This HTTP client does not support asynchronous requests.");
            }

            return this.CoreRequestLoopAsync(
                request,
                (IHttpRequest req, CancellationToken ct) => this.asyncHttpClient.ExecuteAsync(req, ct),
                (int count, bool throttle, CancellationToken ct) => this.PauseAsync(count, throttle, ct),
                cancellationToken);
        }

        IHttpResponse IRequestExecutor.Execute(IHttpRequest request)
        {
            if (!this.httpClient.IsSynchronousSupported || this.syncHttpClient == null)
            {
                throw new Exception("This HTTP client does not support synchronous requests.");
            }

            // We know what we're doing here, even though this looks like async-over-sync.
            // The synchronous path will only ever create completed tasks, so awaiting
            // these tasks will continue to execute synchronously.
            return this.CoreRequestLoopAsync(
                request,
                (IHttpRequest req, CancellationToken _) =>
                {
                    return Task.FromResult(this.syncHttpClient.Execute(req));
                },
                (int count, bool throttle, CancellationToken _) =>
                {
                    this.PauseSync(count, throttle);
                    return CompletedTask;
                },
                CancellationToken.None).GetAwaiter().GetResult();
        }

        private async Task<IHttpResponse> CoreRequestLoopAsync(
            IHttpRequest request,
            Func<IHttpRequest, CancellationToken, Task<IHttpResponse>> executeAction,
            Func<int, bool, CancellationToken, Task> pauseAction,
            CancellationToken cancellationToken)
        {
            var attempts = 0;
            bool throttling = false;

            Uri currentUri = request.CanonicalUri.ToUri();

            while (true)
            {
                var currentRequest = new DefaultHttpRequest(request, overrideUri: currentUri);

                // Sign and build request
                this.requestAuthenticator.Authenticate(currentRequest, this.apiKey);

                try
                {
                    if (attempts > 1)
                    {
                        this.logger.Trace("Pausing before retry", "DefaultRequestExecutor.CoreRequestLoopAsync");
                        await pauseAction(attempts - 1, throttling, cancellationToken).ConfigureAwait(false);
                    }

                    var response = await executeAction(currentRequest, cancellationToken).ConfigureAwait(false);
                    if (this.IsRedirect(response))
                    {
                        currentUri = response.Headers.Location;
                        this.logger.Trace($"Redirected to {currentUri}", "DefaultRequestExecutor.CoreRequestLoopAsync");

                        continue; // re-execute request, not counted as a retry
                    }

                    var statusCode = response.StatusCode;

                    if (response.TransportError && attempts < this.maxAttemptsPerRequest)
                    {
                        this.logger.Warn($"Recoverable transport error during request, retrying", "DefaultRequestExecutor.CoreRequestLoopAsync");

                        attempts++;
                        continue; // retry request
                    }

                    // HTTP 429
                    if (statusCode == TooManyRequests && attempts < this.maxAttemptsPerRequest)
                    {
                        throttling = true;
                        this.logger.Warn($"Got HTTP 429, throttling, retrying", "DefaultRequestExecutor.CoreRequestLoopAsync");

                        attempts++;
                        continue; // retry request
                    }

                    // HTTP 5xx
                    if (response.IsServerError() && attempts < this.maxAttemptsPerRequest)
                    {
                        this.logger.Warn($"Got HTTP {statusCode}, retrying", "DefaultRequestExecutor.CoreRequestLoopAsync");

                        attempts++;
                        continue; // retry request
                    }

                    // HTTP 409 (modified) during delete
                    if (statusCode == Conflict && request.Method == HttpMethod.Delete)
                    {
                        this.logger.Warn($"Got HTTP {statusCode} during delete, retrying", "DefaultRequestExecutor.CoreRequestLoopAsync");

                        attempts++;
                        continue; // retry request
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    if (this.WasCanceled(ex, cancellationToken))
                    {
                        this.logger.Trace("Request task was canceled. Rethrowing TaskCanceledException", "DefaultRequestExecutor.CoreRequestLoopAsync");
                        throw;
                    }
                    else
                    {
                        throw new RequestException("Unable to execute HTTP request.", ex);
                    }
                }
            }
        }

        private bool IsRedirect(IHttpResponse response)
        {
            bool moved =
                response.StatusCode == MovedPermanently ||
                response.StatusCode == Redirect ||
                response.StatusCode == TemporaryRedirect;
            bool hasNewLocation = !string.IsNullOrEmpty(response.Headers.Location?.AbsoluteUri);

            return moved && hasNewLocation;
        }

        private Task PauseAsync(int retryCount, bool isThrottling, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var delayMilliseconds = this.GetDelayMilliseconds(retryCount, isThrottling);
            this.logger.Trace($"Pausing for {delayMilliseconds}", "DefaultRequestExecutor.PauseAsync");

            return Task.Delay(delayMilliseconds);
        }

        private void PauseSync(int retryCount, bool isThrottling)
        {
            var delayMilliseconds = this.GetDelayMilliseconds(retryCount, isThrottling);
            this.logger.Trace($"Pausing for {delayMilliseconds}", "DefaultRequestExecutor.PauseSync");

            Task.Delay(delayMilliseconds).Wait();
        }

        private int GetDelayMilliseconds(int retryCount, bool isThrottling)
        {
            var delayMilliseconds = 0;

            if (isThrottling)
            {
                delayMilliseconds = this.throttlingBackoffStrategy.GetDelayMilliseconds(retryCount);
            }
            else
            {
                delayMilliseconds = this.defaultBackoffStrategy.GetDelayMilliseconds(retryCount);
            }

            return delayMilliseconds;
        }

        private bool WasCanceled(Exception exception, CancellationToken cancellationToken)
        {
            bool wasCanceledByUser =
                (exception as OperationCanceledException)?.CancellationToken == cancellationToken;

            return wasCanceledByUser;
        }

        private void Dispose(bool disposing)
        {
            if (!this.alreadyDisposed)
            {
                if (disposing)
                {
                    this.httpClient.Dispose();
                }

                this.alreadyDisposed = true;
            }
        }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }
    }
}
