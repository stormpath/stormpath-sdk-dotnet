// <copyright file="Take_tests.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Tests.Common.Fakes;
using Xunit;

namespace Stormpath.SDK.Tests.Linq
{
    public class Take_tests : Linq_test<IAccount>
    {
        [Fact]
        public async Task Take_with_constant_becomes_limit()
        {
            await this.Queryable
                .Take(10)
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("limit=10");
        }

        [Fact]
        public async Task Take_with_variable_becomes_limit()
        {
            var limit = 20;
            await this.Queryable
                .Take(limit)
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("limit=20");
        }

        [Fact]
        public async Task Take_with_function_becomes_limit()
        {
            var limitFunc = new Func<int>(() => 25);
            await this.Queryable
                .Take(limitFunc())
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("limit=25");
        }

        [Fact]
        public async Task Multiple_calls_are_LIFO()
        {
            await this.Queryable
                .Take(10).Take(5)
                .MoveNextAsync();

            // Expected behavior: the last call will be kept
            this.ShouldBeCalledWithArguments("limit=5");
        }

        [Fact]
        public async Task Zero_is_ignored()
        {
            await this.Queryable
                .Take(0)
                .MoveNextAsync();

            // Expected behavior: the last call will be kept
            this.FakeHttpClient.Calls.Single().CanonicalUri.ToString().ShouldContain("limit=100");
        }

        [Fact]
        public async Task Throws_for_invalid_value()
        {
            // TODO ArgumentOutOfRangeException after Shouldly Mono fix
            await Should.ThrowAsync<Exception>(async () =>
            {
                await this.Queryable
                    .Take(-1)
                    .MoveNextAsync();
            });
        }

        [Fact]
        public async Task ToListAsync_observes_take_limit()
        {
            // Scenario: .Take() functions a little differently than the limit=? parameter
            // in Stormpath, even though that's what it translates to. .Take() represents an
            // upper limit to the items that are returned. Take(5) returns 5 items, Take(500) returns 500.
            // In the underyling API, the limi=? parameter has a hard maximum of 100.
            this.InitializeClientWithCollection(Enumerable.Repeat(TestAccounts.C3PO, 250));

            var longList = await this.Queryable
                .Take(7)
                .ToListAsync();

            longList.Count.ShouldBe(7);
            this.FakeHttpClient.Calls.Count().ShouldBe(1);

            // When Taking(<= 100), the limit parameter should match
            this.FakeHttpClient.Calls.First().CanonicalUri.ToString().ShouldEndWith("?limit=7");
        }

        [Fact]
        public async Task ToListAsync_pages_until_take_limit_is_reached()
        {
            // Scenario: .Take() functions a little differently than the limit=? parameter
            // in Stormpath, even though that's what it translates to. .Take() represents an
            // upper limit to the items that are returned. Take(5) returns 5 items, Take(500) returns 500.
            // In the underyling API, the limi=? parameter has a hard maximum of 100.
            this.InitializeClientWithCollection(Enumerable.Repeat(TestAccounts.DarthVader, 750));

            var longList = await this.Queryable
                .Take(500)
                .ToListAsync();

            longList.Count.ShouldBe(500);
            this.FakeHttpClient.Calls.Count().ShouldBe(5); // Max 100 per call

            // When Taking(> 100), the limit parameter should be 100
            this.FakeHttpClient.Calls.First().CanonicalUri.ToString().ShouldEndWith("?limit=100");
        }
    }
}
