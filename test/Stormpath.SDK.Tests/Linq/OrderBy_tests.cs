// <copyright file="OrderBy_tests.cs" company="Stormpath, Inc.">
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

using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Xunit;

namespace Stormpath.SDK.Tests.Linq
{
    public class OrderBy_tests : Linq_test<IAccount>
    {
        [Fact]
        public async Task Sort_by_field()
        {
            await this.Queryable
                .OrderBy(x => x.GivenName)
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("orderBy=givenName");
        }

        [Fact]
        public async Task Sort_by_field_descending()
        {
            await this.Queryable
                .OrderByDescending(x => x.Email)
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("orderBy=email+desc");
        }

        [Fact]
        public async Task Sort_with_additional_expressions_before()
        {
            await this.Queryable
                .Take(10)
                .OrderBy(x => x.GivenName)
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("limit=10", "orderBy=givenName");
        }

        [Fact]
        public async Task Sort_with_additional_expressions_after()
        {
            await this.Queryable
                .OrderByDescending(x => x.GivenName)
                .Take(10)
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("limit=10", "orderBy=givenName+desc");
        }

        [Fact]
        public async Task Sort_by_multiple_fields()
        {
            await this.Queryable
                .OrderBy(x => x.GivenName)
                .ThenByDescending(x => x.Username)
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("orderBy=givenName,username+desc");
        }
    }
}
