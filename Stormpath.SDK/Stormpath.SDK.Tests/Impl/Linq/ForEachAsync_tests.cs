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
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Tests.Common;
using Stormpath.SDK.Tests.Common.Fakes;
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
        public async Task Can_be_cancelled()
        {
            this.InitializeClientWithCollection(TestAccounts.GalacticEmpire);
            var cts = new CancellationTokenSource();
            var reachedIndex = -1;

            try
            {
                await this.Queryable.ForEachAsync(
                    acct =>
                {
                    reachedIndex++;

                    if (reachedIndex == 2)
                    {
                        cts.Cancel();
                    }
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
    }
}
