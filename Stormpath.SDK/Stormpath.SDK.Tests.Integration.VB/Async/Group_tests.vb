' <copyright file="Group_tests.vb" company="Stormpath, Inc.">
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
Imports Stormpath.SDK.Application
Imports Stormpath.SDK.Directory
Imports Stormpath.SDK.Group
Imports Stormpath.SDK.Tests.Common.Integration
Imports Xunit

Namespace Async
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class Group_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_tenant_groups(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()
            Dim groups = Await tenant.GetGroups().ToListAsync()

            groups.Count.ShouldBeGreaterThan(0)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_group_tenant(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim group = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryGroupHref)

            ' Verify data from IntegrationTestData
            Dim tenantHref = (Await group.GetTenantAsync()).Href
            tenantHref.ShouldBe(Me.fixture.TenantHref)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_application_groups(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim groups = Await app.GetGroups().ToListAsync()

            groups.Count.ShouldBeGreaterThan(0)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_directory_groups(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim directory = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            Dim groups = Await directory.GetGroups().ToListAsync()

            groups.Count.ShouldBeGreaterThan(0)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_account_groups(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim luke = Await app.GetAccounts().Where(Function(x) x.Email.StartsWith("lskywalker")).SingleAsync()

            Dim groups = Await luke.GetGroups().ToListAsync()

            groups.Count.ShouldBeGreaterThan(0)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_group_accounts(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim humans = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)

            Dim count1 = Await humans.GetAccounts().CountAsync()
            count1.ShouldBeGreaterThan(0)
            Dim count2 = Await humans.GetAccountMemberships().CountAsync()
            count2.ShouldBeGreaterThan(0)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Modifying_group(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim directory = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)

            Dim droids = client.Instantiate(Of IGroup)() _
                .SetName($"Droids (.NET ITs {fixture.TestRunIdentifier})") _
                .SetDescription("Mechanical entities") _
                .SetStatus(GroupStatus.Enabled)

            Await directory.CreateGroupAsync(droids)
            Me.fixture.CreatedGroupHrefs.Add(droids.Href)

            droids.SetStatus(GroupStatus.Disabled)
            Dim result = Await droids.SaveAsync()

            result.Status.ShouldBe(GroupStatus.Disabled)

            ' Clean up
            Assert.True(Await droids.DeleteAsync())
            Me.fixture.CreatedGroupHrefs.Remove(droids.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Saving_with_response_options(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim directory = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)

            Dim newGroup = client.Instantiate(Of IGroup)() _
                .SetName($"Another Group (.NET ITs {fixture.TestRunIdentifier})") _
                .SetStatus(GroupStatus.Disabled)

            Await directory.CreateGroupAsync(newGroup)
            Me.fixture.CreatedGroupHrefs.Add(newGroup.Href)

            newGroup.SetDescription("foobar")
            Await newGroup.SaveAsync(Function(response) response.Expand(Function(x) x.GetAccounts(0, 10)))

            ' Clean up
            Assert.True(Await newGroup.DeleteAsync())
            Me.fixture.CreatedGroupHrefs.Remove(newGroup.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_account_to_group(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim humans = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)

            Dim lando = Await app.GetAccounts() _
                .Where(Function(x) x.Email.StartsWith("lcalrissian")) _
                .SingleAsync()
            Dim membership = Await humans.AddAccountAsync(lando)

            membership.ShouldNotBeNull()
            membership.Href.ShouldNotBeNullOrEmpty()

            Assert.True(Await lando.IsMemberOfGroupAsync(humans.Href))

            Assert.True(Await humans.RemoveAccountAsync(lando))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_group_membership(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim humans = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)

            Dim lando = Await app.GetAccounts() _
                .Where(Function(x) x.Email.StartsWith("lcalrissian")) _
                .SingleAsync()
            Dim membership = Await humans.AddAccountAsync(lando)

            ' Should also be seen in the master membership list
            Dim allMemberships = Await humans.GetAccountMemberships() _
                .ToListAsync()
            allMemberships.ShouldContain(Function(x) x.Href = membership.Href)

            Dim result1 = Await membership.GetAccountAsync()
            result1.Href.ShouldBe(lando.Href)
            Dim result2 = Await membership.GetGroupAsync()
            result2.Href.ShouldBe(humans.Href)

            Assert.True(Await membership.DeleteAsync())

            Assert.False(Await lando.IsMemberOfGroupAsync(humans.Href))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_account_to_group_by_group_href(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim leia = Await app.GetAccounts() _
                .Where(Function(x) x.Email.StartsWith("leia.organa")) _
                .SingleAsync()
            Await leia.AddGroupAsync(Me.fixture.PrimaryGroupHref)

            Assert.True(Await leia.IsMemberOfGroupAsync(Me.fixture.PrimaryGroupHref))

            Assert.True(Await leia.RemoveGroupAsync(Me.fixture.PrimaryGroupHref))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_account_to_group_by_group_name(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim groupName = (Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)).Name

            Dim han = Await app.GetAccounts() _
                .Where(Function(x) x.Email.StartsWith("han.solo")) _
                .SingleAsync()
            Await han.AddGroupAsync(groupName)

            Assert.True(Await han.IsMemberOfGroupAsync(Me.fixture.PrimaryGroupHref))

            Assert.True(Await han.RemoveGroupAsync(groupName))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_account_to_group_by_account_href(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim humans = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)

            Dim leia = Await app.GetAccounts() _
                .Where(Function(x) x.Email.StartsWith("leia.organa")) _
                .SingleAsync()
            Await humans.AddAccountAsync(leia.Href)

            Assert.True(Await leia.IsMemberOfGroupAsync(Me.fixture.PrimaryGroupHref))

            Assert.True(Await humans.RemoveAccountAsync(leia.Href))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Adding_account_to_group_by_account_email(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim humans = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)

            Dim han = Await app.GetAccounts() _
                .Where(Function(x) x.Email.StartsWith("han.solo")) _
                .SingleAsync()
            Await humans.AddAccountAsync(han.Email)

            Assert.True(Await han.IsMemberOfGroupAsync(Me.fixture.PrimaryGroupHref))

            Assert.True(Await humans.RemoveAccountAsync(han.Email))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Creating_group_in_application(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim instance = client.Instantiate(Of IGroup)()
            instance.SetName($".NET ITs New Test Group {fixture.TestRunIdentifier}")
            instance.SetDescription("A nu start")
            instance.SetStatus(GroupStatus.Disabled)

            Dim created = Await app.CreateGroupAsync(instance)
            created.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedGroupHrefs.Add(created.Href)

            created.Name.ShouldBe($".NET ITs New Test Group {fixture.TestRunIdentifier}")
            created.Description.ShouldBe("A nu start")
            created.Status.ShouldBe(GroupStatus.Disabled)

            Assert.True(Await created.DeleteAsync())
            Me.fixture.CreatedGroupHrefs.Remove(created.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Creating_group_in_application_with_convenience_method(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim name = $".NET ITs New Convenient Test Group {fixture.TestRunIdentifier}"
            Dim created = Await app.CreateGroupAsync(name, "I can has lazy")
            created.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedGroupHrefs.Add(created.Href)

            created.Name.ShouldBe(name)
            created.Description.ShouldBe("I can has lazy")
            created.Status.ShouldBe(GroupStatus.Enabled)

            Call (Await created.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedGroupHrefs.Remove(created.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Creating_group_in_directory(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim directory = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)

            Dim instance = client.Instantiate(Of IGroup)()
            Dim directoryName = $".NET ITs New Test Group #2 ({fixture.TestRunIdentifier} - {clientBuilder.Name})"
            instance.SetName(directoryName)
            instance.SetDescription("A nu start")
            instance.SetStatus(GroupStatus.Enabled)

            Dim created = Await directory.CreateGroupAsync(instance)
            created.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedGroupHrefs.Add(created.Href)

            created.Name.ShouldBe(directoryName)
            created.Description.ShouldBe("A nu start")
            created.Status.ShouldBe(GroupStatus.Enabled)

            Assert.True(Await created.DeleteAsync())
            Me.fixture.CreatedGroupHrefs.Remove(created.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Creating_group_with_custom_data(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim instance = client.Instantiate(Of IGroup)()
            instance.SetName($".NET ITs Custom Data Group ({fixture.TestRunIdentifier} - {clientBuilder.Name})")
            instance.CustomData.Put("isNeat", True)
            instance.CustomData.Put("roleBasedSecurity", "pieceOfCake")

            Dim created = Await app.CreateGroupAsync(instance)
            Me.fixture.CreatedGroupHrefs.Add(created.Href)

            Dim customData = Await created.GetCustomDataAsync()
            CBool(customData("isNeat")).ShouldBe(True)
            CStr(customData("roleBasedSecurity")).ShouldBe("pieceOfCake")

            Assert.True(Await created.DeleteAsync())
            Me.fixture.CreatedGroupHrefs.Remove(created.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Creating_group_with_response_options(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim group = client.Instantiate(Of IGroup)() _
                .SetName($".NET ITs Custom Data Group #2 ({fixture.TestRunIdentifier} - {clientBuilder.Name})")

            Await app.CreateGroupAsync(group, Function(opt) opt.ResponseOptions.Expand(Function(x) x.GetCustomData()))

            group.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedGroupHrefs.Add(group.Href)

            Assert.True(Await group.DeleteAsync())
            Me.fixture.CreatedGroupHrefs.Remove(group.Href)
        End Function
    End Class
End Namespace