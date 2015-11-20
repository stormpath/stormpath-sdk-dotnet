// <copyright file="Where_tests.cs" company="Stormpath, Inc.">
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
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class Where_tests : Linq_test<IAccount>
    {
        [Fact]
        public async Task Throws_for_constant_true()
        {
            // TODO NotSupportedException after Shouldly Mono fix
            await Should.ThrowAsync<Exception>(async () =>
            {
                await this.Queryable
                    .Where(x => true)
                    .MoveNextAsync();
            });
        }

        [Fact]
        public async Task Tthrows_for_constant_false()
        {
            // TODO NotSupportedException after Shouldly Mono fix
            await Should.ThrowAsync<Exception>(async () =>
            {
                await this.Queryable
                    .Where(x => false)
                    .MoveNextAsync();
            });
        }

        [Fact]
        public async Task Throws_for_unsupported_comparison_operators()
        {
            // TODO NotSupportedException after Shouldly Mono fix
            await Should.ThrowAsync<Exception>(async () =>
            {
                await this.Queryable
                    .Where(x => x.Email != "foo")
                    .MoveNextAsync();
            });
        }

        [Fact]
        public async Task Throws_for_complex_overload_of_Equals()
        {
            // TODO NotSupportedException after Shouldly Mono fix
            await Should.ThrowAsync<Exception>(async () =>
            {
                await this.Queryable
                    .Where(x => x.Email.Equals("bar", StringComparison.CurrentCulture))
                    .MoveNextAsync();
            });
        }

        [Fact]
        public async Task Throws_for_complex_overload_of_StartsWith()
        {
            // TODO NotSupportedException after Shouldly Mono fix
            await Should.ThrowAsync<Exception>(async () =>
            {
                await this.Queryable
                    .Where(x => x.Email.StartsWith("foo", StringComparison.InvariantCultureIgnoreCase))
                    .MoveNextAsync();
            });
        }

        [Fact]
        public async Task Throws_for_unsupported_helper_methods()
        {
            // TODO NotSupportedException after Shouldly Mono fix
            await Should.ThrowAsync<Exception>(async () =>
            {
                await this.Queryable
                    .Where(x => x.Email.ToUpper() == "FOO")
                    .MoveNextAsync();
            });
        }

        [Fact]
        public async Task Throws_for_binary_or()
        {
            // TODO NotSupportedException after Shouldly Mono fix
            await Should.ThrowAsync<Exception>(async () =>
            {
                await this.Queryable
                    .Where(x => x.Email == "foo" || x.Email == "bar")
                    .MoveNextAsync();
            });
        }

        [Fact]
        public async Task Where_attribute_equals()
        {
            await this.Queryable
                .Where(x => x.Email == "tk421@deathstar.co")
                .MoveNextAsync();

            this.FakeHttpClient.Calls.Single().ShouldContain("email=tk421%40deathstar.co");
        }

        [Fact]
        public async Task Where_attribute_equals_using_helper_method()
        {
            await this.Queryable
                .Where(x => x.Email.Equals("tk421@deathstar.co"))
                .MoveNextAsync();

            this.FakeHttpClient.Calls.Single().ShouldContain("email=tk421%40deathstar.co");
        }

        [Fact]
        public async Task Where_attribute_equals_using_variable()
        {
            var email = "tk421@deathstar.co";

            await this.Queryable
                .Where(x => x.Email.Equals(email))
                .MoveNextAsync();

            this.FakeHttpClient.Calls.Single().ShouldContain("email=tk421%40deathstar.co");
        }

        [Fact]
        public async Task Where_attribute_equals_using_formatted_string()
        {
            var domain = "deathstar.co";

            await this.Queryable
                .Where(x => x.Email.Equals($"tk421@{domain}"))
                .MoveNextAsync();

            this.FakeHttpClient.Calls.Single().ShouldContain("email=tk421%40deathstar.co");
        }

        [Fact]
        public async Task Where_attribute_starts_with()
        {
            await this.Queryable
                .Where(x => x.Email.StartsWith("tk421"))
                .MoveNextAsync();

            this.FakeHttpClient.Calls.Single().ShouldContain("email=tk421*");
        }

        [Fact]
        public async Task Where_attribute_ends_with()
        {
            await this.Queryable
                .Where(x => x.Email.EndsWith("deathstar.co"))
                .MoveNextAsync();

            this.FakeHttpClient.Calls.Single().ShouldContain("email=*deathstar.co");
        }

        [Fact]
        public async Task Where_attribute_contains()
        {
            await this.Queryable
                .Where(x => x.Email.Contains("421"))
                .MoveNextAsync();

            this.FakeHttpClient.Calls.Single().ShouldContain("email=*421*");
        }

        [Fact]
        public async Task Where_multiple_attributes_with_and()
        {
            await this.Queryable
                .Where(x => x.Email == "tk421@deathstar.co" && x.Username == "tk421")
                .MoveNextAsync();

            this.FakeHttpClient.Calls.Single().ShouldContain("email=tk421%40deathstar.co&username=tk421");
        }

        [Fact]
        public async Task Where_multiple_wheres()
        {
            await this.Queryable
                .Where(x => x.Email == "tk421@deathstar.co")
                .Where(x => x.Username.StartsWith("tk421"))
                .MoveNextAsync();

            this.FakeHttpClient.Calls.Single().ShouldContain("email=tk421%40deathstar.co&username=tk421*");
        }

        [Fact]
        public async Task Where_date_attribute_greater_than()
        {
            var testDate = new DateTimeOffset(2015, 01, 01, 06, 00, 00, TimeSpan.Zero);
            await this.Queryable
                .Where(x => x.CreatedAt > testDate)
                .MoveNextAsync();

            this.FakeHttpClient.Calls.Single().ShouldContain("createdAt=(2015-01-01T06:00:00Z,]");
        }

        [Fact]
        public async Task Where_date_attribute_greater_than_or_equalto()
        {
            var testDate = new DateTimeOffset(2015, 01, 01, 06, 00, 00, TimeSpan.Zero);
            await this.Queryable
                .Where(x => x.CreatedAt >= testDate)
                .MoveNextAsync();

            this.FakeHttpClient.Calls.Single().ShouldContain("createdAt=[2015-01-01T06:00:00Z,]");
        }

        [Fact]
        public async Task Where_date_attribute_less_than()
        {
            var testDate = new DateTimeOffset(2016, 01, 01, 12, 00, 00, TimeSpan.Zero);
            await this.Queryable
                .Where(x => x.ModifiedAt < testDate)
                .MoveNextAsync();

            this.FakeHttpClient.Calls.Single().ShouldContain("modifiedAt=[,2016-01-01T12:00:00Z)");
        }

        [Fact]
        public async Task Where_date_attribute_less_than_or_equalto()
        {
            var testDate = new DateTimeOffset(2016, 01, 01, 12, 00, 00, TimeSpan.Zero);
            await this.Queryable
                .Where(x => x.ModifiedAt <= testDate)
                .MoveNextAsync();

            this.FakeHttpClient.Calls.Single().ShouldContain("modifiedAt=[,2016-01-01T12:00:00Z]");
        }

        [Fact]
        public async Task Where_date_attribute_between()
        {
            var testStartDate = new DateTimeOffset(2015, 01, 01, 00, 00, 00, TimeSpan.Zero);
            var testEndDate = new DateTimeOffset(2015, 12, 31, 23, 59, 59, TimeSpan.Zero);
            await this.Queryable
                .Where(x => x.CreatedAt > testStartDate && x.CreatedAt <= testEndDate)
                .MoveNextAsync();

            this.FakeHttpClient.Calls.Single().ShouldContain("createdAt=(2015-01-01T00:00:00Z,2015-12-31T23:59:59Z]");
        }

        [Fact]
        public async Task Where_date_attribute_using_implicit_DateTime()
        {
            var testDate = new DateTime(2016, 01, 01, 12, 00, 00);
            await this.Queryable
                .Where(x => x.ModifiedAt < testDate)
                .MoveNextAsync();

            var timezoneOffset = new DateTimeOffset(testDate).Offset;
            var adjustedHour = (int)(12 - timezoneOffset.TotalHours);

            this.FakeHttpClient.Calls.Single().ShouldContain($"modifiedAt=[,2016-01-01T{adjustedHour}:00:00Z)");
        }

        [Fact]
        public async Task Where_date_attribute_equals()
        {
            var testDate = new DateTime(2016, 01, 01, 12, 00, 00);

            // TODO NotSupportedException after Shouldly Mono fix
            await Should.ThrowAsync<Exception>(async () =>
            {
                await this.Queryable
                    .Where(x => x.ModifiedAt == testDate)
                    .MoveNextAsync();
            });
        }

        [Fact]
        public async Task Alternate_query_syntax_is_okay()
        {
            var query = from account in this.Queryable
                        where account.Email == "tk421@deathstar.co"
                        select account;
            await query.MoveNextAsync();

            this.FakeHttpClient.Calls.Single().ShouldContain("email=tk421%40deathstar.co");
        }
    }
}
