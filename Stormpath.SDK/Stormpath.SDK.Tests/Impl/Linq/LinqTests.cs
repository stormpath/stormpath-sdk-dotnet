// <copyright file="LinqTests.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    [TestClass]
    public class LinqTests
    {
        private static string url = "http://f.oo";
        private static string resource = "bar";

        [TestClass]
        public class FilterExtensionMethod
        {
            [TestMethod]
            public void Filter_with_simple_parameter()
            {
                // Arrange
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                // Act
                harness.Queryable
                    .Filter("Joe")
                    .ToList();

                // Assert
                harness.WasCalledWithArguments("q=Joe");
            }

            [TestMethod]
            public void Filter_multiple_calls_are_LIFO()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .Filter("Joe")
                    .Filter("Joey")
                    .ToList();

                // Expected behavior: the last call will be kept
                harness.WasCalledWithArguments("q=Joey");
            }
        }

        [TestClass]
        public class ExpandExtensionMethod
        {
            [TestMethod]
            public void Expand_one_link()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .Expand(x => x.GetDirectoryAsync())
                    .ToList();

                harness.WasCalledWithArguments("expand=directory");
            }

            [TestMethod]
            public void Expand_multiple_links()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .Expand(x => x.GetDirectoryAsync())
                    .Expand(x => x.GetTenantAsync())
                    .ToList();

                harness.WasCalledWithArguments("expand=directory,tenant");
            }

            [TestMethod]
            public void Expand_collection_query_with_offset()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .Expand(x => x.GetGroupsAsync(), offset: 10)
                    .ToList();

                harness.WasCalledWithArguments("expand=groups(offset:10)");
            }

            [TestMethod]
            public void Expand_collection_query_with_limit()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .Expand(x => x.GetGroupsAsync(), limit: 20)
                    .ToList();

                harness.WasCalledWithArguments("expand=groups(limit:20)");
            }

            [TestMethod]
            public void Expand_collection_query_with_both_parameters()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .Expand(x => x.GetGroupsAsync(), 5, 15)
                    .ToList();

                harness.WasCalledWithArguments("expand=groups(offset:5,limit:15)");
            }

            [TestMethod]
            public void Expand_all_the_things()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .Expand(x => x.GetTenantAsync())
                    .Expand(x => x.GetGroupsAsync(), 10, 20)
                    .Expand(x => x.GetDirectoryAsync())
                    .ToList();

                harness.WasCalledWithArguments("expand=tenant,groups(offset:10,limit:20),directory");
            }

            [TestMethod]
            public void Expand_throws_if_used_on_an_attribute()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Expand(x => x.Email).ToList();
                });
            }

            [TestMethod]
            public void Expand_throws_if_parameters_are_supplied_for_link()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Expand(x => x.GetDirectoryAsync(), limit: 10).ToList();
                });
            }

            [TestMethod]
            public void Expand_throws_if_syntax_is_dumb()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Expand(x => x.GetTenantAsync().GetAwaiter()).ToList();
                });
            }
        }

        [TestClass]
        public class Where
        {
            [TestMethod]
            public void Where_throws_for_constant()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Where(x => true).ToList();
                });

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Where(x => false).ToList();
                });
            }

            [TestMethod]
            public void Where_throws_for_unsupported_comparison_operators()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Where(x => x.Email != "foo").ToList();
                });
            }

            [TestMethod]
            public void Where_throws_for_more_complex_overloads_of_helper_methods()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Where(x => x.Email.Equals("bar", StringComparison.CurrentCulture)).ToList();
                });

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Where(x => x.Email.StartsWith("foo", StringComparison.OrdinalIgnoreCase)).ToList();
                });
            }

            [TestMethod]
            public void Where_throws_for_unsupported_helper_methods()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Where(x => x.Email.ToUpper() == "FOO").ToList();
                });
            }

            [TestMethod]
            public void Where_throws_for_binary_or()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Where(x => x.Email == "foo" || x.Email == "bar").ToList();
                });
            }

            [TestMethod]
            public void Where_attribute_equals()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .Where(x => x.Email == "tk421@deathstar.co")
                    .ToList();

                harness.WasCalledWithArguments("email=tk421@deathstar.co");
            }

            [TestMethod]
            public void Where_attribute_equals_using_helper_method()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .Where(x => x.Email.Equals("tk421@deathstar.co"))
                    .ToList();

                harness.WasCalledWithArguments("email=tk421@deathstar.co");
            }

            [TestMethod]
            public void Where_attribute_starts_with()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .Where(x => x.Email.StartsWith("tk421"))
                    .ToList();

                harness.WasCalledWithArguments("email=tk421*");
            }

            [TestMethod]
            public void Where_attribute_ends_with()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .Where(x => x.Email.EndsWith("deathstar.co"))
                    .ToList();

                harness.WasCalledWithArguments("email=*deathstar.co");
            }

            [TestMethod]
            public void Where_attribute_contains()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .Where(x => x.Email.Contains("421"))
                    .ToList();

                harness.WasCalledWithArguments("email=*421*");
            }

            [TestMethod]
            public void Where_multiple_attributes_with_and()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .Where(x => x.Email == "tk421@deathstar.co" && x.Username == "tk421")
                    .ToList();

                harness.WasCalledWithArguments("email=tk421@deathstar.co&username=tk421");
            }

            [TestMethod]
            public void Where_multiple_wheres()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .Where(x => x.Email == "tk421@deathstar.co")
                    .Where(x => x.Username.StartsWith("tk421"))
                    .ToList();

                harness.WasCalledWithArguments("email=tk421@deathstar.co&username=tk421*");
            }

            [TestMethod]
            public void Where_date_attribute_greater_than()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                var testDate = new DateTimeOffset(2015, 01, 01, 06, 00, 00, TimeSpan.Zero);
                harness.Queryable
                    .Where(x => x.CreatedAt > testDate)
                    .ToList();

                harness.WasCalledWithArguments("createdAt=(2015-01-01T06:00:00.000Z,]");
            }

            [TestMethod]
            public void Where_date_attribute_greater_than_or_equalto()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                var testDate = new DateTimeOffset(2015, 01, 01, 06, 00, 00, TimeSpan.Zero);
                harness.Queryable
                    .Where(x => x.CreatedAt >= testDate)
                    .ToList();

                harness.WasCalledWithArguments("createdAt=[2015-01-01T06:00:00.000Z,]");
            }

            [TestMethod]
            public void Where_date_attribute_less_than()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                var testDate = new DateTimeOffset(2016, 01, 01, 12, 00, 00, TimeSpan.Zero);
                harness.Queryable
                    .Where(x => x.ModifiedAt < testDate)
                    .ToList();

                harness.WasCalledWithArguments("modifiedAt=[,2016-01-01T12:00:00.000Z)");
            }

            [TestMethod]
            public void Where_date_attribute_less_than_or_equalto()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                var testDate = new DateTimeOffset(2016, 01, 01, 12, 00, 00, TimeSpan.Zero);
                harness.Queryable
                    .Where(x => x.ModifiedAt <= testDate)
                    .ToList();

                harness.WasCalledWithArguments("modifiedAt=[,2016-01-01T12:00:00.000Z]");
            }

            [TestMethod]
            public void Where_date_attribute_between()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                var testStartDate = new DateTimeOffset(2015, 01, 01, 00, 00, 00, TimeSpan.Zero);
                var testEndDate = new DateTimeOffset(2015, 12, 31, 23, 59, 59, TimeSpan.Zero);
                harness.Queryable
                    .Where(x => x.CreatedAt > testStartDate && x.CreatedAt <= testEndDate)
                    .ToList();

                harness.WasCalledWithArguments("createdAt=(2015-01-01T00:00:00.000Z,2015-12-31T23:59:59.000Z]");
            }

            [TestMethod]
            public void Where_date_using_shorthand()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .Where(x => x.CreatedAt == Shorthand.Year(2015))
                    .ToList();

                harness.WasCalledWithArguments("createdAt=2015");
            }
        }

        [TestClass]
        public class Limit
        {
            [TestMethod]
            public void Take_with_constant_becomes_limit()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .Take(10)
                    .ToList();

                harness.WasCalledWithArguments("limit=10");
            }

            [TestMethod]
            public void Take_with_variable_becomes_limit()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                var limit = 20;
                harness.Queryable
                    .Take(limit)
                    .ToList();

                harness.WasCalledWithArguments("limit=20");
            }

            [TestMethod]
            public void Take_with_function_becomes_limit()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                var limitFunc = new Func<int>(() => 25);
                harness.Queryable
                    .Take(limitFunc())
                    .ToList();

                harness.WasCalledWithArguments("limit=25");
            }

            [TestMethod]
            public void Take_multiple_calls_are_LIFO()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .Take(10).Take(5)
                    .ToList();

                // Expected behavior: the last call will be kept
                harness.WasCalledWithArguments("limit=5");
            }
        }

        [TestClass]
        public class Offset
        {
            [TestMethod]
            public void Skip_becomes_offset()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .Skip(10)
                    .ToList();

                harness.WasCalledWithArguments("offset=10");
            }

            [TestMethod]
            public void Skip_with_variable_becomes_offset()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                var offset = 20;
                harness.Queryable
                    .Skip(offset)
                    .ToList();

                harness.WasCalledWithArguments("offset=20");
            }

            [TestMethod]
            public void Skip_with_function_becomes_offset()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                var offsetFunc = new Func<int>(() => 25);
                harness.Queryable
                    .Skip(offsetFunc())
                    .ToList();

                harness.WasCalledWithArguments("offset=25");
            }

            [TestMethod]
            public void Skip_multiple_calls_are_LIFO()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .Skip(10).Skip(5)
                    .ToList();

                // Expected behavior: the last call will be kept
                harness.WasCalledWithArguments("offset=5");
            }
        }

        [TestClass]
        public class OrderBy
        {
            [TestMethod]
            public void Order_by_a_field()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .OrderBy(x => x.GivenName)
                    .ToList();

                harness.WasCalledWithArguments("orderBy=givenName");
            }

            [TestMethod]
            public void Order_by_a_field_descending()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .OrderByDescending(x => x.Email)
                    .ToList();

                harness.WasCalledWithArguments("orderBy=email desc");
            }

            [TestMethod]
            public void Order_by_multiple_fields()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                harness.Queryable
                    .OrderBy(x => x.GivenName)
                    .ThenByDescending(x => x.Username)
                    .ToList();

                harness.WasCalledWithArguments("orderBy=givenName,username desc");
            }

            [TestMethod]
            public void Order_throws_for_complex_overloads()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.OrderBy(x => x.GivenName, Substitute.For<IComparer<string>>()).ToList();
                });
            }
        }

        [TestClass]
        public class UnsupportedFilters
        {
            [TestMethod]
            public void Aggregate_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Aggregate((x, y) => x);
                });
            }

            [TestMethod]
            public void All_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.All(x => x.Email == "foo");
                });
            }

            [TestMethod]
            public void Average_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Average(x => 1.0);
                });
            }

            [TestMethod]
            public void Cast_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Cast<Tenant.ITenant>().ToList();
                });
            }

            [TestMethod]
            public void Concat_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Concat(Enumerable.Empty<IAccount>()).ToList();
                });
            }

            [TestMethod]
            public void Contains_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Contains(Substitute.For<IAccount>());
                });
            }

            [TestMethod]
            public void Distinct_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Distinct().ToList();
                });
            }

            [TestMethod]
            public void Except_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Except(Enumerable.Empty<IAccount>()).ToList();
                });
            }

            [TestMethod]
            public void GroupBy_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.GroupBy(x => x.Email).ToList();
                });
            }

            [TestMethod]
            public void GroupJoin_clause_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.GroupJoin(Enumerable.Empty<IAccount>(),
                        outer => outer.Email,
                        inner => inner.Username,
                        (outer, results) => new { outer.CreatedAt, results })
                    .ToList();
                });
            }

            [TestMethod]
            public void Intersect_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Intersect(Enumerable.Empty<IAccount>()).ToList();
                });
            }

            [TestMethod]
            public void Join_clause_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Join(Enumerable.Empty<IAccount>(),
                        outer => outer.Email,
                        inner => inner.Username,
                        (outer, inner) => outer.CreatedAt).ToList();
                });
            }

            [TestMethod]
            public void Last_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

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
            public void Max_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Max();
                });
            }

            [TestMethod]
            public void Min_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Min();
                });
            }

            [TestMethod]
            public void OfType_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.OfType<IAccount>().ToList();
                });
            }

            [TestMethod]
            public void Reverse_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Reverse().ToList();
                });
            }

            [TestMethod]
            public void Select_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable
                    .Select(x => x.Email)
                    .ToList();
                });
            }

            [TestMethod]
            public void SelectMany_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.SelectMany(x => x.Email).ToList();
                });
            }

            [TestMethod]
            public void SequenceEqual_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.SequenceEqual(Enumerable.Empty<IAccount>());
                });
            }

            [TestMethod]
            public void SkipWhile_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.SkipWhile(x => x.Email == "foobar").ToList();
                });
            }

            [TestMethod]
            public void Sum_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Sum(x => 1);
                });
            }

            [TestMethod]
            public void TakeWhile_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.TakeWhile(x => x.Email == "foobar").ToList();
                });
            }

            [TestMethod]
            public void Union_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Union(Enumerable.Empty<IAccount>()).ToList();
                });
            }

            [TestMethod]
            public void Zip_is_unsupported()
            {
                var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

                Should.Throw<NotSupportedException>(() =>
                {
                    harness.Queryable.Zip(Enumerable.Empty<IAccount>(),
                        (first, second) => first.Email == second.Email).ToList();
                });
            }
        }
    }
}
