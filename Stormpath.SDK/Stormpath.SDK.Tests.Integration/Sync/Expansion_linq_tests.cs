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
using Stormpath.SDK.Tests.Common.Integration;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Sync
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
        public void Expanding_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetAccounts()
                .Synchronously()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetCustomData())
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_directory(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetAccounts()
                .Synchronously()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetDirectory())
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_group_memberships(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetAccounts()
                .Synchronously()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetGroupMemberships())
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_groups(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetAccounts()
                .Synchronously()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetGroups())
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_provider_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetAccounts()
                .Synchronously()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetProviderData())
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_tenant(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetAccounts()
                .Synchronously()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Expand(x => x.GetTenant())
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_accounts(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetApplications()
                .Synchronously()
                .Where(x => x.Description == "The Battle of Endor")
                .Expand(x => x.GetAccounts())
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_account_store_mappings(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetApplications()
                .Synchronously()
                .Where(x => x.Description == "The Battle of Endor")
                .Expand(x => x.GetAccountStoreMappings(null, 10))
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetApplications()
                .Synchronously()
                .Where(x => x.Description == "The Battle of Endor")
                .Expand(x => x.GetDefaultAccountStore())
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_default_group_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetApplications()
                .Synchronously()
                .Where(x => x.Description == "The Battle of Endor")
                .Expand(x => x.GetDefaultGroupStore())
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_provider(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetDirectories()
                .Synchronously()
                .Filter("(primary)")
                .Expand(x => x.GetProvider())
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_account_memberships(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var account = tenant
                .GetGroups()
                .Synchronously()
                .Where(x => x.Description == "Humans")
                .Expand(x => x.GetAccountMemberships(null, 10))
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_membership_account(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var group = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref);

            var membership = group
                .GetAccountMemberships()
                .Synchronously()
                .Expand(x => x.GetAccount())
                .FirstOrDefault();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_membership_group(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var group = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref);

            var membership = group
                .GetAccountMemberships()
                .Synchronously()
                .Expand(x => x.GetGroup())
                .FirstOrDefault();
        }
    }
}
