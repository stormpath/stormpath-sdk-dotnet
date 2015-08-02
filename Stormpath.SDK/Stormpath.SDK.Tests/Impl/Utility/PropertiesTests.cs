using System;
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
                var props = new Properties("");
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
                var props = new Properties("");
                Assert.IsNull(props.GetProperty("foo"));
            }

            [TestMethod]
            [TestCategory("Impl.Utility")]
            public void Getting_nonexistent_property_with_default_value_returns_default()
            {
                var props = new Properties("");
                Assert.AreEqual("bar", props.GetProperty("foo", defaultValue: "bar"));
            }
        }
    }
}
