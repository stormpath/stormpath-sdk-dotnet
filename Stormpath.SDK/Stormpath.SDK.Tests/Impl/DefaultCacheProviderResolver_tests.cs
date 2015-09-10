// <copyright file="DefaultCacheProviderResolver_tests.cs" company="Stormpath, Inc.">
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

using NSubstitute;
using Shouldly;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Impl.Client;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class DefaultCacheProviderResolver_tests
    {
        [Fact]
        public void Caching_is_disabled_by_default()
        {
            var resolver = new DefaultCacheProviderResolver();

            resolver.UseCache.ShouldBe(false);
            resolver.CustomProvider.ShouldBeNull();
        }

        [Fact]
        public void Provides_NullCacheProvider_when_cache_is_disabled()
        {
            var resolver = new DefaultCacheProviderResolver();

            resolver.UseCache = false;

            var provider = resolver.GetProvider();
            provider.ShouldBeOfType<NullCacheProvider>();
            provider.ShouldNotBeNull();
        }

        [Fact]
        public void Provides_InMemoryCacheProvider_when_cache_is_enabled()
        {
            var resolver = new DefaultCacheProviderResolver();

            resolver.UseCache = true;

            var provider = resolver.GetProvider();
            provider.ShouldBeOfType<InMemoryCacheProvider>();
            provider.ShouldNotBeNull();
        }

        [Fact]
        public void Provides_custom_provider()
        {
            var resolver = new DefaultCacheProviderResolver();
            var customProvider = Substitute.For<ICacheProvider>();

            resolver.UseCache = true;
            resolver.CustomProvider = customProvider;

            var provider = resolver.GetProvider();
            provider.ShouldBe(customProvider);
        }
    }
}
