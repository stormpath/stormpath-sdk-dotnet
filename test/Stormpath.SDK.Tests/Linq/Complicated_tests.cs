// <copyright file="Complicated_tests.cs" company="Stormpath, Inc.">
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

using System;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Xunit;

namespace Stormpath.SDK.Tests.Linq
{
    /// <summary>
    /// Tests that combine many different query expressions and formats.
    /// These are especially important for any refactoring of the underlying LINQ code.
    /// </summary>
    public class Complicated_tests : Linq_test<IAccount>
    {
        [Fact]
        public async Task Case_1()
        {
            await this.Queryable
                .Filter("vader")
                .Where(x => x.Email.EndsWith("@galacticempire.co") &&
                            x.Status == AccountStatus.Enabled)
                .Where(x => x.CreatedAt.Within(2015))
                .Skip(0)
                .Take(1)
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments(
                "q=vader",
                "email=*%40galacticempire.co",
                "status=ENABLED",
                "createdAt=2015",
                "limit=1");
        }

        [Fact]
        public async Task Case_2()
        {
            await this.Queryable
                .OrderBy(x => x.Email)
                .ThenByDescending(x => x.Username)
                .Skip(10)
                .Where(x => x.ModifiedAt.Within(2015, 1) && x.MiddleName == "test")
                .Where(x => x.GivenName.Equals("foo"))
                .Where(x => x.CreatedAt >= new DateTimeOffset(2015, 1, 1, 0, 0, 0, TimeSpan.Zero) && x.CreatedAt < new DateTimeOffset(2015, 6, 30, 0, 0, 0, TimeSpan.Zero))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments(
                "orderBy=email,username+desc",
                "offset=10",
                "modifiedAt=2015-01",
                "middleName=test",
                "givenName=foo",
                "createdAt=[2015-01-01T00:00:00Z,2015-06-30T00:00:00Z)");
        }

        [Fact]
        public async Task Case_3()
        {
            await this.Queryable
                .Filter("luke")
                .Expand(x => x.GetCustomData())
                .Expand(x => x.GetDirectory())
                .Where(x => x.Status == AccountStatus.Unverified)
                .Expand(x => x.GetGroups(10, 10))
                .Take(2)
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments(
                "q=luke",
                "expand=customData,directory,groups(offset:10,limit:10)",
                "status=UNVERIFIED",
                "limit=2");
        }
    }
}
