// <copyright file="PropertiesTests.cs" company="Stormpath, Inc.">
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Tests.Impl.Utility
{
    [TestClass]
    public class PropertiesTests
    {
        [TestClass]
        public class ConstructorParsing
        {
            [TestMethod]
            [TestCategory("Impl.Utility")]
            public void Null_string_returns_empty_dictionary()
            {
                var props = new Properties(null);
                Assert.AreEqual(0, props.Count());
            }

            [TestMethod]
            [TestCategory("Impl.Utility")]
            public void Empty_string_returns_empty_dictionary()
            {
                var props = new Properties(string.Empty);
                Assert.AreEqual(0, props.Count());
            }

            [TestMethod]
            [TestCategory("Impl.Utility")]
            public void Commented_lines_are_ignored()
            {
                var input =
                    "# This line is commented\r\n" +
                    "! so = is this one";

                var props = new Properties(input);
                Assert.AreEqual(0, props.Count());
            }

            [TestMethod]
            [TestCategory("Impl.Utility")]
            public void Valid_lines_are_parsed()
            {
                var input = "key1 = value1";

                var props = new Properties(input);
                Assert.AreEqual(1, props.Count());
                Assert.AreEqual("value1", props.GetProperty("key1"));
            }

            [TestMethod]
            [TestCategory("Impl.Utility")]
            public void Whitespace_around_separator_matters_not()
            {
                var input = "key1=value1";

                var props = new Properties(input);
                Assert.AreEqual(1, props.Count());
                Assert.AreEqual("value1", props.GetProperty("key1"));
            }
        }

        [TestClass]
        public class GetPropertyMethod
        {
            [TestMethod]
            [TestCategory("Impl.Utility")]
            public void Getting_nonexistent_property_returns_null()
            {
                var props = new Properties(string.Empty);
                Assert.IsNull(props.GetProperty("foo"));
            }

            [TestMethod]
            [TestCategory("Impl.Utility")]
            public void Getting_nonexistent_property_with_default_value_returns_default()
            {
                var props = new Properties(string.Empty);
                Assert.AreEqual("bar", props.GetProperty("foo", defaultValue: "bar"));
            }
        }
    }
}
