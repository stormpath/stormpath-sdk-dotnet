// <copyright file="DefaultRequestExecutor_tests.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Stormpath.SDK.Client;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Http.Support;
using Stormpath.SDK.Shared;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class DefaultRequestExecutor_tests
    {
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
                var fakeSyncHttpClient = Substitute.For<ISynchronousHttpClient>();
                fakeSyncHttpClient.IsSynchronousSupported.Returns(false);

                IRequestExecutor requestExecutor = new DefaultRequestExecutor(
                    fakeSyncHttpClient,
                    FakeApiKey.Create(valid: true),
                    AuthenticationScheme.Basic,
                    Substitute.For<ILogger>());

                Assert.Throws<ApplicationException>(() =>
                {
                    var response = requestExecutor.Execute(Substitute.For<IHttpRequest>());
                });
            }

            [Fact]
            public void Retries_request_on_recoverable_error()
            {
                // Set up a fake HttpClient that mysteriously always fails with recoverable errors
                var failingHttpClient = Substitute.For<ISynchronousHttpClient>();
                failingHttpClient.IsSynchronousSupported.Returns(true);
                failingHttpClient
                    .Execute(Arg.Any<IHttpRequest>())
                    .Returns(new DefaultHttpResponse(0, null, new HttpHeaders(), null, null, ResponseErrorType.Recoverable));

                var defaultBackoffStrategy = GetFakeBackoffStrategy();
                var throttlingBackoffStrategy = GetFakeBackoffStrategy();
                IRequestExecutor requestExecutor = new DefaultRequestExecutor(
                    failingHttpClient,
                    FakeApiKey.Create(valid: true),
                    AuthenticationScheme.Basic,
                    Substitute.For<ILogger>(),
                    defaultBackoffStrategy,
                    throttlingBackoffStrategy);

                var dummyRequest = new DefaultHttpRequest(HttpMethod.Delete, new CanonicalUri("http://api.foo.bar/foo"));

                Assert.Throws<RequestException>(() =>
                {
                    requestExecutor.Execute(dummyRequest);
                });

                defaultBackoffStrategy.ReceivedWithAnyArgs(4).GetDelayMilliseconds(0);
                throttlingBackoffStrategy.DidNotReceiveWithAnyArgs().GetDelayMilliseconds(0);
            }

            [Fact]
            public void Retries_request_with_throttling_on_HTTP_429()
            {
                // Set up a fake HttpClient that always returns HTTP 429
                var failingHttpClient = Substitute.For<ISynchronousHttpClient>();
                failingHttpClient.IsSynchronousSupported.Returns(true);
                failingHttpClient
                    .Execute(Arg.Any<IHttpRequest>())
                    .Returns(new DefaultHttpResponse(429, null, new HttpHeaders(), null, null, ResponseErrorType.None));

                var defaultBackoffStrategy = GetFakeBackoffStrategy();
                var throttlingBackoffStrategy = GetFakeBackoffStrategy();
                IRequestExecutor requestExecutor = new DefaultRequestExecutor(
                    failingHttpClient,
                    FakeApiKey.Create(valid: true),
                    AuthenticationScheme.Basic,
                    Substitute.For<ILogger>(),
                    defaultBackoffStrategy,
                    throttlingBackoffStrategy);

                var dummyRequest = new DefaultHttpRequest(HttpMethod.Delete, new CanonicalUri("http://api.foo.bar/foo"));

                Assert.Throws<RequestException>(() =>
                {
                    requestExecutor.Execute(dummyRequest);
                });

                defaultBackoffStrategy.DidNotReceiveWithAnyArgs().GetDelayMilliseconds(0);
                throttlingBackoffStrategy.ReceivedWithAnyArgs(4).GetDelayMilliseconds(0);
            }

            [Fact]
            public void Retries_request_on_HTTP_503()
            {
                // Set up a fake HttpClient that awlays returns HTTP 503
                var failingHttpClient = Substitute.For<ISynchronousHttpClient>();
                failingHttpClient.IsSynchronousSupported.Returns(true);
                failingHttpClient
                    .Execute(Arg.Any<IHttpRequest>())
                    .Returns(new DefaultHttpResponse(503, null, new HttpHeaders(), null, null, ResponseErrorType.None));

                var defaultBackoffStrategy = GetFakeBackoffStrategy();
                var throttlingBackoffStrategy = GetFakeBackoffStrategy();
                IRequestExecutor requestExecutor = new DefaultRequestExecutor(
                    failingHttpClient,
                    FakeApiKey.Create(valid: true),
                    AuthenticationScheme.Basic,
                    Substitute.For<ILogger>(),
                    defaultBackoffStrategy,
                    throttlingBackoffStrategy);

                var dummyRequest = new DefaultHttpRequest(HttpMethod.Delete, new CanonicalUri("http://api.foo.bar/foo"));

                Assert.Throws<RequestException>(() =>
                {
                    requestExecutor.Execute(dummyRequest);
                });

                defaultBackoffStrategy.ReceivedWithAnyArgs(4).GetDelayMilliseconds(0);
                throttlingBackoffStrategy.DidNotReceiveWithAnyArgs().GetDelayMilliseconds(0);
            }
        }

        public class Async_tests
        {
            [Fact]
            public void Throws_error_when_no_async_path_is_available_for_request()
            {
                var fakeAsyncHttpClient = Substitute.For<IAsynchronousHttpClient>();
                fakeAsyncHttpClient.IsAsynchronousSupported.Returns(false);

                IRequestExecutor requestExecutor = new DefaultRequestExecutor(
                    fakeAsyncHttpClient,
                    FakeApiKey.Create(valid: true),
                    AuthenticationScheme.Basic,
                    Substitute.For<ILogger>());

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

                IRequestExecutor requestExecutor = new DefaultRequestExecutor(
                    throwingHttpClient,
                    FakeApiKey.Create(valid: true),
                    AuthenticationScheme.Basic,
                    Substitute.For<ILogger>());

                var canceled = new CancellationTokenSource();
                canceled.Cancel();

                var dummyRequest = new DefaultHttpRequest(HttpMethod.Delete, new CanonicalUri("http://api.foo.bar/foo"));

                await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                {
                    await requestExecutor.ExecuteAsync(dummyRequest, canceled.Token);
                });

                // Should only have 1 call: no retries!
                throwingHttpClient.Received(1).ExecuteAsync(Arg.Any<IHttpRequest>(), Arg.Any<CancellationToken>()).IgnoreAwait();
            }

            [Fact]
            public async Task Retries_request_on_recoverable_error()
            {
                // Set up a fake HttpClient that mysteriously always fails with recoverable errors
                var failingHttpClient = Substitute.For<IAsynchronousHttpClient>();
                failingHttpClient.IsAsynchronousSupported.Returns(true);
                failingHttpClient
                    .ExecuteAsync(Arg.Any<IHttpRequest>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult<IHttpResponse>(
                        new DefaultHttpResponse(0, null, new HttpHeaders(), null, null, ResponseErrorType.Recoverable)));

                var defaultBackoffStrategy = GetFakeBackoffStrategy();
                var throttlingBackoffStrategy = GetFakeBackoffStrategy();
                IRequestExecutor requestExecutor = new DefaultRequestExecutor(
                    failingHttpClient,
                    FakeApiKey.Create(valid: true),
                    AuthenticationScheme.Basic,
                    Substitute.For<ILogger>(),
                    defaultBackoffStrategy,
                    throttlingBackoffStrategy);

                var dummyRequest = new DefaultHttpRequest(HttpMethod.Delete, new CanonicalUri("http://api.foo.bar/foo"));

                await Assert.ThrowsAsync<RequestException>(async () =>
                {
                    await requestExecutor.ExecuteAsync(dummyRequest, CancellationToken.None);
                });

                defaultBackoffStrategy.ReceivedWithAnyArgs(4).GetDelayMilliseconds(0);
                throttlingBackoffStrategy.DidNotReceiveWithAnyArgs().GetDelayMilliseconds(0);
            }

            [Fact]
            public async Task Retries_request_with_throttling_on_HTTP_429()
            {
                // Set up a fake HttpClient that always returns HTTP 429
                var failingHttpClient = Substitute.For<IAsynchronousHttpClient>();
                failingHttpClient.IsAsynchronousSupported.Returns(true);
                failingHttpClient
                    .ExecuteAsync(Arg.Any<IHttpRequest>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult<IHttpResponse>(
                        new DefaultHttpResponse(429, null, new HttpHeaders(), null, null, ResponseErrorType.None)));

                var defaultBackoffStrategy = GetFakeBackoffStrategy();
                var throttlingBackoffStrategy = GetFakeBackoffStrategy();
                IRequestExecutor requestExecutor = new DefaultRequestExecutor(
                    failingHttpClient,
                    FakeApiKey.Create(valid: true),
                    AuthenticationScheme.Basic,
                    Substitute.For<ILogger>(),
                    defaultBackoffStrategy,
                    throttlingBackoffStrategy);

                var dummyRequest = new DefaultHttpRequest(HttpMethod.Delete, new CanonicalUri("http://api.foo.bar/foo"));

                await Assert.ThrowsAsync<RequestException>(async () =>
                {
                    await requestExecutor.ExecuteAsync(dummyRequest, CancellationToken.None);
                });

                defaultBackoffStrategy.DidNotReceiveWithAnyArgs().GetDelayMilliseconds(0);
                throttlingBackoffStrategy.ReceivedWithAnyArgs(4).GetDelayMilliseconds(0);
            }

            [Fact]
            public async Task Retries_request_on_HTTP_503()
            {
                // Set up a fake HttpClient that awlays returns HTTP 503
                var failingHttpClient = Substitute.For<IAsynchronousHttpClient>();
                failingHttpClient.IsAsynchronousSupported.Returns(true);
                failingHttpClient
                    .ExecuteAsync(Arg.Any<IHttpRequest>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult<IHttpResponse>(
                        new DefaultHttpResponse(503, null, new HttpHeaders(), null, null, ResponseErrorType.None)));

                var defaultBackoffStrategy = GetFakeBackoffStrategy();
                var throttlingBackoffStrategy = GetFakeBackoffStrategy();
                IRequestExecutor requestExecutor = new DefaultRequestExecutor(
                    failingHttpClient,
                    FakeApiKey.Create(valid: true),
                    AuthenticationScheme.Basic,
                    Substitute.For<ILogger>(),
                    defaultBackoffStrategy,
                    throttlingBackoffStrategy);

                var dummyRequest = new DefaultHttpRequest(HttpMethod.Delete, new CanonicalUri("http://api.foo.bar/foo"));

                await Assert.ThrowsAsync<RequestException>(async () =>
                {
                    await requestExecutor.ExecuteAsync(dummyRequest, CancellationToken.None);
                });

                defaultBackoffStrategy.ReceivedWithAnyArgs(4).GetDelayMilliseconds(0);
                throttlingBackoffStrategy.DidNotReceiveWithAnyArgs().GetDelayMilliseconds(0);
            }
        }
    }
}
