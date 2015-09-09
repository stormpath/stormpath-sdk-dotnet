using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Impl.DataStore.FilterChain;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Http.Support;
using Stormpath.SDK.Shared;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class FilterChain_tests
    {
        public class Sync_tests
        {
            internal class CreateInterceptorFilter : ISynchronousFilter
            {
                IHttpResponse ISynchronousFilter.Execute(IHttpRequest request, ISynchronousFilterChain chain, ILogger logger)
                {
                    if (request.Method == HttpMethod.Post)
                        return new DefaultHttpResponse(200, "OK", null, request.Body, request.BodyContentType);
                    else
                        return chain.Execute(request, logger);
                }
            }

            internal class DeleteInterceptorFilter : ISynchronousFilter
            {
                IHttpResponse ISynchronousFilter.Execute(IHttpRequest request, ISynchronousFilterChain chain, ILogger logger)
                {
                    if (request.Method == HttpMethod.Delete)
                        return new DefaultHttpResponse(204, "No Content", null, null, null);
                    else
                        return chain.Execute(request, logger);
                }
            }

            [Fact]
            public void Sync_chain_terminating_on_first()
            {
                ISynchronousFilterChain filterChain = new DefaultSynchronousFilterChain()
                    .Add(new CreateInterceptorFilter())
                    .Add(new DeleteInterceptorFilter());

                var request = new DefaultHttpRequest(HttpMethod.Post, new CanonicalUri("http://api.foo.bar"), null, null, "{ data }", "text/plain");
                var result = filterChain.Execute(request, Substitute.For<ILogger>());

                result.HttpStatus.ShouldBe(200);
                result.Body.ShouldBe("{ data }");
            }

            [Fact]
            public void Sync_chain_terminating_on_second()
            {
                ISynchronousFilterChain filterChain = new DefaultSynchronousFilterChain()
                    .Add(new CreateInterceptorFilter())
                    .Add(new DeleteInterceptorFilter());

                var request = new DefaultHttpRequest(HttpMethod.Delete, new CanonicalUri("http://api.foo.bar"), null, null, null, null);
                var result = filterChain.Execute(request, Substitute.For<ILogger>());

                result.HttpStatus.ShouldBe(204);
                result.Body.ShouldBe(null);
            }

            [Fact]
            public void Sync_with_inline_filter()
            {
                ISynchronousFilterChain defaultFilterChain = new DefaultSynchronousFilterChain()
                    .Add(new DeleteInterceptorFilter());

                var request = new DefaultHttpRequest(HttpMethod.Post, new CanonicalUri("http://api.foo.bar"), null, null, "{ data }", "text/plain");

                ISynchronousFilterChain finalChain = new DefaultSynchronousFilterChain(defaultFilterChain as DefaultSynchronousFilterChain)
                    .Add(new DefaultSynchronousFilter((req, chain, logger) =>
                    {
                        if (request.Method == HttpMethod.Post)
                            return new DefaultHttpResponse(200, "OK", null, request.Body, request.BodyContentType);
                        else
                            return chain.Execute(request, logger);
                    }));

                var result = finalChain.Execute(request, Substitute.For<ILogger>());

                result.HttpStatus.ShouldBe(200);
                result.Body.ShouldBe("{ data }");
            }
        }

        public class Async_tests
        {
            internal class CreateInterceptorFilter : IAsynchronousFilter
            {
                Task<IHttpResponse> IAsynchronousFilter.ExecuteAsync(IHttpRequest request, IAsynchronousFilterChain chain, ILogger logger, CancellationToken cancellationToken)
                {
                    if (request.Method == HttpMethod.Post)
                        return Task.FromResult<IHttpResponse>(new DefaultHttpResponse(200, "OK", null, request.Body, request.BodyContentType));
                    else
                        return chain.ExecuteAsync(request, logger, cancellationToken);
                }
            }

            internal class DeleteInterceptorFilter : IAsynchronousFilter
            {
                Task<IHttpResponse> IAsynchronousFilter.ExecuteAsync(IHttpRequest request, IAsynchronousFilterChain chain, ILogger logger, CancellationToken cancellationToken)
                {
                    if (request.Method == HttpMethod.Delete)
                        return Task.FromResult(new DefaultHttpResponse(204, "No Content", null, null, null) as IHttpResponse);
                    else
                        return chain.ExecuteAsync(request, logger, cancellationToken);
                }
            }

            [Fact]
            public async Task Async_chain_terminating_on_first()
            {
                IAsynchronousFilterChain filterChain = new DefaultAsynchronousFilterChain()
                    .Add(new CreateInterceptorFilter())
                    .Add(new DeleteInterceptorFilter());

                var request = new DefaultHttpRequest(HttpMethod.Post, new CanonicalUri("http://api.foo.bar"), null, null, "{ data }", "text/plain");
                var result = await filterChain.ExecuteAsync(request, Substitute.For<ILogger>(), CancellationToken.None);

                result.HttpStatus.ShouldBe(200);
                result.Body.ShouldBe("{ data }");
            }

            [Fact]
            public async Task Async_chain_terminating_on_second()
            {
                IAsynchronousFilterChain filterChain = new DefaultAsynchronousFilterChain()
                    .Add(new CreateInterceptorFilter())
                    .Add(new DeleteInterceptorFilter());

                var request = new DefaultHttpRequest(HttpMethod.Delete, new CanonicalUri("http://api.foo.bar"), null, null, null, null);
                var result = await filterChain.ExecuteAsync(request, Substitute.For<ILogger>(), CancellationToken.None);

                result.HttpStatus.ShouldBe(204);
                result.Body.ShouldBe(null);
            }

            [Fact]
            public async Task Async_with_inline_filter()
            {
                IAsynchronousFilterChain defaultFilterChain = new DefaultAsynchronousFilterChain()
                    .Add(new DeleteInterceptorFilter());

                var request = new DefaultHttpRequest(HttpMethod.Post, new CanonicalUri("http://api.foo.bar"), null, null, "{ data }", "text/plain");

                IAsynchronousFilterChain finalChain = new DefaultAsynchronousFilterChain(defaultFilterChain as DefaultAsynchronousFilterChain)
                    .Add(new DefaultAsynchronousFilter((req, chain, logger, ct) =>
                    {
                        if (request.Method == HttpMethod.Post)
                            return Task.FromResult<IHttpResponse>(new DefaultHttpResponse(200, "OK", null, request.Body, request.BodyContentType));
                        else
                            return chain.ExecuteAsync(request, logger, ct);
                    }));

                var result = await finalChain.ExecuteAsync(request, Substitute.For<ILogger>(), CancellationToken.None);

                result.HttpStatus.ShouldBe(200);
                result.Body.ShouldBe("{ data }");
            }
        }
    }
}
