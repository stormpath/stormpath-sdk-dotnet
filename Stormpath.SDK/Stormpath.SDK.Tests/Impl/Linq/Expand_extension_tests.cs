// <copyright file="Expand_extension_tests.cs" company="Stormpath, Inc.">
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
            var query = Harness.Queryable
                .Expand(x => x.GetDirectoryAsync());

            query.GeneratedArgumentsWere(Url, Resource, "expand=directory");
        }

        [Fact]
        public void Expand_multiple_links()
        {
            var query = Harness.Queryable
                .Expand(x => x.GetDirectoryAsync())
                .Expand(x => x.GetTenantAsync());

            query.GeneratedArgumentsWere(Url, Resource, "expand=directory,tenant");
        }

        [Fact]
        public void Expand_collection_query_with_offset()
        {
            var query = Harness.Queryable
                .Expand(x => x.GetGroupsAsync(), offset: 10);

            query.GeneratedArgumentsWere(Url, Resource, "expand=groups(offset:10)");
        }

        [Fact]
        public void Expand_collection_query_with_limit()
        {
            var query = Harness.Queryable
                .Expand(x => x.GetGroupsAsync(), limit: 20);

            query.GeneratedArgumentsWere(Url, Resource, "expand=groups(limit:20)");
        }

        [Fact]
        public void Expand_collection_query_with_both_parameters()
        {
            var query = Harness.Queryable
                .Expand(x => x.GetGroupsAsync(), 5, 15);

            query.GeneratedArgumentsWere(Url, Resource, "expand=groups(offset:5,limit:15)");
        }

        [Fact]
        public void Expand_all_the_things()
        {
            var query = Harness.Queryable
                .Expand(x => x.GetTenantAsync())
                .Expand(x => x.GetGroupsAsync(), 10, 20)
                .Expand(x => x.GetDirectoryAsync());

            query.GeneratedArgumentsWere(Url, Resource, "expand=tenant,groups(offset:10,limit:20),directory");
        }

        [Fact]
        public void Expand_throws_if_used_on_an_attribute()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = Harness.Queryable.Expand(x => x.Email);
                query.GeneratedArgumentsWere(Url, Resource, "<not evaluated>");
            });
        }

        [Fact]
        public void Expand_throws_if_parameters_are_supplied_for_link()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = Harness.Queryable.Expand(x => x.GetDirectoryAsync(), limit: 10);
                query.GeneratedArgumentsWere(Url, Resource, "<not evaluated>");
            });
        }

        [Fact]
        public void Expand_throws_if_syntax_is_dumb()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = Harness.Queryable.Expand(x => x.GetTenantAsync().GetAwaiter());
                query.GeneratedArgumentsWere(Url, Resource, "<not evaluated>");
            });
        }
    }
}
