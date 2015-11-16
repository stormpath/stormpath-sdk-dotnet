// <copyright file="Application_tests.cs" company="Stormpath, Inc.">
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

using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Async
{
    [Collection(nameof(IntegrationTestCollection))]
    public class Application_tests
    {
        private readonly IntegrationTestFixture fixture;

        public Application_tests(IntegrationTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Getting_tenant_applications(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();
            var applications = await tenant.GetApplications().ToListAsync();

            applications.Count.ShouldNotBe(0);
            applications
                .Any(app => app.Status == ApplicationStatus.Enabled)
                .ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Getting_application_tenant(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            // Verify data from IntegrationTestData
            var tenantHref = (await application.GetTenantAsync()).Href;
            tenantHref.ShouldBe(this.fixture.TenantHref);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Creating_application_without_directory(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var newApplicationName = $".NET IT {this.fixture.TestRunIdentifier} Application #2";
            var createdApplication = await tenant.CreateApplicationAsync(newApplicationName, createDirectory: false);

            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);
            createdApplication.Name.ShouldBe(newApplicationName);
            createdApplication.Status.ShouldBe(ApplicationStatus.Enabled);

            var defaultAccountStore = await createdApplication.GetDefaultAccountStoreAsync();
            if (!string.IsNullOrEmpty(defaultAccountStore?.Href))
                this.fixture.CreatedDirectoryHrefs.Add(defaultAccountStore.Href);

            defaultAccountStore.ShouldBeNull(); // no auto-created directory = no default account store

            // Clean up
            var deleted = await createdApplication.DeleteAsync();
            if (deleted)
                this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Getting_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var defaultAccountStore = await app.GetDefaultAccountStoreAsync();
            var asDirectory = defaultAccountStore as IDirectory;
            asDirectory.ShouldNotBeNull();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Getting_default_group_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var defaultGroupStore = await app.GetDefaultGroupStoreAsync();
            var asDirectory = defaultGroupStore as IDirectory;
            asDirectory.ShouldNotBeNull();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Creating_application_with_response_options(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var newApp = client
                .Instantiate<IApplication>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier} Application #3");

            await tenant.CreateApplicationAsync(newApp, opt => opt.ResponseOptions.Expand(x => x.GetCustomDataAsync));

            newApp.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(newApp.Href);

            // Clean up
            await newApp.DeleteAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Updating_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var application = await tenant.GetApplications()
                .Where(app => app.Name.StartsWith($".NET IT (disabled) {this.fixture.TestRunIdentifier}"))
                .SingleAsync();

            application.SetDescription("The Battle of Yavin - Victory!");
            var saveResult = await application.SaveAsync();

            saveResult.Description.ShouldBe("The Battle of Yavin - Victory!");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Saving_with_response_options(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var application = await tenant.GetApplications()
                .Where(app => app.Name.StartsWith($".NET IT (disabled) {this.fixture.TestRunIdentifier}"))
                .SingleAsync();

            application.SetStatus(ApplicationStatus.Disabled);
            await application.SaveAsync(response => response.Expand(x => x.GetAccounts));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Searching_by_name(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var application = await tenant.GetApplications()
                .Where(app => app.Name.StartsWith($".NET IT (primary) {this.fixture.TestRunIdentifier}"))
                .SingleAsync();

            application.Description.ShouldBe("The Battle of Endor");
            application.Status.ShouldBe(ApplicationStatus.Enabled);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Searching_by_description(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var applications = await tenant.GetApplications()
                .Where(app => app.Description == "The Battle Of Endor")
                .ToListAsync();

            applications
                .Any(app => app.Name.StartsWith($".NET IT (primary) {this.fixture.TestRunIdentifier}"))
                .ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Searching_by_status(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var applications = await tenant.GetApplications()
                .Where(app => app.Status == ApplicationStatus.Disabled)
                .ToListAsync();

            applications
                .Any(app => app.Name.StartsWith($".NET IT (disabled) {this.fixture.TestRunIdentifier}"))
                .ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Reset_password_for_valid_account(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var token = await application.SendPasswordResetEmailAsync("vader@galacticempire.co");

            var validTokenResponse = await application.VerifyPasswordResetTokenAsync(token.GetValue());
            validTokenResponse.Email.ShouldBe("vader@galacticempire.co");

            var resetPasswordResponse = await application.ResetPasswordAsync(token.GetValue(), "Ifindyourlackofsecuritydisturbing!1");
            resetPasswordResponse.Email.ShouldBe("vader@galacticempire.co");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Reset_password_for_account_in_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);
            var accountStore = await application.GetDefaultAccountStoreAsync();

            var token = await application.SendPasswordResetEmailAsync("vader@galacticempire.co", accountStore);

            var validTokenResponse = await application.VerifyPasswordResetTokenAsync(token.GetValue());
            validTokenResponse.Email.ShouldBe("vader@galacticempire.co");

            var resetPasswordResponse = await application.ResetPasswordAsync(token.GetValue(), "Ifindyourlackofsecuritydisturbing!1");
            resetPasswordResponse.Email.ShouldBe("vader@galacticempire.co");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Creating_account_store_mapping(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Directly Test Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            IAccountStore directory = await client.GetResourceAsync<IDirectory>(this.fixture.PrimaryDirectoryHref);

            var mapping = client.Instantiate<IAccountStoreMapping>();
            mapping.SetAccountStore(directory);
            mapping.SetListIndex(500);
            await createdApplication.CreateAccountStoreMappingAsync(mapping);

            (await mapping.GetAccountStoreAsync()).Href.ShouldBe(directory.Href);
            (await mapping.GetApplicationAsync()).Href.ShouldBe(createdApplication.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Adding_directory_as_account_store_to_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Directory Test Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var directory = await client.GetResourceAsync<IDirectory>(this.fixture.PrimaryDirectoryHref);
            var mapping = await createdApplication.AddAccountStoreAsync(directory);

            (await mapping.GetAccountStoreAsync()).Href.ShouldBe(directory.Href);
            (await mapping.GetApplicationAsync()).Href.ShouldBe(createdApplication.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Adding_group_as_account_store_to_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Group Test Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var group = await client.GetResourceAsync<IGroup>(this.fixture.PrimaryGroupHref);
            var mapping = await createdApplication.AddAccountStoreAsync(group);

            (await mapping.GetAccountStoreAsync()).Href.ShouldBe(group.Href);
            (await mapping.GetApplicationAsync()).Href.ShouldBe(createdApplication.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Setting_mapped_directory_to_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Setting Existing Directory AccountStore Default Test Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var directory = await client.GetResourceAsync<IDirectory>(this.fixture.PrimaryDirectoryHref);
            var mapping = await createdApplication.AddAccountStoreAsync(directory);

            await createdApplication.SetDefaultAccountStoreAsync(directory);

            mapping.IsDefaultAccountStore.ShouldBeTrue();
            mapping.IsDefaultGroupStore.ShouldBeFalse();

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Setting_mapped_group_to_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Setting Existing Group AccountStore Default Test Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var group = await client.GetResourceAsync<IGroup>(this.fixture.PrimaryGroupHref);
            var mapping = await createdApplication.AddAccountStoreAsync(group);

            await createdApplication.SetDefaultAccountStoreAsync(group);

            mapping.IsDefaultAccountStore.ShouldBeTrue();
            mapping.IsDefaultGroupStore.ShouldBeFalse();

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Setting_unmapped_directory_to_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Setting Existing AccountStore Default Test Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var directory = await client.GetResourceAsync<IDirectory>(this.fixture.PrimaryDirectoryHref);
            await createdApplication.SetDefaultAccountStoreAsync(directory);

            var mapping = await createdApplication.GetAccountStoreMappings().SingleAsync();
            mapping.IsDefaultAccountStore.ShouldBeTrue();
            mapping.IsDefaultGroupStore.ShouldBeFalse();

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Setting_unmapped_group_to_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Setting Existing AccountStore Default Test Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var group = await client.GetResourceAsync<IGroup>(this.fixture.PrimaryGroupHref);
            await createdApplication.SetDefaultAccountStoreAsync(group);

            var mapping = await createdApplication.GetAccountStoreMappings().SingleAsync();
            mapping.IsDefaultAccountStore.ShouldBeTrue();
            mapping.IsDefaultGroupStore.ShouldBeFalse();

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Setting_mapped_directory_to_default_group_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Setting Existing Directory AccountStore Default Group Test Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var directory = await client.GetResourceAsync<IDirectory>(this.fixture.PrimaryDirectoryHref);
            var mapping = await createdApplication.AddAccountStoreAsync(directory);

            await createdApplication.SetDefaultGroupStoreAsync(directory);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeTrue();

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Setting_unmapped_directory_to_default_group_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Setting Existing AccountStore Default Group Test Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var directory = await client.GetResourceAsync<IDirectory>(this.fixture.PrimaryDirectoryHref);
            await createdApplication.SetDefaultGroupStoreAsync(directory);

            var mapping = await createdApplication.GetAccountStoreMappings().SingleAsync();
            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeTrue();

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Setting_group_group_store_throws(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Setting Group as GroupStore",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var group = await client.GetResourceAsync<IGroup>(this.fixture.PrimaryGroupHref);

            // If this errors, the server-side API behavior has changed.
            Should.Throw<Exception>(async () =>
            {
                await createdApplication.SetDefaultGroupStoreAsync(group);
            });

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Adding_directory_as_account_store_by_href_to_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Directory By Href Test Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var mapping = await createdApplication.AddAccountStoreAsync(this.fixture.PrimaryDirectoryHref);

            (await mapping.GetAccountStoreAsync()).Href.ShouldBe(this.fixture.PrimaryDirectoryHref);
            (await mapping.GetApplicationAsync()).Href.ShouldBe(createdApplication.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Adding_group_as_account_store_by_href_to_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Group Test By Href Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var mapping = await createdApplication.AddAccountStoreAsync(this.fixture.PrimaryGroupHref);

            (await mapping.GetAccountStoreAsync()).Href.ShouldBe(this.fixture.PrimaryGroupHref);
            (await mapping.GetApplicationAsync()).Href.ShouldBe(createdApplication.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Adding_directory_as_account_store_by_name_to_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Directory By Name Test Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var testDirectory = client
                .Instantiate<IDirectory>()
                .SetName($".NET Test {this.fixture.TestRunIdentifier} Add Directory As AccountStore By Name");
            await client.CreateDirectoryAsync(testDirectory);
            testDirectory.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(testDirectory.Href);

            var mapping = await createdApplication.AddAccountStoreAsync($".NET Test {this.fixture.TestRunIdentifier} Add Directory As AccountStore By Name");

            (await mapping.GetAccountStoreAsync()).Href.ShouldBe(testDirectory.Href);
            (await mapping.GetApplicationAsync()).Href.ShouldBe(createdApplication.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);

            (await testDirectory.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(testDirectory.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Adding_group_as_account_store_by_name_to_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Group By Name Test Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Needs to have a default GroupStore
            var mapping = await createdApplication.AddAccountStoreAsync(this.fixture.PrimaryDirectoryHref);
            mapping.SetDefaultGroupStore(true);
            await mapping.SaveAsync();

            var testGroup = client
                .Instantiate<IGroup>()
                .SetName($".NET Test {this.fixture.TestRunIdentifier} Add Group As AccountStore By Name");
            await createdApplication.CreateGroupAsync(testGroup);
            testGroup.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedGroupHrefs.Add(testGroup.Href);

            var newMapping = await createdApplication.AddAccountStoreAsync($".NET Test {this.fixture.TestRunIdentifier} Add Group As AccountStore By Name");

            (await newMapping.GetAccountStoreAsync()).Href.ShouldBe(testGroup.Href);
            (await newMapping.GetApplicationAsync()).Href.ShouldBe(createdApplication.Href);

            newMapping.IsDefaultAccountStore.ShouldBeFalse();
            newMapping.IsDefaultGroupStore.ShouldBeFalse();
            newMapping.ListIndex.ShouldBe(1);

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);

            (await testGroup.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedGroupHrefs.Remove(testGroup.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Adding_directory_as_account_store_to_application_by_query(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Directory By Query Test Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var directoryName = (await client.GetResourceAsync<IDirectory>(this.fixture.PrimaryDirectoryHref)).Name;
            var mapping = await createdApplication
                .AddAccountStoreAsync<IDirectory>(dirs => dirs.Where(d => d.Name.EndsWith(directoryName.Substring(1))));

            (await mapping.GetAccountStoreAsync()).Href.ShouldBe(this.fixture.PrimaryDirectoryHref);
            (await mapping.GetApplicationAsync()).Href.ShouldBe(createdApplication.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Adding_group_as_account_store_to_application_by_query(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Group By Query Test Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var groupName = (await client.GetResourceAsync<IGroup>(this.fixture.PrimaryGroupHref)).Name;
            var mapping = await createdApplication
                .AddAccountStoreAsync<IGroup>(groups => groups.Where(g => g.Name.EndsWith(groupName.Substring(1))));

            (await mapping.GetAccountStoreAsync()).Href.ShouldBe(this.fixture.PrimaryGroupHref);
            (await mapping.GetApplicationAsync()).Href.ShouldBe(createdApplication.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Adding_directory_as_account_store_to_application_by_query_throws_for_multiple_results(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Directory By Query Throws Test Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            Should.Throw<Exception>(async () =>
            {
                var mapping = await createdApplication
                    .AddAccountStoreAsync<IDirectory>(allDirs => allDirs);
            });

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Adding_group_as_account_store_to_application_by_query_throws_for_multiple_results(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Group By Query Throws Test Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var dummyGroup = client
                .Instantiate<IGroup>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier} Dummy Test Group for Adding Multiple Groups as AccountStore");
            var primaryDirectory = await client.GetResourceAsync<IDirectory>(this.fixture.PrimaryDirectoryHref);
            await primaryDirectory.CreateGroupAsync(dummyGroup);
            dummyGroup.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedGroupHrefs.Add(dummyGroup.Href);

            Should.Throw<Exception>(async () =>
            {
                var mapping = await createdApplication
                    .AddAccountStoreAsync<IGroup>(allGroups => allGroups);
            });

            // Clean up
            (await dummyGroup.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedGroupHrefs.Remove(dummyGroup.Href);

            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }
    }
}
