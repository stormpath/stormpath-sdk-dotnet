// <copyright file="QueryString_tests.cs" company="Stormpath, Inc.">
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
using Shouldly;
using Stormpath.SDK.Http;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class QueryString_tests
    {
        public class Construction_tests
        {
            [Fact]
            public void With_no_parameters()
            {
                var uri = new Uri("http://foo/bar");
                var qs = new QueryString(uri);

                qs.ToString().ShouldBe(string.Empty);
                qs.ToString(canonical: true).ShouldBe(string.Empty);
            }

            [Fact]
            public void Creating_from_string()
            {
                var qs = new QueryString("http://foo.bar/baz/?test1=2&foo=bar");

                qs.ToString().ShouldBe("foo=bar&test1=2");
            }

            [Fact]
            public void Creating_from_empty_string()
            {
                var qs = new QueryString(string.Empty);

                qs.ToString().ShouldBe(string.Empty);
            }

            [Fact]
            public void Creating_from_map()
            {
                var args = new Dictionary<string, string>()
            {
                { "foo", "bar" },
                { "abc", "123" },
            };
                var qs = new QueryString(args);

                qs.ToString().ShouldBe("abc=123&foo=bar");
            }

            [Fact]
            public void Creating_from_null_map()
            {
                var qs = new QueryString((Dictionary<string, string>)null);

                qs.ToString().ShouldBe(string.Empty);
            }

            [Fact]
            public void Creating_from_empty_map()
            {
                var args = new Dictionary<string, string>();
                var qs = new QueryString(args);

                qs.ToString().ShouldBe(string.Empty);
            }

            [Fact]
            public void Datetime_search_strings_are_not_escaped()
            {
                /* Special case: when createdAt=[] or modifiedAt=[] terms
                 * appear in query string they should not be urlEncoded
                 */

                var notApplicable = new QueryString("foo=[bar:baz]");
                notApplicable.ToString().ShouldBe("foo=%5Bbar%3Abaz%5D");

                var createdAtSearch = new QueryString("createdAt=[2015-06-01T12:00:59Z,)");
                createdAtSearch.ToString().ShouldBe("createdAt=[2015-06-01T12:00:59Z,)");

                var modifiedAtSearch = new QueryString("modifiedAt=(2015-01-01T06:01:59Z,2016-01-01T22:30:59Z]");
                modifiedAtSearch.ToString().ShouldBe("modifiedAt=(2015-01-01T06:01:59Z,2016-01-01T22:30:59Z]");
            }

            [Fact]
            public void Datetime_search_strings_are_escaped_when_canonical_flag_is_set()
            {
                /* If the canonical flag is set, the above special case does not apply.
                 * The SAuthc1 algorithm expects that the entire string is URLEncoded,
                 * even if pieces of the actual request are not.
                 */

                var createdAtSearch = new QueryString("createdAt=[2015-06-01T12:00:59Z,)");
                createdAtSearch.ToString(canonical: true).ShouldBe("createdAt=%5B2015-06-01T12%3A00%3A59Z%2C%29");
            }
        }

        public class ToString_tests
        {
            [Fact]
            public void Keys_and_value_case_is_preserved()
            {
                var qs = new QueryString(new Uri("http://foo.bar/baz?TEST=foo&bar=BAZ"));

                qs.ToString().ShouldBe("bar=BAZ&TEST=foo");
            }

            [Fact]
            public void Keys_are_sorted()
            {
                var qs = new QueryString(new Uri("https://foo.bar/?zulu=5&beta=foo&alpha=9"));

                qs.ToString(canonical: true).ShouldBe("alpha=9&beta=foo&zulu=5");
            }

            [Fact]
            public void Keys_with_no_values_are_ok()
            {
                var qs = new QueryString(new Uri("https://foo.bar?foo=bar&baz=&three"));

                qs.ToString().ShouldBe("baz=&foo=bar&three=");
            }

            [Fact]
            public void Canonicalize_flag_is_observed()
            {
                var qs = new QueryString("search=start*&test=one two&tilde=~");

                qs.ToString().ShouldBe("search=start*&test=one+two&tilde=%7E");
                qs.ToString(canonical: true).ShouldBe("search=start%2A&test=one%20two&tilde=~");
            }
        }

        public class Merge_tests
        {
            [Fact]
            public void Merging_with_null_is_idempotent()
            {
                var qs1 = new QueryString("foo=bar");

                var merged = qs1.Merge(null);

                merged.ShouldBe(qs1);
            }

            [Fact]
            public void Merging_with_empty_list_is_idempotent()
            {
                var qs1 = new QueryString("foo=bar");

                var merged = qs1.Merge(new QueryString(string.Empty));

                merged.ShouldBe(qs1);
            }

            [Fact]
            public void Merging_with_no_conflicts_is_additive()
            {
                var qs1 = new QueryString("foo=bar");
                var qs2 = new QueryString("bar=baz");

                var merged = qs1.Merge(qs2);

                merged.ToString().ShouldBe("bar=baz&foo=bar");
            }

            [Fact]
            public void Merging_with_conflicts_keeps_original_items()
            {
                var qs1 = new QueryString("foo=bar&abc=123");
                var qs2 = new QueryString("foo=baz");

                var merged = qs1.Merge(qs2);

                merged.ToString().ShouldBe("abc=123&foo=bar");
            }
        }

        public class Equality_tests
        {
            [Fact]
            public void Same_items_are_equal()
            {
                var qs1 = new QueryString("foo=bar&baz=123");
                var qs2 = new QueryString("foo=bar&baz=123");

                qs1.ShouldBe(qs2);
                (qs1 == qs2).ShouldBeTrue();
            }

            [Fact]
            public void Different_items_are_equal()
            {
                var qs1 = new QueryString("foo=bar&baz=123");
                var qs2 = new QueryString("foo=bar&baz=abc");

                qs1.ShouldNotBe(qs2);
                (qs1 != qs2).ShouldBeTrue();
            }
        }
    }
}
