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
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Organization;
using Stormpath.SDK.Tests.Common.Integration;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Async
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
        public async Task Getting_tenant_organizations(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();
            var orgs = await tenant.GetOrganizations().ToListAsync();

            orgs.Count.ShouldBeGreaterThan(0);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Getting_organization_tenant(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var org = await client.GetResourceAsync<IOrganization>(this.fixture.PrimaryOrganizationHref);

            // Verify data from IntegrationTestData
            var tenantHref = (await org.GetTenantAsync()).Href;
            tenantHref.ShouldBe(this.fixture.TenantHref);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Getting_organization_accounts(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var org = await client.GetResourceAsync<IOrganization>(this.fixture.PrimaryOrganizationHref);

            (await org.GetAccounts().CountAsync()).ShouldBeGreaterThan(0);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Getting_organization_groups(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var org = await client.GetResourceAsync<IOrganization>(this.fixture.PrimaryOrganizationHref);

            (await org.GetGroups().CountAsync()).ShouldBeGreaterThan(0);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Getting_organization_by_nameKey(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var org = await client.GetOrganizationAsync(this.fixture.PrimaryOrganizationHref);

            var orgByNameKey = await client.GetOrganizationByNameKeyAsync(org.NameKey);

            org.Href.ShouldBe(orgByNameKey.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Saving_organization(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var trek = client.Instantiate<IOrganization>()
                .SetName($"Star Trek (.NET ITs {this.fixture.TestRunIdentifier}-{clientBuilder.Name})")
                .SetNameKey($"dotnet-test-trek-{this.fixture.TestRunIdentifier}")
                .SetDescription("Star Trek")
                .SetStatus(OrganizationStatus.Enabled);

            await client.CreateOrganizationAsync(trek);
            this.fixture.CreatedOrganizationHrefs.Add(trek.Href);

            trek.SetStatus(OrganizationStatus.Disabled);
            var result = await trek.SaveAsync();

            result.Status.ShouldBe(OrganizationStatus.Disabled);

            // Clean up
            (await trek.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(trek.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Updating_organization_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var org = await client.GetResourceAsync<IOrganization>(this.fixture.PrimaryOrganizationHref);

            org.CustomData.Put("multiTenant", true);
            org.CustomData.Put("someInts", 12345);
            await org.SaveAsync();

            var customData = await org.GetCustomDataAsync();

            customData["multiTenant"].ShouldBe(true);
            customData["someInts"].ShouldBe(12345);

            // Clean up
            (await customData.DeleteAsync()).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Saving_with_response_options(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var bsg = client.Instantiate<IOrganization>()
                .SetName($"Battlestar Galactica (.NET ITs {this.fixture.TestRunIdentifier}-{clientBuilder.Name})")
                .SetNameKey($"dotnet-test-bsg-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}")
                .SetDescription("BSG")
                .SetStatus(OrganizationStatus.Enabled);

            await client.CreateOrganizationAsync(bsg);
            this.fixture.CreatedOrganizationHrefs.Add(bsg.Href);

            bsg.SetDescription("Battlestar Galactica");
            await bsg.SaveAsync(response => response.Expand(x => x.GetAccounts(0, 10)));

            // Clean up
            (await bsg.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(bsg.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Creating_with_directory(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var name = $"Created Organization 1 (.NET ITs {this.fixture.TestRunIdentifier}-{clientBuilder.Name})";
            var nameKey = $"dotnet-test1-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}";
            var newOrg = client.Instantiate<IOrganization>()
                .SetName(name)
                .SetNameKey(nameKey)
                .SetStatus(OrganizationStatus.Enabled);

            var directoryName = $"Foobar Created Org 1 Directory-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}";
            await client.CreateOrganizationAsync(newOrg, opt =>
            {
                opt.CreateDirectory = true;
                opt.DirectoryName = directoryName;
            });
            this.fixture.CreatedOrganizationHrefs.Add(newOrg.Href);
            var createdDirectory = await newOrg.GetDefaultAccountStoreAsync() as IDirectory;
            this.fixture.CreatedDirectoryHrefs.Add(createdDirectory.Href);

            newOrg.Name.ShouldBe(name);
            newOrg.NameKey.ShouldBe(nameKey);
            newOrg.Status.ShouldBe(OrganizationStatus.Enabled);

            createdDirectory.Name.ShouldBe(directoryName);

            // Clean up
            (await createdDirectory.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(createdDirectory.Href);

            (await newOrg.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(newOrg.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Creating_without_directory(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var name = $"Created Organization 2 (.NET ITs {this.fixture.TestRunIdentifier}-{clientBuilder.Name})";
            var nameKey = $"dotnet-test2-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}";
            var newOrg = client.Instantiate<IOrganization>()
                .SetName(name)
                .SetNameKey(nameKey)
                .SetStatus(OrganizationStatus.Disabled);

            await client.CreateOrganizationAsync(newOrg, opt => opt.CreateDirectory = false);
            this.fixture.CreatedOrganizationHrefs.Add(newOrg.Href);

            newOrg.Name.ShouldBe(name);
            newOrg.NameKey.ShouldBe(nameKey);
            newOrg.Status.ShouldBe(OrganizationStatus.Disabled);

            var createdDirectory = await newOrg.GetDefaultAccountStoreAsync();
            createdDirectory.ShouldBeNull();

            // Clean up
            (await newOrg.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(newOrg.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Creating_with_response_options(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var name = $"Created Organization 3 (.NET ITs {this.fixture.TestRunIdentifier}-{clientBuilder.Name})";
            var nameKey = $"dotnet-test3-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}";
            var newOrg = client.Instantiate<IOrganization>()
                .SetName(name)
                .SetNameKey(nameKey)
                .SetStatus(OrganizationStatus.Disabled);

            await client.CreateOrganizationAsync(newOrg, opt =>
            {
                opt.CreateDirectory = false;
                opt.ResponseOptions.Expand(x => x.GetCustomData());
            });
            this.fixture.CreatedOrganizationHrefs.Add(newOrg.Href);

            newOrg.Name.ShouldBe(name);
            newOrg.NameKey.ShouldBe(nameKey);
            newOrg.Status.ShouldBe(OrganizationStatus.Disabled);

            // Clean up
            (await newOrg.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(newOrg.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Creating_with_convenience_method(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var name = $"Created Organization 4 (.NET ITs {this.fixture.TestRunIdentifier}-{clientBuilder.Name})";
            var nameKey = $"dotnet-test4-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}";

            var newOrg = await client.CreateOrganizationAsync(name, nameKey);
            newOrg.ShouldNotBeNull();
            this.fixture.CreatedOrganizationHrefs.Add(newOrg.Href);

            newOrg.Name.ShouldBe(name);
            newOrg.NameKey.ShouldBe(nameKey);
            newOrg.Status.ShouldBe(OrganizationStatus.Enabled);

            // Clean up
            (await newOrg.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(newOrg.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Creating_account_store_mapping(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directly Test Organization")
                .SetNameKey($"dotnet-test4-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}");
            await tenant.CreateOrganizationAsync(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            IAccountStore directory = await client.GetResourceAsync<IDirectory>(this.fixture.PrimaryDirectoryHref);

            var mapping = client.Instantiate<IOrganizationAccountStoreMapping>();
            mapping.SetAccountStore(directory);
            mapping.SetListIndex(500);
            await createdOrganization.CreateAccountStoreMappingAsync(mapping);

            (await mapping.GetAccountStoreAsync()).Href.ShouldBe(directory.Href);
            (await mapping.GetOrganizationAsync()).Href.ShouldBe(createdOrganization.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            (await createdOrganization.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Creating_second_account_store_mapping_at_zeroth_index(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding Two AccountStores Directly Test Organization")
                .SetNameKey($"dotnet-test5-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}");
            await tenant.CreateOrganizationAsync(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var mapping1 = await createdOrganization.AddAccountStoreAsync(this.fixture.PrimaryDirectoryHref);

            var group = await client.GetResourceAsync<IGroup>(this.fixture.PrimaryGroupHref);
            var mapping2 = client.Instantiate<IOrganizationAccountStoreMapping>();
            mapping2.SetAccountStore(group);
            mapping2.SetListIndex(0);
            await createdOrganization.CreateAccountStoreMappingAsync(mapping2);

            mapping2.ListIndex.ShouldBe(0);
            mapping1.ListIndex.ShouldBe(1);

            // Clean up
            (await createdOrganization.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Adding_directory_as_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directory Test Organization")
                .SetNameKey($"dotnet-test6-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}");
            await tenant.CreateOrganizationAsync(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var directory = await client.GetResourceAsync<IDirectory>(this.fixture.PrimaryDirectoryHref);
            var mapping = await createdOrganization.AddAccountStoreAsync(directory);

            (await mapping.GetAccountStoreAsync()).Href.ShouldBe(directory.Href);
            (await mapping.GetOrganizationAsync()).Href.ShouldBe(createdOrganization.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            (await createdOrganization.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Adding_group_as_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Group Test Organization")
                .SetNameKey($"dotnet-test7-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}");
            await tenant.CreateOrganizationAsync(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var group = await client.GetResourceAsync<IGroup>(this.fixture.PrimaryGroupHref);
            var mapping = await createdOrganization.AddAccountStoreAsync(group);

            (await mapping.GetAccountStoreAsync()).Href.ShouldBe(group.Href);
            (await mapping.GetOrganizationAsync()).Href.ShouldBe(createdOrganization.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            (await createdOrganization.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Setting_mapped_directory_to_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Setting Existing Directory AccountStore Default Test Organization")
                .SetNameKey($"dotnet-test8-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}");
            await tenant.CreateOrganizationAsync(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var directory = await client.GetResourceAsync<IDirectory>(this.fixture.PrimaryDirectoryHref);
            var mapping = await createdOrganization.AddAccountStoreAsync(directory);

            await createdOrganization.SetDefaultAccountStoreAsync(directory);

            mapping.IsDefaultAccountStore.ShouldBeTrue();
            mapping.IsDefaultGroupStore.ShouldBeFalse();

            // Clean up
            (await createdOrganization.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Setting_mapped_group_to_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Setting Existing Group AccountStore Default Test Organization")
                .SetNameKey($"dotnet-test9-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}");
            await tenant.CreateOrganizationAsync(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var group = await client.GetResourceAsync<IGroup>(this.fixture.PrimaryGroupHref);
            var mapping = await createdOrganization.AddAccountStoreAsync(group);

            await createdOrganization.SetDefaultAccountStoreAsync(group);

            mapping.IsDefaultAccountStore.ShouldBeTrue();
            mapping.IsDefaultGroupStore.ShouldBeFalse();

            // Clean up
            (await createdOrganization.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Setting_unmapped_directory_to_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Setting New Directory AccountStore Default Test Organization")
                .SetNameKey($"dotnet-test10-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}");
            await tenant.CreateOrganizationAsync(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var directory = await client.GetResourceAsync<IDirectory>(this.fixture.PrimaryDirectoryHref);
            await createdOrganization.SetDefaultAccountStoreAsync(directory);

            var mapping = await createdOrganization.GetAccountStoreMappings().SingleAsync();
            mapping.IsDefaultAccountStore.ShouldBeTrue();
            mapping.IsDefaultGroupStore.ShouldBeFalse();

            // Clean up
            (await createdOrganization.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Setting_unmapped_group_to_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Setting New Group AccountStore Default Test Organization")
                .SetNameKey($"dotnet-test11-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}");
            await tenant.CreateOrganizationAsync(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var group = await client.GetResourceAsync<IGroup>(this.fixture.PrimaryGroupHref);
            await createdOrganization.SetDefaultAccountStoreAsync(group);

            var mapping = await createdOrganization.GetAccountStoreMappings().SingleAsync();
            mapping.IsDefaultAccountStore.ShouldBeTrue();
            mapping.IsDefaultGroupStore.ShouldBeFalse();

            // Clean up
            (await createdOrganization.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Setting_mapped_directory_to_default_group_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Setting Existing Directory GroupStore Default Test Organization")
                .SetNameKey($"dotnet-test12-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}");
            await tenant.CreateOrganizationAsync(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var directory = await client.GetResourceAsync<IDirectory>(this.fixture.PrimaryDirectoryHref);
            var mapping = await createdOrganization.AddAccountStoreAsync(directory);

            await createdOrganization.SetDefaultGroupStoreAsync(directory);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeTrue();

            // Clean up
            (await createdOrganization.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Setting_unmapped_directory_to_default_group_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Setting Existing Directory GroupStore Default Test Organization")
                .SetNameKey($"dotnet-test13-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}");
            await tenant.CreateOrganizationAsync(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var directory = await client.GetResourceAsync<IDirectory>(this.fixture.PrimaryDirectoryHref);
            await createdOrganization.SetDefaultGroupStoreAsync(directory);

            var mapping = await createdOrganization.GetAccountStoreMappings().SingleAsync();
            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeTrue();

            // Clean up
            (await createdOrganization.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory(Skip = "Fix shouldly async throw tests")]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Setting_group_group_store_throws(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Setting Group as GroupStore Test Organization")
                .SetNameKey($"dotnet-test14-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}");
            await tenant.CreateOrganizationAsync(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var group = await client.GetResourceAsync<IGroup>(this.fixture.PrimaryGroupHref);

            // If this errors, the server-side API behavior has changed.
            // TOTO ResourceException after Shouldly Mono update
            Should.Throw<Exception>(async () =>
            {
                await createdOrganization.SetDefaultGroupStoreAsync(group);
            });

            // Clean up
            (await createdOrganization.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Adding_directory_as_account_store_by_href(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directory By Href Test Organization")
                .SetNameKey($"dotnet-test15-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}");
            await tenant.CreateOrganizationAsync(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var mapping = await createdOrganization.AddAccountStoreAsync(this.fixture.PrimaryDirectoryHref);

            (await mapping.GetAccountStoreAsync()).Href.ShouldBe(this.fixture.PrimaryDirectoryHref);
            (await mapping.GetOrganizationAsync()).Href.ShouldBe(createdOrganization.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            (await createdOrganization.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Adding_group_as_account_store_by_href(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Group By Href Test Organization")
                .SetNameKey($"dotnet-test16-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}");
            await tenant.CreateOrganizationAsync(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var mapping = await createdOrganization.AddAccountStoreAsync(this.fixture.PrimaryGroupHref);

            (await mapping.GetAccountStoreAsync()).Href.ShouldBe(this.fixture.PrimaryGroupHref);
            (await mapping.GetOrganizationAsync()).Href.ShouldBe(createdOrganization.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            (await createdOrganization.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Adding_directory_as_account_store_by_name(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directory By Name Test Organization")
                .SetNameKey($"dotnet-test17-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}");
            await tenant.CreateOrganizationAsync(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var directoryName = $".NET IT Organization Test {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Add Directory As AccountStore By Name";
            var testDirectory = client
                .Instantiate<IDirectory>()
                .SetName(directoryName);
            await client.CreateDirectoryAsync(testDirectory);
            testDirectory.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(testDirectory.Href);

            var mapping = await createdOrganization.AddAccountStoreAsync(directoryName);

            (await mapping.GetAccountStoreAsync()).Href.ShouldBe(testDirectory.Href);
            (await mapping.GetOrganizationAsync()).Href.ShouldBe(createdOrganization.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            (await createdOrganization.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);

            (await testDirectory.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(testDirectory.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Adding_group_as_account_store_by_name(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Group By Name Test Organization")
                .SetNameKey($"dotnet-test18-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}");
            await tenant.CreateOrganizationAsync(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            // Needs to have a default GroupStore
            var mapping = await createdOrganization.AddAccountStoreAsync(this.fixture.PrimaryDirectoryHref);
            mapping.SetDefaultGroupStore(true);
            await mapping.SaveAsync();

            var groupName = $".NET IT Organization Test {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Add Group As AccountStore By Name";
            var testGroup = client
                .Instantiate<IGroup>()
                .SetName(groupName);
            await createdOrganization.CreateGroupAsync(testGroup);
            testGroup.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedGroupHrefs.Add(testGroup.Href);

            var newMapping = await createdOrganization.AddAccountStoreAsync(groupName);

            (await newMapping.GetAccountStoreAsync()).Href.ShouldBe(testGroup.Href);
            (await newMapping.GetOrganizationAsync()).Href.ShouldBe(createdOrganization.Href);

            newMapping.IsDefaultAccountStore.ShouldBeFalse();
            newMapping.IsDefaultGroupStore.ShouldBeFalse();
            newMapping.ListIndex.ShouldBe(1);

            // Clean up
            (await createdOrganization.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);

            (await testGroup.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedGroupHrefs.Remove(testGroup.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Adding_directory_as_account_store_by_query(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directory By Query Test Organization")
                .SetNameKey($"dotnet-test19-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}");
            await tenant.CreateOrganizationAsync(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var directoryName = (await client.GetResourceAsync<IDirectory>(this.fixture.PrimaryDirectoryHref)).Name;
            var mapping = await createdOrganization
                .AddAccountStoreAsync<IDirectory>(dirs => dirs.Where(d => d.Name.EndsWith(directoryName.Substring(1))));

            (await mapping.GetAccountStoreAsync()).Href.ShouldBe(this.fixture.PrimaryDirectoryHref);
            (await mapping.GetOrganizationAsync()).Href.ShouldBe(createdOrganization.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            (await createdOrganization.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Adding_group_as_account_store_by_query(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Group By Query Test Organization")
                .SetNameKey($"dotnet-test20-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}");
            await tenant.CreateOrganizationAsync(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var groupName = (await client.GetResourceAsync<IGroup>(this.fixture.PrimaryGroupHref)).Name;
            var mapping = await createdOrganization
                .AddAccountStoreAsync<IGroup>(groups => groups.Where(g => g.Name.EndsWith(groupName.Substring(1))));

            (await mapping.GetAccountStoreAsync()).Href.ShouldBe(this.fixture.PrimaryGroupHref);
            (await mapping.GetOrganizationAsync()).Href.ShouldBe(createdOrganization.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            (await createdOrganization.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory(Skip = "Fix shouldly async throw tests")]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Adding_directory_as_account_store_by_query_throws_for_multiple_results(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directory By Query Throws Test Organization")
                .SetNameKey($"dotnet-test21-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}");
            await tenant.CreateOrganizationAsync(createdOrganization, opt => opt.CreateDirectory = false);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var dir1 = await client.CreateDirectoryAsync($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Directory Query Results1", string.Empty, DirectoryStatus.Enabled);
            var dir2 = await client.CreateDirectoryAsync($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Directory Query Results2", string.Empty, DirectoryStatus.Enabled);

            this.fixture.CreatedDirectoryHrefs.Add(dir1.Href);
            this.fixture.CreatedDirectoryHrefs.Add(dir2.Href);

            // TODO ArgumentException after Shouldly Mono update
            Should.Throw<Exception>(async () =>
            {
                // Throws because multiple matching results exist
                var mapping = await createdOrganization
                    .AddAccountStoreAsync<IDirectory>(dirs => dirs.Where(d => d.Name.StartsWith($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Directory Query Results")));
            });

            // Clean up
            (await dir1.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(dir1.Href);

            (await dir2.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(dir2.Href);

            (await createdOrganization.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }

        [Theory(Skip = "Fix shouldly async throw tests")]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Adding_group_as_account_store_by_query_throws_for_multiple_results(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdOrganization = client.Instantiate<IOrganization>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Group By Query Throws Test Organization")
                .SetNameKey($"dotnet-test22-{this.fixture.TestRunIdentifier}-{clientBuilder.Name}");
            await tenant.CreateOrganizationAsync(createdOrganization, opt => opt.CreateDirectory = true);

            createdOrganization.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href);

            var defaultGroupStore = await createdOrganization.GetDefaultGroupStoreAsync() as IDirectory;
            defaultGroupStore.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(defaultGroupStore.Href);

            var group1 = await createdOrganization.CreateGroupAsync($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Group Query Results1", string.Empty);
            var group2 = await createdOrganization.CreateGroupAsync($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Group Query Results2", string.Empty);

            this.fixture.CreatedGroupHrefs.Add(group1.Href);
            this.fixture.CreatedGroupHrefs.Add(group2.Href);

            // TODO ArgumentException after Shouldly Mono update
            Should.Throw<Exception>(async () =>
            {
                // Throws because multiple matching results exist
                var mapping = await createdOrganization
                    .AddAccountStoreAsync<IGroup>(groups => groups.Where(x => x.Name.StartsWith($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Group Query Results")));
            });

            // Clean up
            (await group1.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedGroupHrefs.Remove(group1.Href);

            (await group2.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedGroupHrefs.Remove(group2.Href);

            (await defaultGroupStore.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(defaultGroupStore.Href);

            (await createdOrganization.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href);
        }
    }
}
