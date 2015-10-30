// <copyright file="Expand_extension_tests.cs" company="Stormpath, Inc.">
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
    public class Expand_extension_tests : Linq_tests
    {
        [Fact]
        public void Expand_one_link()
        {
            var query = this.Harness.Queryable
                .Expand(x => x.GetDirectoryAsync);

            query.GeneratedArgumentsWere(this.Href, "expand=directory");
        }

        [Fact]
        public void Expand_multiple_links()
        {
            var query = this.Harness.Queryable
                .Expand(x => x.GetDirectoryAsync)
                .Expand(x => x.GetTenantAsync);

            query.GeneratedArgumentsWere(this.Href, "expand=directory,tenant");
        }

        [Fact]
        public void Expand_collection_query_with_no_parameters()
        {
            var query = this.Harness.Queryable
                .Expand(x => x.GetGroups);

            query.GeneratedArgumentsWere(this.Href, "expand=groups");
        }

        [Fact]
        public void Expand_collection_query_with_offset()
        {
            var query = this.Harness.Queryable
                .Expand(x => x.GetGroups, offset: 10);

            query.GeneratedArgumentsWere(this.Href, "expand=groups(offset:10)");
        }

        [Fact]
        public void Expand_collection_query_with_limit()
        {
            var query = this.Harness.Queryable
                .Expand(x => x.GetGroups, limit: 20);

            query.GeneratedArgumentsWere(this.Href, "expand=groups(limit:20)");
        }

        [Fact]
        public void Expand_collection_query_with_both_parameters()
        {
            var query = this.Harness.Queryable
                .Expand(x => x.GetGroups, 5, 15);

            query.GeneratedArgumentsWere(this.Href, "expand=groups(offset:5,limit:15)");
        }

        [Fact]
        public void Expand_all_the_things()
        {
            var query = this.Harness.Queryable
                .Expand(x => x.GetTenantAsync)
                .Expand(x => x.GetGroups, 10, 20)
                .Expand(x => x.GetDirectoryAsync);

            query.GeneratedArgumentsWere(this.Href, "expand=tenant,groups(offset:10,limit:20),directory");
        }

        [Fact]
        public void Throws_for_negative_paging_values()
        {
            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                var query = this.Harness.Queryable.Expand(x => x.GetGroups, -1, 0);
            });

            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                var query = this.Harness.Queryable.Expand(x => x.GetGroups, 0, -1);
            });
        }
    }
}