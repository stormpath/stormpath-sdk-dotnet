// <copyright file="DefaultRequestExecutor_tests.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
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
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Client;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Http.Support;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Tests.Common.Fakes;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class DefaultRequestExecutor_tests
    {
        private static ISynchronousHttpClient GetSynchronousClient(IHttpResponse mockResponse)
        {
            var fakeClient = Substitute.For<ISynchronousHttpClient>();
            fakeClient.IsSynchronousSupported.Returns(true);
            fakeClient
                .Execute(Arg.Any<IHttpRequest>())
                .Returns(mockResponse);

            return fakeClient;
        }

        private static IAsynchronousHttpClient GetAsynchronousClient(IHttpResponse mockResponse)
        {
            var fakeClient = Substitute.For<IAsynchronousHttpClient>();
            fakeClient.IsAsynchronousSupported.Returns(true);
            fakeClient
                .ExecuteAsync(Arg.Any<IHttpRequest>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(mockResponse));

            return fakeClient;
        }

        private static IRequestExecutor GetRequestExecutor(IHttpClient client, IBackoffStrategy defaultStrategy = null, IBackoffStrategy throttlingStrategy = null)
        {
            var defaultBackoffStrategy = defaultStrategy ?? GetFakeBackoffStrategy();
            var throttlingBackoffStrategy = throttlingStrategy ?? GetFakeBackoffStrategy();

            return new DefaultRequestExecutor(
                client,
                FakeApiKey.Create(valid: true),
                AuthenticationScheme.Basic,
                Substitute.For<ILogger>(),
                defaultBackoffStrategy,
                throttlingBackoffStrategy);
        }

        private static IBackoffStrategy GetFakeBackoffStrategy()
        {
            var strategy = Substitute.For<IBackoffStrategy>();
            strategy.GetDelayMilliseconds(0).ReturnsForAnyArgs(1);

            return strategy;
        }

        public class Sync_tests
        {
            [Fact]
            public void Throws_error_when_no_sync_path_is_available_for_request()
            {
                var noSyncPathClient = Substitute.For<ISynchronousHttpClient>();
                noSyncPathClient.IsSynchronousSupported.Returns(false);

                var requestExecutor = GetRequestExecutor(noSyncPathClient);

                Assert.Throws<ApplicationException>(() =>
                {
                    var response = requestExecutor.Execute(Substitute.For<IHttpRequest>());
                });
            }

            [Fact]
            public void Retries_request_on_recoverable_error()
            {
                // Set up a fake HttpClient that mysteriously always fails with recoverable errors
                var failingHttpClient = GetSynchronousClient(
                    new DefaultHttpResponse(0, null, new HttpHeaders(), null, null, transportError: true));

                var defaultBackoffStrategy = GetFakeBackoffStrategy();
                var throttlingBackoffStrategy = GetFakeBackoffStrategy();
                var requestExecutor = GetRequestExecutor(failingHttpClient, defaultBackoffStrategy, throttlingBackoffStrategy);
                var dummyRequest = new DefaultHttpRequest(HttpMethod.Delete, new CanonicalUri("http://api.foo.bar/foo"));

                var response = requestExecutor.Execute(dummyRequest);
                response.TransportError.ShouldBeTrue();

                // Used default backoff strategy to pause after each retry (4 times)
                defaultBackoffStrategy.ReceivedWithAnyArgs(4).GetDelayMilliseconds(0);
                throttlingBackoffStrategy.DidNotReceiveWithAnyArgs().GetDelayMilliseconds(0);
            }

            [Fact]
            public void Retries_request_with_throttling_on_HTTP_429()
            {
                // Set up a fake HttpClient that always returns HTTP 429
                var failingHttpClient = GetSynchronousClient(
                    new DefaultHttpResponse(429, null, new HttpHeaders(), null, null, transportError: false));

                var defaultBackoffStrategy = GetFakeBackoffStrategy();
                var throttlingBackoffStrategy = GetFakeBackoffStrategy();
                var requestExecutor = GetRequestExecutor(failingHttpClient, defaultBackoffStrategy, throttlingBackoffStrategy);
                var dummyRequest = new DefaultHttpRequest(HttpMethod.Delete, new CanonicalUri("http://api.foo.bar/foo"));

                var response = requestExecutor.Execute(dummyRequest);
                response.IsClientError().ShouldBeTrue();

                // Used throttling backoff strategy to pause after each retry (4 times)
                defaultBackoffStrategy.DidNotReceiveWithAnyArgs().GetDelayMilliseconds(0);
                throttlingBackoffStrategy.ReceivedWithAnyArgs(4).GetDelayMilliseconds(0);
            }

            [Fact]
            public void Retries_request_on_HTTP_503()
            {
                // Set up a fake HttpClient that awlays returns HTTP 503
                var failingHttpClient = GetSynchronousClient(
                    new DefaultHttpResponse(503, null, new HttpHeaders(), null, null, transportError: false));

                var defaultBackoffStrategy = GetFakeBackoffStrategy();
                var throttlingBackoffStrategy = GetFakeBackoffStrategy();
                var requestExecutor = GetRequestExecutor(failingHttpClient, defaultBackoffStrategy, throttlingBackoffStrategy);
                var dummyRequest = new DefaultHttpRequest(HttpMethod.Delete, new CanonicalUri("http://api.foo.bar/foo"));

                var response = requestExecutor.Execute(dummyRequest);
                response.IsServerError().ShouldBeTrue();

                // Used default backoff strategy to pause after each retry (4 times)
                defaultBackoffStrategy.ReceivedWithAnyArgs(4).GetDelayMilliseconds(0);
                throttlingBackoffStrategy.DidNotReceiveWithAnyArgs().GetDelayMilliseconds(0);
            }
        }

        public class Async_tests
        {
            [Fact]
            public void Throws_error_when_no_async_path_is_available_for_request()
            {
                var noAsyncPathClient = Substitute.For<IAsynchronousHttpClient>();
                noAsyncPathClient.IsAsynchronousSupported.Returns(false);

                var requestExecutor = GetRequestExecutor(noAsyncPathClient);

                Assert.Throws<ApplicationException>(() =>
                {
                    var response = requestExecutor.ExecuteAsync(Substitute.For<IHttpRequest>(), CancellationToken.None);
                });
            }

            [Fact]
            public async Task Does_not_retry_when_task_is_canceled()
            {
                // Set up a fake HttpClient that throws for cancellation
                var throwingHttpClient = Substitute.For<IAsynchronousHttpClient>();
                throwingHttpClient.IsAsynchronousSupported.Returns(true);
                throwingHttpClient
                    .When(fake => fake.ExecuteAsync(Arg.Any<IHttpRequest>(), Arg.Any<CancellationToken>()))
                    .Do(call =>
                    {
                        call.Arg<CancellationToken>().ThrowIfCancellationRequested();
                    });

                var requestExecutor = GetRequestExecutor(throwingHttpClient);

                var canceled = new CancellationTokenSource();
                canceled.Cancel();

                var dummyRequest = new DefaultHttpRequest(HttpMethod.Delete, new CanonicalUri("http://api.foo.bar/foo"));

                await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                {
                    await requestExecutor.ExecuteAsync(dummyRequest, canceled.Token);
                });

                // Should only have 1 call: no retries!
                await throwingHttpClient.Received(1).ExecuteAsync(Arg.Any<IHttpRequest>(), Arg.Any<CancellationToken>());
            }

            [Fact]
            public async Task Retries_request_on_recoverable_error()
            {
                // Set up a fake HttpClient that mysteriously always fails with recoverable errors
                var failingHttpClient = GetAsynchronousClient(
                    new DefaultHttpResponse(0, null, new HttpHeaders(), null, null, transportError: true));

                var defaultBackoffStrategy = GetFakeBackoffStrategy();
                var throttlingBackoffStrategy = GetFakeBackoffStrategy();
                var requestExecutor = GetRequestExecutor(failingHttpClient, defaultBackoffStrategy, throttlingBackoffStrategy);
                var dummyRequest = new DefaultHttpRequest(HttpMethod.Delete, new CanonicalUri("http://api.foo.bar/foo"));

                var response = await requestExecutor.ExecuteAsync(dummyRequest, CancellationToken.None);
                response.TransportError.ShouldBeTrue();

                // Used default backoff strategy to pause after each retry (4 times)
                defaultBackoffStrategy.ReceivedWithAnyArgs(4).GetDelayMilliseconds(0);
                throttlingBackoffStrategy.DidNotReceiveWithAnyArgs().GetDelayMilliseconds(0);
            }

            [Fact]
            public async Task Retries_request_with_throttling_on_HTTP_429()
            {
                // Set up a fake HttpClient that always returns HTTP 429
                var failingHttpClient = GetAsynchronousClient(new DefaultHttpResponse(429, null, new HttpHeaders(), null, null, transportError: false));

                var defaultBackoffStrategy = GetFakeBackoffStrategy();
                var throttlingBackoffStrategy = GetFakeBackoffStrategy();
                var requestExecutor = GetRequestExecutor(failingHttpClient, defaultBackoffStrategy, throttlingBackoffStrategy);
                var dummyRequest = new DefaultHttpRequest(HttpMethod.Delete, new CanonicalUri("http://api.foo.bar/foo"));

                var response = await requestExecutor.ExecuteAsync(dummyRequest, CancellationToken.None);
                response.IsClientError().ShouldBeTrue();

                // Used throttling backoff strategy to pause after each retry (4 times)
                defaultBackoffStrategy.DidNotReceiveWithAnyArgs().GetDelayMilliseconds(0);
                throttlingBackoffStrategy.ReceivedWithAnyArgs(4).GetDelayMilliseconds(0);
            }

            [Fact]
            public async Task Retries_request_on_HTTP_503()
            {
                // Set up a fake HttpClient that awlays returns HTTP 503
                var failingHttpClient = GetAsynchronousClient(new DefaultHttpResponse(503, null, new HttpHeaders(), null, null, transportError: false));

                var defaultBackoffStrategy = GetFakeBackoffStrategy();
                var throttlingBackoffStrategy = GetFakeBackoffStrategy();
                var requestExecutor = GetRequestExecutor(failingHttpClient, defaultBackoffStrategy, throttlingBackoffStrategy);
                var dummyRequest = new DefaultHttpRequest(HttpMethod.Delete, new CanonicalUri("http://api.foo.bar/foo"));

                var response = await requestExecutor.ExecuteAsync(dummyRequest, CancellationToken.None);
                response.IsServerError().ShouldBeTrue();

                // Used default backoff strategy to pause after each retry (4 times)
                defaultBackoffStrategy.ReceivedWithAnyArgs(4).GetDelayMilliseconds(0);
                throttlingBackoffStrategy.DidNotReceiveWithAnyArgs().GetDelayMilliseconds(0);
            }
        }
    }
}
