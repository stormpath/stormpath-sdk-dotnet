// <copyright file="Expansion_retrieval_tests.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
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

using System;
using System.Linq;
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Group;
using Stormpath.SDK.Organization;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tenant;
using Stormpath.SDK.Tests.Common.Integration;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Sync
{
    [Collection(nameof(IntegrationTestCollection))]
    public class Expansion_retrieval_tests
    {
        private readonly TestFixture fixture;

        public Expansion_retrieval_tests(TestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var account = client.GetResource<IAccount>(this.fixture.PrimaryAccountHref, o => o.Expand(x => x.GetCustomData()));
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_directory(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var account = client.GetResource<IAccount>(this.fixture.PrimaryAccountHref, o => o.Expand(x => x.GetDirectory()));
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_group_memberships(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var account = client.GetResource<IAccount>(this.fixture.PrimaryAccountHref, o => o.Expand(x => x.GetGroupMemberships(0, 10)));
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_groups(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var account = client.GetResource<IAccount>(this.fixture.PrimaryAccountHref, o => o.Expand(x => x.GetGroups(0, 10)));
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_tenant(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var account = client.GetResource<IAccount>(this.fixture.PrimaryAccountHref, o => o.Expand(x => x.GetTenant()));
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var mapping = app.GetAccountStoreMappings().Synchronously().First();

            client.GetResource<IAccountStoreMapping>(mapping.Href, o => o.Expand(x => x.GetAccountStore()));
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        [Obsolete("Remove this test after 1.0 breaking change.")]
        public void Expanding_application_from_generic_mapping(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var mapping = app.GetAccountStoreMappings().Synchronously().First();

            client.GetResource<IAccountStoreMapping>(mapping.Href, o => o.Expand(x => x.GetApplication()));
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var mapping = app.GetAccountStoreMappings().Synchronously().First();

            client.GetResource<IApplicationAccountStoreMapping>(mapping.Href, o => o.Expand(x => x.GetApplication()));
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_organization(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = client.GetResource<IOrganization>(this.fixture.PrimaryOrganizationHref);

            var mapping = app.GetAccountStoreMappings().Synchronously().First();

            client.GetResource<IOrganizationAccountStoreMapping>(mapping.Href, o => o.Expand(x => x.GetOrganization()));
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_accounts(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref, o => o.Expand(x => x.GetAccounts(0, 10)));
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_account_store_mappings(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref, o => o.Expand(x => x.GetAccountStoreMappings(0, 10)));
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_organization_account_store_mappings(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var app = client.GetResource<IOrganization>(this.fixture.PrimaryOrganizationHref, o => o.Expand(x => x.GetAccountStoreMappings(0, 10)));
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_account_memberships(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var group = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref, o => o.Expand(x => x.GetAccountMemberships(0, 10)));
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_applications(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var app = client.GetResource<ITenant>(this.fixture.TenantHref, o => o.Expand(x => x.GetApplications(0, 10)));
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_directories(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var app = client.GetResource<ITenant>(this.fixture.TenantHref, o => o.Expand(x => x.GetDirectories(0, 10)));
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_account(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var account = client.GetResource<IAccount>(this.fixture.PrimaryAccountHref);
            var membership = account.GetGroupMemberships().Synchronously().First();

            client.GetResource<IGroupMembership>(membership.Href, o => o.Expand(x => x.GetAccount()));
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_group(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var account = client.GetResource<IAccount>(this.fixture.PrimaryAccountHref);
            var membership = account.GetGroupMemberships().Synchronously().First();

            client.GetResource<IGroupMembership>(membership.Href, o => o.Expand(x => x.GetGroup()));
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Expanding_oAuthPolicy(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref, o => o.Expand(x => x.GetOauthPolicy()));
        }
    }
}
