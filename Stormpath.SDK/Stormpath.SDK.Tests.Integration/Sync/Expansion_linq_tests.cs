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

using System.Linq;
using Stormpath.SDK.Group;
using Stormpath.SDK.Sync;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Sync
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
        public void Expanding_custom_data(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetAccounts()
                .Synchronously()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetCustomData)
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_directory(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetAccounts()
                .Synchronously()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetDirectory)
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_group_memberships(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetAccounts()
                .Synchronously()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetGroupMemberships)
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_groups(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetAccounts()
                .Synchronously()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetGroups)
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_provider_data(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetAccounts()
                .Synchronously()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetProviderData)
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_tenant(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetAccounts()
                .Synchronously()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetTenant)
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_accounts(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetApplications()
                .Synchronously()
                .Where(x => x.Description == "The Battle of Endor")
                .Expand(x => x.GetAccounts, limit: 10)
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_account_store_mappings(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetApplications()
                .Synchronously()
                .Where(x => x.Description == "The Battle of Endor")
                .Expand(x => x.GetAccountStoreMappings, limit: 10)
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_default_account_store(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetApplications()
                .Synchronously()
                .Where(x => x.Description == "The Battle of Endor")
                .Expand(x => x.GetDefaultAccountStore)
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_default_group_store(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetApplications()
                .Synchronously()
                .Where(x => x.Description == "The Battle of Endor")
                .Expand(x => x.GetDefaultGroupStore)
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_provider(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetDirectories()
                .Synchronously()
                .Filter("(primary)")
                .Expand(x => x.GetProvider)
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_account_memberships(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetGroups()
                .Synchronously()
                .Where(x => x.Description == "Humans")
                .Expand(x => x.GetAccountMemberships, limit: 10)
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_membership_account(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var group = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref);

            var membership = group
                .GetAccountMemberships()
                .Synchronously()
                .Expand(x => x.GetAccount)
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_membership_group(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var group = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref);

            var membership = group
                .GetAccountMemberships()
                .Synchronously()
                .Expand(x => x.GetGroup)
                .FirstOrDefault();
        }
    }
}
