// <copyright file="AnyAsync_tests.cs" company="Stormpath, Inc.">
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

using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Tests.Common.Fakes;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class AnyAsync_tests : Linq_test<IAccount>
    {
        [Fact]
        public async Task Returns_false_for_empty_collection()
        {
            this.InitializeClientWithCollection(Enumerable.Empty<IAccount>());

            (await this.Queryable.AnyAsync()).ShouldBeFalse();
        }

        [Fact]
        public async Task Returns_true_for_nonempty_collection()
        {
            this.InitializeClientWithCollection(Enumerable.Repeat(TestAccounts.R2D2, 73));

            (await this.Queryable.AnyAsync()).ShouldBeTrue();
        }

        [Fact]
        public async Task Limits_result_to_one_item()
        {
            this.InitializeClientWithCollection(Enumerable.Empty<IAccount>());
            await this.Queryable.AnyAsync();

            this.ShouldBeCalledWithArguments("limit=1");
        }
    }
}
