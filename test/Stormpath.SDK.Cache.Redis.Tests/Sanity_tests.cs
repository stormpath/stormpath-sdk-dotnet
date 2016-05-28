// <copyright file="Sanity_tests.cs" company="Stormpath, Inc.">
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

using System.Linq;
using System.Reflection;
using FluentAssertions;
using Stormpath.SDK.Tests.Common;
using Xunit;

namespace Stormpath.SDK.Cache.Redis.Tests
{
    public class Sanity_tests
    {
        private static readonly string Self = $"{nameof(Sanity_tests)}.{nameof(All_tests_must_be_debug_only)}";

        [Fact]
        public void All_tests_must_be_debug_only()
        {
            var facts = typeof(Sanity_tests)
                .GetTypeInfo()
                .Assembly
                .GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes<FactAttribute>(false).Count() > 0);

            var theories = typeof(Sanity_tests)
                .GetTypeInfo()
                .Assembly
                .GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes<TheoryAttribute>(false).Count() > 0);

            var nonDebugFactNames = facts
                .Where(m => m.GetCustomAttributes<DebugOnlyFactAttribute>(false).Count() == 0)
                .Select(m => $"{m.DeclaringType.Name}.{m.Name}")
                .Except(new string[] { Self });

            var nonDebugTheoryNames = theories
                .Where(m => m.GetCustomAttributes<DebugOnlyTheoryAttribute>(false).Count() == 0)
                .Select(m => $"{m.DeclaringType.Name}.{m.Name}");

            nonDebugFactNames.Count().Should().Be(0);
            nonDebugTheoryNames.Count().Should().Be(0);
        }
    }
}
