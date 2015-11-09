// <copyright file="Uri_WithoutQueryAndFragment_tests.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
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

using System;
using Shouldly;
using Stormpath.SDK.Impl.Extensions;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Extensions
{
    public class Uri_WithoutQueryAndFragment_tests
    {
        [Fact]
        public void Idempotent_when_no_query_or_fragment_exists()
        {
            var test = new Uri("http://foobar.com/foo");

            test.WithoutQueryAndFragment().ToString()
                .ShouldBe("http://foobar.com/foo");
        }

        [Fact]
        public void Removes_query_part()
        {
            var test = new Uri("http://foobar.com/foo?bar=123");

            test.WithoutQueryAndFragment().ToString()
                .ShouldBe("http://foobar.com/foo");
        }

        [Fact]
        public void Removes_query_and_fragment_parts()
        {
            var test = new Uri("http://foobar.com/foo?bar=123#section1");

            test.WithoutQueryAndFragment().ToString()
                .ShouldBe("http://foobar.com/foo");
        }
    }
}
