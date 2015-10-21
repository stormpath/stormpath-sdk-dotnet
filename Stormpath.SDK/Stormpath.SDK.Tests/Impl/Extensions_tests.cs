// <copyright file="Extensions_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Extensions;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class Extensions_tests
    {
        public class String_Nullable
        {
            [Fact]
            public void Returns_null_when_string_is_null()
            {
                ((string)null).Nullable().ShouldBeNull();
            }

            [Fact]
            public void Returns_null_when_string_is_empty()
            {
                string.Empty.Nullable().ShouldBeNull();
            }

            [Fact]
            public void Returns_string()
            {
                "foobar".Nullable().ShouldBe("foobar");
            }
        }

        public class String_ToBase64
        {
            [Fact]
            public void Returns_null_when_string_is_null()
            {
                ((string)null).ToBase64(System.Text.Encoding.UTF8).ShouldBeNull();
            }

            [Fact]
            public void Returns_empty_when_string_is_empty()
            {
                string.Empty.ToBase64(System.Text.Encoding.UTF8).ShouldBe(string.Empty);
            }

            [Fact]
            public void Returns_Zm9vYmFy_for_foobar()
            {
                "foobar".ToBase64(System.Text.Encoding.UTF8).ShouldBe("Zm9vYmFy");
            }
        }

        public class String_FromBase64
        {
            [Fact]
            public void Returns_null_when_string_is_null()
            {
                ((string)null).FromBase64(System.Text.Encoding.UTF8).ShouldBeNull();
            }

            [Fact]
            public void Returns_empty_when_string_is_empty()
            {
                string.Empty.FromBase64(System.Text.Encoding.UTF8).ShouldBe(string.Empty);
            }

            [Fact]
            public void Returns_Zm9vYmFy_for_foobar()
            {
                "Zm9vYmFy".FromBase64(System.Text.Encoding.UTF8).ShouldBe("foobar");
            }
        }

        public class String_SplitToKeyValuePair
        {
            [Fact]
            public void Throws_when_string_is_null()
            {
                Should.Throw<FormatException>(() =>
                {
                    var bad = ((string)null).SplitToKeyValuePair('=');
                });
            }

            [Fact]
            public void Throws_when_string_does_not_contain_separator()
            {
                Should.Throw<FormatException>(() =>
                {
                    var bad = "one,two".SplitToKeyValuePair('=');
                });
            }

            [Fact]
            public void Throws_when_string_does_not_have_two_sub_items()
            {
                Should.Throw<FormatException>(() =>
                {
                    var bad = "one=two=three".SplitToKeyValuePair('=');
                });
            }

            [Fact]
            public void Returns_split_items_as_key_value_pair()
            {
                var args = "foo=bar".SplitToKeyValuePair('=');

                args.Key.ShouldBe("foo");
                args.Value.ShouldBe("bar");
            }
        }

        public class Uri_WithoutQueryAndFragment
        {
            [Fact]
            public void Idempotent_when_no_query_or_fragment_exists()
            {
                var test = new Uri("http://foobar.com/foo");

                test.WithoutQueryAndFragment().ToString()
                    .ShouldBe("http://foobar.com/foo");
            }

            [Fact]
            public void Removes_query_part()
            {
                var test = new Uri("http://foobar.com/foo?bar=123");

                test.WithoutQueryAndFragment().ToString()
                    .ShouldBe("http://foobar.com/foo");
            }

            [Fact]
            public void Removes_query_and_fragment_parts()
            {
                var test = new Uri("http://foobar.com/foo?bar=123#section1");

                test.WithoutQueryAndFragment().ToString()
                    .ShouldBe("http://foobar.com/foo");
            }
        }

        public class DateTime_ToUnixTimestamp
        {
            [Fact]
            public void Start_of_Unix_epoch_is_zero()
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                epoch.ToUnixTimestamp().ShouldBe(0);
            }

            [Fact]
            public void Dec_31_2012_is_1356134399000()
            {
                var endOfMayanLongCountCycle = new DateTime(2012, 12, 21, 23, 59, 59, DateTimeKind.Utc);

                // Editor's note: World did not cataclysmically end on this date.
                endOfMayanLongCountCycle.ToUnixTimestamp().ShouldBe(1356134399000);
            }
        }
    }
}
