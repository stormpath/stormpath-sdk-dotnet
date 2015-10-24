﻿// <copyright file="InMemoryCacheProviderBuilder_tests.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Impl.Cache;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Cache
{
    public class InMemoryCacheProviderBuilder_tests
    {
        private readonly ICacheProviderBuilder builder;

        public InMemoryCacheProviderBuilder_tests()
        {
            this.builder = new InMemoryCacheProviderBuilder();
        }

        [Fact]
        public void With_default_TTL_set()
        {
            this.builder.WithDefaultTimeToLive(TimeSpan.FromSeconds(1337));

            var cacheProvider = this.builder.Build();

            (cacheProvider as InMemoryCacheProvider).DefaultTimeToLive.ShouldBe(TimeSpan.FromSeconds(1337));
            (cacheProvider as InMemoryCacheProvider).DefaultTimeToIdle.ShouldBeNull();
        }

        [Fact]
        public void With_default_TTI_set()
        {
            this.builder.WithDefaultTimeToIdle(TimeSpan.FromSeconds(1337));

            var cacheProvider = this.builder.Build();

            (cacheProvider as InMemoryCacheProvider).DefaultTimeToLive.ShouldBeNull();
            (cacheProvider as InMemoryCacheProvider).DefaultTimeToIdle.ShouldBe(TimeSpan.FromSeconds(1337));
        }

        [Fact]
        public void Caches_without_configuration_are_created_with_default_TTL_and_TTI()
        {
            var defaultTtl = TimeSpan.FromSeconds(50);
            var defaultTti = TimeSpan.FromSeconds(25);

            this.builder.WithDefaultTimeToLive(defaultTtl);
            this.builder.WithDefaultTimeToIdle(defaultTti);
            var cacheProvider = this.builder.Build() as ISynchronousCacheProvider;

            var cache = cacheProvider.GetCache<string, object>("foobar");

            cache.TimeToLive.ShouldBe(defaultTtl);
            cache.TimeToIdle.ShouldBe(defaultTti);
        }

        [Fact]
        public void Caches_with_configuration_ignore_defaults()
        {
            var cacheTtl = TimeSpan.FromDays(1);
            var cacheTti = TimeSpan.FromDays(0.5);

            this.builder.WithDefaultTimeToLive(TimeSpan.FromSeconds(50));
            this.builder.WithDefaultTimeToIdle(TimeSpan.FromSeconds(25));

            ICacheConfigurationBuilder cacheConfigBuilder = new DefaultCacheConfigurationBuilder("foobar");
            cacheConfigBuilder.WithTimeToLive(cacheTtl);
            cacheConfigBuilder.WithTimeToIdle(cacheTti);
            this.builder.WithCache(cacheConfigBuilder);

            var cacheProvider = this.builder.Build() as ISynchronousCacheProvider;
            var cache = cacheProvider.GetCache<string, object>("foobar");

            cache.TimeToLive.ShouldBe(cacheTtl);
            cache.TimeToIdle.ShouldBe(cacheTti);
        }
    }
}