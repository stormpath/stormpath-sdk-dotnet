// <copyright file="OrderBy_tests.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class OrderBy_tests : Linq_tests
    {
        [Fact]
        public void Order_by_a_field()
        {
            var query = Harness.Queryable
                .OrderBy(x => x.GivenName);

            query.GeneratedArgumentsWere(Url, Resource, "orderBy=givenName");
        }

        [Fact]
        public void Order_by_a_field_descending()
        {
            var query = Harness.Queryable
                .OrderByDescending(x => x.Email);

            query.GeneratedArgumentsWere(Url, Resource, "orderBy=email desc");
        }

        [Fact]
        public void Order_by_multiple_fields()
        {
            var query = Harness.Queryable
                .OrderBy(x => x.GivenName)
                .ThenByDescending(x => x.Username);

            query.GeneratedArgumentsWere(Url, Resource, "orderBy=givenName,username desc");
        }

        [Fact]
        public void Order_throws_for_complex_overloads()
        {
            Should.Throw<NotSupportedException>(() =>
            {
                Harness.Queryable.OrderBy(x => x.GivenName, Substitute.For<IComparer<string>>()).ToList();
            });
        }
    }
}
