// <copyright file="Expansion_retrieval_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Group;
using Stormpath.SDK.Tenant;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Async
{
    [Collection(nameof(IntegrationTestCollection))]
    public class Expansion_retrieval_tests
    {
        private readonly IntegrationTestFixture fixture;

        public Expansion_retrieval_tests(IntegrationTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_custom_data(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = await client.GetResourceAsync<IAccount>(this.fixture.PrimaryAccountHref, o => o.Expand(x => x.GetCustomDataAsync));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_directory(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = await client.GetResourceAsync<IAccount>(this.fixture.PrimaryAccountHref, o => o.Expand(x => x.GetDirectoryAsync));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_group_memberships(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = await client.GetResourceAsync<IAccount>(this.fixture.PrimaryAccountHref, o => o.Expand(x => x.GetGroupMemberships, 0, 10));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_groups(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = await client.GetResourceAsync<IAccount>(this.fixture.PrimaryAccountHref, o => o.Expand(x => x.GetGroups, 0, 10));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_tenant(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = await client.GetResourceAsync<IAccount>(this.fixture.PrimaryAccountHref, o => o.Expand(x => x.GetTenantAsync));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_account_store(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var app = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var mapping = await app.GetAccountStoreMappings().FirstAsync();

            await client.GetResourceAsync<IAccountStoreMapping>(mapping.Href, o => o.Expand(x => x.GetAccountStoreAsync));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_application(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var app = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var mapping = await app.GetAccountStoreMappings().FirstAsync();

            await client.GetResourceAsync<IAccountStoreMapping>(mapping.Href, o => o.Expand(x => x.GetApplicationAsync));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_accounts(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var app = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref, o => o.Expand(x => x.GetAccounts, 0, 10));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_account_store_mappings(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var app = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref, o => o.Expand(x => x.GetAccountStoreMappings, 0, 10));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_account_memberships(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var group = await client.GetResourceAsync<IGroup>(this.fixture.PrimaryGroupHref, o => o.Expand(x => x.GetAccountMemberships, 0, 10));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_applications(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var app = await client.GetResourceAsync<ITenant>(this.fixture.TenantHref, o => o.Expand(x => x.GetApplications, 0, 10));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_directories(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var app = await client.GetResourceAsync<ITenant>(this.fixture.TenantHref, o => o.Expand(x => x.GetDirectories, 0, 10));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_account(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = await client.GetResourceAsync<IAccount>(this.fixture.PrimaryAccountHref);
            var membership = await account.GetGroupMemberships().FirstAsync();

            await client.GetResourceAsync<IGroupMembership>(membership.Href, o => o.Expand(x => x.GetAccountAsync));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Expanding_group(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = await client.GetResourceAsync<IAccount>(this.fixture.PrimaryAccountHref);
            var membership = await account.GetGroupMemberships().FirstAsync();

            await client.GetResourceAsync<IGroupMembership>(membership.Href, o => o.Expand(x => x.GetGroupAsync));
        }
    }
}
