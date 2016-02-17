// <copyright file="_Meta_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.Configuration;
using Stormpath.SDK.Tests.Common.Integration;
using Xunit;
using Xunit.Abstractions;

namespace Stormpath.SDK.Tests.Integration
{
#pragma warning disable SA1300 // Element must begin with upper-case letter
    public class _Meta_tests
    {
        private readonly ITestOutputHelper output;

        public _Meta_tests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Output_API_base_URL()
        {
            this.output.WriteLine($"ITs running against base URL: {TestClients.CurrentConfiguration.Client.BaseUrl}");
        }
    }
#pragma warning restore SA1300 // Element must begin with upper-case letter
}
