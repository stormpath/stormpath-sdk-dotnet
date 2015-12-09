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
Imports Stormpath.SDK.Sync
Imports Stormpath.SDK.Tests.Common.Integration
Imports Xunit

Namespace Stormpath.SDK.Tests.Integration.VB.Sync
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class Application_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_tenant_applications(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()
            Dim applications = tenant.GetApplications().Synchronously().ToList()

            applications.Count.ShouldNotBe(0)
            applications.Any(Function(app) app.Status = ApplicationStatus.Enabled).ShouldBeTrue()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_application_tenant(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            ' Verify data from IntegrationTestData
            Dim tenantHref = application.GetTenant().Href
            tenantHref.ShouldBe(Me.fixture.TenantHref)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_application_without_directory(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim newApplicationName = $".NET IT {fixture.TestRunIdentifier} Application #2"
            Dim createdApplication = tenant.CreateApplication(newApplicationName, createDirectory:=False)

            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)
            createdApplication.Name.ShouldBe(newApplicationName)
            createdApplication.Status.ShouldBe(ApplicationStatus.Enabled)

            Dim defaultAccountStore = createdApplication.GetDefaultAccountStore()
            If Not String.IsNullOrEmpty(defaultAccountStore?.Href) Then
                Me.fixture.CreatedDirectoryHrefs.Add(defaultAccountStore.Href)
            End If

            defaultAccountStore.ShouldBeNull()
            ' no auto-created directory = no default account store
            ' Clean up
            createdApplication.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_default_account_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim defaultAccountStore = app.GetDefaultAccountStore()
            defaultAccountStore.ShouldNotBeNull()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_default_group_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim defaultGroupStore = app.GetDefaultGroupStore()
            defaultGroupStore.ShouldNotBeNull()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_application_with_response_options(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim newApp = client.Instantiate(Of IApplication)() _
                .SetName($".NET IT {fixture.TestRunIdentifier} Application #3 - Sync")

            tenant.CreateApplication(newApp, Function(opt) opt.ResponseOptions.Expand(Function(x) x.GetCustomData()))

            newApp.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(newApp.Href)

            ' Clean up
            newApp.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(newApp.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Updating_application(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim application = tenant.GetApplications() _
                .Synchronously() _
                .Where(Function(app) app.Name.StartsWith($".NET IT (disabled) {fixture.TestRunIdentifier}")) _
                .[Single]()

            application.SetDescription("The Battle of Yavin - Victory!")
            Dim saveResult = application.Save()

            saveResult.Description.ShouldBe("The Battle of Yavin - Victory!")
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Saving_with_response_options(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim application = tenant.GetApplications() _
                .Synchronously() _
                .Where(Function(app) app.Name.StartsWith($".NET IT (disabled) {fixture.TestRunIdentifier}")) _
                .[Single]()

            application.SetStatus(ApplicationStatus.Disabled)
            application.Save(Function(response) response.Expand(Function(x) x.GetAccounts()))
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Searching_by_name(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim application = tenant.GetApplications() _
                .Synchronously() _
                .Where(Function(app) app.Name.StartsWith($".NET IT (primary) {fixture.TestRunIdentifier}")) _
                .[Single]()

            application.Description.ShouldBe("The Battle of Endor")
            application.Status.ShouldBe(ApplicationStatus.Enabled)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Searching_by_description(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim applications = tenant.GetApplications() _
                .Synchronously() _
                .Where(Function(app) app.Description = "The Battle Of Endor").ToList()

            applications.Any(Function(app) app.Name.StartsWith($".NET IT (primary) {fixture.TestRunIdentifier}")).ShouldBeTrue()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Searching_by_status(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim applications = tenant.GetApplications() _
                .Synchronously() _
                .Where(Function(app) app.Status = ApplicationStatus.Disabled).ToList()

            applications.Any(Function(app) app.Name.StartsWith($".NET IT (disabled) {fixture.TestRunIdentifier}")).ShouldBeTrue()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Reset_password_for_valid_account(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim token = application.SendPasswordResetEmail("vader@galacticempire.co")

            Dim validTokenResponse = application.VerifyPasswordResetToken(token.GetValue())
            validTokenResponse.Email.ShouldBe("vader@galacticempire.co")

            Dim resetPasswordResponse = application.ResetPassword(token.GetValue(), "Ifindyourlackofsecuritydisturbing!1")
            resetPasswordResponse.Email.ShouldBe("vader@galacticempire.co")
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Reset_password_with_encoded_jwt(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim token = application.SendPasswordResetEmail("vader@galacticempire.co")

            ' When reset tokens are sent via email, the JWT . separator is encoded as %2E
            Dim encodedToken = token.GetValue().Replace(".", "%2E")

            Dim validTokenResponse = application.VerifyPasswordResetToken(encodedToken)
            validTokenResponse.Email.ShouldBe("vader@galacticempire.co")

            Dim resetPasswordResponse = application.ResetPassword(encodedToken, "Ifindyourlackofsecuritydisturbing!1")
            resetPasswordResponse.Email.ShouldBe("vader@galacticempire.co")
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Reset_password_for_account_in_account_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim accountStore = application.GetDefaultAccountStore()

            Dim token = application.SendPasswordResetEmail("vader@galacticempire.co", accountStore)

            Dim validTokenResponse = application.VerifyPasswordResetToken(token.GetValue())
            validTokenResponse.Email.ShouldBe("vader@galacticempire.co")

            Dim resetPasswordResponse = application.ResetPassword(token.GetValue(), "Ifindyourlackofsecuritydisturbing!1")
            resetPasswordResponse.Email.ShouldBe("vader@galacticempire.co")
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_account_store_mapping(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Directly Test Application (Sync)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim directory As IAccountStore = client.GetResource(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)

            Dim mapping = client.Instantiate(Of IAccountStoreMapping)()
            mapping.SetAccountStore(directory)
            mapping.SetListIndex(500)
            createdApplication.CreateAccountStoreMapping(mapping)

            mapping.GetAccountStore().Href.ShouldBe(directory.Href)
            mapping.GetApplication().Href.ShouldBe(createdApplication.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            createdApplication.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_second_account_store_mapping_at_zeroth_index(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier} Adding Two AccountStores Directly Test Application (Sync)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim mapping1 = createdApplication.AddAccountStore(Me.fixture.PrimaryDirectoryHref)

            Dim group = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref)
            Dim mapping2 = client.Instantiate(Of IAccountStoreMapping)()
            mapping2.SetAccountStore(group)
            mapping2.SetListIndex(0)
            createdApplication.CreateAccountStoreMapping(mapping2)

            mapping2.ListIndex.ShouldBe(0)
            mapping1.ListIndex.ShouldBe(1)

            ' Clean up
            createdApplication.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_directory_as_account_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Directory Test Application (Sync)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim directory = client.GetResource(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            Dim mapping = createdApplication.AddAccountStore(directory)

            mapping.GetAccountStore().Href.ShouldBe(directory.Href)
            mapping.GetApplication().Href.ShouldBe(createdApplication.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            createdApplication.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_group_as_account_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Group Test Application (Sync)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim group = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref)
            Dim mapping = createdApplication.AddAccountStore(group)

            mapping.GetAccountStore().Href.ShouldBe(group.Href)
            mapping.GetApplication().Href.ShouldBe(createdApplication.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            createdApplication.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Setting_mapped_directory_to_default_account_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier} Setting Existing Directory AccountStore Default Test Application (Sync)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim directory = client.GetResource(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            Dim mapping = createdApplication.AddAccountStore(directory)

            createdApplication.SetDefaultAccountStore(directory)

            mapping.IsDefaultAccountStore.ShouldBeTrue()
            mapping.IsDefaultGroupStore.ShouldBeFalse()

            ' Clean up
            createdApplication.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Setting_mapped_group_to_default_account_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier} Setting Existing Group AccountStore Default Test Application (Sync)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim group = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref)
            Dim mapping = createdApplication.AddAccountStore(group)

            createdApplication.SetDefaultAccountStore(group)

            mapping.IsDefaultAccountStore.ShouldBeTrue()
            mapping.IsDefaultGroupStore.ShouldBeFalse()

            ' Clean up
            createdApplication.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Setting_unmapped_directory_to_default_account_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier} Setting Existing AccountStore Default Test Application (Sync)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim directory = client.GetResource(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            createdApplication.SetDefaultAccountStore(directory)

            Dim mapping = createdApplication.GetAccountStoreMappings().Synchronously().[Single]()
            mapping.IsDefaultAccountStore.ShouldBeTrue()
            mapping.IsDefaultGroupStore.ShouldBeFalse()

            ' Clean up
            createdApplication.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Setting_unmapped_group_to_default_account_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier} Setting Existing AccountStore Default Test Application (Sync)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim group = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref)
            createdApplication.SetDefaultAccountStore(group)

            Dim mapping = createdApplication.GetAccountStoreMappings().Synchronously().[Single]()
            mapping.IsDefaultAccountStore.ShouldBeTrue()
            mapping.IsDefaultGroupStore.ShouldBeFalse()

            ' Clean up
            createdApplication.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Setting_mapped_directory_to_default_group_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier} Setting Existing Directory AccountStore Default Group Test Application (Sync)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim directory = client.GetResource(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            Dim mapping = createdApplication.AddAccountStore(directory)

            createdApplication.SetDefaultGroupStore(directory)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeTrue()

            ' Clean up
            createdApplication.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Setting_unmapped_directory_to_default_group_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier} Setting Existing AccountStore Default Group Test Application (Sync)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim directory = client.GetResource(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            createdApplication.SetDefaultGroupStore(directory)

            Dim mapping = createdApplication.GetAccountStoreMappings().Synchronously().[Single]()
            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeTrue()

            ' Clean up
            createdApplication.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Setting_group_group_store_throws(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier} Setting Group as GroupStore (Sync)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim group = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref)

            ' If this errors, the server-side API behavior has changed.
            Should.[Throw](Of Exception)(Sub()
                                             createdApplication.SetDefaultGroupStore(group)
                                         End Sub)

            ' Clean up
            createdApplication.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_directory_as_account_store_by_href(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Directory By Href Test Application (Sync)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim mapping = createdApplication.AddAccountStore(Me.fixture.PrimaryDirectoryHref)

            mapping.GetAccountStore().Href.ShouldBe(Me.fixture.PrimaryDirectoryHref)
            mapping.GetApplication().Href.ShouldBe(createdApplication.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            createdApplication.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_group_as_account_store_by_href(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Group Test By Href Application (Sync)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim mapping = createdApplication.AddAccountStore(Me.fixture.PrimaryGroupHref)

            mapping.GetAccountStore().Href.ShouldBe(Me.fixture.PrimaryGroupHref)
            mapping.GetApplication().Href.ShouldBe(createdApplication.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            createdApplication.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_directory_as_account_store_by_name(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Directory By Name Test Application (Sync)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim testDirectory = client.Instantiate(Of IDirectory)().SetName($".NET IT {fixture.TestRunIdentifier} Add Directory As AccountStore By Name")
            client.CreateDirectory(testDirectory)
            testDirectory.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedDirectoryHrefs.Add(testDirectory.Href)

            Dim mapping = createdApplication.AddAccountStore($".NET IT {fixture.TestRunIdentifier} Add Directory As AccountStore By Name")

            mapping.GetAccountStore().Href.ShouldBe(testDirectory.Href)
            mapping.GetApplication().Href.ShouldBe(createdApplication.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            createdApplication.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)

            testDirectory.Delete().ShouldBeTrue()
            Me.fixture.CreatedDirectoryHrefs.Remove(testDirectory.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_group_as_account_store_by_name(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Group By Name Test Application (Sync)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Needs to have a default GroupStore
            Dim mapping = createdApplication.AddAccountStore(Me.fixture.PrimaryDirectoryHref)
            mapping.SetDefaultGroupStore(True)
            mapping.Save()

            Dim testGroup = client.Instantiate(Of IGroup)().SetName($".NET IT {fixture.TestRunIdentifier} Add Group As AccountStore By Name (Sync)")
            createdApplication.CreateGroup(testGroup)
            testGroup.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedGroupHrefs.Add(testGroup.Href)

            Dim newMapping = createdApplication.AddAccountStore($".NET IT {fixture.TestRunIdentifier} Add Group As AccountStore By Name (Sync)")

            newMapping.GetAccountStore().Href.ShouldBe(testGroup.Href)
            newMapping.GetApplication().Href.ShouldBe(createdApplication.Href)

            newMapping.IsDefaultAccountStore.ShouldBeFalse()
            newMapping.IsDefaultGroupStore.ShouldBeFalse()
            newMapping.ListIndex.ShouldBe(1)

            ' Clean up
            createdApplication.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)

            testGroup.Delete().ShouldBeTrue()
            Me.fixture.CreatedGroupHrefs.Remove(testGroup.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_directory_as_account_store_by_query(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Directory By Query Test Application (Sync)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim directoryName = client.GetResource(Of IDirectory)(Me.fixture.PrimaryDirectoryHref).Name
            Dim mapping = createdApplication.AddAccountStore(Of IDirectory)(Function(dirs) dirs.Where(Function(d) d.Name.EndsWith(directoryName.Substring(1))))

            mapping.GetAccountStore().Href.ShouldBe(Me.fixture.PrimaryDirectoryHref)
            mapping.GetApplication().Href.ShouldBe(createdApplication.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            createdApplication.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_group_as_account_store_by_query(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Group By Query Test Application (Sync)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim groupName = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref).Name
            Dim mapping = createdApplication.AddAccountStore(Of IGroup)(Function(groups) groups.Where(Function(g) g.Name.EndsWith(groupName.Substring(1))))

            mapping.GetAccountStore().Href.ShouldBe(Me.fixture.PrimaryGroupHref)
            mapping.GetApplication().Href.ShouldBe(createdApplication.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            createdApplication.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_directory_as_account_store_by_query_throws_for_multiple_results(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Directory By Query Throws Test Application (Sync)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Should.[Throw](Of Exception)(Sub()
                                             Dim mapping = createdApplication.AddAccountStore(Of IDirectory)(Function(allDirs) allDirs)
                                         End Sub)

            ' Clean up
            createdApplication.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_group_as_account_store_by_query_throws_for_multiple_results(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier} Adding AccountStore Group By Query Throws Test Application (Sync)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            Dim dummyGroup = client.Instantiate(Of IGroup)().SetName($".NET IT {fixture.TestRunIdentifier} Dummy Test Group for Adding Multiple Groups as AccountStore (Sync)")
            Dim primaryDirectory = client.GetResource(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            primaryDirectory.CreateGroup(dummyGroup)
            dummyGroup.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedGroupHrefs.Add(dummyGroup.Href)

            Should.[Throw](Of Exception)(Sub()
                                             Dim mapping = createdApplication.AddAccountStore(Of IGroup)(Function(allGroups) allGroups)
                                         End Sub)

            ' Clean up
            dummyGroup.Delete().ShouldBeTrue()
            Me.fixture.CreatedGroupHrefs.Remove(dummyGroup.Href)

            createdApplication.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub
    End Class
End Namespace