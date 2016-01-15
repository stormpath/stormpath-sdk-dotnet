' <copyright file="Group_tests.vb" company="Stormpath, Inc.">
' Copyright (c) 2016 Stormpath, Inc.
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
Imports Stormpath.SDK.Sync
Imports Stormpath.SDK.Tests.Common.Integration
Imports Xunit

Namespace Sync
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class Group_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_tenant_groups(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()
            Dim groups = tenant.GetGroups().Synchronously().ToList()

            groups.Count.ShouldBeGreaterThan(0)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_group_tenant(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim group = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref)

            ' Verify data from IntegrationTestData
            Dim tenantHref = group.GetTenant().Href
            tenantHref.ShouldBe(Me.fixture.TenantHref)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_application_groups(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim groups = app.GetGroups().Synchronously().ToList()

            groups.Count.ShouldBeGreaterThan(0)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_group_applications(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim group = client.GetGroup(Me.fixture.PrimaryGroupHref)

            Dim apps = group.GetApplications().Synchronously().ToList()
            apps.Where(Function(x) x.Href = Me.fixture.PrimaryApplicationHref).Any().ShouldBeTrue()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_directory_groups(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim directory = client.GetResource(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
            Dim groups = directory.GetGroups().Synchronously().ToList()

            groups.Count.ShouldBeGreaterThan(0)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_account_groups(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim luke = app.GetAccounts().Synchronously().Where(Function(x) x.Email.StartsWith("lskywalker")).[Single]()

            Dim groups = luke.GetGroups().Synchronously().ToList()

            groups.Count.ShouldBeGreaterThan(0)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_group_accounts(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim humans = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref)

            humans.GetAccounts().Synchronously().Count().ShouldBeGreaterThan(0)
            humans.GetAccountMemberships().Synchronously().Count().ShouldBeGreaterThan(0)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Modifying_group(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim directory = client.GetResource(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)

            Dim droids = client.Instantiate(Of IGroup)() _
                .SetName($"Droids (.NET ITs {fixture.TestRunIdentifier}) - Sync") _
                .SetDescription("Mechanical entities") _
                .SetStatus(GroupStatus.Enabled)

            directory.CreateGroup(droids)
            Me.fixture.CreatedGroupHrefs.Add(droids.Href)

            droids.SetStatus(GroupStatus.Disabled)
            Dim result = droids.Save()

            result.Status.ShouldBe(GroupStatus.Disabled)

            ' Clean up
            droids.Delete()
            Me.fixture.CreatedGroupHrefs.Remove(droids.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Saving_with_response_options(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim directory = client.GetResource(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)

            Dim newGroup = client.Instantiate(Of IGroup)() _
                .SetName($"Another Group (.NET ITs {fixture.TestRunIdentifier})") _
                .SetStatus(GroupStatus.Disabled)

            directory.CreateGroup(newGroup)
            Me.fixture.CreatedGroupHrefs.Add(newGroup.Href)

            newGroup.SetDescription("foobar")
            newGroup.Save(Function(response) response.Expand(Function(x) x.GetAccounts(0, 10)))

            ' Clean up
            newGroup.Delete()
            Me.fixture.CreatedGroupHrefs.Remove(newGroup.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_account_to_group(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim humans = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref)

            Dim lando = app.GetAccounts() _
                .Synchronously() _
                .Where(Function(x) x.Email.StartsWith("lcalrissian")) _
                .[Single]()
            Dim membership = humans.AddAccount(lando)

            membership.ShouldNotBeNull()
            membership.Href.ShouldNotBeNullOrEmpty()

            lando.IsMemberOfGroup(humans.Href).ShouldBeTrue()

            humans.RemoveAccount(lando).ShouldBeTrue()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_group_membership(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim humans = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref)

            Dim lando = app.GetAccounts() _
                .Synchronously() _
                .Where(Function(x) x.Email.StartsWith("lcalrissian")) _
                .[Single]()
            Dim membership = humans.AddAccount(lando)

            ' Should also be seen in the master membership list
            Dim allMemberships = humans.GetAccountMemberships().Synchronously().ToList()
            allMemberships.ShouldContain(Function(x) x.Href = membership.Href)

            membership.GetAccount().Href.ShouldBe(lando.Href)
            membership.GetGroup().Href.ShouldBe(humans.Href)

            membership.Delete().ShouldBeTrue()

            lando.IsMemberOfGroup(humans.Href).ShouldBeFalse()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_account_to_group_by_group_href(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim leia = app.GetAccounts() _
                .Synchronously() _
                .Where(Function(x) x.Email.StartsWith("leia.organa")) _
                .[Single]()
            leia.AddGroup(Me.fixture.PrimaryGroupHref)

            leia.IsMemberOfGroup(Me.fixture.PrimaryGroupHref).ShouldBeTrue()

            leia.RemoveGroup(Me.fixture.PrimaryGroupHref).ShouldBeTrue()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_account_to_group_by_group_name(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim groupName = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref).Name

            Dim han = app.GetAccounts() _
                .Synchronously() _
                .Where(Function(x) x.Email.StartsWith("han.solo")) _
                .[Single]()
            han.AddGroup(groupName)

            han.IsMemberOfGroup(Me.fixture.PrimaryGroupHref).ShouldBeTrue()

            han.RemoveGroup(groupName).ShouldBeTrue()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_account_to_group_by_account_href(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim humans = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref)

            Dim leia = app.GetAccounts() _
                .Synchronously() _
                .Where(Function(x) x.Email.StartsWith("leia.organa")) _
                .[Single]()
            humans.AddAccount(leia.Href)

            leia.IsMemberOfGroup(Me.fixture.PrimaryGroupHref).ShouldBeTrue()

            humans.RemoveAccount(leia.Href).ShouldBeTrue()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Adding_account_to_group_by_account_email(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim humans = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref)

            Dim han = app.GetAccounts() _
                .Synchronously() _
                .Where(Function(x) x.Email.StartsWith("han.solo")) _
                .[Single]()
            humans.AddAccount(han.Email)

            han.IsMemberOfGroup(Me.fixture.PrimaryGroupHref).ShouldBeTrue()

            humans.RemoveAccount(han.Email).ShouldBeTrue()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_group_in_application(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim instance = client.Instantiate(Of IGroup)()
            instance.SetName($".NET ITs New Test Group {fixture.TestRunIdentifier} - Sync")
            instance.SetDescription("A nu start")
            instance.SetStatus(GroupStatus.Disabled)

            Dim created = app.CreateGroup(instance)
            created.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedGroupHrefs.Add(created.Href)

            created.Name.ShouldBe($".NET ITs New Test Group {fixture.TestRunIdentifier} - Sync")
            created.Description.ShouldBe("A nu start")
            created.Status.ShouldBe(GroupStatus.Disabled)

            created.Delete().ShouldBeTrue()
            Me.fixture.CreatedGroupHrefs.Remove(created.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_group_in_application_with_convenience_method(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim name = $".NET ITs New Convenient Test Group {fixture.TestRunIdentifier}"
            Dim created = app.CreateGroup(name, "I can has lazy")
            created.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedGroupHrefs.Add(created.Href)

            created.Name.ShouldBe(name)
            created.Description.ShouldBe("I can has lazy")
            created.Status.ShouldBe(GroupStatus.Enabled)

            created.Delete().ShouldBeTrue()
            Me.fixture.CreatedGroupHrefs.Remove(created.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_group_in_directory(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim directory = client.GetResource(Of IApplication)(Me.fixture.PrimaryDirectoryHref)

            Dim instance = client.Instantiate(Of IGroup)()
            instance.SetName($".NET ITs New Test Group #2 {fixture.TestRunIdentifier} - Sync")
            instance.SetDescription("A nu start")
            instance.SetStatus(GroupStatus.Enabled)

            Dim created = directory.CreateGroup(instance)
            created.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedGroupHrefs.Add(created.Href)

            created.Name.ShouldBe($".NET ITs New Test Group #2 {fixture.TestRunIdentifier} - Sync")
            created.Description.ShouldBe("A nu start")
            created.Status.ShouldBe(GroupStatus.Enabled)

            created.Delete().ShouldBeTrue()
            Me.fixture.CreatedGroupHrefs.Remove(created.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_group_with_custom_data(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim instance = client.Instantiate(Of IGroup)()
            instance.SetName($".NET ITs Custom Data Group {fixture.TestRunIdentifier} - Sync")
            instance.CustomData.Put("isNeat", True)
            instance.CustomData.Put("roleBasedSecurity", "pieceOfCake")

            Dim created = app.CreateGroup(instance)
            Me.fixture.CreatedGroupHrefs.Add(created.Href)

            Dim customData = created.GetCustomData()
            CBool(customData("isNeat")).ShouldBe(True)
            CStr(customData("roleBasedSecurity")).ShouldBe("pieceOfCake")

            created.Delete().ShouldBeTrue()
            Me.fixture.CreatedGroupHrefs.Remove(created.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_group_with_response_options(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim group = client.Instantiate(Of IGroup)() _
                .SetName($".NET ITs Custom Data Group #2 ({fixture.TestRunIdentifier} - {clientBuilder.Name})")

            app.CreateGroup(group, Function(opt) opt.ResponseOptions.Expand(Function(x) x.GetCustomData()))

            group.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedGroupHrefs.Add(group.Href)

            group.Delete().ShouldBeTrue()
            Me.fixture.CreatedGroupHrefs.Remove(group.Href)
        End Sub
    End Class
End Namespace