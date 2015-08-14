// <copyright file="Properties_tests.cs" company="Stormpath, Inc.">
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

using Shouldly;
using Stormpath.SDK.Impl.Utility;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Utility
{
    public class Properties_tests
    {
        public class Constructor_parsing
        {
            [Fact]
            public void Null_string_returns_empty_dictionary()
            {
                var props = new Properties(null);
                props.Count().ShouldBe(0);
            }

            [Fact]
            public void Empty_string_returns_empty_dictionary()
            {
                var props = new Properties(string.Empty);
                props.Count().ShouldBe(0);
            }

            [Fact]
            public void Commented_lines_are_ignored()
            {
                var input =
                    "# This line is commented\r\n" +
                    "! so = is this one";

                var props = new Properties(input);
                props.Count().ShouldBe(0);
            }

            [Fact]
            public void Valid_lines_are_parsed()
            {
                var input = "key1 = value1";

                var props = new Properties(input);
                props.Count().ShouldBe(1);
                props.GetProperty("key1").ShouldBe("value1");
            }

            [Fact]
            public void Whitespace_around_separator_matters_not()
            {
                var input = "key1=value1";

                var props = new Properties(input);
                props.Count().ShouldBe(1);
                props.GetProperty("key1").ShouldBe("value1");
            }
        }

        public class GetProperty_method
        {
            [Fact]
            public void Returns_null_for_missing_property()
            {
                var props = new Properties(string.Empty);
                props.GetProperty("foo").ShouldBe(null);
            }

            [Fact]
            public void Returns_default_value_for_missing_property()
            {
                var props = new Properties(string.Empty);
                props.GetProperty("foo", defaultValue: "bar").ShouldBe("bar");
            }

            [Fact]
            public void Returns_value()
            {
                var props = new Properties("foo=baz");
                props.GetProperty("foo").ShouldBe("baz");
            }
        }
    }
}
