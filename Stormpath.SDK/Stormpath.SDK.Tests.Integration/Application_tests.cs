// <copyright file="Application_tests.cs" company="Stormpath, Inc.">
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
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Tests.Integration.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Integration
{
    [Collection("Live tenant tests")]
    public class Application_tests
    {
        private readonly IntegrationTestFixture fixture;

        public Application_tests(IntegrationTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Getting_tenant_applications(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();
            var applications = await tenant.GetApplications().ToListAsync();

            applications.Count.ShouldNotBe(0);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Searching_by_name(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            var application = await tenant.GetApplications()
                .Where(app => app.Name.StartsWith($".NET ITs {this.fixture.TestIdentifier}"))
                .FirstAsync();

            application.Description.ShouldBe("The Battle of Endor");
        }
    }
}
