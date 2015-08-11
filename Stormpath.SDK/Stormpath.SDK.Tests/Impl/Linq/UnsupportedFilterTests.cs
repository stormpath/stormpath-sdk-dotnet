// <copyright file="UnsupportedFilterTests.cs" company="Stormpath, Inc.">
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
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Account;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    [TestClass]
    public class UnsupportedFilterTests
    {
        private static string url = "http://f.oo";
        private static string resource = "bar";

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Aggregate_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                harness.Queryable.Aggregate((x, y) => x);
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void All_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                harness.Queryable.All(x => x.Email == "foo");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Average_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                harness.Queryable.Average(x => 1.0);
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Cast_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Cast<Tenant.ITenant>();
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Concat_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Concat(Enumerable.Empty<IAccount>());
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Contains_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                harness.Queryable.Contains(Substitute.For<IAccount>());
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Distinct_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Distinct();
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Except_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Except(Enumerable.Empty<IAccount>());
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void GroupBy_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.GroupBy(x => x.Email);
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void GroupJoin_clause_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.GroupJoin(Enumerable.Empty<IAccount>(),
                    outer => outer.Email,
                    inner => inner.Username,
                    (outer, results) => new { outer.CreatedAt, results });
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Intersect_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Intersect(Enumerable.Empty<IAccount>());
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Join_clause_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Join(Enumerable.Empty<IAccount>(),
                    outer => outer.Email,
                    inner => inner.Username,
                    (outer, inner) => outer.CreatedAt);
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Last_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                harness.Queryable.Last();
            });

            Should.Throw<NotSupportedException>(() =>
            {
                harness.Queryable.LastOrDefault();
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Max_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                harness.Queryable.Max();
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Min_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                harness.Queryable.Min();
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void OfType_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.OfType<IAccount>();
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Reverse_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Reverse();
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Select_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable
                    .Select(x => x.Email);
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void SelectMany_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.SelectMany(x => x.Email);
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void SequenceEqual_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                harness.Queryable.SequenceEqual(Enumerable.Empty<IAccount>());
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void SkipWhile_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.SkipWhile(x => x.Email == "foobar");
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Sum_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                harness.Queryable.Sum(x => 1);
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void TakeWhile_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.TakeWhile(x => x.Email == "foobar");
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Union_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Union(Enumerable.Empty<IAccount>());
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Zip_is_unsupported()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Zip(Enumerable.Empty<IAccount>(),
                    (first, second) => first.Email == second.Email);
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }
    }
}
