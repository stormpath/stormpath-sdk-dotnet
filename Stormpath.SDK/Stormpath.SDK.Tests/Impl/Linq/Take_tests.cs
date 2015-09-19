// <copyright file="Take_tests.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class Take_tests : Linq_tests
    {
        [Fact]
        public void Take_with_constant_becomes_limit()
        {
            var query = this.Harness.Queryable
                .Take(10);

            query.GeneratedArgumentsWere(this.Href, "limit=10");
        }

        [Fact]
        public void Take_with_variable_becomes_limit()
        {
            var limit = 20;
            var query = this.Harness.Queryable
                .Take(limit);

            query.GeneratedArgumentsWere(this.Href, "limit=20");
        }

        [Fact]
        public void Take_with_function_becomes_limit()
        {
            var limitFunc = new Func<int>(() => 25);
            var query = this.Harness.Queryable
                .Take(limitFunc());

            query.GeneratedArgumentsWere(this.Href, "limit=25");
        }

        [Fact]
        public void Multiple_calls_are_LIFO()
        {
            var query = this.Harness.Queryable
                .Take(10).Take(5);

            // Expected behavior: the last call will be kept
            query.GeneratedArgumentsWere(this.Href, "limit=5");
        }

        [Fact]
        public void Throws_for_invalid_value()
        {
            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                var query = this.Harness.Queryable
                    .Take(-1);

                query.GeneratedArgumentsWere(this.Href, "<not evaluated>");
            });
        }

        [Fact]
        public async Task ToListAsync_observes_take_limit()
        {
            // Scenario: .Take() functions a little differently than the limit=? parameter
            // in Stormpath, even though that's what it translates to. .Take() represents an
            // upper limit to the items that are returned. Take(5) returns 5 items, Take(500) returns 500.
            // In the underyling API, the limi=? parameter has a hard maximum of 100.
            var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Repeat(new FakeAccount(), 250));
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(this.Href, fakeDataStore);

            var longList = await harness.Queryable
                .Take(7)
                .ToListAsync();

            longList.Count.ShouldBe(7);
            fakeDataStore.GetCalls().Count().ShouldBe(1);

            // When Taking(<= 100), the limit parameter should match
            fakeDataStore.GetCalls().First().ShouldEndWith("?limit=7");
        }

        [Fact]
        public async Task ToListAsync_pages_until_take_limit_is_reached()
        {
            // Scenario: .Take() functions a little differently than the limit=? parameter
            // in Stormpath, even though that's what it translates to. .Take() represents an
            // upper limit to the items that are returned. Take(5) returns 5 items, Take(500) returns 500.
            // In the underyling API, the limi=? parameter has a hard maximum of 100.
            var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Repeat(new FakeAccount(), 750));
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(this.Href, fakeDataStore);

            var longList = await harness.Queryable
                .Take(500)
                .ToListAsync();

            longList.Count.ShouldBe(500);
            fakeDataStore.GetCalls().Count().ShouldBe(5); // Max 100 per call

            // When Taking(> 100), the limit parameter should be 100
            fakeDataStore.GetCalls().First().ShouldEndWith("?limit=100");
        }
    }
}
