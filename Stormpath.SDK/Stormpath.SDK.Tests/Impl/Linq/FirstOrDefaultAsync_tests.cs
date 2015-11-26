// <copyright file="FirstOrDefaultAsync_tests.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Tests.Common.Fakes;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class FirstOrDefaultAsync_tests : Linq_test<IAccount>
    {
        [Fact]
        public async Task Returns_first_item()
        {
            this.InitializeClientWithCollection(new List<IAccount>()
                {
                    TestAccounts.LukeSkywalker,
                    TestAccounts.HanSolo
                });

            var luke = await this.Queryable.FirstOrDefaultAsync();

            luke.Surname.ShouldBe("Skywalker");
        }

        [Fact]
        public async Task Returns_null_when_no_items_exist()
        {
            var notLuke = await this.Queryable.FirstOrDefaultAsync();

            notLuke.ShouldBeNull();
        }
    }
}
