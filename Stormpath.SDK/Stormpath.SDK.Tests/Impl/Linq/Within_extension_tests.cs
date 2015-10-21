// <copyright file="Within_extension_tests.cs" company="Stormpath, Inc.">
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
using Shouldly;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class Within_extension_tests : Linq_tests
    {
        [Fact]
        public void Within_throws_when_using_outside_LINQ()
        {
            var dto = new DateTimeOffset(DateTime.Now);

            Should.Throw<NotSupportedException>(() =>
            {
                var test = dto.Within(2015);
            });
        }

        [Fact]
        public void Throws_for_multiple_series_calls()
        {
            var query = this.Harness.Queryable
                .Where(x => x.CreatedAt.Within(2015))
                .Where(x => x.CreatedAt.Within(2015, 01, 01));

            try
            {
                query.GeneratedArgumentsWere(this.Href, "<not evaluated>");
            }
            catch (NotSupportedException)
            {
                // all good
            }
        }

        [Fact]
        public void Throws_for_multiple_parallel_calls()
        {
            var query = this.Harness.Queryable
                .Where(x => x.CreatedAt.Within(2015) && x.CreatedAt.Within(2015, 01, 01));

            Should.Throw<NotSupportedException>(() =>
            {
                query.GeneratedArgumentsWere(this.Href, "<not evaluated>");
            });
        }

        [Fact]
        public void Throws_when_mixing_Within_and_date_comparison()
        {
            var query = this.Harness.Queryable
                .Where(x => x.CreatedAt.Within(2015))
                .Where(x => x.CreatedAt > DateTime.Now);

            Should.Throw<NotSupportedException>(() =>
            {
                query.GeneratedArgumentsWere(this.Href, "<not evaluated>");
            });
        }

        [Fact]
        public void Both_created_and_modified_fields_in_series()
        {
            var query = this.Harness.Queryable
                .Where(x => x.CreatedAt.Within(2015))
                .Where(x => x.ModifiedAt.Within(2015, 1));

            query.GeneratedArgumentsWere(this.Href, "createdAt=2015&modifiedAt=2015-01");
        }

        [Fact]
        public void Both_created_and_modified_fields_in_parallel()
        {
            var query = this.Harness.Queryable
                .Where(x => x.CreatedAt.Within(2015) && x.ModifiedAt.Within(2015, 1));

            query.GeneratedArgumentsWere(this.Href, "createdAt=2015&modifiedAt=2015-01");
        }

        [Fact]
        public void With_other_Where_terms_in_series()
        {
            var query = this.Harness.Queryable
                .Where(x => x.Email == "foo@bar.co")
                .Where(x => x.CreatedAt.Within(2015));

            query.GeneratedArgumentsWere(this.Href, "email=foo@bar.co&createdAt=2015");
        }

        [Fact]
        public void With_other_Where_terms_in_parallel()
        {
            var query = this.Harness.Queryable
                .Where(x => x.Email == "foo@bar.co" && x.CreatedAt.Within(2015, 1));

            query.GeneratedArgumentsWere(this.Href, "email=foo@bar.co&createdAt=2015-01");
        }

        [Fact]
        public void Where_date_using_shorthand_for_year()
        {
            var query = this.Harness.Queryable
                .Where(x => x.CreatedAt.Within(2015));

            query.GeneratedArgumentsWere(this.Href, "createdAt=2015");
        }

        [Fact]
        public void Where_date_using_shorthand_for_month()
        {
            var query = this.Harness.Queryable
                .Where(x => x.CreatedAt.Within(2015, 01));

            query.GeneratedArgumentsWere(this.Href, "createdAt=2015-01");
        }

        [Fact]
        public void Where_date_using_shorthand_for_day()
        {
            var query = this.Harness.Queryable
                .Where(x => x.CreatedAt.Within(2015, 01, 01));

            query.GeneratedArgumentsWere(this.Href, "createdAt=2015-01-01");
        }

        [Fact]
        public void Where_date_using_shorthand_for_hour()
        {
            var query = this.Harness.Queryable
                .Where(x => x.CreatedAt.Within(2015, 01, 01, 12));

            query.GeneratedArgumentsWere(this.Href, "createdAt=2015-01-01T12");
        }

        [Fact]
        public void Where_date_using_shorthand_for_minute()
        {
            var query = this.Harness.Queryable
                .Where(x => x.CreatedAt.Within(2015, 01, 01, 12, 30));

            query.GeneratedArgumentsWere(this.Href, "createdAt=2015-01-01T12:30");
        }

        [Fact]
        public void Where_date_using_shorthand_for_second()
        {
            var query = this.Harness.Queryable
                .Where(x => x.CreatedAt.Within(2015, 01, 01, 12, 30, 31));

            query.GeneratedArgumentsWere(this.Href, "createdAt=2015-01-01T12:30:31");
        }
    }
}
