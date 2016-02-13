// <copyright file="Skip_tests.cs" company="Stormpath, Inc.">
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
using Xunit;

namespace Stormpath.SDK.Tests.Linq
{
    public class Skip_tests : Linq_test<IAccount>
    {
        [Fact]
        public async Task Skip_becomes_offset()
        {
            await this.Queryable
                .Skip(10)
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("offset=10");
        }

        [Fact]
        public async Task Skip_with_variable_becomes_offset()
        {
            var offset = 20;
            await this.Queryable
                .Skip(offset)
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("offset=20");
        }

        [Fact]
        public async Task Skip_with_function_becomes_offset()
        {
            var offsetFunc = new Func<int>(() => 25);
            await this.Queryable
                .Skip(offsetFunc())
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("offset=25");
        }

        [Fact]
        public async Task Multiple_calls_are_LIFO()
        {
            await this.Queryable
                .Skip(10).Skip(5)
                .MoveNextAsync();

            // Expected behavior: the last call will be kept
            this.ShouldBeCalledWithArguments("offset=5");
        }

        [Fact]
        public async Task Zero_is_ignored()
        {
            await this.Queryable
                .Skip(0)
                .MoveNextAsync();

            // Expected behavior: the last call will be kept
            this.FakeHttpClient.Calls.Single().CanonicalUri.ToString().ShouldNotContain("offset");
        }

        [Fact]
        public async Task Throws_for_invalid_value()
        {
            // TODO ArgumentOutOfRangeException after Shouldly Mono fix
            await Should.ThrowAsync<Exception>(async () =>
            {
                await this.Queryable
                    .Skip(-1)
                .MoveNextAsync();
            });
        }
    }
}
