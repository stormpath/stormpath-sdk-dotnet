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

using System;
using System.Linq;
using System.Reflection;
using Shouldly;
using Xunit;

namespace Stormpath.SDK.Tests.Integration
{
    public class Sanity_tests
    {
        [Fact]
        public void Equal_numbers_of_sync_and_async_tests()
        {
            var asyncTests = typeof(Tests.Integration.IntegrationTestCollection).Assembly
                .GetTypes()
                .Where(x => x.Namespace == "Stormpath.SDK.Tests.Integration.Async")
                .SelectMany(x => x.GetMethods())
                .Where(m => m.GetCustomAttributes().OfType<TheoryAttribute>().Any() || m.GetCustomAttributes().OfType<FactAttribute>().Any())
                .Select(x => GetQualifiedMethodName(x));

            asyncTests.ShouldNotBeEmpty();

            var syncTests = typeof(Tests.Integration.IntegrationTestCollection).Assembly
                .GetTypes()
                .Where(x => x.Namespace == "Stormpath.SDK.Tests.Integration.Sync")
                .SelectMany(x => x.GetMethods())
                .Where(m => m.GetCustomAttributes().OfType<TheoryAttribute>().Any() || m.GetCustomAttributes().OfType<FactAttribute>().Any())
                .Select(x => GetQualifiedMethodName(x));

            syncTests.ShouldNotBeEmpty();

            var asyncButNotSync = asyncTests
                .Except(syncTests)
                .ToList();

            var syncButNotAsync = syncTests
                .Except(asyncTests)
                .ToList();

            asyncButNotSync.Count.ShouldBe(
                0,
                $"These async tests do not have a corresponding sync test:{Environment.NewLine}{string.Join(", ", asyncButNotSync)}");

            syncButNotAsync.Count.ShouldBe(
                0,
                $"These sync tests do not have a corresponding async test:{Environment.NewLine}{string.Join(", ", syncButNotAsync)}");
        }

        private static string GetQualifiedMethodName(MethodInfo m)
            => $"{m.DeclaringType.Name}.{m.Name}";

    }
}
