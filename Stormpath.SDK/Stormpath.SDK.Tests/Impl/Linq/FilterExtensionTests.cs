// <copyright file="FilterExtensionTests.cs" company="Stormpath, Inc.">
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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stormpath.SDK.Account;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    [TestClass]
    public class FilterExtensionTests
    {
        private static string url = "http://f.oo";
        private static string resource = "bar";

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Filter_with_simple_parameter()
        {
            // Arrange
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            // Act
            var query = harness.Queryable
                .Filter("Joe");

            // Assert
            query.GeneratedArgumentsWere(url, resource, "q=Joe");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Filter_multiple_calls_are_LIFO()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            var query = harness.Queryable
                .Filter("Joe")
                .Filter("Joey");

            // Expected behavior: the last call will be kept
            query.GeneratedArgumentsWere(url, resource, "q=Joey");
        }
    }
}
