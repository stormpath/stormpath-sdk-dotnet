// <copyright file="Application_tests.cs" company="Stormpath, Inc.">
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
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Error;
using Stormpath.SDK.Group;
using Stormpath.SDK.Tests.Common.Integration;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Async
{
    [Collection(nameof(IntegrationTestCollection))]
    public class Application_tests
    {
        private readonly TestFixture fixture;

        public Application_tests(TestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Getting_application_tenant(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            // Verify data from IntegrationTestData
            var tenantHref = (await application.GetTenantAsync()).Href;
            tenantHref.ShouldBe(this.fixture.TenantHref);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
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
            {
                this.fixture.CreatedDirectoryHrefs.Add(defaultAccountStore.Href);
            }

            defaultAccountStore.ShouldBeNull(); // no auto-created directory = no default account store

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Getting_default_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var defaultAccountStore = await app.GetDefaultAccountStoreAsync();
            defaultAccountStore.ShouldNotBeNull();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Getting_default_group_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var defaultGroupStore = await app.GetDefaultGroupStoreAsync();
            defaultGroupStore.ShouldNotBeNull();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Creating_application_with_response_options(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var newApp = client
                .Instantiate<IApplication>()
                .SetName($".NET IT {this.fixture.TestRunIdentifier} Application #3");

            await tenant.CreateApplicationAsync(newApp, opt => opt.ResponseOptions.Expand(x => x.GetCustomData()));

            newApp.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(newApp.Href);

            // Clean up
            (await newApp.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(newApp.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Saving_with_response_options(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var application = await tenant.GetApplications()
                .Where(app => app.Name.StartsWith($".NET IT (disabled) {this.fixture.TestRunIdentifier}"))
                .SingleAsync();

            application.SetStatus(ApplicationStatus.Disabled);
            await application.SaveAsync(response => response.Expand(x => x.GetAccounts()));
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Reset_password_with_encoded_jwt(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var token = await application.SendPasswordResetEmailAsync("vader@galacticempire.co");

            // When reset tokens are sent via email, the JWT . separator is encoded as %2E
            var encodedToken = token.GetValue()
                .Replace(".", "%2E");

            var validTokenResponse = await application.VerifyPasswordResetTokenAsync(encodedToken);
            validTokenResponse.Email.ShouldBe("vader@galacticempire.co");

            var resetPasswordResponse = await application.ResetPasswordAsync(encodedToken, "Ifindyourlackofsecuritydisturbing!1");
            resetPasswordResponse.Email.ShouldBe("vader@galacticempire.co");
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Reset_password_for_account_in_organization_by_nameKey(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);
            var accountStore = await application.GetDefaultAccountStoreAsync();

            var token = await application.SendPasswordResetEmailAsync("vader@galacticempire.co", this.fixture.PrimaryOrganizationNameKey);

            var validTokenResponse = await application.VerifyPasswordResetTokenAsync(token.GetValue());
            validTokenResponse.Email.ShouldBe("vader@galacticempire.co");

            var resetPasswordResponse = await application.ResetPasswordAsync(token.GetValue(), "Ifindyourlackofsecuritydisturbing!1");
            resetPasswordResponse.Email.ShouldBe("vader@galacticempire.co");
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Creating_second_account_store_mapping_at_zeroth_index(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Adding Two AccountStores Directly Test Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var mapping1 = await createdApplication.AddAccountStoreAsync(this.fixture.PrimaryDirectoryHref);

            var group = await client.GetResourceAsync<IGroup>(this.fixture.PrimaryGroupHref);
            var mapping2 = client.Instantiate<IAccountStoreMapping>();
            mapping2.SetAccountStore(group);
            mapping2.SetListIndex(0);
            await createdApplication.CreateAccountStoreMappingAsync(mapping2);

            mapping2.ListIndex.ShouldBe(0);
            mapping1.ListIndex.ShouldBe(1);

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Adding_directory_as_account_store(TestClientProvider clientBuilder)
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Adding_group_as_account_store(TestClientProvider clientBuilder)
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Saving_new_mapping_as_default(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Creating New AccountStore as Default Test Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var directory = await client.GetResourceAsync<IDirectory>(this.fixture.PrimaryDirectoryHref);
            var mapping = client.Instantiate<IAccountStoreMapping>()
                .SetAccountStore(directory)
                .SetApplication(createdApplication)
                .SetDefaultAccountStore(true)
                .SetDefaultGroupStore(true);

            await createdApplication.CreateAccountStoreMappingAsync(mapping);

            // Default links should be updated without having to re-retrieve the Application resource
            (await createdApplication.GetDefaultAccountStoreAsync()).Href.ShouldBe(this.fixture.PrimaryDirectoryHref);
            (await createdApplication.GetDefaultGroupStoreAsync()).Href.ShouldBe(this.fixture.PrimaryDirectoryHref);

            // Retrieving it again should have the same result
            var updated = await client.GetResourceAsync<IApplication>(createdApplication.Href);
            updated.ShouldNotBeNull();

            var updatedDefaultAccountStore = await updated.GetDefaultAccountStoreAsync();
            updatedDefaultAccountStore.ShouldNotBeNull();
            updatedDefaultAccountStore.Href.ShouldBe(this.fixture.PrimaryDirectoryHref);

            var updatedDefaultGroupStore = await updated.GetDefaultGroupStoreAsync();
            updatedDefaultGroupStore.ShouldNotBeNull();
            updatedDefaultGroupStore.Href.ShouldBe(this.fixture.PrimaryDirectoryHref);

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
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

            (await createdApplication.GetDefaultAccountStoreAsync()).Href.ShouldBe(this.fixture.PrimaryDirectoryHref);
            mapping.IsDefaultAccountStore.ShouldBeTrue();
            mapping.IsDefaultGroupStore.ShouldBeFalse();

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
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

            (await createdApplication.GetDefaultAccountStoreAsync()).Href.ShouldBe(this.fixture.PrimaryGroupHref);
            mapping.IsDefaultAccountStore.ShouldBeTrue();
            mapping.IsDefaultGroupStore.ShouldBeFalse();

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
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

            (await createdApplication.GetDefaultAccountStoreAsync()).Href.ShouldBe(this.fixture.PrimaryDirectoryHref);

            var mapping = await createdApplication.GetAccountStoreMappings().SingleAsync();
            mapping.IsDefaultAccountStore.ShouldBeTrue();
            mapping.IsDefaultGroupStore.ShouldBeFalse();

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
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

            (await createdApplication.GetDefaultAccountStoreAsync()).Href.ShouldBe(this.fixture.PrimaryGroupHref);

            var mapping = await createdApplication.GetAccountStoreMappings().SingleAsync();
            mapping.IsDefaultAccountStore.ShouldBeTrue();
            mapping.IsDefaultGroupStore.ShouldBeFalse();

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
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

            (await createdApplication.GetDefaultGroupStoreAsync()).Href.ShouldBe(this.fixture.PrimaryDirectoryHref);
            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeTrue();

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
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

            (await createdApplication.GetDefaultGroupStoreAsync()).Href.ShouldBe(this.fixture.PrimaryDirectoryHref);

            var mapping = await createdApplication.GetAccountStoreMappings().SingleAsync();
            mapping.IsDefaultAccountStore.ShouldBeFalse();
            mapping.IsDefaultGroupStore.ShouldBeTrue();

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
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
            Func<Task> act = async () =>
            {
                await createdApplication.SetDefaultGroupStoreAsync(group);
            };
            act.ShouldThrow<ResourceException>();

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Adding_directory_as_account_store_by_href(TestClientProvider clientBuilder)
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Adding_group_as_account_store_by_href(TestClientProvider clientBuilder)
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Adding_directory_as_account_store_by_name(TestClientProvider clientBuilder)
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
                .SetName($".NET IT {this.fixture.TestRunIdentifier} Add Directory As AccountStore By Name");
            await client.CreateDirectoryAsync(testDirectory);
            testDirectory.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(testDirectory.Href);

            var mapping = await createdApplication.AddAccountStoreAsync($".NET IT {this.fixture.TestRunIdentifier} Add Directory As AccountStore By Name");

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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Adding_group_as_account_store_by_name(TestClientProvider clientBuilder)
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
                .SetName($".NET IT {this.fixture.TestRunIdentifier} Add Group As AccountStore By Name");
            await createdApplication.CreateGroupAsync(testGroup);
            testGroup.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedGroupHrefs.Add(testGroup.Href);

            var newMapping = await createdApplication.AddAccountStoreAsync($".NET IT {this.fixture.TestRunIdentifier} Add Group As AccountStore By Name");

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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Adding_directory_as_account_store_by_query(TestClientProvider clientBuilder)
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Adding_group_as_account_store_by_query(TestClientProvider clientBuilder)
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Adding_directory_as_account_store_by_query_throws_for_multiple_results(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Directory By Query Throws Test Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var dir1 = await client.CreateDirectoryAsync($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Application Multiple Directory Query Results1", string.Empty, DirectoryStatus.Enabled);
            var dir2 = await client.CreateDirectoryAsync($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Application Multiple Directory Query Results2", string.Empty, DirectoryStatus.Enabled);

            this.fixture.CreatedDirectoryHrefs.Add(dir1.Href);
            this.fixture.CreatedDirectoryHrefs.Add(dir2.Href);

            Func<Task> act = async () =>
            {
                // Throws because multiple matching results exist
                var mapping = await createdApplication
                    .AddAccountStoreAsync<IDirectory>(dirs => dirs.Where(d => d.Name.StartsWith($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Application Multiple Directory Query Results")));
            };
            act.ShouldThrow<ArgumentException>();

            // Clean up
            (await dir1.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(dir1.Href);

            (await dir2.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(dir2.Href);

            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Adding_group_as_account_store_by_query_throws_for_multiple_results(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Adding AccountStore Group By Query Throws Test Application",
                createDirectory: true);

            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var defaultGroupStore = await createdApplication.GetDefaultGroupStoreAsync() as IDirectory;
            defaultGroupStore.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(defaultGroupStore.Href);

            var group1 = await createdApplication.CreateGroupAsync($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Application Multiple Group Query Results1", string.Empty);
            var group2 = await createdApplication.CreateGroupAsync($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Application Multiple Group Query Results2", string.Empty);

            this.fixture.CreatedGroupHrefs.Add(group1.Href);
            this.fixture.CreatedGroupHrefs.Add(group2.Href);

            Func<Task> act = async () =>
            {
                // Throws because multiple matching results exist
                var mapping = await createdApplication
                    .AddAccountStoreAsync<IGroup>(groups => groups.Where(x => x.Name.StartsWith($".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Application Multiple Group Query Results")));
            };
            act.ShouldThrow<ArgumentException>();

            // Clean up
            (await group1.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedGroupHrefs.Remove(group1.Href);

            (await group2.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedGroupHrefs.Remove(group2.Href);

            (await defaultGroupStore.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(defaultGroupStore.Href);

            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Updating_authorized_callback_uris(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            var application = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier} Updating Authorized Callback URIs Application",
                createDirectory: false);
            application.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(application.Href);

            application.AuthorizedCallbackUris.Any().ShouldBeFalse();

            // Update #1
            application.SetAuthorizedCallbackUris(new string[] {"http://foo", "http://bar"});
            application.AuthorizedCallbackUris.Count.ShouldBe(2);
            await application.SaveAsync();

            // Update #2
            var updatedUriList = 
                new string[] {"http://baz/callback", "https://secure/qux"}
                .Concat(application.AuthorizedCallbackUris);
            application.SetAuthorizedCallbackUris(updatedUriList);

            await application.SaveAsync();

            application.AuthorizedCallbackUris.ShouldContain("http://foo");
            application.AuthorizedCallbackUris.ShouldContain("http://bar");
            application.AuthorizedCallbackUris.ShouldContain("http://baz/callback");
            application.AuthorizedCallbackUris.ShouldContain("https://secure/qux");
            application.AuthorizedCallbackUris.Count.ShouldBe(4);

            // Clean up
            (await application.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(application.Href);
        }
    }
}
