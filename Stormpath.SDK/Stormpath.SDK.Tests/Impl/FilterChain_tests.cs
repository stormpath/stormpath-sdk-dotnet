// <copyright file="FilterChain_tests.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.DataStore.Filters;
using Stormpath.SDK.Logging;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class FilterChain_tests
    {
        public class Sync_tests
        {
            internal class CreateInterceptorFilter : ISynchronousFilter
            {
                IResourceDataResult ISynchronousFilter.Filter(IResourceDataRequest request, ISynchronousFilterChain chain, ILogger logger)
                {
                    if (request.Action == ResourceAction.Create)
                    {
                        return new DefaultResourceDataResult(
                            ResourceAction.Create,
                            typeof(IDictionary<string, object>),
                            request.Uri,
                            httpStatus: 200,
                            body: new Dictionary<string, object>() { { "Foo", "bar" } });
                    }
                    else
                    {
                        return chain.Filter(request, logger);
                    }
                }
            }

            internal class DeleteInterceptorFilter : ISynchronousFilter
            {
                IResourceDataResult ISynchronousFilter.Filter(IResourceDataRequest request, ISynchronousFilterChain chain, ILogger logger)
                {
                    if (request.Action == ResourceAction.Delete)
                        return new DefaultResourceDataResult(ResourceAction.Delete, null, null, 204, null);
                    else
                        return chain.Filter(request, logger);
                }
            }

            [Fact]
            public void Sync_chain_terminating_on_first()
            {
                ISynchronousFilterChain filterChain = new DefaultSynchronousFilterChain()
                    .Add(new CreateInterceptorFilter())
                    .Add(new DeleteInterceptorFilter());

                var request = new DefaultResourceDataRequest(ResourceAction.Create, typeof(IAccount), new CanonicalUri("http://api.foo.bar"));
                var result = filterChain.Filter(request, Substitute.For<ILogger>());

                result.Action.ShouldBe(ResourceAction.Create);
                result.Body.ShouldContainKeyAndValue("Foo", "bar");
            }

            [Fact]
            public void Sync_chain_terminating_on_second()
            {
                ISynchronousFilterChain filterChain = new DefaultSynchronousFilterChain()
                    .Add(new CreateInterceptorFilter())
                    .Add(new DeleteInterceptorFilter());

                var request = new DefaultResourceDataRequest(ResourceAction.Delete, typeof(IAccount), new CanonicalUri("http://api.foo.bar"));
                var result = filterChain.Filter(request, Substitute.For<ILogger>());

                result.Action.ShouldBe(ResourceAction.Delete);
                result.Body.ShouldBeNull();
            }

            [Fact]
            public void Sync_with_inline_filter()
            {
                ISynchronousFilterChain defaultFilterChain = new DefaultSynchronousFilterChain()
                    .Add(new DeleteInterceptorFilter());

                ISynchronousFilterChain finalChain = new DefaultSynchronousFilterChain(defaultFilterChain as DefaultSynchronousFilterChain)
                    .Add(new DefaultSynchronousFilter((req, next, logger) =>
                    {
                        return new DefaultResourceDataResult(
                            ResourceAction.Create,
                            typeof(IDictionary<string, object>),
                            req.Uri,
                            httpStatus: 200,
                            body: new Dictionary<string, object>() { { "Foo", "bar" } });
                    }));

                var request = new DefaultResourceDataRequest(ResourceAction.Create, typeof(IAccount), new CanonicalUri("http://api.foo.bar"));
                var result = finalChain.Filter(request, Substitute.For<ILogger>());

                result.Action.ShouldBe(ResourceAction.Create);
                result.Body.ShouldContainKeyAndValue("Foo", "bar");
            }
        }

        public class Async_tests
        {
            internal class CreateInterceptorFilter : IAsynchronousFilter
            {
                Task<IResourceDataResult> IAsynchronousFilter.FilterAsync(IResourceDataRequest request, IAsynchronousFilterChain chain, ILogger logger, CancellationToken cancellationToken)
                {
                    if (request.Action == ResourceAction.Create)
                    {
                        return Task.FromResult<IResourceDataResult>(new DefaultResourceDataResult(
                            ResourceAction.Create,
                            typeof(IDictionary<string, object>),
                            request.Uri,
                            httpStatus: 200,
                            body: new Dictionary<string, object>() { { "Foo", "bar" } }));
                    }
                    else
                    {
                        return chain.FilterAsync(request, logger, cancellationToken);
                    }
                }
            }

            internal class DeleteInterceptorFilter : IAsynchronousFilter
            {
                Task<IResourceDataResult> IAsynchronousFilter.FilterAsync(IResourceDataRequest request, IAsynchronousFilterChain chain, ILogger logger, CancellationToken cancellationToken)
                {
                    if (request.Action == ResourceAction.Delete)
                    {
                        return Task.FromResult<IResourceDataResult>(
                            new DefaultResourceDataResult(ResourceAction.Delete, null, null, 204, null));
                    }
                    else
                    {
                        return chain.FilterAsync(request, logger, cancellationToken);
                    }
                }
            }

            [Fact]
            public async Task Async_chain_terminating_on_first()
            {
                IAsynchronousFilterChain filterChain = new DefaultAsynchronousFilterChain()
                    .Add(new CreateInterceptorFilter())
                    .Add(new DeleteInterceptorFilter());

                var request = new DefaultResourceDataRequest(ResourceAction.Create, typeof(IAccount), new CanonicalUri("http://api.foo.bar"));
                var result = await filterChain.FilterAsync(request, Substitute.For<ILogger>(), CancellationToken.None);

                result.Action.ShouldBe(ResourceAction.Create);
                result.Body.ShouldContainKeyAndValue("Foo", "bar");
            }

            [Fact]
            public async Task Async_chain_terminating_on_second()
            {
                IAsynchronousFilterChain filterChain = new DefaultAsynchronousFilterChain()
                    .Add(new CreateInterceptorFilter())
                    .Add(new DeleteInterceptorFilter());

                var request = new DefaultResourceDataRequest(ResourceAction.Delete, typeof(IAccount), new CanonicalUri("http://api.foo.bar"));
                var result = await filterChain.FilterAsync(request, Substitute.For<ILogger>(), CancellationToken.None);

                result.Action.ShouldBe(ResourceAction.Delete);
                result.Body.ShouldBeNull();
            }

            [Fact]
            public async Task Async_with_inline_filter()
            {
                IAsynchronousFilterChain defaultFilterChain = new DefaultAsynchronousFilterChain()
                    .Add(new DeleteInterceptorFilter());

                IAsynchronousFilterChain finalChain = new DefaultAsynchronousFilterChain(defaultFilterChain as DefaultAsynchronousFilterChain)
                    .Add(new DefaultAsynchronousFilter((req, next, logger, ct) =>
                    {
                        return Task.FromResult<IResourceDataResult>(new DefaultResourceDataResult(
                            ResourceAction.Create,
                            typeof(IDictionary<string, object>),
                            req.Uri,
                            httpStatus: 200,
                            body: new Dictionary<string, object>() { { "Foo", "bar" } }));
                    }));

                var request = new DefaultResourceDataRequest(ResourceAction.Create, typeof(IAccount), new CanonicalUri("http://api.foo.bar"));
                var result = await finalChain.FilterAsync(request, Substitute.For<ILogger>(), CancellationToken.None);

                result.Action.ShouldBe(ResourceAction.Create);
                result.Body.ShouldContainKeyAndValue("Foo", "bar");
            }
        }
    }
}
