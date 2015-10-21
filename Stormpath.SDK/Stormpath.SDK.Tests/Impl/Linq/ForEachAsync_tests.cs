// <copyright file="ForEachAsync_tests.cs" company="Stormpath, Inc.">
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class ForEachAsync_tests : Linq_tests
    {
        [Fact]
        public async Task Operates_on_every_item()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(
                this.Href,
                new FakeDataStore<IAccount>(FakeAccounts.RebelAlliance));
            var gmailAlliance = new List<string>();

            await harness.Queryable.ForEachAsync(acct =>
            {
                gmailAlliance.Add($"{acct.GivenName.ToLower()}@gmail.com");
            });

            gmailAlliance.Count.ShouldBe(FakeAccounts.RebelAlliance.Count);
        }

        [Fact]
        public async Task Indexes_every_item()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(
                this.Href,
                new FakeDataStore<IAccount>(FakeAccounts.GalacticEmpire));
            var empireFirstNameLookup = new Dictionary<int, string>();

            await harness.Queryable.ForEachAsync((acct, index) =>
            {
                empireFirstNameLookup.Add(index, $"{acct.GivenName} {acct.Surname}");
            });

            empireFirstNameLookup[2].ShouldBe(FakeAccounts.GalacticEmpire.ElementAt(2).GivenName + " " + FakeAccounts.GalacticEmpire.ElementAt(2).Surname);
        }

        [Fact]
        public async Task Can_be_cancelled()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(
                this.Href,
                new FakeDataStore<IAccount>(FakeAccounts.GalacticEmpire));
            var cts = new CancellationTokenSource();
            var reachedIndex = -1;

            try
            {
                await harness.Queryable.ForEachAsync(
                    (acct, index) =>
                {
                    reachedIndex = index;

                    if (index == 2)
                        cts.Cancel();
                }, cts.Token);

                Assertly.Fail("Should not reach here!");
            }
            catch (OperationCanceledException)
            {
                reachedIndex.ShouldBe(2);
            }
        }

        [Fact]
        public async Task Can_break_gracefully()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(
                this.Href,
                new FakeDataStore<IAccount>(FakeAccounts.GalacticEmpire));
            var cts = new CancellationTokenSource();
            var reachedIndex = -1;

            await harness.Queryable.ForEachAsync(
                (acct, index) =>
                {
                    reachedIndex = index;

                    return index == 2;
                }, cts.Token);

            reachedIndex.ShouldBe(2);
        }
    }
}
