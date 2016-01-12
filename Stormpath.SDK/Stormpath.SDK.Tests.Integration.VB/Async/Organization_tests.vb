
' <copyright file="Organization_tests.cs" company="Stormpath, Inc.">
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
Imports Stormpath.SDK.Tests.Common.Integration
Imports Xunit

Namespace Async
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class Organization_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_tenant_organizations(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()
            Dim orgs = Await tenant.GetOrganizations().ToListAsync()

            orgs.Count.ShouldBeGreaterThan(0)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_organization_tenant(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim org = Await client.GetResourceAsync(Of IOrganization)(Me.fixture.PrimaryOrganizationHref)

            ' Verify data from IntegrationTestData
            Dim tenantHref = (Await org.GetTenantAsync()).Href
            tenantHref.ShouldBe(Me.fixture.TenantHref)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_organization_accounts(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim org = Await client.GetResourceAsync(Of IOrganization)(Me.fixture.PrimaryOrganizationHref)

            Call (Await org.GetAccounts().CountAsync()).ShouldBeGreaterThan(0)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_organization_groups(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim org = Await client.GetResourceAsync(Of IOrganization)(Me.fixture.PrimaryOrganizationHref)

            Call (Await org.GetGroups().CountAsync()).ShouldBeGreaterThan(0)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Saving_organization(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim trek = client.Instantiate(Of IOrganization)() _
                .SetName($"Star Trek (.NET ITs {fixture.TestRunIdentifier}-{clientBuilder.Name} - VB)") _
                .SetNameKey($"dotnet-test-trek-{fixture.TestRunIdentifier}-vb") _
                .SetDescription("Star Trek") _
                .SetStatus(OrganizationStatus.Enabled)

            Await client.CreateOrganizationAsync(trek)
            Me.fixture.CreatedOrganizationHrefs.Add(trek.Href)

            trek.SetStatus(OrganizationStatus.Disabled)
            Dim result = Await trek.SaveAsync()

            result.Status.ShouldBe(OrganizationStatus.Disabled)

            ' Clean up
            Call (Await trek.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(trek.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Updating_organization_custom_data(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim org = Await client.GetResourceAsync(Of IOrganization)(Me.fixture.PrimaryOrganizationHref)

            org.CustomData.Put("multiTenant", True)
            org.CustomData.Put("someInts", 12345)
            Await org.SaveAsync()

            Dim customData = Await org.GetCustomDataAsync()

            CBool(customData("multiTenant")).ShouldBe(True)
            CInt(customData("someInts")).ShouldBe(12345)

            ' Clean up
            Call (Await customData.DeleteAsync()).ShouldBeTrue()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Saving_with_response_options(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim bsg = client.Instantiate(Of IOrganization)() _
                .SetName($"Battlestar Galactica (.NET ITs {fixture.TestRunIdentifier}-{clientBuilder.Name} - VB)") _
                .SetNameKey($"dotnet-test-bsg-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb") _
                .SetDescription("BSG") _
                .SetStatus(OrganizationStatus.Enabled)

            Await client.CreateOrganizationAsync(bsg)
            Me.fixture.CreatedOrganizationHrefs.Add(bsg.Href)

            bsg.SetDescription("Battlestar Galactica")
            Await bsg.SaveAsync(Function(response) response.Expand(Function(x) x.GetAccounts(0, 10)))

            ' Clean up
            Call (Await bsg.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(bsg.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Creating_with_directory(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim name = $"Created Organization 1 (.NET ITs {fixture.TestRunIdentifier}-{clientBuilder.Name} - VB)"
            Dim nameKey = $"dotnet-test1-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb"
            Dim newOrg = client.Instantiate(Of IOrganization)() _
                .SetName(name) _
                .SetNameKey(nameKey) _
                .SetStatus(OrganizationStatus.Enabled)

            Dim directoryName = $"Foobar Created Org 1 Directory-{fixture.TestRunIdentifier}-{clientBuilder.Name} - VB"
            Await client.CreateOrganizationAsync(newOrg, Sub(opt)
                                                             opt.CreateDirectory = True
                                                             opt.DirectoryName = directoryName
                                                         End Sub)
            Me.fixture.CreatedOrganizationHrefs.Add(newOrg.Href)
            Dim createdDirectory = TryCast(Await newOrg.GetDefaultAccountStoreAsync(), IDirectory)
            Me.fixture.CreatedDirectoryHrefs.Add(createdDirectory.Href)

            newOrg.Name.ShouldBe(name)
            newOrg.NameKey.ShouldBe(nameKey)
            newOrg.Status.ShouldBe(OrganizationStatus.Enabled)

            createdDirectory.Name.ShouldBe(directoryName)

            ' Clean up
            Call (Await createdDirectory.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedDirectoryHrefs.Remove(createdDirectory.Href)

            Call (Await newOrg.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(newOrg.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Creating_without_directory(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim name = $"Created Organization 2 (.NET ITs {fixture.TestRunIdentifier}-{clientBuilder.Name} - VB)"
            Dim nameKey = $"dotnet-test2-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb"
            Dim newOrg = client.Instantiate(Of IOrganization)() _
                .SetName(name) _
                .SetNameKey(nameKey) _
                .SetStatus(OrganizationStatus.Disabled)

            Await client.CreateOrganizationAsync(newOrg, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))
            Me.fixture.CreatedOrganizationHrefs.Add(newOrg.Href)

            newOrg.Name.ShouldBe(name)
            newOrg.NameKey.ShouldBe(nameKey)
            newOrg.Status.ShouldBe(OrganizationStatus.Disabled)

            Dim createdDirectory = Await newOrg.GetDefaultAccountStoreAsync()
            createdDirectory.ShouldBeNull()

            ' Clean up
            Call (Await newOrg.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(newOrg.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Creating_with_response_options(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim name = $"Created Organization 3 (.NET ITs {fixture.TestRunIdentifier}-{clientBuilder.Name} - VB)"
            Dim nameKey = $"dotnet-test3-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb"
            Dim newOrg = client.Instantiate(Of IOrganization)() _
                .SetName(name) _
                .SetNameKey(nameKey) _
                .SetStatus(OrganizationStatus.Disabled)

            Await client.CreateOrganizationAsync(newOrg, Sub(opt)
                                                             opt.CreateDirectory = False
                                                             opt.ResponseOptions.Expand(Function(x) x.GetCustomData())
                                                         End Sub)
            Me.fixture.CreatedOrganizationHrefs.Add(newOrg.Href)

            newOrg.Name.ShouldBe(name)
            newOrg.NameKey.ShouldBe(nameKey)
            newOrg.Status.ShouldBe(OrganizationStatus.Disabled)

            ' Clean up
            Call (Await newOrg.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(newOrg.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Creating_with_convenience_method(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim name = $"Created Organization 4 (.NET ITs {fixture.TestRunIdentifier}-{clientBuilder.Name} - VB)"
            Dim nameKey = $"dotnet-test4-{fixture.TestRunIdentifier}-{clientBuilder.Name}"

            Dim newOrg = Await client.CreateOrganizationAsync(name, nameKey)
            newOrg.ShouldNotBeNull()
            Me.fixture.CreatedOrganizationHrefs.Add(newOrg.Href)

            newOrg.Name.ShouldBe(name)
            newOrg.NameKey.ShouldBe(nameKey)
            newOrg.Status.ShouldBe(OrganizationStatus.Enabled)

            ' Clean up
            Call (Await newOrg.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(newOrg.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Creating_account_store_mapping(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directly Test Organization - VB") _
                .SetNameKey($"dotnet-test4-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb")
            Await tenant.CreateOrganizationAsync(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim directory As IAccountStore = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)

            Dim mapping = client.Instantiate(Of IOrganizationAccountStoreMapping)()
            mapping.SetAccountStore(directory)
            mapping.SetListIndex(500)
            Await createdOrganization.CreateAccountStoreMappingAsync(mapping)

            Call (Await mapping.GetAccountStoreAsync()).Href.ShouldBe(directory.Href)
            Call (Await mapping.GetOrganizationAsync()).Href.ShouldBe(createdOrganization.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            Call (Await createdOrganization.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Creating_second_account_store_mapping_at_zeroth_index(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding Two AccountStores Directly Test Organization - VB") _
                .SetNameKey($"dotnet-test5-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb")
            Await tenant.CreateOrganizationAsync(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim mapping1 = Await createdOrganization.AddAccountStoreAsync(Me.fixture.PrimaryDirectoryHref)

            Dim group = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)
            Dim mapping2 = client.Instantiate(Of IOrganizationAccountStoreMapping)()
            mapping2.SetAccountStore(group)
            mapping2.SetListIndex(0)
            Await createdOrganization.CreateAccountStoreMappingAsync(mapping2)

            mapping2.ListIndex.ShouldBe(0)
            mapping1.ListIndex.ShouldBe(1)

            ' Clean up
            Call (Await createdOrganization.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_directory_as_account_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directory Test Organization - VB") _
                .SetNameKey($"dotnet-test6-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb")
            Await tenant.CreateOrganizationAsync(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim directory = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            Dim mapping = Await createdOrganization.AddAccountStoreAsync(directory)

            Call (Await mapping.GetAccountStoreAsync()).Href.ShouldBe(directory.Href)
            Call (Await mapping.GetOrganizationAsync()).Href.ShouldBe(createdOrganization.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            Call (Await createdOrganization.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_group_as_account_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Group Test Organization - VB") _
                .SetNameKey($"dotnet-test7-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb")
            Await tenant.CreateOrganizationAsync(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim group = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)
            Dim mapping = Await createdOrganization.AddAccountStoreAsync(group)

            Call (Await mapping.GetAccountStoreAsync()).Href.ShouldBe(group.Href)
            Call (Await mapping.GetOrganizationAsync()).Href.ShouldBe(createdOrganization.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            Call (Await createdOrganization.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Setting_mapped_directory_to_default_account_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Setting Existing Directory AccountStore Default Test Organization - VB") _
                .SetNameKey($"dotnet-test8-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb")
            Await tenant.CreateOrganizationAsync(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim directory = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            Dim mapping = Await createdOrganization.AddAccountStoreAsync(directory)

            Await createdOrganization.SetDefaultAccountStoreAsync(directory)

            mapping.IsDefaultAccountStore.ShouldBeTrue()
            mapping.IsDefaultGroupStore.ShouldBeFalse()

            ' Clean up
            Call (Await createdOrganization.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Setting_mapped_group_to_default_account_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Setting Existing Group AccountStore Default Test Organization - VB") _
                .SetNameKey($"dotnet-test9-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb")
            Await tenant.CreateOrganizationAsync(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim group = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)
            Dim mapping = Await createdOrganization.AddAccountStoreAsync(group)

            Await createdOrganization.SetDefaultAccountStoreAsync(group)

            mapping.IsDefaultAccountStore.ShouldBeTrue()
            mapping.IsDefaultGroupStore.ShouldBeFalse()

            ' Clean up
            Call (Await createdOrganization.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Setting_unmapped_directory_to_default_account_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Setting New Directory AccountStore Default Test Organization - VB") _
                .SetNameKey($"dotnet-test10-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb")
            Await tenant.CreateOrganizationAsync(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim directory = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            Await createdOrganization.SetDefaultAccountStoreAsync(directory)

            Dim mapping = Await createdOrganization.GetAccountStoreMappings().SingleAsync()
            mapping.IsDefaultAccountStore.ShouldBeTrue()
            mapping.IsDefaultGroupStore.ShouldBeFalse()

            ' Clean up
            Call (Await createdOrganization.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Setting_unmapped_group_to_default_account_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Setting New Group AccountStore Default Test Organization - VB") _
                .SetNameKey($"dotnet-test11-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb")
            Await tenant.CreateOrganizationAsync(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim group = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)
            Await createdOrganization.SetDefaultAccountStoreAsync(group)

            Dim mapping = Await createdOrganization.GetAccountStoreMappings().SingleAsync()
            mapping.IsDefaultAccountStore.ShouldBeTrue()
            mapping.IsDefaultGroupStore.ShouldBeFalse()

            ' Clean up
            Call (Await createdOrganization.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Setting_mapped_directory_to_default_group_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Setting Existing Directory GroupStore Default Test Organization - VB") _
                .SetNameKey($"dotnet-test12-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb")
            Await tenant.CreateOrganizationAsync(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim directory = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            Dim mapping = Await createdOrganization.AddAccountStoreAsync(directory)

            Await createdOrganization.SetDefaultGroupStoreAsync(directory)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeTrue()

            ' Clean up
            Call (Await createdOrganization.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Setting_unmapped_directory_to_default_group_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Setting Existing Directory GroupStore Default Test Organization - VB") _
                .SetNameKey($"dotnet-test13-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb")
            Await tenant.CreateOrganizationAsync(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim directory = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            Await createdOrganization.SetDefaultGroupStoreAsync(directory)

            Dim mapping = Await createdOrganization.GetAccountStoreMappings().SingleAsync()
            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeTrue()

            ' Clean up
            Call (Await createdOrganization.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Setting_group_group_store_throws(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Setting Group as GroupStore Test Organization - VB") _
                .SetNameKey($"dotnet-test14-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb")
            Await tenant.CreateOrganizationAsync(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim group = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)

            ' If this errors, the server-side API behavior has changed.
            Should.[Throw](Of ResourceException)(Async Function()
                                                     Await createdOrganization.SetDefaultGroupStoreAsync(group)
                                                 End Function)

            ' Clean up
            Call (Await createdOrganization.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_directory_as_account_store_by_href(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directory By Href Test Organization - VB") _
                .SetNameKey($"dotnet-test15-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb")
            Await tenant.CreateOrganizationAsync(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim mapping = Await createdOrganization.AddAccountStoreAsync(Me.fixture.PrimaryDirectoryHref)

            Call (Await mapping.GetAccountStoreAsync()).Href.ShouldBe(Me.fixture.PrimaryDirectoryHref)
            Call (Await mapping.GetOrganizationAsync()).Href.ShouldBe(createdOrganization.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            Call (Await createdOrganization.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_group_as_account_store_by_href(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Group By Href Test Organization - VB") _
                .SetNameKey($"dotnet-test16-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb")
            Await tenant.CreateOrganizationAsync(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim mapping = Await createdOrganization.AddAccountStoreAsync(Me.fixture.PrimaryGroupHref)

            Call (Await mapping.GetAccountStoreAsync()).Href.ShouldBe(Me.fixture.PrimaryGroupHref)
            Call (Await mapping.GetOrganizationAsync()).Href.ShouldBe(createdOrganization.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            Call (Await createdOrganization.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_directory_as_account_store_by_name(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directory By Name Test Organization - VB") _
                .SetNameKey($"dotnet-test17-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb")
            Await tenant.CreateOrganizationAsync(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim directoryName = ".NET IT Organization Test {fixture.TestRunIdentifier}-{clientBuilder.Name} Add Directory As AccountStore By Name - VB"
            Dim testDirectory = client.Instantiate(Of IDirectory)() _
                .SetName(directoryName)
            Await client.CreateDirectoryAsync(testDirectory)
            testDirectory.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedDirectoryHrefs.Add(testDirectory.Href)

            Dim mapping = Await createdOrganization.AddAccountStoreAsync(directoryName)

            Call (Await mapping.GetAccountStoreAsync()).Href.ShouldBe(testDirectory.Href)
            Call (Await mapping.GetOrganizationAsync()).Href.ShouldBe(createdOrganization.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            Call (Await createdOrganization.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)

            Call (Await testDirectory.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedDirectoryHrefs.Remove(testDirectory.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_group_as_account_store_by_name(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Group By Name Test Organization - VB") _
                .SetNameKey($"dotnet-test18-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb")
            Await tenant.CreateOrganizationAsync(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            ' Needs to have a default GroupStore
            Dim mapping = Await createdOrganization.AddAccountStoreAsync(Me.fixture.PrimaryDirectoryHref)
            mapping.SetDefaultGroupStore(True)
            Await mapping.SaveAsync()

            Dim groupName = ".NET IT Organization Test {fixture.TestRunIdentifier}-{clientBuilder.Name} Add Group As AccountStore By Name - VB"
            Dim testGroup = client.Instantiate(Of IGroup)() _
                .SetName(groupName)
            Await createdOrganization.CreateGroupAsync(testGroup)
            testGroup.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedGroupHrefs.Add(testGroup.Href)

            Dim newMapping = Await createdOrganization.AddAccountStoreAsync(groupName)

            Call (Await newMapping.GetAccountStoreAsync()).Href.ShouldBe(testGroup.Href)
            Call (Await newMapping.GetOrganizationAsync()).Href.ShouldBe(createdOrganization.Href)

            newMapping.IsDefaultAccountStore.ShouldBeFalse()
            newMapping.IsDefaultGroupStore.ShouldBeFalse()
            newMapping.ListIndex.ShouldBe(1)

            ' Clean up
            Call (Await createdOrganization.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)

            Call (Await testGroup.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedGroupHrefs.Remove(testGroup.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_directory_as_account_store_by_query(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directory By Query Test Organization - VB") _
                .SetNameKey($"dotnet-test19-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb")
            Await tenant.CreateOrganizationAsync(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim directoryName = (Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)).Name
            Dim mapping = Await createdOrganization.AddAccountStoreAsync(Of IDirectory)(Function(dirs) dirs.Where(Function(d) d.Name.EndsWith(directoryName.Substring(1))))

            Call (Await mapping.GetAccountStoreAsync()).Href.ShouldBe(Me.fixture.PrimaryDirectoryHref)
            Call (Await mapping.GetOrganizationAsync()).Href.ShouldBe(createdOrganization.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            Call (Await createdOrganization.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_group_as_account_store_by_query(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Group By Query Test Organization - VB") _
                .SetNameKey($"dotnet-test20-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb")
            Await tenant.CreateOrganizationAsync(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim groupName = (Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)).Name
            Dim mapping = Await createdOrganization.AddAccountStoreAsync(Of IGroup)(Function(groups) groups.Where(Function(g) g.Name.EndsWith(groupName.Substring(1))))

            Call (Await mapping.GetAccountStoreAsync()).Href.ShouldBe(Me.fixture.PrimaryGroupHref)
            Call (Await mapping.GetOrganizationAsync()).Href.ShouldBe(createdOrganization.Href)

            mapping.IsDefaultAccountStore.ShouldBeFalse()
            mapping.IsDefaultGroupStore.ShouldBeFalse()
            mapping.ListIndex.ShouldBe(0)

            ' Clean up
            Call (Await createdOrganization.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_directory_as_account_store_by_query_throws_for_multiple_results(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Directory By Query Throws Test Organization - VB") _
                .SetNameKey($"dotnet-test21-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb")
            Await tenant.CreateOrganizationAsync(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, False))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim dir1 = Await client.CreateDirectoryAsync($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Directory Query Results1", String.Empty, DirectoryStatus.Enabled)
            Dim dir2 = Await client.CreateDirectoryAsync($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Directory Query Results2", String.Empty, DirectoryStatus.Enabled)

            Me.fixture.CreatedDirectoryHrefs.Add(dir1.Href)
            Me.fixture.CreatedDirectoryHrefs.Add(dir2.Href)

            Should.[Throw](Of ArgumentException)(Async Function()
                                                     ' Throws because multiple matching results exist
                                                     Dim mapping = Await createdOrganization.AddAccountStoreAsync(Of IDirectory)(Function(dirs) dirs.Where(Function(d) d.Name.StartsWith($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Directory Query Results")))
                                                 End Function)

            ' Clean up
            Call (Await dir1.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedDirectoryHrefs.Remove(dir1.Href)

            Call (Await dir2.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedDirectoryHrefs.Remove(dir2.Href)

            Call (Await createdOrganization.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_group_as_account_store_by_query_throws_for_multiple_results(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim createdOrganization = client.Instantiate(Of IOrganization)() _
                .SetName($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Adding AccountStore Group By Query Throws Test Organization - VB") _
                .SetNameKey($"dotnet-test22-{fixture.TestRunIdentifier}-{clientBuilder.Name}-vb")
            Await tenant.CreateOrganizationAsync(createdOrganization, Function(opt) InlineAssignHelper(opt.CreateDirectory, True))

            createdOrganization.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedOrganizationHrefs.Add(createdOrganization.Href)

            Dim defaultGroupStore = TryCast(Await createdOrganization.GetDefaultGroupStoreAsync(), IDirectory)
            defaultGroupStore.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedDirectoryHrefs.Add(defaultGroupStore.Href)

            Dim group1 = Await createdOrganization.CreateGroupAsync($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Group Query Results1", String.Empty)
            Dim group2 = Await createdOrganization.CreateGroupAsync($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Group Query Results2", String.Empty)

            Me.fixture.CreatedGroupHrefs.Add(group1.Href)
            Me.fixture.CreatedGroupHrefs.Add(group2.Href)

            Should.[Throw](Of ArgumentException)(Async Function()
                                                     ' Throws because multiple matching results exist
                                                     Dim mapping = Await createdOrganization.AddAccountStoreAsync(Of IGroup)(Function(groups) groups.Where(Function(x) x.Name.StartsWith($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Organization Multiple Group Query Results")))
                                                 End Function)

            ' Clean up
            Call (Await group1.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedGroupHrefs.Remove(group1.Href)

            Call (Await group2.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedGroupHrefs.Remove(group2.Href)

            Call (Await defaultGroupStore.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedDirectoryHrefs.Remove(defaultGroupStore.Href)

            Call (Await createdOrganization.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedOrganizationHrefs.Remove(createdOrganization.Href)
        End Function
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function
    End Class
End Namespace