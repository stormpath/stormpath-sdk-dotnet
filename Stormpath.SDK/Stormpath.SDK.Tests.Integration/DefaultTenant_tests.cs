// <copyright file="DefaultTenant_tests.cs" company="Stormpath, Inc.">
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

using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Stormpath.SDK.Tests.Integration
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single class", Justification = "Reviewed.")]
    public class DefaultTenant_tests
    {
        private static async Task Impl_Getting_tenant_applications(IntegrationHarness harness)
        {
            var tenant = await harness.Client.GetCurrentTenantAsync();
            var applications = await tenant.GetApplications().ToListAsync();

            applications.Count.ShouldNotBe(0);
        }

        public class DefaultClient_Basic_tests : BasicAuth_integration_tests
        {
            [Fact]
            public async Task Getting_tenant_applications() => await Impl_Getting_tenant_applications(harness);
        }

        public class DefaultClient_SAuthc1_tests : SAuthc1_integration_tests
        {
            [Fact]
            public async Task Getting_tenant_applications() => await Impl_Getting_tenant_applications(harness);
        }
    }
}