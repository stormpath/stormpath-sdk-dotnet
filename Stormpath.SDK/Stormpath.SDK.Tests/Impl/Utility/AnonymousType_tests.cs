// <copyright file="AnonymousType_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl;
using Stormpath.SDK.Impl.Utility;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Utility
{
    public class AnonymousType_tests
    {
        [Fact]
        public void Converting_anonymous_type_to_dictionary()
        {
            var anon = new
            {
                juliet = "Wherefore art thou, Romeo?",
                romeo = "I take thee at thy word.",
                terribleIdea = true
            };

            var result = AnonymousType.ToDictionary(anon);

            result.Count.ShouldBe(3);
            result["juliet"].ShouldBe("Wherefore art thou, Romeo?");
            result["romeo"].ShouldBe("I take thee at thy word.");
            result["terribleIdea"].ShouldBe(true);
        }

        [Fact]
        public void Returns_null_for_other_types()
        {
            AnonymousType.ToDictionary(null).ShouldBe(null);
            AnonymousType.ToDictionary("test").ShouldBe(null);
            AnonymousType.ToDictionary(new Dictionary<string, object>() { { "foo", "bar" } }).ShouldBe(null);
            AnonymousType.ToDictionary(new NullLogger()).ShouldBe(null);
        }
    }
}
