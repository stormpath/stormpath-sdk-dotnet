using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stormpath.SDK.Impl.Extensions;

namespace Stormpath.SDK.Tests.Impl.ExtensionsTests
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestClass]
        public class OrIfEmptyUseTests
        {
            [TestMethod]
            [TestCategory("Impl.Extensions")]
            public void Default_is_used_when_source_is_null()
            {
                string source = null;
                var result = source.OrIfEmptyUse("foo");
                Assert.AreEqual("foo", result);
            }

            [TestMethod]
            [TestCategory("Impl.Extensions")]
            public void Default_is_used_when_source_is_empty()
            {
                var source = string.Empty;
                var result = source.OrIfEmptyUse("bar");
                Assert.AreEqual("bar", result);
            }

            [TestMethod]
            [TestCategory("Impl.Extensions")]
            public void String_is_returned_if_not_null()
            {
                var source = "foo";
                string result = source.OrIfEmptyUse("bar");
                Assert.AreEqual("foo", result);
            }
        }
    }
}
