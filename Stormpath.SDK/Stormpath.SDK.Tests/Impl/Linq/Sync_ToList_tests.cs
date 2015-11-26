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
using Stormpath.SDK.Tests.Common.Fakes;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class Sync_ToList_tests : Linq_test<IAccount>
    {
        [Fact]
        public void Returns_empty_list_for_no_items()
        {
            var empty = this.Queryable.Synchronously().ToList();

            empty.ShouldBeEmpty();
        }

        [Fact]
        public void Retrieves_all_items()
        {
            this.InitializeClientWithCollection(TestAccounts.RebelAlliance);

            var alliance = this.Queryable.Synchronously().ToList();

            alliance.Count.ShouldBe(TestAccounts.RebelAlliance.Count);
        }

        [Fact]
        public void Checks_for_new_items_after_last_page()
        {
            // Scenario: 51 items in a server-side collection. The default limit is 25,
            // so two calls will return 25 items, and the 3rd will return 1. However, ToListAsync
            // will make another call to the server, just to make sure another item hasn't been added
            // to the end while we were enumerating.
            this.InitializeClientWithCollection(Enumerable.Repeat(TestAccounts.BobaFett, 51));

            var longList = this.Queryable.Synchronously().ToList();

            longList.Count.ShouldBe(51);
            this.FakeHttpClient.Calls.Count().ShouldBe(4);
        }
    }
}
