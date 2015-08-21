// <copyright file="ToListAsync_tests.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class ToListAsync_tests : Linq_tests
    {
        [Fact]
        public async Task ToListAsync_returns_empty_list_for_no_items()
        {
            var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Empty<IAccount>());
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(Href, fakeDataStore);

            var empty = await harness.Queryable.ToListAsync();

            empty.ShouldBeEmpty();
        }

        [Fact]
        public async Task ToListAsync_retrieves_all_items()
        {
            var fakeDataStore = new FakeDataStore<IAccount>(FakeAccounts.RebelAlliance);
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(Href, fakeDataStore);

            var alliance = await harness.Queryable.ToListAsync();

            alliance.Count.ShouldBe(FakeAccounts.RebelAlliance.Count);
        }

        [Fact]
        public async Task ToListAsync_checks_for_new_items_after_last_page()
        {
            // Scenario: 51 items in a server-side collection. The default limit is 25,
            // so two calls will return 25 items, and the 3rd will return 1. However, ToListAsync
            // will make another call to the server, just to make sure another item hasn't been added
            // to the end while we were enumerating.
            var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Repeat(new FakeAccount(), 51));
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(Href, fakeDataStore);

            var longList = await harness.Queryable.ToListAsync();

            longList.Count.ShouldBe(51);
            fakeDataStore.GetCalls().Count().ShouldBe(4);
        }

        [Fact]
        public async Task ToListAsync_observes_take_limit()
        {
            // Scenario: .Take() functions a little differently than the limit=? parameter
            // in Stormpath, even though that's what it translates to. .Take() represents an
            // upper limit to the items that are returned. Take(5) returns 5 items, Take(500) returns 500.
            var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Repeat(new FakeAccount(), 51));
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(Href, fakeDataStore);

            var longList = await harness.Queryable
                .Take(7)
                .ToListAsync();

            longList.Count.ShouldBe(7);
            fakeDataStore.GetCalls().Count().ShouldBe(1);
        }

        [Fact]
        public async Task ToListAsync_pages_until_take_limit_is_reached()
        {
            // Scenario: .Take() functions a little differently than the limit=? parameter
            // in Stormpath, even though that's what it translates to. .Take() represents an
            // upper limit to the items that are returned. Take(5) returns 5 items, Take(500) returns 500.
            var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Repeat(new FakeAccount(), 750));
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(Href, fakeDataStore);

            var longList = await harness.Queryable
                .Take(500)
                .ToListAsync();

            longList.Count.ShouldBe(500);
            fakeDataStore.GetCalls().Count().ShouldBe(5); // Max 100 per call
        }

        [Fact]
        public async Task ForEachAsync_operates_on_every_item()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(
                Href,
                new FakeDataStore<IAccount>(FakeAccounts.RebelAlliance));
            var gmailAlliance = new List<string>();

            await harness.Queryable.ForEachAsync(acct =>
            {
                gmailAlliance.Add($"{acct.GivenName.ToLower()}@gmail.com");
            });

            gmailAlliance.Count.ShouldBe(FakeAccounts.RebelAlliance.Count);
        }

        [Fact]
        public async Task ForEachAsync_indexes_every_item()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(
                Href,
                new FakeDataStore<IAccount>(FakeAccounts.GalacticEmpire));
            var empireFirstNameLookup = new Dictionary<int, string>();

            await harness.Queryable.ForEachAsync((acct, index) =>
            {
                empireFirstNameLookup.Add(index, $"{acct.GivenName} {acct.Surname}");
            });

            empireFirstNameLookup[2].ShouldBe(FakeAccounts.GalacticEmpire.ElementAt(2).GivenName + " " + FakeAccounts.GalacticEmpire.ElementAt(2).Surname);
        }

        [Fact]
        public async Task ForEachAsync_can_be_cancelled()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(
                Href,
                new FakeDataStore<IAccount>(FakeAccounts.GalacticEmpire));
            var cts = new CancellationTokenSource();
            var reachedIndex = -1;

            try
            {
                await harness.Queryable.ForEachAsync(
                    (acct, index) =>
                {
                    reachedIndex = index;

                    if (index == 2)
                        cts.Cancel();
                }, cts.Token);

                Assertly.Fail("Should not reach here!");
            }
            catch (OperationCanceledException)
            {
                reachedIndex.ShouldBe(2);
            }
        }
    }
}
