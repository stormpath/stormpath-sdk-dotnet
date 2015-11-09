// <copyright file="Dictionary_TryGetValueAsString_tests.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using Shouldly;
using Stormpath.SDK.Impl.Extensions;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Extensions
{
    public class Dictionary_TryGetValueAsString_tests
    {
        [Fact]
        public void Returns_null_when_key_does_not_exist()
        {
            var dict = new Dictionary<string, object>() { ["foo"] = "bar" };

            string baz = null;
            var result = dict.TryGetValueAsString("baz", out baz);

            result.ShouldBeFalse();
            baz.ShouldBeNull();
        }

        [Fact]
        public void Returns_null_when_value_is_null()
        {
            var dict = new Dictionary<string, object>() { ["foo"] = null };

            string foo = null;
            var result = dict.TryGetValueAsString("foo", out foo);

            result.ShouldBeTrue();
            foo.ShouldBeNull();
        }

        [Fact]
        public void Returns_string_value()
        {
            var dict = new Dictionary<string, object>() { ["foo"] = "bar" };

            string foo = null;
            var result = dict.TryGetValueAsString("foo", out foo);

            result.ShouldBeTrue();
            foo.ShouldBe("bar");
        }

        [Fact]
        public void Returns_value_as_string()
        {
            var dict = new Dictionary<string, object>() { ["foo"] = 123 };

            string foo = null;
            var result = dict.TryGetValueAsString("foo", out foo);

            result.ShouldBeTrue();
            foo.ShouldBe("123");
        }
    }
}
