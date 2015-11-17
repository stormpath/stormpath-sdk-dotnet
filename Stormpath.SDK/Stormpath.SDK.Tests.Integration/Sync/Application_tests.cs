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
using Shouldly;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Sync;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Sync
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
        public void Getting_tenant_applications(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();
            var applications = tenant.GetApplications().Synchronously().ToList();

            applications.Count.ShouldNotBe(0);
            applications
                .Any(app => app.Status == ApplicationStatus.Enabled)
                .ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Getting_application_tenant(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            // Verify data from IntegrationTestData
            var tenantHref = application.GetTenant().Href;
            tenantHref.ShouldBe(this.fixture.TenantHref);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Creating_application_without_directory(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var newApplicationName = $".NET IT {this.fixture.TestRunIdentifier} Application #2";
            var createdApplication = tenant.CreateApplication(newApplicationName, createDirectory: false);

            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);
            createdApplication.Name.ShouldBe(newApplicationName);
            createdApplication.Status.ShouldBe(ApplicationStatus.Enabled);

            var defaultAccountStore = createdApplication.GetDefaultAccountStore();
            if (!string.IsNullOrEmpty(defaultAccountStore?.Href))
                this.fixture.CreatedDirectoryHrefs.Add(defaultAccountStore.Href);

            defaultAccountStore.ShouldBeNull(); // no auto-created directory = no default account store

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Getting_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var defaultAccountStore = app.GetDefaultAccountStore();
            var asDirectory = defaultAccountStore as IDirectory;
            asDirectory.ShouldNotBeNull();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Getting_default_group_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var defaultGroupStore = app.GetDefaultGroupStore();
            var asDirectory = defaultGroupStore as IDirectory;
            asDirectory.ShouldNotBeNull();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Creating_application_with_response_options(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var newApp = client
                .Instantiate<IApplication>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier} Application #3 - Sync");

            tenant.CreateApplication(newApp, opt => opt.ResponseOptions.Expand(x => x.GetCustomData));

            newApp.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(newApp.Href);

            // Clean up
            newApp.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(newApp.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Updating_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var application = tenant.GetApplications()
                .Synchronously()
                .Where(app => app.Name.StartsWith($".NET IT (disabled) {this.fixture.TestRunIdentifier}"))
                .Single();

            application.SetDescription("The Battle of Yavin - Victory!");
            var saveResult = application.Save();

            saveResult.Description.ShouldBe("The Battle of Yavin - Victory!");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Saving_with_response_options(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var application = tenant.GetApplications()
                .Synchronously()
                .Where(app => app.Name.StartsWith($".NET IT (disabled) {this.fixture.TestRunIdentifier}"))
                .Single();

            application.SetStatus(ApplicationStatus.Disabled);
            application.Save(response => response.Expand(x => x.GetAccounts));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Searching_by_name(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var application = tenant.GetApplications()
                .Synchronously()
                .Where(app => app.Name.StartsWith($".NET IT (primary) {this.fixture.TestRunIdentifier}"))
                .Single();

            application.Description.ShouldBe("The Battle of Endor");
            application.Status.ShouldBe(ApplicationStatus.Enabled);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Searching_by_description(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var applications = tenant.GetApplications()
                .Synchronously()
                .Where(app => app.Description == "The Battle Of Endor")
                .ToList();

            applications
                .Any(app => app.Name.StartsWith($".NET IT (primary) {this.fixture.TestRunIdentifier}"))
                .ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Searching_by_status(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var applications = tenant.GetApplications()
                .Synchronously()
                .Where(app => app.Status == ApplicationStatus.Disabled)
                .ToList();

            applications
                .Any(app => app.Name.StartsWith($".NET IT (disabled) {this.fixture.TestRunIdentifier}"))
                .ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Reset_password_for_valid_account(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var token = application.SendPasswordResetEmail("vader@galacticempire.co");

            var validTokenResponse = application.VerifyPasswordResetToken(token.GetValue());
            validTokenResponse.Email.ShouldBe("vader@galacticempire.co");

            var resetPasswordResponse = application.ResetPassword(token.GetValue(), "Ifindyourlackofsecuritydisturbing!1");
            resetPasswordResponse.Email.ShouldBe("vader@galacticempire.co");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Reset_password_for_account_in_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);
            var accountStore = application.GetDefaultAccountStore();

            var token = application.SendPasswordResetEmail("vader@galacticempire.co", accountStore);

            var validTokenResponse = application.VerifyPasswordResetToken(token.GetValue());
            validTokenResponse.Email.ShouldBe("vader@galacticempire.co");

            var resetPasswordResponse = application.ResetPassword(token.GetValue(), "Ifindyourlackofsecuritydisturbing!1");
            resetPasswordResponse.Email.ShouldBe("vader@galacticempire.co");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Creating_account_store_mapping(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Directly Test Application (Sync)",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            IAccountStore directory = client.GetResource<IDirectory>(this.fixture.PrimaryDirectoryHref);

            var mapping = client.Instantiate<IAccountStoreMapping>();
            mapping.SetAccountStore(directory);
            mapping.SetListIndex(500);
            createdApplication.CreateAccountStoreMapping(mapping);

            mapping.GetAccountStore().Href.ShouldBe(directory.Href);
            mapping.GetApplication().Href.ShouldBe(createdApplication.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Adding_directory_as_account_store_to_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Directory Test Application (Sync)",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var directory = client.GetResource<IDirectory>(this.fixture.PrimaryDirectoryHref);
            var mapping = createdApplication.AddAccountStore(directory);

            mapping.GetAccountStore().Href.ShouldBe(directory.Href);
            mapping.GetApplication().Href.ShouldBe(createdApplication.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Adding_group_as_account_store_to_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Group Test Application (Sync)",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var group = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref);
            var mapping = createdApplication.AddAccountStore(group);

            mapping.GetAccountStore().Href.ShouldBe(group.Href);
            mapping.GetApplication().Href.ShouldBe(createdApplication.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Setting_mapped_directory_to_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier} Setting Existing Directory AccountStore Default Test Application (Sync)",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var directory = client.GetResource<IDirectory>(this.fixture.PrimaryDirectoryHref);
            var mapping = createdApplication.AddAccountStore(directory);

            createdApplication.SetDefaultAccountStore(directory);

            mapping.IsDefaultAccountStore.ShouldBeTrue();
            mapping.IsDefaultGroupStore.ShouldBeFalse();

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Setting_mapped_group_to_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier} Setting Existing Group AccountStore Default Test Application (Sync)",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var group = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref);
            var mapping = createdApplication.AddAccountStore(group);

            createdApplication.SetDefaultAccountStore(group);

            mapping.IsDefaultAccountStore.ShouldBeTrue();
            mapping.IsDefaultGroupStore.ShouldBeFalse();

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Setting_unmapped_directory_to_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier} Setting Existing AccountStore Default Test Application (Sync)",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var directory = client.GetResource<IDirectory>(this.fixture.PrimaryDirectoryHref);
            createdApplication.SetDefaultAccountStore(directory);

            var mapping = createdApplication.GetAccountStoreMappings().Synchronously().Single();
            mapping.IsDefaultAccountStore.ShouldBeTrue();
            mapping.IsDefaultGroupStore.ShouldBeFalse();

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Setting_unmapped_group_to_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier} Setting Existing AccountStore Default Test Application (Sync)",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var group = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref);
            createdApplication.SetDefaultAccountStore(group);

            var mapping = createdApplication.GetAccountStoreMappings().Synchronously().Single();
            mapping.IsDefaultAccountStore.ShouldBeTrue();
            mapping.IsDefaultGroupStore.ShouldBeFalse();

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Setting_mapped_directory_to_default_group_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier} Setting Existing Directory AccountStore Default Group Test Application (Sync)",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var directory = client.GetResource<IDirectory>(this.fixture.PrimaryDirectoryHref);
            var mapping = createdApplication.AddAccountStore(directory);

            createdApplication.SetDefaultGroupStore(directory);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeTrue();

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Setting_unmapped_directory_to_default_group_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier} Setting Existing AccountStore Default Group Test Application (Sync)",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var directory = client.GetResource<IDirectory>(this.fixture.PrimaryDirectoryHref);
            createdApplication.SetDefaultGroupStore(directory);

            var mapping = createdApplication.GetAccountStoreMappings().Synchronously().Single();
            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeTrue();

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Setting_group_group_store_throws(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier} Setting Group as GroupStore (Sync)",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var group = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref);

            // If this errors, the server-side API behavior has changed.
            Should.Throw<Exception>(() =>
            {
                createdApplication.SetDefaultGroupStore(group);
            });

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Adding_directory_as_account_store_by_href_to_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Directory By Href Test Application (Sync)",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var mapping = createdApplication.AddAccountStore(this.fixture.PrimaryDirectoryHref);

            mapping.GetAccountStore().Href.ShouldBe(this.fixture.PrimaryDirectoryHref);
            mapping.GetApplication().Href.ShouldBe(createdApplication.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Adding_group_as_account_store_by_href_to_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Group Test By Href Application (Sync)",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var mapping = createdApplication.AddAccountStore(this.fixture.PrimaryGroupHref);

            mapping.GetAccountStore().Href.ShouldBe(this.fixture.PrimaryGroupHref);
            mapping.GetApplication().Href.ShouldBe(createdApplication.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Adding_directory_as_account_store_by_name_to_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Directory By Name Test Application (Sync)",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var testDirectory = client
                .Instantiate<IDirectory>()
                .SetName($".NET Test {this.fixture.TestRunIdentifier} Add Directory As AccountStore By Name");
            client.CreateDirectory(testDirectory);
            testDirectory.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(testDirectory.Href);

            var mapping = createdApplication.AddAccountStore($".NET Test {this.fixture.TestRunIdentifier} Add Directory As AccountStore By Name");

            mapping.GetAccountStore().Href.ShouldBe(testDirectory.Href);
            mapping.GetApplication().Href.ShouldBe(createdApplication.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);

            testDirectory.Delete().ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(testDirectory.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Adding_group_as_account_store_by_name_to_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Group By Name Test Application (Sync)",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Needs to have a default GroupStore
            var mapping = createdApplication.AddAccountStore(this.fixture.PrimaryDirectoryHref);
            mapping.SetDefaultGroupStore(true);
            mapping.Save();

            var testGroup = client
                .Instantiate<IGroup>()
                .SetName($".NET Test {this.fixture.TestRunIdentifier} Add Group As AccountStore By Name (Sync)");
            createdApplication.CreateGroup(testGroup);
            testGroup.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedGroupHrefs.Add(testGroup.Href);

            var newMapping = createdApplication.AddAccountStore($".NET Test {this.fixture.TestRunIdentifier} Add Group As AccountStore By Name (Sync)");

            newMapping.GetAccountStore().Href.ShouldBe(testGroup.Href);
            newMapping.GetApplication().Href.ShouldBe(createdApplication.Href);

            newMapping.IsDefaultAccountStore.ShouldBeFalse();
            newMapping.IsDefaultGroupStore.ShouldBeFalse();
            newMapping.ListIndex.ShouldBe(1);

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);

            testGroup.Delete().ShouldBeTrue();
            this.fixture.CreatedGroupHrefs.Remove(testGroup.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Adding_directory_as_account_store_to_application_by_query(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Directory By Query Test Application (Sync)",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var directoryName = client.GetResource<IDirectory>(this.fixture.PrimaryDirectoryHref).Name;
            var mapping = createdApplication
                .AddAccountStore<IDirectory>(dirs => dirs.Where(d => d.Name.EndsWith(directoryName.Substring(1))));

            mapping.GetAccountStore().Href.ShouldBe(this.fixture.PrimaryDirectoryHref);
            mapping.GetApplication().Href.ShouldBe(createdApplication.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Adding_group_as_account_store_to_application_by_query(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Group By Query Test Application (Sync)",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var groupName = client.GetResource<IGroup>(this.fixture.PrimaryGroupHref).Name;
            var mapping = createdApplication
                .AddAccountStore<IGroup>(groups => groups.Where(g => g.Name.EndsWith(groupName.Substring(1))));

            mapping.GetAccountStore().Href.ShouldBe(this.fixture.PrimaryGroupHref);
            mapping.GetApplication().Href.ShouldBe(createdApplication.Href);

            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeFalse();
            mapping.ListIndex.ShouldBe(0);

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Adding_directory_as_account_store_to_application_by_query_throws_for_multiple_results(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Directory By Query Throws Test Application (Sync)",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            Should.Throw<Exception>(() =>
            {
                var mapping = createdApplication
                    .AddAccountStore<IDirectory>(allDirs => allDirs);
            });

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Adding_group_as_account_store_to_application_by_query_throws_for_multiple_results(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Group By Query Throws Test Application (Sync)",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var dummyGroup = client
                .Instantiate<IGroup>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier} Dummy Test Group for Adding Multiple Groups as AccountStore (Sync)");
            var primaryDirectory = client.GetResource<IDirectory>(this.fixture.PrimaryDirectoryHref);
            primaryDirectory.CreateGroup(dummyGroup);
            dummyGroup.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedGroupHrefs.Add(dummyGroup.Href);

            Should.Throw<Exception>(() =>
            {
                var mapping = createdApplication
                    .AddAccountStore<IGroup>(allGroups => allGroups);
            });

            // Clean up
            dummyGroup.Delete().ShouldBeTrue();
            this.fixture.CreatedGroupHrefs.Remove(dummyGroup.Href);

            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }
    }
}
