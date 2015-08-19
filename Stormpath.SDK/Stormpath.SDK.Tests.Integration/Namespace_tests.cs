// <copyright file="Namespace_tests.cs" company="Stormpath, Inc.">
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

using System.Linq;
using System.Reflection;
using Shouldly;
using Xunit;

namespace Stormpath.SDK.Tests.Integration
{
    public class Namespace_tests
    {
        [Fact]
        public void Impl_members_are_hidden()
        {
            var typesInNamespace = Assembly
                .GetAssembly(typeof(Stormpath.SDK.Client.IClient))
                .GetAllTypesInNamespace("Stormpath.SDK.Impl")
                .ToList();

            typesInNamespace.Count.ShouldBe(0, customMessage: () =>
            {
                return $"These types are visible: {string.Join(", ", typesInNamespace)}";
            });
        }
    }
}
