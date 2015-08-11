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
                var mockDataStore = new MockDataStore<IAccount>(new List<IAccount>()
                {
                    new MockAccount() { GivenName = "Luke", Surname = "Skywalker" },
                    new MockAccount() { GivenName = "Han", Surname = "Solo" }
                });
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, mockDataStore);

                var luke = await harness.Queryable.FirstAsync();

                luke.Surname.ShouldBe("Skywalker");
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public void FirstAsync_throws_when_no_items_exist()
            {
                var mockDataStore = new MockDataStore<IAccount>(Enumerable.Empty<IAccount>());
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, mockDataStore);

                Should.Throw<InvalidOperationException>(async () =>
                {
                    var jabba = await harness.Queryable.FirstAsync();
                });
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public async Task FirstOrDefaultAsync_returns_first()
            {
                var mockDataStore = new MockDataStore<IAccount>(new List<IAccount>()
                {
                    new MockAccount() { GivenName = "Luke", Surname = "Skywalker" },
                    new MockAccount() { GivenName = "Han", Surname = "Solo" }
                });
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, mockDataStore);

                var luke = await harness.Queryable.FirstOrDefaultAsync();

                luke.Surname.ShouldBe("Skywalker");
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public async Task FirstOrDefaultAsync_returns_null_when_no_items_exist()
            {
                var mockDataStore = new MockDataStore<IAccount>(Enumerable.Empty<IAccount>());
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, mockDataStore);

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
                var mockDataStore = new MockDataStore<IAccount>(new List<IAccount>()
                {
                    new MockAccount() { GivenName = "Han", Surname = "Solo" },
                });
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, mockDataStore);

                var han = await harness.Queryable.SingleAsync();

                han.Surname.ShouldBe("Solo");
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public void SingleAsync_throws_when_more_than_one_item_exists()
            {
                var mockDataStore = new MockDataStore<IAccount>(new List<IAccount>()
                {
                    new MockAccount() { GivenName = "Han", Surname = "Solo" },
                    new MockAccount() { GivenName = "Luke", Surname = "Skywalker" }
                });
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, mockDataStore);

                Should.Throw<InvalidOperationException>(async () =>
                {
                    var han = await harness.Queryable.SingleAsync();
                });
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public void SingleAsync_throws_when_no_items_exist()
            {
                var mockDataStore = new MockDataStore<IAccount>(Enumerable.Empty<IAccount>());
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, mockDataStore);

                Should.Throw<InvalidOperationException>(async () =>
                {
                    var jabba = await harness.Queryable.SingleAsync();
                });
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public async Task SingleOrDefaultAsync_returns_one()
            {
                var mockDataStore = new MockDataStore<IAccount>(new List<IAccount>()
                {
                    new MockAccount() { GivenName = "Han", Surname = "Solo" },
                });
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, mockDataStore);

                var han = await harness.Queryable.SingleOrDefaultAsync();

                han.Surname.ShouldBe("Solo");
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public void SingleOrDefaultAsync_throws_when_more_than_one_item_exists()
            {
                var mockDataStore = new MockDataStore<IAccount>(new List<IAccount>()
                {
                    new MockAccount() { GivenName = "Han", Surname = "Solo" },
                    new MockAccount() { GivenName = "Luke", Surname = "Skywalker" }
                });
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, mockDataStore);

                Should.Throw<InvalidOperationException>(async () =>
                {
                    var han = await harness.Queryable.SingleOrDefaultAsync();
                });
            }

            [TestMethod]
            [TestCategory("Impl.Linq")]
            public async Task SingleOrDefaultAsync_returns_null_when_no_items_exist()
            {
                var mockDataStore = new MockDataStore<IAccount>(Enumerable.Empty<IAccount>());
                var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource, mockDataStore);

                var notHan = await harness.Queryable.SingleOrDefaultAsync();

                notHan.ShouldBe(null);
            }
        }
    }
}
