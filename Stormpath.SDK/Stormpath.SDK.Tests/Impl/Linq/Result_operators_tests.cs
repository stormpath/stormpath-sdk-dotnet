// <copyright file="Result_operators_tests.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class Result_operators_tests : Linq_tests
    {
        public Result_operators_tests()
            : base(new FakeDataStore<IAccount>(FakeAccounts.RebelAlliance))
        {
        }

        [Fact]
        public void First_generates_proper_arguments()
        {
            // Execution behavior:
            // Limit the query to 1 result so we can minimize transfer over the wire
            var firstRebel = this.Harness.Queryable
                .First();

            this.Harness.DataStore.WasCalledWithArguments<IAccount>(this.Href, "limit=1");
            firstRebel.ShouldBe(FakeAccounts.RebelAlliance.First());
        }

        [Fact]
        public void FirstOrDefault_generates_proper_arguments()
        {
            // (Empty data store)
            this.Harness = CollectionTestHarness<IAccount>.Create<IAccount>(this.Href);

            // Execution behavior:
            // Limit the query to 1 result so we can minimize transfer over the wire
            var firstRebel = this.Harness.Queryable
                .FirstOrDefault();

            this.Harness.DataStore.WasCalledWithArguments<IAccount>(this.Href, "limit=1");
            firstRebel.ShouldBe(null);
        }

        [Fact]
        public void Single_generates_proper_arguments()
        {
            this.Harness = CollectionTestHarness<IAccount>.Create<IAccount>(
                this.Href,
                new FakeDataStore<IAccount>(new List<IAccount>() { FakeAccounts.BobaFett }));

            // Execution behavior:
            // Limit the query to 1 result so we can minimize transfer over the wire
            var boba = this.Harness.Queryable
                .Single();

            this.Harness.DataStore.WasCalledWithArguments<IAccount>(this.Href, "limit=1");
            boba.ShouldBe(FakeAccounts.BobaFett);
        }

        [Fact]
        public void SingleOrDefault_generates_proper_arguments()
        {
            // (Empty data store)
            this.Harness = CollectionTestHarness<IAccount>.Create<IAccount>(this.Href);

            // Execution behavior:
            // Limit the query to 1 result so we can minimize transfer over the wire
            var boba = this.Harness.Queryable
                .SingleOrDefault();

            this.Harness.DataStore.WasCalledWithArguments<IAccount>(this.Href, "limit=1");
            boba.ShouldBe(null);
        }
    }
}
