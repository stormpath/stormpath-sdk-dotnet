// <copyright file="OrderBy_tests.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class OrderBy_tests : Linq_tests
    {
        [Fact]
        public void Sort_by_field()
        {
            var query = this.Harness.Queryable
                .OrderBy(x => x.GivenName);

            query.GeneratedArgumentsWere(this.Href, "orderBy=givenName");
        }

        [Fact]
        public void Sort_by_field_descending()
        {
            var query = this.Harness.Queryable
                .OrderByDescending(x => x.Email);

            query.GeneratedArgumentsWere(this.Href, "orderBy=email desc");
        }

        [Fact]
        public void Sort_with_additional_expressions_before()
        {
            var query = this.Harness.Queryable
                .Take(10)
                .OrderBy(x => x.GivenName);

            query.GeneratedArgumentsWere(this.Href, "limit=10&orderBy=givenName");
        }

        [Fact]
        public void Sort_with_additional_expressions_after()
        {
            var query = this.Harness.Queryable
                .OrderByDescending(x => x.GivenName)
                .Take(10);

            query.GeneratedArgumentsWere(this.Href, "limit=10&orderBy=givenName desc");
        }

        [Fact]
        public void Sort_by_multiple_fields()
        {
            var query = this.Harness.Queryable
                .OrderBy(x => x.GivenName)
                .ThenByDescending(x => x.Username);

            query.GeneratedArgumentsWere(this.Href, "orderBy=givenName,username desc");
        }
    }
}
