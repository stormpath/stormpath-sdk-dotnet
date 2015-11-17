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
using Stormpath.SDK.Tests.Common;
using Stormpath.SDK.Tests.Common.Fakes;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class ForEachAsync_tests : Linq_test<IAccount>
    {
        [Fact]
        public async Task Operates_on_every_item()
        {
            this.InitializeClientWithCollection(TestAccounts.RebelAlliance);
            var gmailAlliance = new List<string>();

            await this.Queryable.ForEachAsync(acct =>
            {
                gmailAlliance.Add($"{acct.GivenName.ToLower()}@gmail.com");
            });

            gmailAlliance.Count.ShouldBe(TestAccounts.RebelAlliance.Count);
        }

        [Fact]
        public async Task Operates_on_every_item_asynchronously()
        {
            this.InitializeClientWithCollection(TestAccounts.RebelAlliance);
            var gmailAlliance = new List<string>();

            Func<string, Task> addAsynchronously = async str =>
            {
                await Task.Yield();
                gmailAlliance.Add($"{str.ToLower()}@gmail.com");
            };

            await this.Queryable.ForEachAsync(acct =>
            {
                return addAsynchronously(acct.GivenName);
            });

            gmailAlliance.Count.ShouldBe(TestAccounts.RebelAlliance.Count);
        }

        [Fact]
        public async Task Indexes_every_item()
        {
            this.InitializeClientWithCollection(TestAccounts.GalacticEmpire);
            var empireFirstNameLookup = new Dictionary<int, string>();

            await this.Queryable.ForEachAsync((acct, index) =>
            {
                empireFirstNameLookup.Add(index, $"{acct.GivenName} {acct.Surname}");
            });

            empireFirstNameLookup[2].ShouldBe(TestAccounts.GalacticEmpire.ElementAt(2).GivenName + " " + TestAccounts.GalacticEmpire.ElementAt(2).Surname);
        }

        [Fact]
        public async Task Indexes_every_item_asynchronously()
        {
            this.InitializeClientWithCollection(TestAccounts.GalacticEmpire);
            var empireFirstNameLookup = new Dictionary<int, string>();

            Func<string, string, int, Task> addAsynchronously = async (str1, str2, index) =>
            {
                await Task.Yield();
                empireFirstNameLookup.Add(index, $"{str1} {str2}");
            };

            await this.Queryable.ForEachAsync(async (acct, index) =>
            {
                await addAsynchronously(acct.GivenName, acct.Surname, index);
            });

            empireFirstNameLookup[2].ShouldBe(TestAccounts.GalacticEmpire.ElementAt(2).GivenName + " " + TestAccounts.GalacticEmpire.ElementAt(2).Surname);
        }

        [Fact]
        public async Task Can_be_cancelled()
        {
            this.InitializeClientWithCollection(TestAccounts.GalacticEmpire);
            var cts = new CancellationTokenSource();
            var reachedIndex = -1;

            try
            {
                await this.Queryable.ForEachAsync(
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
            this.InitializeClientWithCollection(TestAccounts.GalacticEmpire);
            var reachedIndex = -1;

            await this.Queryable.ForEachAsync(
                acct =>
                {
                    reachedIndex++;

                    return reachedIndex == 2;
                }, CancellationToken.None);

            reachedIndex.ShouldBe(2);
        }

        [Fact]
        public async Task Can_break_gracefully_asynchronously()
        {
            this.InitializeClientWithCollection(TestAccounts.GalacticEmpire);
            var reachedIndex = -1;

            await this.Queryable.ForEachAsync(
                async acct =>
                {
                    await Task.Yield();
                    reachedIndex++;

                    return reachedIndex == 2;
                }, CancellationToken.None);

            reachedIndex.ShouldBe(2);
        }

        [Fact]
        public async Task Can_break_gracefully_when_indexing()
        {
            this.InitializeClientWithCollection(TestAccounts.GalacticEmpire);
            var reachedIndex = -1;

            await this.Queryable.ForEachAsync(
                (acct, index) =>
                {
                    reachedIndex = index;

                    return index == 2;
                }, CancellationToken.None);

            reachedIndex.ShouldBe(2);
        }

        [Fact]
        public async Task Can_break_gracefully_when_indexing_asynchronously()
        {
            this.InitializeClientWithCollection(TestAccounts.GalacticEmpire);
            var reachedIndex = -1;

            await this.Queryable.ForEachAsync(
                async (acct, index) =>
                {
                    await Task.Yield();
                    reachedIndex = index;

                    return index == 2;
                }, CancellationToken.None);

            reachedIndex.ShouldBe(2);
        }
    }
}
