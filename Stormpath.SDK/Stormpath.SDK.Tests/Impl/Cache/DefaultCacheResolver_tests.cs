// <copyright file="DefaultCacheResolver_tests.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Impl.Cache;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Cache
{
    public class DefaultCacheResolver_tests
    {
        [Fact]
        public void Throws_when_getting_unsupported_synchronous_cache()
        {
            var fakeCacheManager = Substitute.For<ICacheProvider>();
            fakeCacheManager
                .IsSynchronousSupported
                .Returns(false);

            ICacheResolver cacheResolver = new DefaultCacheResolver(fakeCacheManager, Substitute.For<ICacheRegionNameResolver>());

            bool didErrorCorrectly = false;
            try
            {
                cacheResolver.GetCache<IAccount>();
            }
            catch (ApplicationException)
            {
                didErrorCorrectly = true;
            }

            Assert.True(didErrorCorrectly, "Did not throw!");
        }

        [Fact]
        public async Task Throws_when_getting_unsupported_asynchronous_cache()
        {
            var fakeCacheManager = Substitute.For<ICacheProvider>();
            fakeCacheManager
                .IsAsynchronousSupported
                .Returns(false);

            ICacheResolver cacheResolver = new DefaultCacheResolver(fakeCacheManager, Substitute.For<ICacheRegionNameResolver>());

            bool didErrorCorrectly = false;
            try
            {
                await cacheResolver.GetCacheAsync<IAccount>(CancellationToken.None);
            }
            catch (ApplicationException)
            {
                didErrorCorrectly = true;
            }

            Assert.True(didErrorCorrectly, "Did not throw!");
        }

        [Fact]
        public void Getting_synchronous_cache()
        {
            var fakeCacheManager = Substitute.For<ISynchronousCacheProvider>();
            fakeCacheManager
                .IsSynchronousSupported
                .Returns(true);
            fakeCacheManager
                .GetCache<string, IDictionary<string, object>>(Arg.Any<string>())
                .Returns(new NullCache<string, IDictionary<string, object>>());

            ICacheResolver cacheResolver = new DefaultCacheResolver(fakeCacheManager, Substitute.For<ICacheRegionNameResolver>());

            var cache = cacheResolver.GetCache<IAccount>();
            cache.ShouldBeOfType<NullCache<string, IDictionary<string, object>>>();
        }

        [Fact]
        public async Task Getting_asynchronous_cache()
        {
            var fakeCacheManager = Substitute.For<IAsynchronousCacheProvider>();
            fakeCacheManager
                .IsAsynchronousSupported
                .Returns(true);
            fakeCacheManager
                .GetCacheAsync<string, IDictionary<string, object>>(
                    Arg.Any<string>(),
                    Arg.Any<CancellationToken>())
                .Returns(async _ =>
                {
                    await Task.Yield();
                    return (IAsynchronousCache<string, IDictionary<string, object>>)new NullCache<string, IDictionary<string, object>>();
                });

            ICacheResolver cacheResolver = new DefaultCacheResolver(fakeCacheManager, Substitute.For<ICacheRegionNameResolver>());

            var cache = await cacheResolver.GetCacheAsync<IAccount>(CancellationToken.None);
            cache.ShouldBeOfType<NullCache<string, IDictionary<string, object>>>();
        }
    }
}
