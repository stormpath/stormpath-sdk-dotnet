' <copyright file="Application_tests.vb" company="Stormpath, Inc.">
' Copyright (c) 2015 Stormpath, Inc.
'
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License.
' You may obtain a copy of the License at
'
'      http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
' </copyright>

Option Strict On
Option Explicit On
Option Infer On
Imports Shouldly
Imports Stormpath.SDK.AccountStore
Imports Stormpath.SDK.Application
Imports Stormpath.SDK.Directory
Imports Stormpath.SDK.Group
Imports Stormpath.SDK.Tests.Common.Integration
Imports Xunit

Namespace Stormpath.SDK.Tests.Integration.VB.Async
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class Application_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_tenant_applications(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()
            Dim applications = Await tenant.GetApplications().ToListAsync()

            applications.Count.ShouldNotBe(0)
            applications.Any(Function(app) app.Status = ApplicationStatus.Enabled).ShouldBeTrue()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_application_tenant(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim application = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            ' Verify data from IntegrationTestData
            Dim tenantHref = (Await application.GetTenantAsync()).Href
            tenantHref.ShouldBe(Me.fixture.TenantHref)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Creating_application_without_directory(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim newApplicationName = $".NET IT {fixture.TestRunIdentifier} Application #2"
            Dim createdApplication = Await tenant.CreateApplicationAsync(newApplicationName, createDirectory:=False)

            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)
            createdApplication.Name.ShouldBe(newApplicationName)
            createdApplication.Status.ShouldBe(ApplicationStatus.Enabled)

            Dim defaultAccountStore = Await createdApplication.GetDefaultAccountStoreAsync()
            If Not String.IsNullOrEmpty(defaultAccountStore?.Href) Then
                Me.fixture.CreatedDirectoryHrefs.Add(defaultAccountStore.Href)
            End If

            defaultAccountStore.ShouldBeNull()
            ' no auto-created directory = no default account store

            ' Clean up
            Dim result = Await createdApplication.DeleteAsync()
            result.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_default_account_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim defaultAccountStore = Await app.GetDefaultAccountStoreAsync()
            defaultAccountStore.ShouldNotBeNull()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_default_group_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim defaultGroupStore = Await app.GetDefaultGroupStoreAsync()
            defaultGroupStore.ShouldNotBeNull()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Creating_application_with_response_options(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim newApp = client.Instantiate(Of IApplication)() _
                .SetName($".NET IT {fixture.TestRunIdentifier} Application #3")

            Await tenant.CreateApplicationAsync(newApp, Function(opt) opt.ResponseOptions.Expand(Function(x) x.GetCustomData()))

            newApp.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(newApp.Href)

            ' Clean up
            Dim result = Await newApp.DeleteAsync()
            result.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(newApp.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Updating_application(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim application = Await tenant.GetApplications() _
                .Where(Function(app) app.Name.StartsWith($".NET IT (disabled) {fixture.TestRunIdentifier}")) _
                .SingleAsync()

            application.SetDescription("The Battle of Yavin - Victory!")
            Dim saveResult = Await application.SaveAsync()

            saveResult.Description.ShouldBe("The Battle of Yavin - Victory!")
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Saving_with_response_options(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim application = Await tenant.GetApplications() _
                .Where(Function(app) app.Name.StartsWith($".NET IT (disabled) {fixture.TestRunIdentifier}")) _
                .SingleAsync()

            application.SetStatus(ApplicationStatus.Disabled)
            Await application.SaveAsync(Function(response) response.Expand(Function(x) x.GetAccounts()))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Searching_by_name(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim application = Await tenant.GetApplications() _
                .Where(Function(app) app.Name.StartsWith($".NET IT (primary) {fixture.TestRunIdentifier}")) _
                .SingleAsync()

            application.Description.ShouldBe("The Battle of Endor")
            application.Status.ShouldBe(ApplicationStatus.Enabled)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Searching_by_description(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim applications = Await tenant.GetApplications() _
                .Where(Function(app) app.Description = "The Battle Of Endor") _
                .ToListAsync()

            applications.Any(Function(app) app.Name.StartsWith($".NET IT (primary) {fixture.TestRunIdentifier}")).ShouldBeTrue()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Searching_by_status(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim applications = Await tenant.GetApplications().Where(Function(app) app.Status = ApplicationStatus.Disabled).ToListAsync()

            applications.Any(Function(app) app.Name.StartsWith($".NET IT (disabled) {fixture.TestRunIdentifier}")).ShouldBeTrue()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Reset_password_for_valid_account(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim application = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim token = Await application.SendPasswordResetEmailAsync("vader@galacticempire.co")

            Dim validTokenResponse = Await application.VerifyPasswordResetTokenAsync(token.GetValue())
            validTokenResponse.Email.ShouldBe("vader@galacticempire.co")

            Dim resetPasswordResponse = Await application.ResetPasswordAsync(token.GetValue(), "Ifindyourlackofsecuritydisturbing!1")
            resetPasswordResponse.Email.ShouldBe("vader@galacticempire.co")
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Reset_password_with_encoded_jwt(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim application = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim token = Await application.SendPasswordResetEmailAsync("vader@galacticempire.co")

            ' When reset tokens are sent via email, the JWT . separator is encoded as %2E
            Dim encodedToken = token.GetValue().Replace(".", "%2E")

            Dim validTokenResponse = Await application.VerifyPasswordResetTokenAsync(encodedToken)
            validTokenResponse.Email.ShouldBe("vader@galacticempire.co")

            Dim resetPasswordResponse = Await application.ResetPasswordAsync(encodedToken, "Ifindyourlackofsecuritydisturbing!1")
            resetPasswordResponse.Email.ShouldBe("vader@galacticempire.co")
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Reset_password_for_account_in_account_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim application = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim accountStore = Await application.GetDefaultAccountStoreAsync()

            Dim token = Await application.SendPasswordResetEmailAsync("vader@galacticempire.co", accountStore)

            Dim validTokenResponse = Await application.VerifyPasswordResetTokenAsync(token.GetValue())
            validTokenResponse.Email.ShouldBe("vader@galacticempire.co")

            Dim resetPasswordResponse = Await application.ResetPasswordAsync(token.GetValue(), "Ifindyourlackofsecuritydisturbing!1")
            resetPasswordResponse.Email.ShouldBe("vader@galacticempire.co")
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Creating_account_store_mapping(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Directly Test Application", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim directory As IAccountStore = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)

            Dim mapping = client.Instantiate(Of IAccountStoreMapping)()
            mapping.SetAccountStore(directory)
            mapping.SetListIndex(500)
            Await createdApplication.CreateAccountStoreMappingAsync(mapping)

            Dim result1 = Await mapping.GetAccountStoreAsync()
            result1.Href.ShouldBe(directory.Href)
            Dim result2 = Await mapping.GetApplicationAsync()
            result2.Href.ShouldBe(createdApplication.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            Dim deleteResult = Await createdApplication.DeleteAsync()
            deleteResult.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Creating_second_account_store_mapping_at_zeroth_index(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier} Adding Two AccountStores Directly Test Application", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim mapping1 = Await createdApplication.AddAccountStoreAsync(Me.fixture.PrimaryDirectoryHref)

            Dim group = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)
            Dim mapping2 = client.Instantiate(Of IAccountStoreMapping)()
            mapping2.SetAccountStore(group)
            mapping2.SetListIndex(0)
            Await createdApplication.CreateAccountStoreMappingAsync(mapping2)

            mapping2.ListIndex.ShouldBe(0)
            mapping1.ListIndex.ShouldBe(1)

            ' Clean up
            Dim result = Await createdApplication.DeleteAsync()
            result.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_directory_as_account_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Directory Test Application", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim directory = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            Dim mapping = Await createdApplication.AddAccountStoreAsync(directory)

            Dim result1 = Await mapping.GetAccountStoreAsync()
            result1.Href.ShouldBe(directory.Href)
            Dim result2 = Await mapping.GetApplicationAsync()
            result2.Href.ShouldBe(createdApplication.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            Dim result = Await createdApplication.DeleteAsync()
            result.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_group_as_account_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Group Test Application", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim group = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)
            Dim mapping = Await createdApplication.AddAccountStoreAsync(group)

            Dim result1 = Await mapping.GetAccountStoreAsync()
            result1.Href.ShouldBe(group.Href)
            Dim result2 = Await mapping.GetApplicationAsync()
            result2.Href.ShouldBe(createdApplication.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            Dim result3 = Await createdApplication.DeleteAsync()
            result3.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Setting_mapped_directory_to_default_account_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier} Setting Existing Directory AccountStore Default Test Application", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim directory = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            Dim mapping = Await createdApplication.AddAccountStoreAsync(directory)

            Await createdApplication.SetDefaultAccountStoreAsync(directory)

            mapping.IsDefaultAccountStore.ShouldBeTrue()
            mapping.IsDefaultGroupStore.ShouldBeFalse()

            ' Clean up
            Dim result = Await createdApplication.DeleteAsync()
            result.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Setting_mapped_group_to_default_account_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier} Setting Existing Group AccountStore Default Test Application", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim group = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)
            Dim mapping = Await createdApplication.AddAccountStoreAsync(group)

            Await createdApplication.SetDefaultAccountStoreAsync(group)

            mapping.IsDefaultAccountStore.ShouldBeTrue()
            mapping.IsDefaultGroupStore.ShouldBeFalse()

            ' Clean up
            Dim result = Await createdApplication.DeleteAsync()
            result.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Setting_unmapped_directory_to_default_account_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier} Setting Existing AccountStore Default Test Application", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim directory = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            Await createdApplication.SetDefaultAccountStoreAsync(directory)

            Dim mapping = Await createdApplication.GetAccountStoreMappings().SingleAsync()
            mapping.IsDefaultAccountStore.ShouldBeTrue()
            mapping.IsDefaultGroupStore.ShouldBeFalse()

            ' Clean up
            Dim result = Await createdApplication.DeleteAsync()
            result.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Setting_unmapped_group_to_default_account_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier} Setting Existing AccountStore Default Test Application", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim group = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)
            Await createdApplication.SetDefaultAccountStoreAsync(group)

            Dim mapping = Await createdApplication.GetAccountStoreMappings().SingleAsync()
            mapping.IsDefaultAccountStore.ShouldBeTrue()
            mapping.IsDefaultGroupStore.ShouldBeFalse()

            ' Clean up
            Dim result = Await createdApplication.DeleteAsync()
            result.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Setting_mapped_directory_to_default_group_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier} Setting Existing Directory AccountStore Default Group Test Application", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim directory = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            Dim mapping = Await createdApplication.AddAccountStoreAsync(directory)

            Await createdApplication.SetDefaultGroupStoreAsync(directory)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeTrue()

            ' Clean up
            Dim result = Await createdApplication.DeleteAsync()
            result.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Setting_unmapped_directory_to_default_group_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier} Setting Existing AccountStore Default Group Test Application", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim directory = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            Await createdApplication.SetDefaultGroupStoreAsync(directory)

            Dim mapping = Await createdApplication.GetAccountStoreMappings().SingleAsync()
            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeTrue()

            ' Clean up
            Dim result = Await createdApplication.DeleteAsync()
            result.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Setting_group_group_store_throws(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier} Setting Group as GroupStore", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim group = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)

            ' If this errors, the server-side API behavior has changed.
            Should.[Throw](Of Exception)(Async Function()
                                             Await createdApplication.SetDefaultGroupStoreAsync(group)
                                         End Function)

            ' Clean up
            Dim result = Await createdApplication.DeleteAsync()
            result.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_directory_as_account_store_by_href(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Directory By Href Test Application", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim mapping = Await createdApplication.AddAccountStoreAsync(Me.fixture.PrimaryDirectoryHref)

            Dim result1 = Await mapping.GetAccountStoreAsync()
            result1.Href.ShouldBe(Me.fixture.PrimaryDirectoryHref)
            Dim result2 = Await mapping.GetApplicationAsync()
            result2.Href.ShouldBe(createdApplication.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            Dim result3 = Await createdApplication.DeleteAsync()
            result3.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_group_as_account_store_by_href(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Group Test By Href Application", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim mapping = Await createdApplication.AddAccountStoreAsync(Me.fixture.PrimaryGroupHref)

            Dim result1 = Await mapping.GetAccountStoreAsync()
            result1.Href.ShouldBe(Me.fixture.PrimaryGroupHref)
            Dim result2 = Await mapping.GetApplicationAsync()
            result2.Href.ShouldBe(createdApplication.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            Dim result3 = Await createdApplication.DeleteAsync()
            result3.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_directory_as_account_store_by_name(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Directory By Name Test Application", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim testDirectory = client.Instantiate(Of IDirectory)().SetName($".NET IT {fixture.TestRunIdentifier} Add Directory As AccountStore By Name")
            Await client.CreateDirectoryAsync(testDirectory)
            testDirectory.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedDirectoryHrefs.Add(testDirectory.Href)

            Dim mapping = Await createdApplication.AddAccountStoreAsync($".NET IT {fixture.TestRunIdentifier} Add Directory As AccountStore By Name")

            Dim result1 = Await mapping.GetAccountStoreAsync()
            result1.Href.ShouldBe(testDirectory.Href)
            Dim result2 = Await mapping.GetApplicationAsync()
            result2.Href.ShouldBe(createdApplication.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            Dim result3 = Await createdApplication.DeleteAsync()
            result3.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)

            Dim result4 = Await testDirectory.DeleteAsync()
            result4.ShouldBeTrue()
            Me.fixture.CreatedDirectoryHrefs.Remove(testDirectory.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_group_as_account_store_by_name(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Group By Name Test Application", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Needs to have a default GroupStore
            Dim mapping = Await createdApplication.AddAccountStoreAsync(Me.fixture.PrimaryDirectoryHref)
            mapping.SetDefaultGroupStore(True)
            Await mapping.SaveAsync()

            Dim testGroup = client.Instantiate(Of IGroup)().SetName($".NET IT {fixture.TestRunIdentifier} Add Group As AccountStore By Name")
            Await createdApplication.CreateGroupAsync(testGroup)
            testGroup.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedGroupHrefs.Add(testGroup.Href)

            Dim newMapping = Await createdApplication.AddAccountStoreAsync($".NET IT {fixture.TestRunIdentifier} Add Group As AccountStore By Name")

            Dim result1 = Await newMapping.GetAccountStoreAsync()
            result1.Href.ShouldBe(testGroup.Href)
            Dim result2 = Await newMapping.GetApplicationAsync()
            result2.Href.ShouldBe(createdApplication.Href)

            newMapping.IsDefaultAccountStore.ShouldBeFalse()
            newMapping.IsDefaultGroupStore.ShouldBeFalse()
            newMapping.ListIndex.ShouldBe(1)

            ' Clean up
            Dim result3 = Await createdApplication.DeleteAsync()
            result3.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)

            Dim result4 = Await testGroup.DeleteAsync()
            result4.ShouldBeTrue()
            Me.fixture.CreatedGroupHrefs.Remove(testGroup.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_directory_as_account_store_by_query(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Directory By Query Test Application", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim directoryName = (Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)).Name
            Dim mapping = Await createdApplication.AddAccountStoreAsync(Of IDirectory)(Function(dirs) dirs.Where(Function(d) d.Name.EndsWith(directoryName.Substring(1))))

            Dim result1 = (Await mapping.GetAccountStoreAsync())
            result1.Href.ShouldBe(Me.fixture.PrimaryDirectoryHref)
            Dim result2 = (Await mapping.GetApplicationAsync())
            result2.Href.ShouldBe(createdApplication.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            Dim result3 = (Await createdApplication.DeleteAsync())
            result3.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_group_as_account_store_by_query(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Group By Query Test Application", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim groupName = (Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)).Name
            Dim mapping = Await createdApplication.AddAccountStoreAsync(Of IGroup)(Function(groups) groups.Where(Function(g) g.Name.EndsWith(groupName.Substring(1))))

            Dim result1 = (Await mapping.GetAccountStoreAsync())
            result1.Href.ShouldBe(Me.fixture.PrimaryGroupHref)
            Dim result2 = (Await mapping.GetApplicationAsync())
            result2.Href.ShouldBe(createdApplication.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            Dim result3 = (Await createdApplication.DeleteAsync())
            result3.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_directory_as_account_store_by_query_throws_for_multiple_results(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Directory By Query Throws Test Application", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Should.[Throw](Of Exception)(Async Function()
                                             Dim mapping = Await createdApplication.AddAccountStoreAsync(Of IDirectory)(Function(allDirs) allDirs)
                                         End Function)

            ' Clean up
            Dim result = (Await createdApplication.DeleteAsync())
            result.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_group_as_account_store_by_query_throws_for_multiple_results(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Group By Query Throws Test Application", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim dummyGroup = client.Instantiate(Of IGroup)().SetName($".NET IT {fixture.TestRunIdentifier} Dummy Test Group for Adding Multiple Groups as AccountStore")
            Dim primaryDirectory = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            Await primaryDirectory.CreateGroupAsync(dummyGroup)
            dummyGroup.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedGroupHrefs.Add(dummyGroup.Href)

            Should.[Throw](Of Exception)(Async Function()
                                             Dim mapping = Await createdApplication.AddAccountStoreAsync(Of IGroup)(Function(allGroups) allGroups)
                                         End Function)

            ' Clean up
            Dim result1 = (Await dummyGroup.DeleteAsync())
            result1.ShouldBeTrue()
            Me.fixture.CreatedGroupHrefs.Remove(dummyGroup.Href)

            Dim result2 = (Await createdApplication.DeleteAsync())
            result2.ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function
    End Class
End Namespace