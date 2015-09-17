﻿// <copyright file="Meta_tests.cs" company="Stormpath, Inc.">
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

using Shouldly;
using Stormpath.SDK.Tests.Integration.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Stormpath.SDK.Tests.Integration
{
    public class Meta_tests
    {
        private readonly ITestOutputHelper output;

        public Meta_tests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Integration_test_API_key_is_valid()
        {
            var apiKey = IntegrationTestClients.GetApiKey();

            apiKey.IsValid().ShouldBe(true);
            apiKey.GetId().ShouldNotBeNullOrEmpty();
            apiKey.GetSecret().ShouldNotBeNullOrEmpty();

            this.output.WriteLine($"ITs running with API key {apiKey.GetId()}");
        }
    }
}