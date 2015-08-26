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
    public class DefaultTenant_tests : IntegrationTest
    {
        [Theory]
        [MemberData(nameof(GetClients))]
        public async Task Getting_tenant_accounts(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();
            var accounts = await tenant.GetAccounts().ToListAsync();

            accounts.Count.ShouldNotBe(0);
        }

        [Theory]
        [MemberData(nameof(GetClients))]
        public async Task Getting_tenant_applications(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();
            var applications = await tenant.GetApplications().ToListAsync();

            applications.Count.ShouldNotBe(0);
        }

        [Theory]
        [MemberData(nameof(GetClients))]
        public async Task Getting_tenant_directories(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();
            var directories = await tenant.GetDirectories().ToListAsync();

            directories.Count.ShouldNotBe(0);
        }

        [Theory(Skip = "Tenant has no groups yet")]
        [MemberData(nameof(GetClients))]
        public async Task Getting_tenant_groups(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();
            var groups = await tenant.GetGroups().ToListAsync();

            groups.Count.ShouldNotBe(0);
        }
    }
}