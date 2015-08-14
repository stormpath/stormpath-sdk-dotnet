// <copyright file="Unsupported_filters.cs" company="Stormpath, Inc.">
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
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class Unsupported_filters_tests : Linq_tests
    {
        private CollectionTestHarness<IAccount> harness;

        public Unsupported_filters_tests() : base()
        {
            harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);
        }

        [Fact]
        public void Aggregate_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                harness.Queryable.Aggregate((x, y) => x);
            });
        }

        [Fact]
        public void All_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                harness.Queryable.All(x => x.Email == "foo");
            });
        }

        [Fact]
        public void Average_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                harness.Queryable.Average(x => 1.0);
            });
        }

        [Fact]
        public void Cast_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Cast<Tenant.ITenant>();
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [Fact]
        public void Concat_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Concat(Enumerable.Empty<IAccount>());
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [Fact]
        public void Contains_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                harness.Queryable.Contains(Substitute.For<IAccount>());
            });
        }

        [Fact]
        public void Distinct_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Distinct();
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [Fact]
        public void Except_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Except(Enumerable.Empty<IAccount>());
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [Fact]
        public void GroupBy_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.GroupBy(x => x.Email);
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [Fact]
        public void GroupJoin_clause_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.GroupJoin(Enumerable.Empty<IAccount>(),
                    outer => outer.Email,
                    inner => inner.Username,
                    (outer, results) => new { outer.CreatedAt, results });
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [Fact]
        public void Intersect_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Intersect(Enumerable.Empty<IAccount>());
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [Fact]
        public void Join_clause_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Join(Enumerable.Empty<IAccount>(),
                    outer => outer.Email,
                    inner => inner.Username,
                    (outer, inner) => outer.CreatedAt);
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [Fact]
        public void Last_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                harness.Queryable.Last();
            });

            Should.Throw<NotSupportedException>(() =>
            {
                harness.Queryable.LastOrDefault();
            });
        }

        [Fact]
        public void Max_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                harness.Queryable.Max();
            });
        }

        [Fact]
        public void Min_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                harness.Queryable.Min();
            });
        }

        [Fact]
        public void OfType_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.OfType<IAccount>();
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [Fact]
        public void Reverse_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Reverse();
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [Fact]
        public void Select_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable
                    .Select(x => x.Email);
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [Fact]
        public void SelectMany_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.SelectMany(x => x.Email);
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [Fact]
        public void SequenceEqual_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                harness.Queryable.SequenceEqual(Enumerable.Empty<IAccount>());
            });
        }

        [Fact]
        public void SkipWhile_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.SkipWhile(x => x.Email == "foobar");
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [Fact]
        public void Sum_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                harness.Queryable.Sum(x => 1);
            });
        }

        [Fact]
        public void TakeWhile_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.TakeWhile(x => x.Email == "foobar");
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [Fact]
        public void Union_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Union(Enumerable.Empty<IAccount>());
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }

        [Fact]
        public void Zip_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = harness.Queryable.Zip(Enumerable.Empty<IAccount>(),
                    (first, second) => first.Email == second.Email);
                query.GeneratedArgumentsWere(url, resource, "<not evaluated>");
            });
        }
    }
}
