﻿// <copyright file="Limit.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class Limit_tests : Linq_tests
    {
        private CollectionTestHarness<IAccount> harness;

        public Limit_tests() : base()
        {
            harness = CollectionTestHarness<IAccount>.Create<IAccount>(url, resource);
        }

        [Fact]
        public void Take_with_constant_becomes_limit()
        {
            var query = harness.Queryable
                .Take(10);

            query.GeneratedArgumentsWere(url, resource, "limit=10");
        }

        [Fact]
        public void Take_with_variable_becomes_limit()
        {
            var limit = 20;
            var query = harness.Queryable
                .Take(limit);

            query.GeneratedArgumentsWere(url, resource, "limit=20");
        }

        [Fact]
        public void Take_with_function_becomes_limit()
        {
            var limitFunc = new Func<int>(() => 25);
            var query = harness.Queryable
                .Take(limitFunc());

            query.GeneratedArgumentsWere(url, resource, "limit=25");
        }

        [Fact]
        public void Take_multiple_calls_are_LIFO()
        {
            var query = harness.Queryable
                .Take(10).Take(5);

            // Expected behavior: the last call will be kept
            query.GeneratedArgumentsWere(url, resource, "limit=5");
        }
    }
}