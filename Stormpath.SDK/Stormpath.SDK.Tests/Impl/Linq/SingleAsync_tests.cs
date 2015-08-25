// <copyright file="SingleAsync_tests.cs" company="Stormpath, Inc.">
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
    public class SingleAsync_tests : Linq_tests
    {
        [Fact]
        public async Task SingleAsync_returns_one()
        {
            var fakeDataStore = new FakeDataStore<IAccount>(new List<IAccount>()
                {
                    FakeAccounts.HanSolo
                });
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(Href, fakeDataStore);

            var han = await harness.Queryable.SingleAsync();

            han.Surname.ShouldBe("Solo");
        }

        [Fact]
        public void SingleAsync_throws_when_more_than_one_item_exists()
        {
            var fakeDataStore = new FakeDataStore<IAccount>(new List<IAccount>()
                {
                    FakeAccounts.HanSolo,
                    FakeAccounts.LukeSkywalker
                });
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(Href, fakeDataStore);

            Should.Throw<InvalidOperationException>(async () =>
            {
                var han = await harness.Queryable.SingleAsync();
            });
        }

        [Fact]
        public void SingleAsync_throws_when_no_items_exist()
        {
            var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Empty<IAccount>());
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(Href, fakeDataStore);

            Should.Throw<InvalidOperationException>(async () =>
            {
                var jabba = await harness.Queryable.SingleAsync();
            });
        }

        [Fact]
        public async Task SingleOrDefaultAsync_returns_one()
        {
            var fakeDataStore = new FakeDataStore<IAccount>(new List<IAccount>()
                {
                    FakeAccounts.HanSolo
                });
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(Href, fakeDataStore);

            var han = await harness.Queryable.SingleOrDefaultAsync();

            han.Surname.ShouldBe("Solo");
        }

        [Fact]
        public void SingleOrDefaultAsync_throws_when_more_than_one_item_exists()
        {
            var fakeDataStore = new FakeDataStore<IAccount>(new List<IAccount>()
                {
                    FakeAccounts.HanSolo,
                    FakeAccounts.LukeSkywalker
                });
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(Href, fakeDataStore);

            Should.Throw<InvalidOperationException>(async () =>
            {
                var han = await harness.Queryable.SingleOrDefaultAsync();
            });
        }

        [Fact]
        public async Task SingleOrDefaultAsync_returns_null_when_no_items_exist()
        {
            var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Empty<IAccount>());
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(Href, fakeDataStore);

            var notHan = await harness.Queryable.SingleOrDefaultAsync();

            notHan.ShouldBe(null);
        }
    }
}
