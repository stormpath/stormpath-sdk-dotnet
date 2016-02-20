// <copyright file="Sync_unsupported_methods_tests.cs" company="Stormpath, Inc.">
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
using System.Linq;
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Common;
using Xunit;

namespace Stormpath.SDK.Tests.Linq
{
    public class Sync_unsupported_methods_tests : Linq_test<IAccount>
    {
        private static string GetGeneratedHref<T>(IQueryable<T> queryable)
        {
            var resourceQueryable = queryable as CollectionResourceQueryable<T>;
            if (resourceQueryable == null)
            {
                Assertly.Fail("This queryable is not a CollectionResourceQueryable.");
            }

            return resourceQueryable.CurrentHref;
        }

        [Fact]
        public void Aggregate_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                this.Queryable.Synchronously().Aggregate((x, y) => x);
            });
        }

        [Fact]
        public void All_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                this.Queryable.Synchronously().All(x => x.Email == "foo");
            });
        }

        [Fact]
        public void Average_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                this.Queryable.Synchronously().Average(x => 1.0);
            });
        }

        [Fact]
        public void Cast_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = this.Queryable.Synchronously().Cast<Tenant.ITenant>();
                GetGeneratedHref(query);
            });
        }

        [Fact]
        public void Concat_is_unsupported()
        {
            var query = this.Queryable.Synchronously().Concat(Enumerable.Empty<IAccount>());

            Should.Throw<NotSupportedException>(() =>
            {
                GetGeneratedHref(query);
            });
        }

        [Fact]
        public void Contains_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                this.Queryable.Synchronously().Contains(Substitute.For<IAccount>());
            });
        }

        [Fact]
        public void Distinct_is_unsupported()
        {
            var query = this.Queryable.Synchronously().Distinct();

            Should.Throw<NotSupportedException>(() =>
            {
                GetGeneratedHref(query);
            });
        }

        [Fact]
        public void Except_is_unsupported()
        {
            var query = this.Queryable.Synchronously().Except(Enumerable.Empty<IAccount>());

            Should.Throw<NotSupportedException>(() =>
            {
                GetGeneratedHref(query);
            });
        }

        [Fact]
        public void GroupBy_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = this.Queryable.Synchronously().GroupBy(x => x.Email);
                GetGeneratedHref(query);
            });
        }

        [Fact]
        public void GroupJoin_clause_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = this.Queryable.Synchronously().GroupJoin(
                    Enumerable.Empty<IAccount>(),
                    outer => outer.Email,
                    inner => inner.Username,
                    (outer, results) => new { outer.CreatedAt, results });
                GetGeneratedHref(query);
            });
        }

        [Fact]
        public void Intersect_is_unsupported()
        {
            var query = this.Queryable.Synchronously().Intersect(Enumerable.Empty<IAccount>());

            Should.Throw<NotSupportedException>(() =>
            {
                GetGeneratedHref(query);
            });
        }

        [Fact]
        public void Join_clause_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = this.Queryable.Synchronously().Join(
                    Enumerable.Empty<IAccount>(),
                    outer => outer.Email,
                    inner => inner.Username,
                    (outer, inner) => outer.CreatedAt);
                GetGeneratedHref(query);
            });
        }

        [Fact]
        public void Last_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                this.Queryable.Synchronously().Last();
            });
        }

        [Fact]
        public void LastOrDefault_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                this.Queryable.Synchronously().LastOrDefault();
            });
        }

        [Fact]
        public void Max_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                this.Queryable.Synchronously().Max();
            });
        }

        [Fact]
        public void Min_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                this.Queryable.Synchronously().Min();
            });
        }

        [Fact]
        public void OfType_is_unsupported()
        {
            var query = this.Queryable.Synchronously().OfType<IAccount>();

            Should.Throw<NotSupportedException>(() =>
            {
                GetGeneratedHref(query);
            });
        }

        [Fact]
        public void OrderBy_throws_for_complex_overloads()
        {
            var query = this.Queryable
                .Synchronously()
                .OrderBy(x => x.GivenName, StringComparer.OrdinalIgnoreCase);

            Should.Throw<NotSupportedException>(() =>
            {
                GetGeneratedHref(query);
            });
        }

        [Fact]
        public void ThenBy_throws_for_complex_overloads()
        {
            var query = this.Queryable
                    .Synchronously()
                    .OrderBy(x => x.GivenName)
                    .ThenBy(x => x.Surname, StringComparer.OrdinalIgnoreCase);

            Should.Throw<NotSupportedException>(() =>
            {
                GetGeneratedHref(query);
            });
        }

        [Fact]
        public void Reverse_is_unsupported()
        {
            var query = this.Queryable.Synchronously().Reverse();

            Should.Throw<NotSupportedException>(() =>
            {
                GetGeneratedHref(query);
            });
        }

        [Fact]
        public void Select_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = this.Queryable
                    .Synchronously()
                    .Select(x => x.Email);
                GetGeneratedHref(query);
            });
        }

        [Fact]
        public void SelectMany_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = this.Queryable.Synchronously().SelectMany(x => x.Email);
                GetGeneratedHref(query);
            });
        }

        [Fact]
        public void SequenceEqual_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                this.Queryable.Synchronously().SequenceEqual(Enumerable.Empty<IAccount>());
            });
        }

        [Fact]
        public void SkipWhile_is_unsupported()
        {
            var query = this.Queryable.Synchronously().SkipWhile(x => x.Email == "foobar");

            Should.Throw<NotSupportedException>(() =>
            {
                GetGeneratedHref(query);
            });
        }

        [Fact]
        public void Sum_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                this.Queryable.Synchronously().Sum(x => 1);
            });
        }

        [Fact]
        public void TakeWhile_is_unsupported()
        {
            var query = this.Queryable.Synchronously().TakeWhile(x => x.Email == "foobar");

            Should.Throw<NotSupportedException>(() =>
            {
                GetGeneratedHref(query);
            });
        }

        [Fact]
        public void Union_is_unsupported()
        {
            var query = this.Queryable.Synchronously().Union(Enumerable.Empty<IAccount>());

            Should.Throw<NotSupportedException>(() =>
            {
                GetGeneratedHref(query);
            });
        }

        [Fact]
        public void Zip_is_unsupported()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var query = this.Queryable.Synchronously().Zip(
                    Enumerable.Empty<IAccount>(),
                    (first, second) => first.Email == second.Email);
                GetGeneratedHref(query);
            });
        }
    }
}
