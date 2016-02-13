// <copyright file="Single_threaded_tests.cs" company="Stormpath, Inc.">
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
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Impl.IdentityMap;
using Stormpath.SDK.Logging;
using Xunit;

namespace Stormpath.SDK.Tests.IdentityMap
{
    public class Single_threaded_tests : IDisposable
    {
        private readonly IIdentityMap<TestEntity> identityMap;

        public Single_threaded_tests()
        {
            // Arbitrary expiration policy. We won't be validating expirations in these tests
            // because it's tricky to do so with MemoryCache.
            this.identityMap = new MemoryCacheIdentityMap<TestEntity>(TimeSpan.FromSeconds(10), Substitute.For<ILogger>());
        }

        private TestEntity CreateEntity(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }

            return new TestEntity(id);
        }

        [Fact]
        public void Creating_one_resource()
        {
            var barId = Guid.NewGuid().ToString();
            var bar = this.identityMap.GetOrAdd(barId, () => this.CreateEntity(barId), storeInfinitely: false);
            bar.SetCount(5);

            bar.Count.ShouldBe(5);
            this.identityMap.LifetimeItemsAdded.ShouldBe(1);
        }

        [Fact]
        public void Creating_two_resources()
        {
            var barId = Guid.NewGuid().ToString();
            var bar = this.identityMap.GetOrAdd(barId, () => this.CreateEntity(barId), storeInfinitely: false);
            bar.SetCount(5);

            var bazId = Guid.NewGuid().ToString();
            var baz = this.identityMap.GetOrAdd(bazId, () => this.CreateEntity(bazId), storeInfinitely: false);
            baz.SetCount(3);

            bar.Count.ShouldBe(5);
            baz.Count.ShouldBe(3);
            bar.ShouldNotBeSameAs(baz);
            this.identityMap.LifetimeItemsAdded.ShouldBe(2);
        }

        [Fact]
        public void Creating_duplicates()
        {
            var id = "foo-id";

            var bar1 = this.identityMap.GetOrAdd(id, () => this.CreateEntity(id), storeInfinitely: false);
            bar1.SetCount(100);

            var bar2 = this.identityMap.GetOrAdd(id, () => this.CreateEntity(id), storeInfinitely: false);
            bar2.Count.ShouldBe(100);

            bar1.ShouldBeSameAs(bar2);
            this.identityMap.LifetimeItemsAdded.ShouldBe(1);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        public void Making_many_items(int items)
        {
            var persistentItemId = $"Making_many_items_{items}";
            var foo = this.identityMap.GetOrAdd(persistentItemId, () => this.CreateEntity(persistentItemId), storeInfinitely: false);
            foo.SetCount(1337);

            for (var i = 0; i < items; i++)
            {
                var itemId = $"item-{i}";
                this.identityMap.GetOrAdd(itemId, () => this.CreateEntity(itemId), storeInfinitely: false).SetCount(i);

                foo.Count.ShouldBe(1337);
                this.identityMap.GetOrAdd(persistentItemId, () => this.CreateEntity(persistentItemId), storeInfinitely: false).Count.ShouldBe(1337);
            }

            this.identityMap.LifetimeItemsAdded.ShouldBe(items + 1);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        public void Making_many_duplicates(int times)
        {
            var persistentItemId = $"Making_many_duplicates_{times}";
            this.identityMap.GetOrAdd(persistentItemId, () => this.CreateEntity(persistentItemId), storeInfinitely: false).SetCount(1337);

            for (var i = 0; i < times; i++)
            {
                var foo = this.identityMap.GetOrAdd(persistentItemId, () => this.CreateEntity(persistentItemId), storeInfinitely: false);
                foo.Count.ShouldBe(1337);
            }

            this.identityMap.LifetimeItemsAdded.ShouldBe(1);
        }

        public void Dispose()
        {
            this.identityMap.Dispose();
        }
    }
}
