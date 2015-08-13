// <copyright file="ExpandExtensionTests.cs" company="Stormpath, Inc.">
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stormpath.SDK.Account;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    [TestClass]
    public class ExpandExtensionTests
    {
        private static string url = "http://f.oo";
        private static string resource = "bar";

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Expand_one_link()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            var query = harness.Queryable
                .Expand(x => x.GetDirectoryAsync());

            query.GeneratedArgumentsWere(url, resource, "expand=directory");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Expand_multiple_links()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            var query = harness.Queryable
                .Expand(x => x.GetDirectoryAsync())
                .Expand(x => x.GetTenantAsync());

            query.GeneratedArgumentsWere(url, resource, "expand=directory,tenant");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Expand_collection_query_with_offset()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            var query = harness.Queryable
                .Expand(x => x.GetGroupsAsync(), offset: 10);

            query.GeneratedArgumentsWere(url, resource, "expand=groups(offset:10)");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Expand_collection_query_with_limit()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            var query = harness.Queryable
                .Expand(x => x.GetGroupsAsync(), limit: 20);

            query.GeneratedArgumentsWere(url, resource, "expand=groups(limit:20)");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Expand_collection_query_with_both_parameters()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            var query = harness.Queryable
                .Expand(x => x.GetGroupsAsync(), 5, 15);

            query.GeneratedArgumentsWere(url, resource, "expand=groups(offset:5,limit:15)");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Expand_all_the_things()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            var query = harness.Queryable
                .Expand(x => x.GetTenantAsync())
                .Expand(x => x.GetGroupsAsync(), 10, 20)
                .Expand(x => x.GetDirectoryAsync());

            query.GeneratedArgumentsWere(url, resource, "expand=tenant,groups(offset:10,limit:20),directory");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Expand_throws_if_used_on_an_attribute()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Expand(x => x.Email);
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Expand_throws_if_parameters_are_supplied_for_link()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Expand(x => x.GetDirectoryAsync(), limit: 10);
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Expand_throws_if_syntax_is_dumb()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Expand(x => x.GetTenantAsync().GetAwaiter());
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }
    }
}
