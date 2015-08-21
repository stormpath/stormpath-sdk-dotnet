// <copyright file="CollectionResourceQueryable_iteration_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class CollectionResourceQueryable_iteration_tests
    {
        private static string href = "http://f.oo/bar";

        [Fact]
        public async Task First_iteration_does_not_modify_pagination_arguments()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(
                href,
                new FakeDataStore<IAccount>(FakeAccounts.RebelAlliance));

            var query = harness.Queryable
                .Take(5)
                .Skip(10);

            await (query as ICollectionResourceQueryable<IAccount>).MoveNextAsync();
            harness.DataStore.WasCalledWithArguments<IAccount>(href, "limit=5&offset=10");
        }

        [Fact]
        public async Task First_iteration_does_not_add_pagination_arguments()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(
                href,
                new FakeDataStore<IAccount>(FakeAccounts.RebelAlliance));

            var query = harness.Queryable;

            await query.MoveNextAsync();
            harness.DataStore.WasCalledWithArguments<IAccount>(href, string.Empty);
        }

        [Fact]
        public async Task Subsequent_iterations_add_pagination_arguments_if_none_exist()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(
                href,
                new FakeDataStore<IAccount>(Enumerable.Repeat(new FakeAccount(), 50)));

            var query = harness.Queryable;

            await query.MoveNextAsync();
            var firstPageCount = query.CurrentPage.Count();
            await query.MoveNextAsync();
            harness.DataStore.WasCalledWithArguments<IAccount>(href, $"offset={firstPageCount}");
        }
    }
}
