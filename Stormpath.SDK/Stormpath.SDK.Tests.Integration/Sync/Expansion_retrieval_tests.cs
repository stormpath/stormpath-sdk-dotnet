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

using System.Linq;
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Group;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tenant;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Sync
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
        public void Expanding_custom_data(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = client.GetResource<IAccount>(this.fixture.PrimaryAccountHref, o => o.Expand(x => x.GetCustomData));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_directory(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = client.GetResource<IAccount>(this.fixture.PrimaryAccountHref, o => o.Expand(x => x.GetDirectory));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_group_memberships(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = client.GetResource<IAccount>(this.fixture.PrimaryAccountHref, o => o.Expand(x => x.GetGroupMemberships, 0, 10));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_groups(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = client.GetResource<IAccount>(this.fixture.PrimaryAccountHref, o => o.Expand(x => x.GetGroups, 0, 10));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_tenant(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = client.GetResource<IAccount>(this.fixture.PrimaryAccountHref, o => o.Expand(x => x.GetTenant));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_account_store(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var mapping = app.GetAccountStoreMappings().Synchronously().First();

            client.GetResource<IAccountStoreMapping>(mapping.Href, o => o.Expand(x => x.GetAccountStore));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_application(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var mapping = app.GetAccountStoreMappings().Synchronously().First();

            client.GetResource<IAccountStoreMapping>(mapping.Href, o => o.Expand(x => x.GetApplication));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_accounts(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref, o => o.Expand(x => x.GetAccounts, 0, 10));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_account_store_mappings(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref, o => o.Expand(x => x.GetAccountStoreMappings, 0, 10));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_account_memberships(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var group = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref, o => o.Expand(x => x.GetAccountMemberships, 0, 10));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_applications(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var app = client.GetResource<ITenant>(this.fixture.TenantHref, o => o.Expand(x => x.GetApplications, 0, 10));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_directories(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var app = client.GetResource<ITenant>(this.fixture.TenantHref, o => o.Expand(x => x.GetDirectories, 0, 10));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_account(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = client.GetResource<IAccount>(this.fixture.PrimaryAccountHref);
            var membership = account.GetGroupMemberships().Synchronously().First();

            client.GetResource<IGroupMembership>(membership.Href, o => o.Expand(x => x.GetAccount));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Expanding_group(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = client.GetResource<IAccount>(this.fixture.PrimaryAccountHref);
            var membership = account.GetGroupMemberships().Synchronously().First();

            client.GetResource<IGroupMembership>(membership.Href, o => o.Expand(x => x.GetGroup));
        }
    }
}
