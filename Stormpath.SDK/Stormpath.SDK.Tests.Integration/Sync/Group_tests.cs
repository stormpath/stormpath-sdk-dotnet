// <copyright file="Group_tests.cs" company="Stormpath, Inc.">
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
using Shouldly;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Common.Integration;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Sync
{
    [Collection(nameof(IntegrationTestCollection))]
    public class Group_tests
    {
        private readonly TestFixture fixture;

        public Group_tests(TestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_tenant_groups(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();
            var groups = tenant.GetGroups().Synchronously().ToList();

            groups.Count.ShouldBeGreaterThan(0);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_group_tenant(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var group = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref);

            // Verify data from IntegrationTestData
            var tenantHref = group.GetTenant().Href;
            tenantHref.ShouldBe(this.fixture.TenantHref);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_application_groups(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);
            var groups = app.GetGroups().Synchronously().ToList();

            groups.Count.ShouldBeGreaterThan(0);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_directory_groups(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var directory = client.GetResource<IDirectory>(this.fixture.PrimaryDirectoryHref);
            var groups = directory.GetGroups().Synchronously().ToList();

            groups.Count.ShouldBeGreaterThan(0);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_account_groups(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);
            var luke = app.GetAccounts().Synchronously().Where(x => x.Email.StartsWith("lskywalker")).Single();

            var groups = luke.GetGroups().Synchronously().ToList();

            groups.Count.ShouldBeGreaterThan(0);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_group_accounts(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var humans = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref);

            humans.GetAccounts().Synchronously().Count().ShouldBeGreaterThan(0);
            humans.GetAccountMemberships().Synchronously().Count().ShouldBeGreaterThan(0);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Modifying_group(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var directory = client.GetResource<IDirectory>(this.fixture.PrimaryDirectoryHref);

            var droids = client
                .Instantiate<IGroup>()
                .SetName($"Droids (.NET ITs {this.fixture.TestRunIdentifier}) - Sync")
                .SetDescription("Mechanical entities")
                .SetStatus(GroupStatus.Enabled);

            directory.CreateGroup(droids);
            this.fixture.CreatedGroupHrefs.Add(droids.Href);

            droids.SetStatus(GroupStatus.Disabled);
            var result = droids.Save();

            result.Status.ShouldBe(GroupStatus.Disabled);

            // Clean up
            droids.Delete();
            this.fixture.CreatedGroupHrefs.Remove(droids.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Saving_with_response_options(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var directory = client.GetResource<IDirectory>(this.fixture.PrimaryDirectoryHref);

            var newGroup = client
                .Instantiate<IGroup>()
                .SetName($"Another Group (.NET ITs {this.fixture.TestRunIdentifier})")
                .SetStatus(GroupStatus.Disabled);

            directory.CreateGroup(newGroup);
            this.fixture.CreatedGroupHrefs.Add(newGroup.Href);

            newGroup.SetDescription("foobar");
            newGroup.Save(response => response.Expand(x => x.GetAccounts, 0, 10));

            // Clean up
            newGroup.Delete();
            this.fixture.CreatedGroupHrefs.Remove(newGroup.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Adding_account_to_group(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);
            var humans = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref);

            var lando = app.GetAccounts().Synchronously().Where(x => x.Email.StartsWith("lcalrissian")).Single();
            var membership = humans.AddAccount(lando);

            membership.ShouldNotBeNull();
            membership.Href.ShouldNotBeNullOrEmpty();

            lando.IsMemberOfGroup(humans.Href).ShouldBeTrue();

            humans.RemoveAccount(lando).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_group_membership(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);
            var humans = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref);

            var lando = app.GetAccounts().Synchronously().Where(x => x.Email.StartsWith("lcalrissian")).Single();
            var membership = humans.AddAccount(lando);

            // Should also be seen in the master membership list
            var allMemberships = humans.GetAccountMemberships().Synchronously().ToList();
            allMemberships.ShouldContain(x => x.Href == membership.Href);

            membership.GetAccount().Href.ShouldBe(lando.Href);
            membership.GetGroup().Href.ShouldBe(humans.Href);

            membership.Delete().ShouldBeTrue();

            lando.IsMemberOfGroup(humans.Href).ShouldBeFalse();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Adding_account_to_group_by_group_href(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var leia = app.GetAccounts().Synchronously().Where(x => x.Email.StartsWith("leia.organa")).Single();
            leia.AddGroup(this.fixture.PrimaryGroupHref);

            leia.IsMemberOfGroup(this.fixture.PrimaryGroupHref).ShouldBeTrue();

            leia.RemoveGroup(this.fixture.PrimaryGroupHref).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Adding_account_to_group_by_group_name(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);
            var groupName = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref).Name;

            var han = app.GetAccounts().Synchronously().Where(x => x.Email.StartsWith("han.solo")).Single();
            han.AddGroup(groupName);

            han.IsMemberOfGroup(this.fixture.PrimaryGroupHref).ShouldBeTrue();

            han.RemoveGroup(groupName).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Adding_account_to_group_by_account_href(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);
            var humans = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref);

            var leia = app.GetAccounts().Synchronously().Where(x => x.Email.StartsWith("leia.organa")).Single();
            humans.AddAccount(leia.Href);

            leia.IsMemberOfGroup(this.fixture.PrimaryGroupHref).ShouldBeTrue();

            humans.RemoveAccount(leia.Href).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Adding_account_to_group_by_account_email(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);
            var humans = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref);

            var han = app.GetAccounts().Synchronously().Where(x => x.Email.StartsWith("han.solo")).Single();
            humans.AddAccount(han.Email);

            han.IsMemberOfGroup(this.fixture.PrimaryGroupHref).ShouldBeTrue();

            humans.RemoveAccount(han.Email).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Creating_group_in_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var instance = client.Instantiate<IGroup>();
            instance.SetName($".NET ITs New Test Group {this.fixture.TestRunIdentifier} - Sync");
            instance.SetDescription("A nu start");
            instance.SetStatus(GroupStatus.Disabled);

            var created = app.CreateGroup(instance);
            created.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedGroupHrefs.Add(created.Href);

            created.Name.ShouldBe($".NET ITs New Test Group {this.fixture.TestRunIdentifier} - Sync");
            created.Description.ShouldBe("A nu start");
            created.Status.ShouldBe(GroupStatus.Disabled);

            created.Delete().ShouldBeTrue();
            this.fixture.CreatedGroupHrefs.Remove(created.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Creating_group_in_directory(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var directory = client.GetResource<IApplication>(this.fixture.PrimaryDirectoryHref);

            var instance = client.Instantiate<IGroup>();
            instance.SetName($".NET ITs New Test Group #2 {this.fixture.TestRunIdentifier} - Sync");
            instance.SetDescription("A nu start");
            instance.SetStatus(GroupStatus.Enabled);

            var created = directory.CreateGroup(instance);
            created.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedGroupHrefs.Add(created.Href);

            created.Name.ShouldBe($".NET ITs New Test Group #2 {this.fixture.TestRunIdentifier} - Sync");
            created.Description.ShouldBe("A nu start");
            created.Status.ShouldBe(GroupStatus.Enabled);

            created.Delete().ShouldBeTrue();
            this.fixture.CreatedGroupHrefs.Remove(created.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Creating_group_with_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var instance = client.Instantiate<IGroup>();
            instance.SetName($".NET ITs Custom Data Group {this.fixture.TestRunIdentifier} - Sync");
            instance.CustomData.Put("isNeat", true);
            instance.CustomData.Put("roleBasedSecurity", "pieceOfCake");

            var created = app.CreateGroup(instance);
            this.fixture.CreatedGroupHrefs.Add(created.Href);

            var customData = created.GetCustomData();
            customData["isNeat"].ShouldBe(true);
            customData["roleBasedSecurity"].ShouldBe("pieceOfCake");

            created.Delete().ShouldBeTrue();
            this.fixture.CreatedGroupHrefs.Remove(created.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Creating_group_with_response_options(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var group = client
                .Instantiate<IGroup>()
                .SetName($".NET ITs Custom Data Group #2 ({this.fixture.TestRunIdentifier} - {clientBuilder.Name})");

            app.CreateGroup(group, opt => opt.ResponseOptions.Expand(x => x.GetCustomData));

            group.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedGroupHrefs.Add(group.Href);

            group.Delete().ShouldBeTrue();
            this.fixture.CreatedGroupHrefs.Remove(group.Href);
        }
    }
}
