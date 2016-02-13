// <copyright file="SyncAdapter_tests.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
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

namespace Stormpath.SDK.Tests.Linq
{
    public class SyncAdapter_tests : Linq_test<IAccount>
    {
        [Fact]
        public void Iterates_through_collection()
        {
            this.InitializeClientWithCollection(Enumerable.Repeat(TestAccounts.DarthVader, 52));

            var items = this.Queryable
                .Synchronously()
                .Skip(10)
                .ToList();

            items.Count.ShouldBe(42);
        }

        [Fact]
        public void Take_is_observed()
        {
            this.InitializeClientWithCollection(Enumerable.Repeat(TestAccounts.DarthVader, 52));

            var items = this.Queryable
                .Synchronously()
                .Take(5)
                .ToList();

            items.Count.ShouldBe(5);
        }
    }
}