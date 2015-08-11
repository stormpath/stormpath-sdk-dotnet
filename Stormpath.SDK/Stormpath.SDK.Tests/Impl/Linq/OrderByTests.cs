// <copyright file="OrderByTests.cs" company="Stormpath, Inc.">
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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stormpath.SDK.Account;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    [TestClass]
    public class OrderByTests
    {
        private static string url = "http://f.oo";
        private static string resource = "bar";

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Skip_becomes_offset()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            var query = harness.Queryable
                .Skip(10);

            query.GeneratedArgumentsWere(url, resource, "offset=10");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Skip_with_variable_becomes_offset()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            var offset = 20;
            var query = harness.Queryable
                .Skip(offset);

            query.GeneratedArgumentsWere(url, resource, "offset=20");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Skip_with_function_becomes_offset()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            var offsetFunc = new Func<int>(() => 25);
            var query = harness.Queryable
                .Skip(offsetFunc());

            query.GeneratedArgumentsWere(url, resource, "offset=25");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Skip_multiple_calls_are_LIFO()
        {
            var harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);

            var query = harness.Queryable
                .Skip(10).Skip(5);

            // Expected behavior: the last call will be kept
            query.GeneratedArgumentsWere(url, resource, "offset=5");
        }
    }
}
