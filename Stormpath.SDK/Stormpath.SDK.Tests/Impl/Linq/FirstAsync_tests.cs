// <copyright file="FirstAsync_tests.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class FirstAsync_tests : Linq_tests
    {
        [Fact]
        public async Task FirstAsync_returns_first()
        {
            var fakeDataStore = new FakeDataStore<IAccount>(new List<IAccount>()
                {
                    FakeAccounts.LukeSkywalker,
                    FakeAccounts.HanSolo
                });
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(Href, fakeDataStore);

            var luke = await harness.Queryable.FirstAsync();

            luke.Surname.ShouldBe("Skywalker");
        }

        [Fact]
        public void FirstAsync_throws_when_no_items_exist()
        {
            var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Empty<IAccount>());
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(Href, fakeDataStore);

            // TODO InvalidOperationException (after xUnit 2.0)
            Should.Throw<Exception>(async () =>
            {
                var jabba = await harness.Queryable.FirstAsync();
            });
        }

        [Fact]
        public async Task FirstOrDefaultAsync_returns_first()
        {
            var fakeDataStore = new FakeDataStore<IAccount>(new List<IAccount>()
                {
                    FakeAccounts.LukeSkywalker,
                    FakeAccounts.HanSolo
                });
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(Href, fakeDataStore);

            var luke = await harness.Queryable.FirstOrDefaultAsync();

            luke.Surname.ShouldBe("Skywalker");
        }

        [Fact]
        public async Task FirstOrDefaultAsync_returns_null_when_no_items_exist()
        {
            var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Empty<IAccount>());
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(Href, fakeDataStore);

            var notLuke = await harness.Queryable.FirstOrDefaultAsync();

            notLuke.ShouldBe(null);
        }
    }
}
