// <copyright file="Expansion_linq_tests.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
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

using System.Threading.Tasks;
using Stormpath.SDK.Group;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Async
{
    [Collection("Live tenant tests")]
    public class Expansion_linq_tests
    {
        private readonly IntegrationTestFixture fixture;

        public Expansion_linq_tests(IntegrationTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_custom_data(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetAccounts()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetCustomDataAsync)
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_directory(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetAccounts()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetDirectoryAsync)
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_group_memberships(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetAccounts()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetGroupMemberships, limit: 10)
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_groups(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetAccounts()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetGroups, limit: 10)
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_provider_data(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetAccounts()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetProviderDataAsync)
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_tenant(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetAccounts()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetTenantAsync)
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_accounts(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetApplications()
                .Where(x => x.Description == "The Battle of Endor")
                .Expand(x => x.GetAccounts, limit: 10)
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_account_store_mappings(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetApplications()
                .Where(x => x.Description == "The Battle of Endor")
                .Expand(x => x.GetAccountStoreMappings, limit: 10)
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_default_account_store(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetApplications()
                .Where(x => x.Description == "The Battle of Endor")
                .Expand(x => x.GetDefaultAccountStoreAsync)
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_default_group_store(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetApplications()
                .Where(x => x.Description == "The Battle of Endor")
                .Expand(x => x.GetDefaultGroupStoreAsync)
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_provider(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetDirectories()
                .Filter("(primary)")
                .Expand(x => x.GetProviderAsync)
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_account_memberships(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetGroups()
                .Where(x => x.Description == "Humans")
                .Expand(x => x.GetAccountMemberships, limit: 10)
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_membership_account(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var group = await client.GetResourceAsync<IGroup>(this.fixture.PrimaryGroupHref);

            var membership = await group
                .GetAccountMemberships()
                .Expand(x => x.GetAccountAsync)
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_membership_group(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var group = await client.GetResourceAsync<IGroup>(this.fixture.PrimaryGroupHref);

            var membership = await group
                .GetAccountMemberships()
                .Expand(x => x.GetGroupAsync)
                .FirstOrDefaultAsync();
        }
    }
}
