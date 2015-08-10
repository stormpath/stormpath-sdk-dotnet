// <copyright file="WithinExtensionTests.cs" company="Stormpath, Inc.">
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
using Shouldly;
using Stormpath.SDK.Account;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    [TestClass]
    public class WithinExtensionTests
    {
        private static string url = "http://f.oo";
        private static string resource = "bar";

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Within_throws_when_using_outside_LINQ()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                var dto = new DateTimeOffset(DateTime.Now);
                var test = dto.Within(2015);
            });
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_date_using_shorthand_for_year()
        {
            var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

            harness.Queryable
                .Where(x => x.CreatedAt.Within(2015))
                .ToList();

            harness.WasCalledWithArguments("createdAt=2015");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_date_using_shorthand_for_month()
        {
            var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

            harness.Queryable
                .Where(x => x.CreatedAt.Within(2015, 01))
                .ToList();

            harness.WasCalledWithArguments("createdAt=2015-01");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_date_using_shorthand_for_day()
        {
            var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

            harness.Queryable
                .Where(x => x.CreatedAt.Within(2015, 01, 01))
                .ToList();

            harness.WasCalledWithArguments("createdAt=2015-01-01");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_date_using_shorthand_for_hour()
        {
            var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

            harness.Queryable
                .Where(x => x.CreatedAt.Within(2015, 01, 01, 12))
                .ToList();

            harness.WasCalledWithArguments("createdAt=2015-01-01T12");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_date_using_shorthand_for_minute()
        {
            var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

            harness.Queryable
                .Where(x => x.CreatedAt.Within(2015, 01, 01, 12, 30))
                .ToList();

            harness.WasCalledWithArguments("createdAt=2015-01-01T12:30");
        }

        [TestMethod]
        [TestCategory("Impl.Linq")]
        public void Where_date_using_shorthand_for_second()
        {
            var harness = TestHarness<IAccount>.Create<IAccount>(url, resource);

            harness.Queryable
                .Where(x => x.CreatedAt.Within(2015, 01, 01, 12, 30, 31))
                .ToList();

            harness.WasCalledWithArguments("createdAt=2015-01-01T12:30:31");
        }
    }
}
