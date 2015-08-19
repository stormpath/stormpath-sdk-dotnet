// <copyright file="Integration_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Api;
using Stormpath.SDK.Client;

namespace Stormpath.SDK.Tests.Integration
{
    public abstract class Integration_tests
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields must be private", Justification = "Reviewed.")]
        protected readonly IntegrationHarness harness;

        public Integration_tests()
        {
            // Expect that API keys are in environment variables. (works with travis-ci)
            var apiKey = ClientApiKeys.Builder().Build();
            apiKey.IsValid().ShouldBe(true);

            var client = Clients
                .Builder()
                .SetApiKey(apiKey)
                .SetAuthenticationScheme(AuthenticationScheme.Basic) // TODO
                .Build();

            harness = new IntegrationHarness(client);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
    public sealed class IntegrationHarness
    {
        public IntegrationHarness(IClient client)
        {
            this.Client = client;
        }

        public IClient Client { get; }
    }
}
