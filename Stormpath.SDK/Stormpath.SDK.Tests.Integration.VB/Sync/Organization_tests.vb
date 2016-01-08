' <copyright file="Organization_tests.vb" company="Stormpath, Inc.">
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
Imports Stormpath.SDK.Directory
Imports Stormpath.SDK.Error
Imports Stormpath.SDK.Group
Imports Stormpath.SDK.Organization
Imports Stormpath.SDK.Sync
Imports Stormpath.SDK.Tests.Common.Integration
Imports Xunit

Namespace Sync
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class Organization_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_tenant_organizations(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()
            Dim orgs = tenant.GetOrganizations().Synchronously().ToList()

            orgs.Count.ShouldBeGreaterThan(0)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_organization_tenant(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim org = client.GetResource(Of IOrganization)(Me.fixture.PrimaryOrganizationHref)

            ' Verify data from IntegrationTestData
            Dim tenantHref = org.GetTenant().Href
            tenantHref.ShouldBe(Me.fixture.TenantHref)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_organization_accounts(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim org = client.GetResource(Of IOrganization)(Me.fixture.PrimaryOrganizationHref)

            org.GetAccounts().Synchronously().Count().ShouldBeGreaterThan(0)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_organization_groups(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim org = client.GetResource(Of IOrganization)(Me.fixture.PrimaryOrganizationHref)

            org.GetGroups().Synchronously().Count().ShouldBeGreaterThan(0)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Saving_organization(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim trek = client.Instantiate(Of IOrganization)() _
                .SetName($"Star Trek (.NET ITs {fixture.TestRunIdentifier}-{clientBuilder.Name} - SyncVB)") _
                .SetNameKey($"dotnet-test-trek-{fixture.TestRunIdentifier}-sync-vb") _
                .SetDescription("Star Trek (Sync)") _
                .SetStatus(OrganizationStatus.Enabled)

            client.CreateOrganization(trek)
            Me.fixture.CreatedOrganizationHrefs.Add(trek.Href)

            trek.SetStatus(OrganizationStatus.Disabled)
            Dim result = trek.Save()

            result.Status.ShouldBe(OrganizationStatus.Disabled)

            ' Clean up
            trek.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(trek.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Updating_organization_custom_data(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim org = client.GetResource(Of IOrganization)(Me.fixture.PrimaryOrganizationHref)

            org.CustomData.Put("multiTenant", True)
            org.CustomData.Put("someInts", 12345)
            org.Save()

            Dim customData = org.GetCustomData()

            CBool(customData("multiTenant")).ShouldBe(True)
            CInt(customData("someInts")).ShouldBe(12345)

            ' Clean up
            customData.Delete().ShouldBeTrue()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Saving_with_response_options(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim bsg = client.Instantiate(Of IOrganization)() _
                .SetName($"Battlestar Galactica (.NET ITs {fixture.TestRunIdentifier}-{clientBuilder.Name} - SyncVB)") _
                .SetNameKey($"dotnet-test-bsg-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb") _
                .SetDescription("BSG (Sync)") _
                .SetStatus(OrganizationStatus.Enabled)

            client.CreateOrganization(bsg)
            Me.fixture.CreatedOrganizationHrefs.Add(bsg.Href)

            bsg.SetDescription("Battlestar Galactica")
            bsg.Save(Function(response) response.Expand(Function(x) x.GetAccounts(0, 10)))

            ' Clean up
            bsg.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(bsg.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_with_directory(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim name = $"Created Organization 1 (.NET ITs {fixture.TestRunIdentifier}-{clientBuilder.Name} - SyncVB)"
            Dim nameKey = $"dotnet-test1-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb"
            Dim newOrg = client.Instantiate(Of IOrganization)() _
                .SetName(name) _
                .SetNameKey(nameKey) _
                .SetStatus(OrganizationStatus.Enabled)

            Dim directoryName = $"Foobar Created Org 1 Directory-{fixture.TestRunIdentifier}-{clientBuilder.Name} - SyncVB"
            client.CreateOrganization(newOrg, Sub(opt)
                                                  opt.CreateDirectory = True
                                                  opt.DirectoryName = directoryName
                                              End Sub)
            Me.fixture.CreatedOrganizationHrefs.Add(newOrg.Href)
            Dim createdDirectory = TryCast(newOrg.GetDefaultAccountStore(), IDirectory)
            Me.fixture.CreatedDirectoryHrefs.Add(createdDirectory.Href)

            newOrg.Name.ShouldBe(name)
            newOrg.NameKey.ShouldBe(nameKey)
            newOrg.Status.ShouldBe(OrganizationStatus.Enabled)

            createdDirectory.Name.ShouldBe(directoryName)

            ' Clean up
            createdDirectory.Delete().ShouldBeTrue()
            Me.fixture.CreatedDirectoryHrefs.Remove(createdDirectory.Href)

            newOrg.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(newOrg.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_without_directory(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim name = $"Created Organization 2 (.NET ITs {fixture.TestRunIdentifier}-{clientBuilder.Name} - SyncVB)"
            Dim nameKey = $"dotnet-test2-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb"
            Dim newOrg = client.Instantiate(Of IOrganization)() _
                .SetName(name) _
                .SetNameKey(nameKey) _
                .SetStatus(OrganizationStatus.Disabled)

            client.CreateOrganization(newOrg, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))
            Me.fixture.CreatedOrganizationHrefs.Add(newOrg.Href)

            newOrg.Name.ShouldBe(name)
            newOrg.NameKey.ShouldBe(nameKey)
            newOrg.Status.ShouldBe(OrganizationStatus.Disabled)

            Dim createdDirectory = newOrg.GetDefaultAccountStore()
            createdDirectory.ShouldBeNull()

            ' Clean up
            newOrg.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(newOrg.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_with_response_options(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim name = $"Created Organization 3 (.NET ITs {fixture.TestRunIdentifier}-{clientBuilder.Name} - SyncVB)"
            Dim nameKey = $"dotnet-test3-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb"
            Dim newOrg = client.Instantiate(Of IOrganization)() _
                .SetName(name) _
                .SetNameKey(nameKey) _
                .SetStatus(OrganizationStatus.Disabled)

            client.CreateOrganization(newOrg, Sub(opt)
                                                  opt.CreateDirectory = False
                                                  opt.ResponseOptions.Expand(Function(x) x.GetCustomData())
                                              End Sub)
            Me.fixture.CreatedOrganizationHrefs.Add(newOrg.Href)

            newOrg.Name.ShouldBe(name)
            newOrg.NameKey.ShouldBe(nameKey)
            newOrg.Status.ShouldBe(OrganizationStatus.Disabled)

            ' Clean up
            newOrg.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(newOrg.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_account_store_mapping(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directly Test Organization - SyncVB") _
                .SetNameKey($"dotnet-test4-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb")
            tenant.CreateOrganization(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim directory As IAccountStore = client.GetResource(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)

            Dim mapping = client.Instantiate(Of IOrganizationAccountStoreMapping)()
            mapping.SetAccountStore(directory)
            mapping.SetListIndex(500)
            createdOrganization.CreateAccountStoreMapping(mapping)

            mapping.GetAccountStore().Href.ShouldBe(directory.Href)
            mapping.GetOrganization().Href.ShouldBe(createdOrganization.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            createdOrganization.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_second_account_store_mapping_at_zeroth_index(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding Two AccountStores Directly Test Organization - SyncVB") _
                .SetNameKey($"dotnet-test5-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb")
            tenant.CreateOrganization(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim mapping1 = createdOrganization.AddAccountStore(Me.fixture.PrimaryDirectoryHref)

            Dim group = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref)
            Dim mapping2 = client.Instantiate(Of IOrganizationAccountStoreMapping)()
            mapping2.SetAccountStore(group)
            mapping2.SetListIndex(0)
            createdOrganization.CreateAccountStoreMapping(mapping2)

            mapping2.ListIndex.ShouldBe(0)
            mapping1.ListIndex.ShouldBe(1)

            ' Clean up
            createdOrganization.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_directory_as_account_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directory Test Organization - SyncVB") _
                .SetNameKey($"dotnet-test6-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb")
            tenant.CreateOrganization(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim directory = client.GetResource(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            Dim mapping = createdOrganization.AddAccountStore(directory)

            mapping.GetAccountStore().Href.ShouldBe(directory.Href)
            mapping.GetOrganization().Href.ShouldBe(createdOrganization.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            createdOrganization.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_group_as_account_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Group Test Organization - SyncVB") _
                .SetNameKey($"dotnet-test7-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb")
            tenant.CreateOrganization(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim group = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref)
            Dim mapping = createdOrganization.AddAccountStore(group)

            mapping.GetAccountStore().Href.ShouldBe(group.Href)
            mapping.GetOrganization().Href.ShouldBe(createdOrganization.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            createdOrganization.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Setting_mapped_directory_to_default_account_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Setting Existing Directory AccountStore Default Test Organization - SyncVB") _
                .SetNameKey($"dotnet-test8-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb")
            tenant.CreateOrganization(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim directory = client.GetResource(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            Dim mapping = createdOrganization.AddAccountStore(directory)

            createdOrganization.SetDefaultAccountStore(directory)

            mapping.IsDefaultAccountStore.ShouldBeTrue()
            mapping.IsDefaultGroupStore.ShouldBeFalse()

            ' Clean up
            createdOrganization.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Setting_mapped_group_to_default_account_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Setting Existing Group AccountStore Default Test Organization - SyncVB") _
                .SetNameKey($"dotnet-test9-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb")
            tenant.CreateOrganization(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim group = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref)
            Dim mapping = createdOrganization.AddAccountStore(group)

            createdOrganization.SetDefaultAccountStore(group)

            mapping.IsDefaultAccountStore.ShouldBeTrue()
            mapping.IsDefaultGroupStore.ShouldBeFalse()

            ' Clean up
            createdOrganization.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Setting_unmapped_directory_to_default_account_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Setting New Directory AccountStore Default Test Organization - SyncVB") _
                .SetNameKey($"dotnet-test10-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb")
            tenant.CreateOrganization(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim directory = client.GetResource(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            createdOrganization.SetDefaultAccountStore(directory)

            Dim mapping = createdOrganization.GetAccountStoreMappings().Synchronously().[Single]()
            mapping.IsDefaultAccountStore.ShouldBeTrue()
            mapping.IsDefaultGroupStore.ShouldBeFalse()

            ' Clean up
            createdOrganization.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Setting_unmapped_group_to_default_account_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Setting New Group AccountStore Default Test Organization - SyncVB") _
                .SetNameKey($"dotnet-test11-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb")
            tenant.CreateOrganization(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim group = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref)
            createdOrganization.SetDefaultAccountStore(group)

            Dim mapping = createdOrganization.GetAccountStoreMappings().Synchronously().[Single]()
            mapping.IsDefaultAccountStore.ShouldBeTrue()
            mapping.IsDefaultGroupStore.ShouldBeFalse()

            ' Clean up
            createdOrganization.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Setting_mapped_directory_to_default_group_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Setting Existing Directory GroupStore Default Test Organization - SyncVB") _
                .SetNameKey($"dotnet-test12-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb")
            tenant.CreateOrganization(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim directory = client.GetResource(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            Dim mapping = createdOrganization.AddAccountStore(directory)

            createdOrganization.SetDefaultGroupStore(directory)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeTrue()

            ' Clean up
            createdOrganization.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Setting_unmapped_directory_to_default_group_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Setting Existing Directory GroupStore Default Test Organization - SyncVB") _
                .SetNameKey($"dotnet-test13-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb")
            tenant.CreateOrganization(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim directory = client.GetResource(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            createdOrganization.SetDefaultGroupStore(directory)

            Dim mapping = createdOrganization.GetAccountStoreMappings().Synchronously().[Single]()
            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeTrue()

            ' Clean up
            createdOrganization.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Setting_group_group_store_throws(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Setting Group as GroupStore Test Organization - SyncVB") _
                .SetNameKey($"dotnet-test14-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb")
            tenant.CreateOrganization(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim group = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref)

            ' If this errors, the server-side API behavior has changed.
            Should.[Throw](Of ResourceException)(Sub()
                                                     createdOrganization.SetDefaultGroupStore(group)
                                                 End Sub)

            ' Clean up
            createdOrganization.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_directory_as_account_store_by_href(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directory By Href Test Organization - SyncVB") _
                .SetNameKey($"dotnet-test15-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb")
            tenant.CreateOrganization(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim mapping = createdOrganization.AddAccountStore(Me.fixture.PrimaryDirectoryHref)

            mapping.GetAccountStore().Href.ShouldBe(Me.fixture.PrimaryDirectoryHref)
            mapping.GetOrganization().Href.ShouldBe(createdOrganization.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            createdOrganization.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_group_as_account_store_by_href(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Group By Href Test Organization - SyncVB") _
                .SetNameKey($"dotnet-test16-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb")
            tenant.CreateOrganization(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim mapping = createdOrganization.AddAccountStore(Me.fixture.PrimaryGroupHref)

            mapping.GetAccountStore().Href.ShouldBe(Me.fixture.PrimaryGroupHref)
            mapping.GetOrganization().Href.ShouldBe(createdOrganization.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            createdOrganization.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_directory_as_account_store_by_name(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directory By Name Test Organization - SyncVB") _
                .SetNameKey($"dotnet-test17-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb")
            tenant.CreateOrganization(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim directoryName = ".NET IT Organization Test {fixture.TestRunIdentifier}-{clientBuilder.Name} Add Directory As AccountStore By Name - SyncVB"
            Dim testDirectory = client.Instantiate(Of IDirectory)() _
                .SetName(directoryName)
            client.CreateDirectory(testDirectory)
            testDirectory.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedDirectoryHrefs.Add(testDirectory.Href)

            Dim mapping = createdOrganization.AddAccountStore(directoryName)

            mapping.GetAccountStore().Href.ShouldBe(testDirectory.Href)
            mapping.GetOrganization().Href.ShouldBe(createdOrganization.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            createdOrganization.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)

            testDirectory.Delete().ShouldBeTrue()
            Me.fixture.CreatedDirectoryHrefs.Remove(testDirectory.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_group_as_account_store_by_name(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Group By Name Test Organization - SyncVB") _
                .SetNameKey($"dotnet-test18-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb")
            tenant.CreateOrganization(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            ' Needs to have a default GroupStore
            Dim mapping = createdOrganization.AddAccountStore(Me.fixture.PrimaryDirectoryHref)
            mapping.SetDefaultGroupStore(True)
            mapping.Save()

            Dim groupName = ".NET IT Organization Test {fixture.TestRunIdentifier}-{clientBuilder.Name} Add Group As AccountStore By Name - SyncVB"
            Dim testGroup = client.Instantiate(Of IGroup)() _
                .SetName(groupName)
            createdOrganization.CreateGroup(testGroup)
            testGroup.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedGroupHrefs.Add(testGroup.Href)

            Dim newMapping = createdOrganization.AddAccountStore(groupName)

            newMapping.GetAccountStore().Href.ShouldBe(testGroup.Href)
            newMapping.GetOrganization().Href.ShouldBe(createdOrganization.Href)

            newMapping.IsDefaultAccountStore.ShouldBeFalse()
            newMapping.IsDefaultGroupStore.ShouldBeFalse()
            newMapping.ListIndex.ShouldBe(1)

            ' Clean up
            createdOrganization.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)

            testGroup.Delete().ShouldBeTrue()
            Me.fixture.CreatedGroupHrefs.Remove(testGroup.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_directory_as_account_store_by_query(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directory By Query Test Organization - SyncVB") _
                .SetNameKey($"dotnet-test19-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb")
            tenant.CreateOrganization(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim directoryName = client.GetResource(Of IDirectory)(Me.fixture.PrimaryDirectoryHref).Name
            Dim mapping = createdOrganization.AddAccountStore(Of IDirectory)(Function(dirs) dirs.Where(Function(d) d.Name.EndsWith(directoryName.Substring(1))))

            mapping.GetAccountStore().Href.ShouldBe(Me.fixture.PrimaryDirectoryHref)
            mapping.GetOrganization().Href.ShouldBe(createdOrganization.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            createdOrganization.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_group_as_account_store_by_query(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Group By Query Test Organization - SyncVB") _
                .SetNameKey($"dotnet-test20-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb")
            tenant.CreateOrganization(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim groupName = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref).Name
            Dim mapping = createdOrganization.AddAccountStore(Of IGroup)(Function(groups) groups.Where(Function(g) g.Name.EndsWith(groupName.Substring(1))))

            mapping.GetAccountStore().Href.ShouldBe(Me.fixture.PrimaryGroupHref)
            mapping.GetOrganization().Href.ShouldBe(createdOrganization.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            createdOrganization.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_directory_as_account_store_by_query_throws_for_multiple_results(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directory By Query Throws Test Organization - SyncVB") _
                .SetNameKey($"dotnet-test21-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb")
            tenant.CreateOrganization(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim dir1 = client.CreateDirectory($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Directory Query Results1", String.Empty, DirectoryStatus.Enabled)
            Dim dir2 = client.CreateDirectory($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Directory Query Results2", String.Empty, DirectoryStatus.Enabled)

            Me.fixture.CreatedDirectoryHrefs.Add(dir1.Href)
            Me.fixture.CreatedDirectoryHrefs.Add(dir2.Href)

            Should.[Throw](Of ArgumentException)(Sub()
                                                     ' Throws because multiple matching results exist
                                                     Dim mapping = createdOrganization.AddAccountStore(Of IDirectory)(Function(dirs) dirs.Where(Function(d) d.Name.StartsWith($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Directory Query Results")))
                                                 End Sub)

            ' Clean up
            dir1.Delete().ShouldBeTrue()
            Me.fixture.CreatedDirectoryHrefs.Remove(dir1.Href)

            dir2.Delete().ShouldBeTrue()
            Me.fixture.CreatedDirectoryHrefs.Remove(dir2.Href)

            createdOrganization.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_group_as_account_store_by_query_throws_for_multiple_results(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Group By Query Throws Test Organization - SyncVB") _
                .SetNameKey($"dotnet-test22-{fixture.TestRunIdentifier}-{clientBuilder.Name}-sync-vb")
            tenant.CreateOrganization(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, True))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim defaultGroupStore = TryCast(createdOrganization.GetDefaultGroupStore(), IDirectory)
            defaultGroupStore.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedDirectoryHrefs.Add(defaultGroupStore.Href)

            Dim group1 = createdOrganization.CreateGroup($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Group Query Results1", String.Empty)
            Dim group2 = createdOrganization.CreateGroup($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Group Query Results2", String.Empty)

            Me.fixture.CreatedGroupHrefs.Add(group1.Href)
            Me.fixture.CreatedGroupHrefs.Add(group2.Href)

            Should.[Throw](Of ArgumentException)(Sub()
                                                     ' Throws because multiple matching results exist
                                                     Dim mapping = createdOrganization.AddAccountStore(Of IGroup)(Function(groups) groups.Where(Function(x) x.Name.StartsWith($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Group Query Results")))
                                                 End Sub)

            ' Clean up
            group1.Delete().ShouldBeTrue()
            Me.fixture.CreatedGroupHrefs.Remove(group1.Href)

            group2.Delete().ShouldBeTrue()
            Me.fixture.CreatedGroupHrefs.Remove(group2.Href)

            defaultGroupStore.Delete().ShouldBeTrue()
            Me.fixture.CreatedDirectoryHrefs.Remove(defaultGroupStore.Href)

            createdOrganization.Delete().ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Sub
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function
    End Class
End Namespace