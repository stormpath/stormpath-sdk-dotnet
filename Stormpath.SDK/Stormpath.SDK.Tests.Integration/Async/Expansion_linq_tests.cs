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
using Stormpath.SDK.Tests.Common.Integration;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Async
{
    [Collection(nameof(IntegrationTestCollection))]
    public class Expansion_linq_tests
    {
        private readonly TestFixture fixture;

        public Expansion_linq_tests(TestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Expanding_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetAccounts()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetCustomData())
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Expanding_directory(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetAccounts()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetDirectory())
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Expanding_group_memberships(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetAccounts()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetGroupMemberships(null, 10))
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Expanding_groups(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetAccounts()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetGroups(null, 10))
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Expanding_provider_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetAccounts()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetProviderData())
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Expanding_tenant(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetAccounts()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetTenant())
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Expanding_accounts(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetApplications()
                .Where(x => x.Description == "The Battle of Endor")
                .Expand(x => x.GetAccounts(null, 10))
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Expanding_account_store_mappings(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetApplications()
                .Where(x => x.Description == "The Battle of Endor")
                .Expand(x => x.GetAccountStoreMappings(null, 10))
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Expanding_organization_account_store_mappings(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetOrganizations()
                .Where(x => x.Description == "Star Wars")
                .Expand(x => x.GetAccountStoreMappings(null, 10))
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Expanding_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetApplications()
                .Where(x => x.Description == "The Battle of Endor")
                .Expand(x => x.GetDefaultAccountStore())
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Expanding_default_group_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetApplications()
                .Where(x => x.Description == "The Battle of Endor")
                .Expand(x => x.GetDefaultGroupStore())
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Expanding_provider(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetDirectories()
                .Filter("(primary)")
                .Expand(x => x.GetProvider())
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Expanding_account_memberships(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var account = await tenant
                .GetGroups()
                .Where(x => x.Description == "Humans")
                .Expand(x => x.GetAccountMemberships(null, 10))
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Expanding_membership_account(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var group = await client.GetResourceAsync<IGroup>(this.fixture.PrimaryGroupHref);

            var membership = await group
                .GetAccountMemberships()
                .Expand(x => x.GetAccount())
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Expanding_membership_group(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var group = await client.GetResourceAsync<IGroup>(this.fixture.PrimaryGroupHref);

            var membership = await group
                .GetAccountMemberships()
                .Expand(x => x.GetGroup())
                .FirstOrDefaultAsync();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Expanding_oAuthPolicy(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var app = await tenant
                .GetApplications()
                .Expand(x => x.GetOauthPolicy())
                .FirstOrDefaultAsync();
        }
    }
}
