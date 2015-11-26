// <copyright file="Enumerable_IsNullOrEmpty_tests.cs" company="Stormpath, Inc.">
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
    public class Enumerable_IsNullOrEmpty_tests
    {
        [Fact]
        public void Returns_true_when_null()
        {
            IDictionary<string, object> dict = null;

            dict.IsNullOrEmpty().ShouldBeTrue();
        }

        [Fact]
        public void Returns_true_when_empty()
        {
            var dict = new Dictionary<string, object>();

            dict.IsNullOrEmpty().ShouldBeTrue();
        }

        [Fact]
        public void Returns_false_when_items_exist()
        {
            var dict = new Dictionary<string, object>()
            {
                ["foo"] = new object()
            };

            dict.IsNullOrEmpty().ShouldBeFalse();
        }
    }
}
