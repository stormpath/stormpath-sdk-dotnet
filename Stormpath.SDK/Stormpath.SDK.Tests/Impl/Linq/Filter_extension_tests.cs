// <copyright file="Filter_extension_tests.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class Filter_extension_tests : Linq_tests
    {
        [Fact]
        public void With_simple_parameter()
        {
            // Act
            var query = this.Harness.Queryable
                .Filter("Joe");

            // Assert
            query.GeneratedArgumentsWere(this.Href, "q=Joe");
        }

        [Fact]
        public void Multiple_calls_are_LIFO()
        {
            var query = this.Harness.Queryable
                .Filter("Joe")
                .Filter("Joey");

            // Expected behavior: the last call will be kept
            query.GeneratedArgumentsWere(this.Href, "q=Joey");
        }
    }
}
