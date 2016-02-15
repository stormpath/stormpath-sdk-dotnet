// <copyright file="String_SplitToKeyValuePair_tests.cs" company="Stormpath, Inc.">
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

using System;
using Shouldly;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Shared.Extensions;
using Xunit;

namespace Stormpath.SDK.Tests.Extensions
{
    public class String_SplitToKeyValuePair_tests
    {
        [Fact]
        public void Throws_when_string_is_null()
        {
            Should.Throw<FormatException>(() =>
            {
                var bad = ((string)null).SplitToKeyValuePair('=');
            });
        }

        [Fact]
        public void Throws_when_string_does_not_contain_separator()
        {
            Should.Throw<FormatException>(() =>
            {
                var bad = "one,two".SplitToKeyValuePair('=');
            });
        }

        [Fact]
        public void Throws_when_string_does_not_have_two_sub_items()
        {
            Should.Throw<FormatException>(() =>
            {
                var bad = "one=two=three".SplitToKeyValuePair('=');
            });
        }

        [Fact]
        public void Returns_split_items_as_key_value_pair()
        {
            var args = "foo=bar".SplitToKeyValuePair('=');

            args.Key.ShouldBe("foo");
            args.Value.ShouldBe("bar");
        }
    }
}
