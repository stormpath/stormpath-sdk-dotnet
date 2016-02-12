// <copyright file="Tenant_tests.cs" company="Stormpath, Inc.">
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

using Shouldly;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Common.Integration;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Sync
{
    [Collection(nameof(IntegrationTestCollection))]
    public class Tenant_tests
    {
        private readonly TestFixture fixture;

        public Tenant_tests(TestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_current_tenant(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            tenant.ShouldNotBe(null);
            tenant.Href.ShouldNotBe(null);
            tenant.Name.ShouldNotBe(null);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_account(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var account = tenant.GetAccount(this.fixture.PrimaryAccountHref);
            account.Href.ShouldBe(this.fixture.PrimaryAccountHref);
            account.FullName.ShouldBe("Luke Skywalker");
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var app = tenant.GetApplication(this.fixture.PrimaryApplicationHref);
            app.Href.ShouldBe(this.fixture.PrimaryApplicationHref);
            app.Description.ShouldBe("The Battle of Endor");
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_directory(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var directory = tenant.GetDirectory(this.fixture.PrimaryDirectoryHref);
            directory.Href.ShouldBe(this.fixture.PrimaryDirectoryHref);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_group(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var group = tenant.GetGroup(this.fixture.PrimaryGroupHref);
            group.Href.ShouldBe(this.fixture.PrimaryGroupHref);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_organization(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var org = tenant.GetOrganization(this.fixture.PrimaryOrganizationHref);
            org.NameKey.ShouldBe(this.fixture.PrimaryOrganizationNameKey);
            org.Href.ShouldBe(this.fixture.PrimaryOrganizationHref);
        }
    }
}