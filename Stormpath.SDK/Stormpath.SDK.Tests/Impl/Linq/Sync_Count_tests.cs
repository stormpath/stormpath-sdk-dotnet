// <copyright file="Sync_Count_tests.cs" company="Stormpath, Inc.">
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
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class Sync_Count_tests : Linq_tests
    {
        [Fact]
        public void Returns_count()
        {
            var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Repeat(FakeAccounts.C3PO, 73));
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(this.Href, fakeDataStore);

            var count = harness.Queryable
                .Synchronously()
                .Count();

            count.ShouldBe(73);
        }

        [Fact]
        public void Returns_long_count()
        {
            // Happy 13.0.0.0.0!
            var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Repeat(FakeAccounts.C3PO, 150)); // I realize that 150 is not an int64, it's just a simple test
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(this.Href, fakeDataStore);

            var count = harness.Queryable
                .Synchronously()
                .LongCount();

            count.ShouldBe(150);
        }
    }
}
