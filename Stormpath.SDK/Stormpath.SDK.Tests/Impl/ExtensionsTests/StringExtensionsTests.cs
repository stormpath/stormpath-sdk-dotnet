// <copyright file="StringExtensionsTests.cs" company="Stormpath, Inc.">
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
