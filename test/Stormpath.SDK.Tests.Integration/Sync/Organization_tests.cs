// <copyright file="Organization_tests.cs" company="Stormpath, Inc.">
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
using Shouldly;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Organization;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Common.Integration;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Sync
{
    [Collection(nameof(IntegrationTestCollection))]
    public class Organization_tests
    {
        private readonly TestFixture fixture;

        public Organization_tests(TestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_tenant_organizations(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();
            var orgs = tenant.GetOrganizations().Synchronously().ToList();

            orgs.Count.ShouldBeGreaterThan(0);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_organization_tenant(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var org = client.GetResource<IOrganization>(this.fixture.PrimaryOrganizationHref);

            // Verify data from IntegrationTestData
            var tenantHref = org.GetTenant().Href;
            tenantHref.ShouldBe(this.fixture.TenantHref);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_organization_accounts(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var org = client.GetResource<IOrganization>(this.fixture.PrimaryOrganizationHref);

            org.GetAccounts().Synchronously().Count().ShouldBeGreaterThan(0);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_organization_groups(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var org = client.GetResource<IOrganization>(this.fixture.PrimaryOrganizationHref);

            org.GetGroups().Synchronously().Count().ShouldBeGreaterThan(0);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_organization_by_nameKey(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var org = client.GetOrganization(this.fixture.PrimaryOrganizationHref);

            var orgByNameKey = client.GetOrganizationByNameKey(org.NameKey);

            org.Href.ShouldBe(orgByNameKey.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Saving_organization(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var trek = client.Instantiate<IOrganization>()
                .SetName($"Star Trek (.NET ITs {this.fixture.TestRunIdentifier}-{clientBuilder.Name} - Sync)")
                .SetNameKey($"dotnet-test-trek-{this.fixture.TestRunIdentifier}-sync")
                .SetDescription("Star Trek (Sync)")
                .SetStatus(OrganizationStatus.Enabled);

            client.CreateOrganization(trek);
            this.fixture.CreatedOrganizationHrefs.Add(trek.Href);

            trek.SetStatus(OrganizationStatus.Disabled);
            var result = trek.Save();

            result.Status.ShouldBe(OrganizationStatus.Disabled);

            // Clean up
            trek.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(trek.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Updating_organization_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var org = client.GetResource<IOrganization>(this.fixture.PrimaryOrganizationHref);

            org.CustomData.Put("multiTenant", true);
            org.CustomData.Put("someInts", 12345);
            org.Save();

            var customData = org.GetCustomData();

            customData["multiTenant"].ShouldBe(true);
            customData["someInts"].ShouldBe(12345);

            // Clean up
            customData.Delete().ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Saving_with_response_options(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var bsg = client.Instantiate<IOrganization>()
                .SetName($"Battlestar Galactica (.NET ITs {this.fixture.TestRunIdentifier}-{clientBuilder.Name} - Sync)")
                .SetNameKey($"dotnet-test-bsg-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync")
                .SetDescription("BSG (Sync)")
                .SetStatus(OrganizationStatus.Enabled);

            client.CreateOrganization(bsg);
            this.fixture.CreatedOrganizationHrefs.Add(bsg.Href);

            bsg.SetDescription("Battlestar Galactica");
            bsg.Save(response => response.Expand(x => x.GetAccounts(0, 10)));

            // Clean up
            bsg.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(bsg.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Creating_with_directory(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var name = $"Created Organization 1 (.NET ITs {this.fixture.TestRunIdentifier}-{clientBuilder.Name} - Sync)";
            var nameKey = $"dotnet-test1-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync";
            var newOrg = client.Instantiate<IOrganization>()
                .SetName(name)
                .SetNameKey(nameKey)
                .SetStatus(OrganizationStatus.Enabled);

            var directoryName = $"Foobar Created Org 1 Directory-{this.fixture.TestRunIdentifier}-{clientBuilder.Name} - Sync";
            client.CreateOrganization(newOrg, opt =>
            {
                opt.CreateDirectory = true;
                opt.DirectoryName = directoryName;
            });
            this.fixture.CreatedOrganizationHrefs.Add(newOrg.Href);
            var createdDirectory = newOrg.GetDefaultAccountStore() as IDirectory;
            this.fixture.CreatedDirectoryHrefs.Add(createdDirectory.Href);

            newOrg.Name.ShouldBe(name);
            newOrg.NameKey.ShouldBe(nameKey);
            newOrg.Status.ShouldBe(OrganizationStatus.Enabled);

            createdDirectory.Name.ShouldBe(directoryName);

            // Clean up
            createdDirectory.Delete().ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(createdDirectory.Href);

            newOrg.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(newOrg.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Creating_without_directory(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var name = $"Created Organization 2 (.NET ITs {this.fixture.TestRunIdentifier}-{clientBuilder.Name} - Sync)";
            var nameKey = $"dotnet-test2-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync";
            var newOrg = client.Instantiate<IOrganization>()
                .SetName(name)
                .SetNameKey(nameKey)
                .SetStatus(OrganizationStatus.Disabled);

            client.CreateOrganization(newOrg, opt => opt.CreateDirectory = false);
            this.fixture.CreatedOrganizationHrefs.Add(newOrg.Href);

            newOrg.Name.ShouldBe(name);
            newOrg.NameKey.ShouldBe(nameKey);
            newOrg.Status.ShouldBe(OrganizationStatus.Disabled);

            var createdDirectory = newOrg.GetDefaultAccountStore();
            createdDirectory.ShouldBeNull();

            // Clean up
            newOrg.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(newOrg.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Creating_with_response_options(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var name = $"Created Organization 3 (.NET ITs {this.fixture.TestRunIdentifier}-{clientBuilder.Name} - Sync)";
            var nameKey = $"dotnet-test3-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync";
            var newOrg = client.Instantiate<IOrganization>()
                .SetName(name)
                .SetNameKey(nameKey)
                .SetStatus(OrganizationStatus.Disabled);

            client.CreateOrganization(newOrg, opt =>
            {
                opt.CreateDirectory = false;
                opt.ResponseOptions.Expand(x => x.GetCustomData());
            });
            this.fixture.CreatedOrganizationHrefs.Add(newOrg.Href);

            newOrg.Name.ShouldBe(name);
            newOrg.NameKey.ShouldBe(nameKey);
            newOrg.Status.ShouldBe(OrganizationStatus.Disabled);

            // Clean up
            newOrg.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(newOrg.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Creating_with_convenience_method(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var name = $"Created Organization 4 (.NET ITs {this.fixture.TestRunIdentifier}-{clientBuilder.Name}) - Sync";
            var nameKey = $"dotnet-test4-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}";

            var newOrg = client.CreateOrganization(name, nameKey);
            newOrg.ShouldNotBeNull();
            this.fixture.CreatedOrganizationHrefs.Add(newOrg.Href);

            newOrg.Name.ShouldBe(name);
            newOrg.NameKey.ShouldBe(nameKey);
            newOrg.Status.ShouldBe(OrganizationStatus.Enabled);

            // Clean up
            newOrg.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(newOrg.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Creating_account_store_mapping(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directly Test Organization - Sync")
                .SetNameKey($"dotnet-test4-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync");
            tenant.CreateOrganization(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            IAccountStore directory = client.GetResource<IDirectory>(this.fixture.PrimaryDirectoryHref);

            var mapping = client.Instantiate<IOrganizationAccountStoreMapping>();
            mapping.SetAccountStore(directory);
            mapping.SetListIndex(500);
            createdOrganization.CreateAccountStoreMapping(mapping);

            mapping.GetAccountStore().Href.ShouldBe(directory.Href);
            mapping.GetOrganization().Href.ShouldBe(createdOrganization.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            createdOrganization.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Creating_second_account_store_mapping_at_zeroth_index(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding Two AccountStores Directly Test Organization - Sync")
                .SetNameKey($"dotnet-test5-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync");
            tenant.CreateOrganization(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var mapping1 = createdOrganization.AddAccountStore(this.fixture.PrimaryDirectoryHref);

            var group = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref);
            var mapping2 = client.Instantiate<IOrganizationAccountStoreMapping>();
            mapping2.SetAccountStore(group);
            mapping2.SetListIndex(0);
            createdOrganization.CreateAccountStoreMapping(mapping2);

            mapping2.ListIndex.ShouldBe(0);
            mapping1.ListIndex.ShouldBe(1);

            // Clean up
            createdOrganization.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Adding_directory_as_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directory Test Organization - Sync")
                .SetNameKey($"dotnet-test6-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync");
            tenant.CreateOrganization(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var directory = client.GetResource<IDirectory>(this.fixture.PrimaryDirectoryHref);
            var mapping = createdOrganization.AddAccountStore(directory);

            mapping.GetAccountStore().Href.ShouldBe(directory.Href);
            mapping.GetOrganization().Href.ShouldBe(createdOrganization.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            createdOrganization.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Adding_group_as_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Group Test Organization - Sync")
                .SetNameKey($"dotnet-test7-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync");
            tenant.CreateOrganization(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var group = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref);
            var mapping = createdOrganization.AddAccountStore(group);

            mapping.GetAccountStore().Href.ShouldBe(group.Href);
            mapping.GetOrganization().Href.ShouldBe(createdOrganization.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            createdOrganization.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Setting_mapped_directory_to_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Setting Existing Directory AccountStore Default Test Organization - Sync")
                .SetNameKey($"dotnet-test8-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync");
            tenant.CreateOrganization(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var directory = client.GetResource<IDirectory>(this.fixture.PrimaryDirectoryHref);
            var mapping = createdOrganization.AddAccountStore(directory);

            createdOrganization.SetDefaultAccountStore(directory);

            mapping.IsDefaultAccountStore.ShouldBeTrue();
            mapping.IsDefaultGroupStore.ShouldBeFalse();

            // Clean up
            createdOrganization.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Setting_mapped_group_to_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Setting Existing Group AccountStore Default Test Organization - Sync")
                .SetNameKey($"dotnet-test9-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync");
            tenant.CreateOrganization(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var group = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref);
            var mapping = createdOrganization.AddAccountStore(group);

            createdOrganization.SetDefaultAccountStore(group);

            mapping.IsDefaultAccountStore.ShouldBeTrue();
            mapping.IsDefaultGroupStore.ShouldBeFalse();

            // Clean up
            createdOrganization.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Setting_unmapped_directory_to_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Setting New Directory AccountStore Default Test Organization - Sync")
                .SetNameKey($"dotnet-test10-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync");
            tenant.CreateOrganization(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var directory = client.GetResource<IDirectory>(this.fixture.PrimaryDirectoryHref);
            createdOrganization.SetDefaultAccountStore(directory);

            var mapping = createdOrganization.GetAccountStoreMappings().Synchronously().Single();
            mapping.IsDefaultAccountStore.ShouldBeTrue();
            mapping.IsDefaultGroupStore.ShouldBeFalse();

            // Clean up
            createdOrganization.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Setting_unmapped_group_to_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Setting New Group AccountStore Default Test Organization - Sync")
                .SetNameKey($"dotnet-test11-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync");
            tenant.CreateOrganization(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var group = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref);
            createdOrganization.SetDefaultAccountStore(group);

            var mapping = createdOrganization.GetAccountStoreMappings().Synchronously().Single();
            mapping.IsDefaultAccountStore.ShouldBeTrue();
            mapping.IsDefaultGroupStore.ShouldBeFalse();

            // Clean up
            createdOrganization.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Setting_mapped_directory_to_default_group_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Setting Existing Directory GroupStore Default Test Organization - Sync")
                .SetNameKey($"dotnet-test12-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync");
            tenant.CreateOrganization(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var directory = client.GetResource<IDirectory>(this.fixture.PrimaryDirectoryHref);
            var mapping = createdOrganization.AddAccountStore(directory);

            createdOrganization.SetDefaultGroupStore(directory);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeTrue();

            // Clean up
            createdOrganization.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Setting_unmapped_directory_to_default_group_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Setting Existing Directory GroupStore Default Test Organization - Sync")
                .SetNameKey($"dotnet-test13-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync");
            tenant.CreateOrganization(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var directory = client.GetResource<IDirectory>(this.fixture.PrimaryDirectoryHref);
            createdOrganization.SetDefaultGroupStore(directory);

            var mapping = createdOrganization.GetAccountStoreMappings().Synchronously().Single();
            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeTrue();

            // Clean up
            createdOrganization.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Setting_group_group_store_throws(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Setting Group as GroupStore Test Organization - Sync")
                .SetNameKey($"dotnet-test14-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync");
            tenant.CreateOrganization(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var group = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref);

            // If this errors, the server-side API behavior has changed.
            // TODO ResourceException after Shouldly Mono update
            Should.Throw<Exception>(() =>
            {
                createdOrganization.SetDefaultGroupStore(group);
            });

            // Clean up
            createdOrganization.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Adding_directory_as_account_store_by_href(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directory By Href Test Organization - Sync")
                .SetNameKey($"dotnet-test15-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync");
            tenant.CreateOrganization(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var mapping = createdOrganization.AddAccountStore(this.fixture.PrimaryDirectoryHref);

            mapping.GetAccountStore().Href.ShouldBe(this.fixture.PrimaryDirectoryHref);
            mapping.GetOrganization().Href.ShouldBe(createdOrganization.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            createdOrganization.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Adding_group_as_account_store_by_href(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Group By Href Test Organization - Sync")
                .SetNameKey($"dotnet-test16-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync");
            tenant.CreateOrganization(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var mapping = createdOrganization.AddAccountStore(this.fixture.PrimaryGroupHref);

            mapping.GetAccountStore().Href.ShouldBe(this.fixture.PrimaryGroupHref);
            mapping.GetOrganization().Href.ShouldBe(createdOrganization.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            createdOrganization.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Adding_directory_as_account_store_by_name(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directory By Name Test Organization - Sync")
                .SetNameKey($"dotnet-test17-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync");
            tenant.CreateOrganization(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var directoryName = $".NET IT Organization Test {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Add Directory As AccountStore By Name - Sync";
            var testDirectory = client
                .Instantiate<IDirectory>()
                .SetName(directoryName);
            client.CreateDirectory(testDirectory);
            testDirectory.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(testDirectory.Href);

            var mapping = createdOrganization.AddAccountStore(directoryName);

            mapping.GetAccountStore().Href.ShouldBe(testDirectory.Href);
            mapping.GetOrganization().Href.ShouldBe(createdOrganization.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            createdOrganization.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);

            testDirectory.Delete().ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(testDirectory.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Adding_group_as_account_store_by_name(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Group By Name Test Organization - Sync")
                .SetNameKey($"dotnet-test18-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync");
            tenant.CreateOrganization(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            // Needs to have a default GroupStore
            var mapping = createdOrganization.AddAccountStore(this.fixture.PrimaryDirectoryHref);
            mapping.SetDefaultGroupStore(true);
            mapping.Save();

            var groupName = $".NET IT Organization Test {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Add Group As AccountStore By Name - Sync";
            var testGroup = client
                .Instantiate<IGroup>()
                .SetName(groupName);
            createdOrganization.CreateGroup(testGroup);
            testGroup.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedGroupHrefs.Add(testGroup.Href);

            var newMapping = createdOrganization.AddAccountStore(groupName);

            newMapping.GetAccountStore().Href.ShouldBe(testGroup.Href);
            newMapping.GetOrganization().Href.ShouldBe(createdOrganization.Href);

            newMapping.IsDefaultAccountStore.ShouldBeFalse();
            newMapping.IsDefaultGroupStore.ShouldBeFalse();
            newMapping.ListIndex.ShouldBe(1);

            // Clean up
            createdOrganization.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);

            testGroup.Delete().ShouldBeTrue();
            this.fixture.CreatedGroupHrefs.Remove(testGroup.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Adding_directory_as_account_store_by_query(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directory By Query Test Organization - Sync")
                .SetNameKey($"dotnet-test19-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync");
            tenant.CreateOrganization(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var directoryName = client.GetResource<IDirectory>(this.fixture.PrimaryDirectoryHref).Name;
            var mapping = createdOrganization
                .AddAccountStore<IDirectory>(dirs => dirs.Where(d => d.Name.EndsWith(directoryName.Substring(1))));

            mapping.GetAccountStore().Href.ShouldBe(this.fixture.PrimaryDirectoryHref);
            mapping.GetOrganization().Href.ShouldBe(createdOrganization.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            createdOrganization.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Adding_group_as_account_store_by_query(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Group By Query Test Organization - Sync")
                .SetNameKey($"dotnet-test20-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync");
            tenant.CreateOrganization(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var groupName = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref).Name;
            var mapping = createdOrganization
                .AddAccountStore<IGroup>(groups => groups.Where(g => g.Name.EndsWith(groupName.Substring(1))));

            mapping.GetAccountStore().Href.ShouldBe(this.fixture.PrimaryGroupHref);
            mapping.GetOrganization().Href.ShouldBe(createdOrganization.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            createdOrganization.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Adding_directory_as_account_store_by_query_throws_for_multiple_results(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directory By Query Throws Test Organization - Sync")
                .SetNameKey($"dotnet-test21-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync");
            tenant.CreateOrganization(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var dir1 = client.CreateDirectory($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Directory Query Results1", string.Empty, DirectoryStatus.Enabled);
            var dir2 = client.CreateDirectory($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Directory Query Results2", string.Empty, DirectoryStatus.Enabled);

            this.fixture.CreatedDirectoryHrefs.Add(dir1.Href);
            this.fixture.CreatedDirectoryHrefs.Add(dir2.Href);

            // TODO ArgumentException after Shouldly Mono update
            Should.Throw<Exception>(() =>
            {
                // Throws because multiple matching results exist
                var mapping = createdOrganization
                    .AddAccountStore<IDirectory>(dirs => dirs.Where(d => d.Name.StartsWith($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Directory Query Results")));
            });

            // Clean up
            dir1.Delete().ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(dir1.Href);

            dir2.Delete().ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(dir2.Href);

            createdOrganization.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Adding_group_as_account_store_by_query_throws_for_multiple_results(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Group By Query Throws Test Organization - Sync")
                .SetNameKey($"dotnet-test22-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}-sync");
            tenant.CreateOrganization(createdOrganization, opt => opt.CreateDirectory = true);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var defaultGroupStore = createdOrganization.GetDefaultGroupStore() as IDirectory;
            defaultGroupStore.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(defaultGroupStore.Href);

            var group1 = createdOrganization.CreateGroup($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Group Query Results1", string.Empty);
            var group2 = createdOrganization.CreateGroup($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Group Query Results2", string.Empty);

            this.fixture.CreatedGroupHrefs.Add(group1.Href);
            this.fixture.CreatedGroupHrefs.Add(group2.Href);

            // TODO ArgumentException after Shouldly Mono update
            Should.Throw<Exception>(() =>
            {
                // Throws because multiple matching results exist
                var mapping = createdOrganization
                    .AddAccountStore<IGroup>(groups => groups.Where(x => x.Name.StartsWith($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Group Query Results")));
            });

            // Clean up
            group1.Delete().ShouldBeTrue();
            this.fixture.CreatedGroupHrefs.Remove(group1.Href);

            group2.Delete().ShouldBeTrue();
            this.fixture.CreatedGroupHrefs.Remove(group2.Href);

            defaultGroupStore.Delete().ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(defaultGroupStore.Href);

            createdOrganization.Delete().ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }
    }
}
