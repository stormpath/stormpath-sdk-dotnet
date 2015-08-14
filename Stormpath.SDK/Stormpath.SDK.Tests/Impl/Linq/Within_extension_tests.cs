// <copyright file="Within_extension.cs" company="Stormpath, Inc.">
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

using System;
using System.Linq;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class Within_extension_tests : Linq_tests
    {
        private CollectionTestHarness<IAccount> harness;

        public Within_extension_tests() : base()
        {
            harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);
        }

        [Fact]
        public void Within_throws_when_using_outside_LINQ()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var dto = new DateTimeOffset(DateTime.Now);
                var test = dto.Within(2015);
            });
        }

        [Fact]
        public void Where_date_using_shorthand_for_year()
        {
            var query = harness.Queryable
                .Where(x => x.CreatedAt.Within(2015));

            query.GeneratedArgumentsWere(url, resource, "createdAt=2015");
        }

        [Fact]
        public void Where_date_using_shorthand_for_month()
        {
            var query = harness.Queryable
                .Where(x => x.CreatedAt.Within(2015, 01));

            query.GeneratedArgumentsWere(url, resource, "createdAt=2015-01");
        }

        [Fact]
        public void Where_date_using_shorthand_for_day()
        {
            var query = harness.Queryable
                .Where(x => x.CreatedAt.Within(2015, 01, 01));

            query.GeneratedArgumentsWere(url, resource, "createdAt=2015-01-01");
        }

        [Fact]
        public void Where_date_using_shorthand_for_hour()
        {
            var query = harness.Queryable
                .Where(x => x.CreatedAt.Within(2015, 01, 01, 12));

            query.GeneratedArgumentsWere(url, resource, "createdAt=2015-01-01T12");
        }

        [Fact]
        public void Where_date_using_shorthand_for_minute()
        {
            var query = harness.Queryable
                .Where(x => x.CreatedAt.Within(2015, 01, 01, 12, 30));

            query.GeneratedArgumentsWere(url, resource, "createdAt=2015-01-01T12:30");
        }

        [Fact]
        public void Where_date_using_shorthand_for_second()
        {
            var query = harness.Queryable
                .Where(x => x.CreatedAt.Within(2015, 01, 01, 12, 30, 31));

            query.GeneratedArgumentsWere(url, resource, "createdAt=2015-01-01T12:30:31");
        }
    }
}
