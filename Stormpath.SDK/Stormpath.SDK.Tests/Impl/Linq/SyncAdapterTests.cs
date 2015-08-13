// <copyright file="SyncAdapterTests.cs" company="Stormpath, Inc.">
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Tests.Mocks;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    [TestClass]
    public class SyncAdapterTests
    {
        private static string url = "http://f.oo";
        private static string resource = "bar";

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void ToList_synchronously_iterates_thru_collection()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource,
                new FakeDataStore<IAccount>(Enumerable.Repeat(FakeAccounts.DarthVader, 52)));

            var items = harness.Queryable
                .Skip(10)
                .Take(5)
                .ToList();

            items.Count.ShouldBe(42);
        }
    }
}
