// <copyright file="Sync_ToList_tests.cs" company="Stormpath, Inc.">
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
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class Sync_ToList_tests : Linq_tests
    {
        [Fact]
        public void Returns_empty_list_for_no_items()
        {
            var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Empty<IAccount>());
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(this.Href, fakeDataStore);

            var empty = harness.Queryable.Synchronously().ToList();

            empty.ShouldBeEmpty();
        }

        [Fact]
        public void Retrieves_all_items()
        {
            var fakeDataStore = new FakeDataStore<IAccount>(FakeAccounts.RebelAlliance);
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(this.Href, fakeDataStore);

            var alliance = harness.Queryable.Synchronously().ToList();

            alliance.Count.ShouldBe(FakeAccounts.RebelAlliance.Count);
        }

        [Fact]
        public void Checks_for_new_items_after_last_page()
        {
            // Scenario: 51 items in a server-side collection. The default limit is 25,
            // so two calls will return 25 items, and the 3rd will return 1. However, ToListAsync
            // will make another call to the server, just to make sure another item hasn't been added
            // to the end while we were enumerating.
            var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Repeat(new FakeAccount(), 51));
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(this.Href, fakeDataStore);

            var longList = harness.Queryable.Synchronously().ToList();

            longList.Count.ShouldBe(51);
            fakeDataStore.GetCalls().Count().ShouldBe(4);
        }
    }
}
