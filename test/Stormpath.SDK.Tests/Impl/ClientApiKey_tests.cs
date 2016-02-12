// <copyright file="ClientApiKey_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Api;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class ClientApiKey_tests
    {
        [Fact]
        public void When_comparing_same_values()
        {
            var apiKey1 = new DefaultClientApiKey("fooId", "fooSecret");
            var apiKey2 = new DefaultClientApiKey("fooId", "fooSecret");

            apiKey1.ShouldBe(apiKey2);
            (apiKey1 == apiKey2).ShouldBeTrue();
        }

        [Fact]
        public void When_comparing_different_values()
        {
            var apiKey1 = new DefaultClientApiKey("fooId", "fooSecret");
            var apiKey2 = new DefaultClientApiKey("barId", "barSecret");

            apiKey1.ShouldNotBe(apiKey2);
            (apiKey1 != apiKey2).ShouldBeTrue();
        }

        [Fact]
        public void When_comparing_values_are_case_sensitive()
        {
            var apiKey1 = new DefaultClientApiKey("fooId", "fooSecret");
            var apiKey2 = new DefaultClientApiKey("fooID", "fooSecret");
            var apiKey3 = new DefaultClientApiKey("fooId", "FooSecret");

            apiKey1.ShouldNotBe(apiKey2);
            apiKey1.ShouldNotBe(apiKey3);
            (apiKey1 == apiKey2).ShouldBeFalse();
            (apiKey2 == apiKey3).ShouldBeFalse();
        }
    }
}
