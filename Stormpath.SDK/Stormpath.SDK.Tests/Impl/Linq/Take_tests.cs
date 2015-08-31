// <copyright file="Take_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class Take_tests : Linq_tests
    {
        [Fact]
        public void Take_with_constant_becomes_limit()
        {
            var query = this.Harness.Queryable
                .Take(10);

            query.GeneratedArgumentsWere(this.Href, "limit=10");
        }

        [Fact]
        public void Take_with_variable_becomes_limit()
        {
            var limit = 20;
            var query = this.Harness.Queryable
                .Take(limit);

            query.GeneratedArgumentsWere(this.Href, "limit=20");
        }

        [Fact]
        public void Take_with_function_becomes_limit()
        {
            var limitFunc = new Func<int>(() => 25);
            var query = this.Harness.Queryable
                .Take(limitFunc());

            query.GeneratedArgumentsWere(this.Href, "limit=25");
        }

        [Fact]
        public void Take_multiple_calls_are_LIFO()
        {
            var query = this.Harness.Queryable
                .Take(10).Take(5);

            // Expected behavior: the last call will be kept
            query.GeneratedArgumentsWere(this.Href, "limit=5");
        }

        [Fact]
        public void Take_limit_is_100()
        {
            var query = this.Harness.Queryable
                .Take(101);

            // Expected behavior: the last call will be kept
            query.GeneratedArgumentsWere(this.Href, "limit=100");
        }
    }
}
