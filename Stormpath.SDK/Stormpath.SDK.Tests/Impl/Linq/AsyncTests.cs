// <copyright file="AsyncTests.cs" company="Stormpath, Inc.">
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Tests.Mocks;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    [TestClass]
    public class AsyncTests
    {
        private static string url = "http://f.oo";
        private static string resource = "bar";

        [TestClass]
        public class FirstAsync
        {
            [TestMethod]
            [TestCategory("Impl.Linq")]
            public async Task FirstAsync_returns_first()
            {
                var fakeDataStore = new FakeDataStore<IAccount>(new List<IAccount>()
                {
                    FakeAccounts.LukeSkywalker,
                    FakeAccounts.HanSolo
                });
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, fakeDataStore);

                var luke = await harness.Queryable.FirstAsync();

                luke.Surname.ShouldBe("Skywalker");
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public void FirstAsync_throws_when_no_items_exist()
            {
                var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Empty<IAccount>());
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, fakeDataStore);

                Should.Throw<InvalidOperationException>(async () =>
                {
                    var jabba = await harness.Queryable.FirstAsync();
                });
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public async Task FirstOrDefaultAsync_returns_first()
            {
                var fakeDataStore = new FakeDataStore<IAccount>(new List<IAccount>()
                {
                    FakeAccounts.LukeSkywalker,
                    FakeAccounts.HanSolo
                });
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, fakeDataStore);

                var luke = await harness.Queryable.FirstOrDefaultAsync();

                luke.Surname.ShouldBe("Skywalker");
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public async Task FirstOrDefaultAsync_returns_null_when_no_items_exist()
            {
                var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Empty<IAccount>());
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, fakeDataStore);

                var notLuke = await harness.Queryable.FirstOrDefaultAsync();

                notLuke.ShouldBe(null);
            }
        }

        [TestClass]
        public class SingleAsync
        {
            [TestMethod]
            [TestCategory("Impl.Linq")]
            public async Task SingleAsync_returns_one()
            {
                var fakeDataStore = new FakeDataStore<IAccount>(new List<IAccount>()
                {
                    FakeAccounts.HanSolo
                });
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, fakeDataStore);

                var han = await harness.Queryable.SingleAsync();

                han.Surname.ShouldBe("Solo");
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public void SingleAsync_throws_when_more_than_one_item_exists()
            {
                var fakeDataStore = new FakeDataStore<IAccount>(new List<IAccount>()
                {
                    FakeAccounts.HanSolo,
                    FakeAccounts.LukeSkywalker
                });
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, fakeDataStore);

                Should.Throw<InvalidOperationException>(async () =>
                {
                    var han = await harness.Queryable.SingleAsync();
                });
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public void SingleAsync_throws_when_no_items_exist()
            {
                var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Empty<IAccount>());
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, fakeDataStore);

                Should.Throw<InvalidOperationException>(async () =>
                {
                    var jabba = await harness.Queryable.SingleAsync();
                });
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public async Task SingleOrDefaultAsync_returns_one()
            {
                var fakeDataStore = new FakeDataStore<IAccount>(new List<IAccount>()
                {
                    FakeAccounts.HanSolo
                });
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, fakeDataStore);

                var han = await harness.Queryable.SingleOrDefaultAsync();

                han.Surname.ShouldBe("Solo");
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public void SingleOrDefaultAsync_throws_when_more_than_one_item_exists()
            {
                var fakeDataStore = new FakeDataStore<IAccount>(new List<IAccount>()
                {
                    FakeAccounts.HanSolo,
                    FakeAccounts.LukeSkywalker
                });
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, fakeDataStore);

                Should.Throw<InvalidOperationException>(async () =>
                {
                    var han = await harness.Queryable.SingleOrDefaultAsync();
                });
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public async Task SingleOrDefaultAsync_returns_null_when_no_items_exist()
            {
                var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Empty<IAccount>());
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, fakeDataStore);

                var notHan = await harness.Queryable.SingleOrDefaultAsync();

                notHan.ShouldBe(null);
            }
        }

        [TestClass]
        public class MyTestClass
        {
            [TestMethod]
            [TestCategory("Impl.Linq")]
            public async Task ToListAsync_returns_empty_list_for_no_items()
            {
                var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Empty<IAccount>());
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, fakeDataStore);

                var empty = await harness.Queryable.ToListAsync();

                empty.ShouldBeEmpty();
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public async Task ToListAsync_retrieves_all_items()
            {
                var fakeDataStore = new FakeDataStore<IAccount>(FakeAccounts.RebelAlliance);
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, fakeDataStore);

                var alliance = await harness.Queryable.ToListAsync();

                alliance.Count.ShouldBe(FakeAccounts.RebelAlliance.Count);
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public async Task ToListAsync_checks_for_new_items_after_last_page()
            {
                // Scenario: 51 items in a server-side collection. The default limit is 25,
                // so two calls will return 25 items, and the 3rd will return 1. However, ToListAsync
                // will make another call to the server, just to make sure another item hasn't been added
                // to the end while we were enumerating.
                var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Repeat<IAccount>(new FakeAccount(), 51));
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, fakeDataStore);

                var longList = await harness.Queryable.ToListAsync();

                longList.Count.ShouldBe(51);
                fakeDataStore.GetCalls().Count().ShouldBe(4);
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public async Task ForEachAsync_operates_on_every_item()
            {
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource,
                    new FakeDataStore<IAccount>(FakeAccounts.RebelAlliance));
                var gmailAlliance = new List<string>();

                await harness.Queryable.ForEachAsync(acct =>
                {
                    gmailAlliance.Add($"{acct.GivenName.ToLower()}@gmail.com");
                });

                gmailAlliance.Count.ShouldBe(FakeAccounts.RebelAlliance.Count);
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public async Task ForEachAsync_indexes_every_item()
            {
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource,
                    new FakeDataStore<IAccount>(FakeAccounts.GalacticEmpire));
                var empireFirstNameLookup = new Dictionary<int, string>();

                await harness.Queryable.ForEachAsync((acct, index) =>
                {
                    empireFirstNameLookup.Add(index, $"{acct.GivenName} {acct.Surname}");
                });

                empireFirstNameLookup[2].ShouldBe(FakeAccounts.GalacticEmpire.ElementAt(2).GivenName + " " + FakeAccounts.GalacticEmpire.ElementAt(2).Surname);
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public async Task ForEachAsync_can_be_cancelled()
            {
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource,
                    new FakeDataStore<IAccount>(FakeAccounts.GalacticEmpire));
                var cts = new CancellationTokenSource();
                var reachedIndex = -1;

                try
                {
                    await harness.Queryable.ForEachAsync((acct, index) =>
                    {
                        reachedIndex = index;

                        if (index == 2)
                            cts.Cancel();
                    }, cts.Token);

                    Assert.Fail("Should not reach here!");
                }
                catch (OperationCanceledException)
                {
                    reachedIndex.ShouldBe(2);
                }
            }
        }
    }
}
