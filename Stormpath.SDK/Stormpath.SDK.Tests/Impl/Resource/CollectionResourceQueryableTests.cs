// <copyright file="CollectionResourceQueryableTests.cs" company="Stormpath, Inc.">
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

using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stormpath.SDK.Account;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tests.Mocks;

namespace Stormpath.SDK.Tests.Impl.Resource
{
    [TestClass]
    public class CollectionResourceQueryableTests
    {
        private static string url = "http://f.oo";
        private static string resource = "bar";

        [TestMethod]
        [TestCategory("Impl.Resource")]
        public async Task First_iteration_does_not_modify_pagination_arguments()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource,
                new FakeDataStore<IAccount>(FakeAccounts.RebelAlliance));

            var query = harness.Queryable
                .Take(5)
                .Skip(10);

            await (query as ICollectionResourceQueryable<IAccount>).MoveNextAsync();
            harness.DataStore.WasCalledWithArguments<IAccount>(url, resource, "limit=5&offset=10");
        }

        [TestMethod]
        [TestCategory("Impl.Resource")]
        public async Task First_iteration_does_not_add_pagination_arguments()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource,
                new FakeDataStore<IAccount>(FakeAccounts.RebelAlliance));

            var query = harness.Queryable;

            await query.MoveNextAsync();
            harness.DataStore.WasCalledWithArguments<IAccount>(url, resource, string.Empty);
        }

        [TestMethod]
        [TestCategory("Impl.Resource")]
        public async Task Subsequent_iterations_increment_existing_pagination_arguments()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource,
                new FakeDataStore<IAccount>(Enumerable.Repeat(new FakeAccount(), 50)));

            var query = harness.Queryable
                .Take(5)
                .Skip(10);

            await (query as ICollectionResourceQueryable<IAccount>).MoveNextAsync();
            await (query as ICollectionResourceQueryable<IAccount>).MoveNextAsync();
            harness.DataStore.WasCalledWithArguments<IAccount>(url, resource, "limit=5&offset=15");
        }

        [TestMethod]
        [TestCategory("Impl.Resource")]
        public async Task Subsequent_iterations_add_pagination_arguments_if_none_exist()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource,
                new FakeDataStore<IAccount>(Enumerable.Repeat(new FakeAccount(), 50)));

            var query = harness.Queryable;

            await query.MoveNextAsync();
            var firstPageCount = query.CurrentPage.Count();
            await query.MoveNextAsync();
            harness.DataStore.WasCalledWithArguments<IAccount>(url, resource, $"offset={firstPageCount}");
        }
    }
}
