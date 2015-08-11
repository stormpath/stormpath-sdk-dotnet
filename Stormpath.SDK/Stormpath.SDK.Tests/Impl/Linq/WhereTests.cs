// <copyright file="WhereTests.cs" company="Stormpath, Inc.">
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stormpath.SDK.Account;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    [TestClass]
    public class WhereTests
    {
        private static string url = "http://f.oo";
        private static string resource = "bar";

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_throws_for_constant()
        {
            var harness = LinqTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Where(x => true);
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Where(x => false);
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_throws_for_unsupported_comparison_operators()
        {
            var harness = LinqTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Where(x => x.Email != "foo");
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_throws_for_more_complex_overloads_of_helper_methods()
        {
            var harness = LinqTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Where(x => x.Email.Equals("bar", StringComparison.CurrentCulture));
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Where(x => x.Email.StartsWith("foo", StringComparison.OrdinalIgnoreCase));
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_throws_for_unsupported_helper_methods()
        {
            var harness = LinqTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Where(x => x.Email.ToUpper() == "FOO");
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_throws_for_binary_or()
        {
            var harness = LinqTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Where(x => x.Email == "foo" || x.Email == "bar");
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_attribute_equals()
        {
            var harness = LinqTestHarness<IAccount>.Create<IAccount>(url, resource);

            var query = harness.Queryable
                .Where(x => x.Email == "tk421@deathstar.co");

            query.GeneratedArgumentsWere(url, resource, "email=tk421@deathstar.co");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_attribute_equals_using_helper_method()
        {
            var harness = LinqTestHarness<IAccount>.Create<IAccount>(url, resource);

            var query = harness.Queryable
                .Where(x => x.Email.Equals("tk421@deathstar.co"));

            query.GeneratedArgumentsWere(url, resource, "email=tk421@deathstar.co");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_attribute_starts_with()
        {
            var harness = LinqTestHarness<IAccount>.Create<IAccount>(url, resource);

            var query = harness.Queryable
                .Where(x => x.Email.StartsWith("tk421"));

            query.GeneratedArgumentsWere(url, resource, "email=tk421*");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_attribute_ends_with()
        {
            var harness = LinqTestHarness<IAccount>.Create<IAccount>(url, resource);

            var query = harness.Queryable
                .Where(x => x.Email.EndsWith("deathstar.co"));

            query.GeneratedArgumentsWere(url, resource, "email=*deathstar.co");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_attribute_contains()
        {
            var harness = LinqTestHarness<IAccount>.Create<IAccount>(url, resource);

            var query = harness.Queryable
                .Where(x => x.Email.Contains("421"));

            query.GeneratedArgumentsWere(url, resource, "email=*421*");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_multiple_attributes_with_and()
        {
            var harness = LinqTestHarness<IAccount>.Create<IAccount>(url, resource);

            var query = harness.Queryable
                .Where(x => x.Email == "tk421@deathstar.co" && x.Username == "tk421");

            query.GeneratedArgumentsWere(url, resource, "email=tk421@deathstar.co&username=tk421");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_multiple_wheres()
        {
            var harness = LinqTestHarness<IAccount>.Create<IAccount>(url, resource);

            var query = harness.Queryable
                .Where(x => x.Email == "tk421@deathstar.co")
                .Where(x => x.Username.StartsWith("tk421"));

            query.GeneratedArgumentsWere(url, resource, "email=tk421@deathstar.co&username=tk421*");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_date_attribute_greater_than()
        {
            var harness = LinqTestHarness<IAccount>.Create<IAccount>(url, resource);

            var testDate = new DateTimeOffset(2015, 01, 01, 06, 00, 00, TimeSpan.Zero);
            var query = harness.Queryable
                .Where(x => x.CreatedAt > testDate);

            query.GeneratedArgumentsWere(url, resource, "createdAt=(2015-01-01T06:00:00.000Z,]");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_date_attribute_greater_than_or_equalto()
        {
            var harness = LinqTestHarness<IAccount>.Create<IAccount>(url, resource);

            var testDate = new DateTimeOffset(2015, 01, 01, 06, 00, 00, TimeSpan.Zero);
            var query = harness.Queryable
                .Where(x => x.CreatedAt >= testDate);

            query.GeneratedArgumentsWere(url, resource, "createdAt=[2015-01-01T06:00:00.000Z,]");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_date_attribute_less_than()
        {
            var harness = LinqTestHarness<IAccount>.Create<IAccount>(url, resource);

            var testDate = new DateTimeOffset(2016, 01, 01, 12, 00, 00, TimeSpan.Zero);
            var query = harness.Queryable
                .Where(x => x.ModifiedAt < testDate);

            query.GeneratedArgumentsWere(url, resource, "modifiedAt=[,2016-01-01T12:00:00.000Z)");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_date_attribute_less_than_or_equalto()
        {
            var harness = LinqTestHarness<IAccount>.Create<IAccount>(url, resource);

            var testDate = new DateTimeOffset(2016, 01, 01, 12, 00, 00, TimeSpan.Zero);
            var query = harness.Queryable
                .Where(x => x.ModifiedAt <= testDate);

            query.GeneratedArgumentsWere(url, resource, "modifiedAt=[,2016-01-01T12:00:00.000Z]");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_date_attribute_between()
        {
            var harness = LinqTestHarness<IAccount>.Create<IAccount>(url, resource);

            var testStartDate = new DateTimeOffset(2015, 01, 01, 00, 00, 00, TimeSpan.Zero);
            var testEndDate = new DateTimeOffset(2015, 12, 31, 23, 59, 59, TimeSpan.Zero);
            var query = harness.Queryable
                .Where(x => x.CreatedAt > testStartDate && x.CreatedAt <= testEndDate);

            query.GeneratedArgumentsWere(url, resource, "createdAt=(2015-01-01T00:00:00.000Z,2015-12-31T23:59:59.000Z]");
        }
    }
}
