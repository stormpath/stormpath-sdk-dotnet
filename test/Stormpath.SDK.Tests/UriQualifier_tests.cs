// <copyright file="UriQualifier_tests.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using Shouldly;
using Stormpath.SDK.Http;
using Xunit;

namespace Stormpath.SDK.Tests
{
    public class UriQualifier_tests
    {
        private readonly string fakeBaseUrl = @"http://api.foo.bar";

        [Fact]
        public void Relative_paths_are_fully_qualified()
        {
            var qualifier = new UriQualifier(this.fakeBaseUrl);

            var uri = qualifier.EnsureFullyQualified("path/to/resource");

            uri.ShouldBe($"{this.fakeBaseUrl}/path/to/resource");
        }

        [Fact]
        public void Relative_paths_with_leading_slash_are_fully_qualified()
        {
            var qualifier = new UriQualifier(this.fakeBaseUrl);

            var uri = qualifier.EnsureFullyQualified("/path/to/resource");

            uri.ShouldBe($"{this.fakeBaseUrl}/path/to/resource");
        }

        [Fact]
        public void Idempotent_on_already_fully_qualified_paths()
        {
            var qualifier = new UriQualifier(this.fakeBaseUrl);

            var alreadyQualified = qualifier.EnsureFullyQualified("http://api.foo.bar/foo/bar");

            alreadyQualified.ShouldBe("http://api.foo.bar/foo/bar");
        }
    }
}
