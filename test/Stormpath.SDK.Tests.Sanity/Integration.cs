// <copyright file="Integration.cs" company="Stormpath, Inc.">
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
using Xunit;

namespace Stormpath.SDK.Tests.Sanity
{
    public class Integration
    {
        [Fact(Skip = "Restore VB sanity tests.")]
        [Obsolete("Restore VB sanity tests.")]
        public void Notice()
        {
            Assert.False(true);
        }

        //[DebugOnlyFact]
        //public void Equal_numbers_of_Csharp_and_Vb_tests_run()
        //{
        //    // TODO terrible hacks. The Equal_numbers_of_Csharp_and_Vb_tests theory can't use DebugOnly
        //    // so this test is just a flag.
        //}

        //[Theory]
        //[InlineData("Async")]
        //[InlineData("Sync")]
        //public void Equal_numbers_of_Csharp_and_Vb_tests(string @namespace)
        //{
        //    var csharpTests = typeof(Tests.Integration.IntegrationTestCollection).Assembly
        //        .GetTypes()
        //        .Where(x => x.Namespace == $"Stormpath.SDK.Tests.Integration.{@namespace}")
        //        .SelectMany(x => x.GetMethods())
        //        .Where(m => m.GetCustomAttributes().OfType<TheoryAttribute>().Any() || m.GetCustomAttributes().OfType<FactAttribute>().Any())
        //        .Select(x => Helpers.GetQualifiedMethodName(x));

        //    var vbAssembly = Helpers.GetVisualBasicIntegrationTestAssembly();
        //    if (vbAssembly == null)
        //    {
        //        if (Debugger.IsAttached)
        //        {
        //            Assertly.Fail("Could not locate VB IT assembly.");
        //        }

        //        return;
        //    }

        //    var vbTests = vbAssembly
        //        .GetTypes()
        //        .Where(x => x.Namespace == $"Stormpath.SDK.Tests.Integration.VB.{@namespace}")
        //        .SelectMany(x => x.GetMethods())
        //        .Where(m => m.GetCustomAttributes().OfType<TheoryAttribute>().Any() || m.GetCustomAttributes().OfType<FactAttribute>().Any())
        //        .Select(x => Helpers.GetQualifiedMethodName(x));

        //    var csharpButNotVb = csharpTests
        //        .Except(vbTests)
        //        .ToList();

        //    var vbButNotCsharp = vbTests
        //        .Except(csharpTests)
        //        .ToList();

        //    csharpButNotVb.Count.ShouldBe(
        //        0,
        //        $"These {@namespace} C# tests do not have a corresponding VB test:{Helpers.NL}{string.Join(", ", csharpButNotVb)}");

        //    vbButNotCsharp.Count.ShouldBe(
        //        0,
        //        $"These {@namespace} VB tests do not have a corresponding C# test:{Helpers.NL}{string.Join(", ", vbButNotCsharp)}");
        //}
    }
}
