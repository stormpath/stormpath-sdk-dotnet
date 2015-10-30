// <copyright file="Complicated_tests.cs" company="Stormpath, Inc.">
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
using System.Linq;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    /// <summary>
    /// Tests that combine many different query expressions and formats.
    /// These are especially important for any refactoring of the underlying LINQ code.
    /// </summary>
    public class Complicated_tests : Linq_tests
    {
        private static string[] GetArguments(string href)
        {
            return href
                .Split('?')[1]
                .Split('&');
        }

        [Fact]
        public void Case_1()
        {
            var query = this.Harness.Queryable
                .Filter("vader")
                .Where(x => x.Email.EndsWith("@galacticempire.co") &&
                            x.Status == AccountStatus.Enabled)
                .Where(x => x.CreatedAt.Within(2015))
                .Skip(0)
                .Take(1);

            var arguments = GetArguments(query.GetGeneratedHref());

            arguments.ShouldContain("q=vader");
            arguments.ShouldContain("email=*@galacticempire.co");
            arguments.ShouldContain("status=ENABLED");
            arguments.ShouldContain("createdAt=2015");
            arguments.ShouldContain("limit=1");
            arguments.Count().ShouldBe(5);
        }

        [Fact]
        public void Case_2()
        {
            var query = this.Harness.Queryable
                .OrderBy(x => x.Email)
                .ThenByDescending(x => x.Username)
                .Skip(10)
                .Where(x => x.ModifiedAt.Within(2015, 1) && x.MiddleName == "test")
                .Where(x => x.GivenName.Equals("foo"))
                .Where(x => x.CreatedAt >= new DateTimeOffset(2015, 1, 1, 0, 0, 0, TimeSpan.Zero) && x.CreatedAt < new DateTimeOffset(2015, 6, 30, 0, 0, 0, TimeSpan.Zero));

            var arguments = GetArguments(query.GetGeneratedHref());

            arguments.ShouldContain("orderBy=email,username desc");
            arguments.ShouldContain("offset=10");
            arguments.ShouldContain("modifiedAt=2015-01");
            arguments.ShouldContain("middleName=test");
            arguments.ShouldContain("givenName=foo");
            arguments.ShouldContain("createdAt=[2015-01-01T00:00:00Z,2015-06-30T00:00:00Z)");
            arguments.Count().ShouldBe(6);
        }

        [Fact]
        public void Case_3()
        {
            var query = this.Harness.Queryable
                .Filter("luke")
                .Expand(x => x.GetCustomDataAsync)
                .Expand(x => x.GetDirectoryAsync)
                .Where(x => x.Status == AccountStatus.Unverified)
                .Expand(x => x.GetGroups, 10, 10)
                .Take(2);

            var arguments = GetArguments(query.GetGeneratedHref());

            arguments.ShouldContain("q=luke");
            arguments.ShouldContain("expand=customData,directory,groups(offset:10,limit:10)");
            arguments.ShouldContain("status=UNVERIFIED");
            arguments.ShouldContain("limit=2");
            arguments.Count().ShouldBe(4);
        }
    }
}
