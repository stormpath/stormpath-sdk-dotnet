// <copyright file="Multi_threaded_tests.cs" company="Stormpath, Inc.">
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
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Impl.IdentityMap;
using Stormpath.SDK.Logging;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.IdentityMap
{
    public class Multi_threaded_tests : IDisposable
    {
        private readonly IIdentityMap<TestEntity> identityMap;

        public Multi_threaded_tests()
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
        public void Creating_items_in_another_thread()
        {
            var foo = this.identityMap.GetOrAdd("foo", () => this.CreateEntity("foo"), storeInfinitely: false);
            foo.SetCount(17);

            var tasks = new List<Task>();

            for (var i = 0; i < 5; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    var itemId = Guid.NewGuid().ToString();
                    var bar = this.identityMap.GetOrAdd(itemId, () => this.CreateEntity(itemId), storeInfinitely: false);
                    bar.SetCount(i * 10);

                    var fooAgain = this.identityMap.GetOrAdd("foo", () => this.CreateEntity("foo"), storeInfinitely: false);
                    fooAgain.Count.ShouldBe(17);
                }));
            }

            Task.WhenAll(tasks).Wait();
            foo.Count.ShouldBe(17);
            this.identityMap.LifetimeItemsAdded.ShouldBe(6);
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

            Parallel.For(0, items, i =>
            {
                var itemId = $"item-{i}";
                this.identityMap.GetOrAdd(itemId, () => this.CreateEntity(itemId), storeInfinitely: false).SetCount(i);

                foo.Count.ShouldBe(1337);
                this.identityMap.GetOrAdd(persistentItemId, () => this.CreateEntity(persistentItemId), storeInfinitely: false).Count.ShouldBe(1337);
            });

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

            Parallel.For(0, times, i =>
            {
                var foo = this.identityMap.GetOrAdd(persistentItemId, () => this.CreateEntity(persistentItemId), storeInfinitely: false);
                foo.Count.ShouldBe(1337);
            });

            this.identityMap.LifetimeItemsAdded.ShouldBe(1);
        }

        public void Dispose()
        {
            this.identityMap.Dispose();
        }
    }
}
