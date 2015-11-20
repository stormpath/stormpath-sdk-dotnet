// <copyright file="Filter_extension_tests.cs" company="Stormpath, Inc.">
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
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class Filter_extension_tests : Linq_test<IAccount>
    {
        [Fact]
        public async Task With_simple_parameter()
        {
            // Act
            await this.Queryable
                .Filter("Joe")
                .MoveNextAsync();

            // Assert
            this.FakeHttpClient.Calls.Single().ShouldContain("q=Joe");
        }

        [Fact]
        public async Task Parameters_are_trimmed()
        {
            // Act
            await this.Queryable
                .Filter("  Joe  ")
                .MoveNextAsync();

            // Assert
            this.FakeHttpClient.Calls.Single().ShouldContain("q=Joe");
        }

        [Fact]
        public async Task Throws_for_multiple_calls()
        {
            // TODO NotSupportedException after Shouldly Mono fix
            await Should.ThrowAsync<Exception>(async () =>
            {
                await this.Queryable
                    .Filter("Joe")
                    .Filter("Joey")
                    .MoveNextAsync();
            });
        }

        [Fact]
        public async Task Throws_for_null()
        {
            // TODO ArgumentException after Shouldly Mono fix
            await Should.ThrowAsync<Exception>(async () =>
            {
                await this.Queryable
                    .Filter(null)
                    .MoveNextAsync();
            });
        }

        [Fact]
        public async Task Throws_for_empty_string()
        {
            // TODO ArgumentException after Shouldly Mono fix
            await Should.ThrowAsync<Exception>(async () =>
            {
                await this.Queryable
                    .Filter(string.Empty)
                    .MoveNextAsync();
            });
        }

        [Fact]
        public async Task Throws_for_whitespace()
        {
            // TODO ArgumentException after Shouldly Mono fix
            await Should.ThrowAsync<Exception>(async () =>
            {
                await this.Queryable
                    .Filter("   ")
                    .MoveNextAsync();
            });
        }
    }
}
