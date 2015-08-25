// <copyright file="IntegrationTest.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using Shouldly;
using Stormpath.SDK.Api;

namespace Stormpath.SDK.Tests.Integration
{
    public abstract class IntegrationTest
    {
        public static IEnumerable<object[]> GetClients()
        {
            yield return new object[] { new TestClientBuilder("Basic") };
            yield return new object[] { new TestClientBuilder("SAuthc1") };
        }

        public static IClientApiKey GetApiKey()
        {
            // Expect that API keys are in environment variables. (works with travis-ci)
            var apiKey = ClientApiKeys.Builder().Build();
            apiKey.IsValid().ShouldBe(true, "These integration tests look for a valid API Key and Secret in your local environment variables.");
            return apiKey;
        }
    }
}
