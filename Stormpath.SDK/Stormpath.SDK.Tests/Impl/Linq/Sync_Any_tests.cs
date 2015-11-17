// <copyright file="Sync_Any_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class Sync_Any_tests : Linq_tests
    {
        [Fact]
        public void Returns_false_for_empty_collection()
        {
            var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Empty<IAccount>());
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(this.Href, fakeDataStore);

            harness.Queryable
                .Synchronously()
                .Any()
                .ShouldBeFalse();
        }

        [Fact]
        public void Returns_true_for_nonempty_collection()
        {
            var fakeDataStore = new FakeDataStore<IAccount>(Enumerable.Repeat(TestAccounts.R2D2, 73));
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(this.Href, fakeDataStore);

            harness.Queryable
                .Synchronously()
                .Any()
                .ShouldBeTrue();
        }
    }
}
