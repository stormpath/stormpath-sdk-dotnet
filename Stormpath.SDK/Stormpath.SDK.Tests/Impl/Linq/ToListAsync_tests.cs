// <copyright file="ToListAsync_tests.cs" company="Stormpath, Inc.">
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

using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Tests.Common.Fakes;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class ToListAsync_tests : Linq_tests
    {
        [Fact]
        public async Task Returns_empty_list_for_no_items()
        {
            var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Empty<IAccount>());
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(this.Href, fakeDataStore);

            var empty = await harness.Queryable.ToListAsync();

            empty.ShouldBeEmpty();
        }

        [Fact]
        public async Task Retrieves_all_items()
        {
            var fakeDataStore = new FakeDataStore<IAccount>(TestAccounts.RebelAlliance);
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(this.Href, fakeDataStore);

            var alliance = await harness.Queryable.ToListAsync();

            alliance.Count.ShouldBe(TestAccounts.RebelAlliance.Count);
        }

        [Fact]
        public async Task Checks_for_new_items_after_last_page()
        {
            // Scenario: 51 items in a server-side collection. The default limit is 25,
            // so two calls will return 25 items, and the 3rd will return 1. However, ToListAsync
            // will make another call to the server, just to make sure another item hasn't been added
            // to the end while we were enumerating.
            var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Repeat(TestAccounts.Chewbacca, 51));
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(this.Href, fakeDataStore);

            var longList = await harness.Queryable.ToListAsync();

            longList.Count.ShouldBe(51);
            fakeDataStore.GetCalls().Count().ShouldBe(4);
        }
    }
}
