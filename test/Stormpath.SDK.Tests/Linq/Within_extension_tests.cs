// <copyright file="Within_extension_tests.cs" company="Stormpath, Inc.">
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
using Shouldly;
using Stormpath.SDK.Account;
using Xunit;

namespace Stormpath.SDK.Tests.Linq
{
    public class Within_extension_tests : Linq_test<IAccount>
    {
        [Fact]
        public void Throws_when_using_outside_LINQ()
        {
            var dto = new DateTimeOffset(DateTime.Now);

            // TODO NotSupportedException after Shouldly Mono fix
            Should.Throw<Exception>(() =>
            {
                var test = dto.Within(2015);
            });
        }

        [Fact]
        public async Task Throws_for_multiple_series_calls()
        {
            // TODO NotSupportedException after Shouldly Mono fix
            await Should.ThrowAsync<Exception>(async () =>
            {
                await this.Queryable
                    .Where(x => x.CreatedAt.Within(2015))
                    .Where(x => x.CreatedAt.Within(2015, 01, 01))
                    .MoveNextAsync();
            });
        }

        [Fact]
        public async Task Throws_for_multiple_parallel_calls()
        {
            // TODO NotSupportedException after Shouldly Mono fix
            await Should.ThrowAsync<Exception>(async () =>
            {
                await this.Queryable
                    .Where(x => x.CreatedAt.Within(2015) && x.CreatedAt.Within(2015, 01, 01))
                    .MoveNextAsync();
            });
        }

        [Fact]
        public async Task Throws_when_mixing_Within_and_date_comparison()
        {
            // TODO NotSupportedException after Shouldly Mono fix
            await Should.ThrowAsync<Exception>(async () =>
            {
                await this.Queryable
                    .Where(x => x.CreatedAt.Within(2015))
                    .Where(x => x.CreatedAt > DateTime.Now)
                    .MoveNextAsync();
            });
        }

        [Fact]
        public async Task Both_created_and_modified_fields_in_series()
        {
            await this.Queryable
                .Where(x => x.CreatedAt.Within(2015))
                .Where(x => x.ModifiedAt.Within(2015, 1))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("createdAt=2015", "modifiedAt=2015-01");
        }

        [Fact]
        public async Task Both_created_and_modified_fields_in_parallel()
        {
            await this.Queryable
                .Where(x => x.CreatedAt.Within(2015) && x.ModifiedAt.Within(2015, 1))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("createdAt=2015", "modifiedAt=2015-01");
        }

        [Fact]
        public async Task With_other_Where_terms_in_series()
        {
            await this.Queryable
                .Where(x => x.Email == "foo@bar.co")
                .Where(x => x.CreatedAt.Within(2015))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("createdAt=2015", "email=foo%40bar.co");
        }

        [Fact]
        public async Task With_other_Where_terms_in_parallel()
        {
            await this.Queryable
                .Where(x => x.Email == "foo@bar.co" && x.CreatedAt.Within(2015, 1))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("createdAt=2015-01", "email=foo%40bar.co");
        }

        [Fact]
        public async Task Where_date_using_shorthand_for_year()
        {
            await this.Queryable
                .Where(x => x.CreatedAt.Within(2015))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("createdAt=2015");
        }

        [Fact]
        public async Task Where_date_using_shorthand_for_month()
        {
            await this.Queryable
                .Where(x => x.CreatedAt.Within(2015, 01))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("createdAt=2015-01");
        }

        [Fact]
        public async Task Where_date_using_shorthand_for_day()
        {
            await this.Queryable
                .Where(x => x.CreatedAt.Within(2015, 01, 01))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("createdAt=2015-01-01");
        }

        [Fact]
        public async Task Where_date_using_shorthand_for_hour()
        {
            await this.Queryable
                .Where(x => x.CreatedAt.Within(2015, 01, 01, 12))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("createdAt=2015-01-01T12");
        }

        [Fact]
        public async Task Where_date_using_shorthand_for_minute()
        {
            await this.Queryable
                .Where(x => x.CreatedAt.Within(2015, 01, 01, 12, 30))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("createdAt=2015-01-01T12:30");
        }

        [Fact]
        public async Task Where_date_using_shorthand_for_second()
        {
            await this.Queryable
                .Where(x => x.CreatedAt.Within(2015, 01, 01, 12, 30, 31))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("createdAt=2015-01-01T12:30:31");
        }

        [Fact]
        public async Task Where_customData_date_using_shorthand()
        {
            await this.Queryable
                .Where(x => ((DateTimeOffset)x.CustomData["birthday"]).Within(2015, 01, 01))
                .MoveNextAsync();

            this.ShouldBeCalledWithArguments("customData.birthday=2015-01-01");
        }
    }
}
