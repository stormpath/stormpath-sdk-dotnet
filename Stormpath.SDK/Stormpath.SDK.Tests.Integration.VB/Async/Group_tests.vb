
' <copyright file="Group_tests.cs" company="Stormpath, Inc.">
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

Imports System.Threading.Tasks
Imports Shouldly
Imports Stormpath.SDK.Application
Imports Stormpath.SDK.Directory
Imports Stormpath.SDK.Group
Imports Stormpath.SDK.Tests.Common.Integration
Imports Xunit

Namespace Stormpath.SDK.Tests.Integration.Async
	<Collection(nameof(IntegrationTestCollection))> _
	Public Class Group_tests
		Private ReadOnly fixture As TestFixture

		Public Sub New(fixture As TestFixture)
			Me.fixture = fixture
		End Sub

		<Theory> _
		<MemberData(nameof(TestClients.GetClients), MemberType := GetType(TestClients))> _
		Public Function Getting_tenant_groups(clientBuilder As TestClientProvider) As Task
			Dim client = clientBuilder.GetClient()
			Dim tenant = Await client.GetCurrentTenantAsync()
			Dim groups = Await tenant.GetGroups().ToListAsync()

			groups.Count.ShouldBeGreaterThan(0)
		End Function

		<Theory> _
		<MemberData(nameof(TestClients.GetClients), MemberType := GetType(TestClients))> _
		Public Function Getting_group_tenant(clientBuilder As TestClientProvider) As Task
			Dim client = clientBuilder.GetClient()
			Dim group = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryGroupHref)

			' Verify data from IntegrationTestData
			Dim tenantHref = (Await group.GetTenantAsync()).Href
			tenantHref.ShouldBe(Me.fixture.TenantHref)
		End Function

		<Theory> _
		<MemberData(nameof(TestClients.GetClients), MemberType := GetType(TestClients))> _
		Public Function Getting_application_groups(clientBuilder As TestClientProvider) As Task
			Dim client = clientBuilder.GetClient()
			Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)
			Dim groups = Await app.GetGroups().ToListAsync()

			groups.Count.ShouldBeGreaterThan(0)
		End Function

		<Theory> _
		<MemberData(nameof(TestClients.GetClients), MemberType := GetType(TestClients))> _
		Public Function Getting_directory_groups(clientBuilder As TestClientProvider) As Task
			Dim client = clientBuilder.GetClient()
			Dim directory = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)
			Dim groups = Await directory.GetGroups().ToListAsync()

			groups.Count.ShouldBeGreaterThan(0)
		End Function

		<Theory> _
		<MemberData(nameof(TestClients.GetClients), MemberType := GetType(TestClients))> _
		Public Function Getting_account_groups(clientBuilder As TestClientProvider) As Task
			Dim client = clientBuilder.GetClient()
			Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)
			Dim luke = Await app.GetAccounts().Where(Function(x) x.Email.StartsWith("lskywalker")).SingleAsync()

			Dim groups = Await luke.GetGroups().ToListAsync()

			groups.Count.ShouldBeGreaterThan(0)
		End Function

		<Theory> _
		<MemberData(nameof(TestClients.GetClients), MemberType := GetType(TestClients))> _
		Public Function Getting_group_accounts(clientBuilder As TestClientProvider) As Task
			Dim client = clientBuilder.GetClient()
			Dim humans = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)

			(Await humans.GetAccounts().CountAsync()).ShouldBeGreaterThan(0)
			(Await humans.GetAccountMemberships().CountAsync()).ShouldBeGreaterThan(0)
		End Function

		<Theory> _
		<MemberData(nameof(TestClients.GetClients), MemberType := GetType(TestClients))> _
		Public Function Modifying_group(clientBuilder As TestClientProvider) As Task
			Dim client = clientBuilder.GetClient()
			Dim directory = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)

			Dim droids = client.Instantiate(Of IGroup)().SetName("Droids (.NET ITs {this.fixture.TestRunIdentifier})").SetDescription("Mechanical entities").SetStatus(GroupStatus.Enabled)

			Await directory.CreateGroupAsync(droids)
			Me.fixture.CreatedGroupHrefs.Add(droids.Href)

			droids.SetStatus(GroupStatus.Disabled)
			Dim result = Await droids.SaveAsync()

			result.Status.ShouldBe(GroupStatus.Disabled)

			' Clean up
			(Await droids.DeleteAsync()).ShouldBeTrue()
			Me.fixture.CreatedGroupHrefs.Remove(droids.Href)
		End Function

		<Theory> _
		<MemberData(nameof(TestClients.GetClients), MemberType := GetType(TestClients))> _
		Public Function Saving_with_response_options(clientBuilder As TestClientProvider) As Task
			Dim client = clientBuilder.GetClient()
			Dim directory = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)

			Dim newGroup = client.Instantiate(Of IGroup)().SetName("Another Group (.NET ITs {this.fixture.TestRunIdentifier})").SetStatus(GroupStatus.Disabled)

			Await directory.CreateGroupAsync(newGroup)
			Me.fixture.CreatedGroupHrefs.Add(newGroup.Href)

			newGroup.SetDescription("foobar")
			Await newGroup.SaveAsync(Function(response) response.Expand(Function(x) x.GetAccounts(0, 10)))

			' Clean up
			(Await newGroup.DeleteAsync()).ShouldBeTrue()
			Me.fixture.CreatedGroupHrefs.Remove(newGroup.Href)
		End Function

		<Theory> _
		<MemberData(nameof(TestClients.GetClients), MemberType := GetType(TestClients))> _
		Public Function Adding_account_to_group(clientBuilder As TestClientProvider) As Task
			Dim client = clientBuilder.GetClient()
			Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)
			Dim humans = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)

			Dim lando = Await app.GetAccounts().Where(Function(x) x.Email.StartsWith("lcalrissian")).SingleAsync()
			Dim membership = Await humans.AddAccountAsync(lando)

			membership.ShouldNotBeNull()
			membership.Href.ShouldNotBeNullOrEmpty()

			(Await lando.IsMemberOfGroupAsync(humans.Href)).ShouldBeTrue()

			(Await humans.RemoveAccountAsync(lando)).ShouldBeTrue()
		End Function

		<Theory> _
		<MemberData(nameof(TestClients.GetClients), MemberType := GetType(TestClients))> _
		Public Function Getting_group_membership(clientBuilder As TestClientProvider) As Task
			Dim client = clientBuilder.GetClient()
			Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)
			Dim humans = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)

			Dim lando = Await app.GetAccounts().Where(Function(x) x.Email.StartsWith("lcalrissian")).SingleAsync()
			Dim membership = Await humans.AddAccountAsync(lando)

			' Should also be seen in the master membership list
			Dim allMemberships = Await humans.GetAccountMemberships().ToListAsync()
			allMemberships.ShouldContain(Function(x) x.Href = membership.Href)

			(Await membership.GetAccountAsync()).Href.ShouldBe(lando.Href)
			(Await membership.GetGroupAsync()).Href.ShouldBe(humans.Href)

			(Await membership.DeleteAsync()).ShouldBeTrue()

			(Await lando.IsMemberOfGroupAsync(humans.Href)).ShouldBeFalse()
		End Function

		<Theory> _
		<MemberData(nameof(TestClients.GetClients), MemberType := GetType(TestClients))> _
		Public Function Adding_account_to_group_by_group_href(clientBuilder As TestClientProvider) As Task
			Dim client = clientBuilder.GetClient()
			Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

			Dim leia = Await app.GetAccounts().Where(Function(x) x.Email.StartsWith("leia.organa")).SingleAsync()
			Await leia.AddGroupAsync(Me.fixture.PrimaryGroupHref)

			(Await leia.IsMemberOfGroupAsync(Me.fixture.PrimaryGroupHref)).ShouldBeTrue()

			(Await leia.RemoveGroupAsync(Me.fixture.PrimaryGroupHref)).ShouldBeTrue()
		End Function

		<Theory> _
		<MemberData(nameof(TestClients.GetClients), MemberType := GetType(TestClients))> _
		Public Function Adding_account_to_group_by_group_name(clientBuilder As TestClientProvider) As Task
			Dim client = clientBuilder.GetClient()
			Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

			Dim groupName = (Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)).Name

			Dim han = Await app.GetAccounts().Where(Function(x) x.Email.StartsWith("han.solo")).SingleAsync()
			Await han.AddGroupAsync(groupName)

			(Await han.IsMemberOfGroupAsync(Me.fixture.PrimaryGroupHref)).ShouldBeTrue()

			(Await han.RemoveGroupAsync(groupName)).ShouldBeTrue()
		End Function

		<Theory> _
		<MemberData(nameof(TestClients.GetClients), MemberType := GetType(TestClients))> _
		Public Function Adding_account_to_group_by_account_href(clientBuilder As TestClientProvider) As Task
			Dim client = clientBuilder.GetClient()
			Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)
			Dim humans = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)

			Dim leia = Await app.GetAccounts().Where(Function(x) x.Email.StartsWith("leia.organa")).SingleAsync()
			Await humans.AddAccountAsync(leia.Href)

			(Await leia.IsMemberOfGroupAsync(Me.fixture.PrimaryGroupHref)).ShouldBeTrue()

			(Await humans.RemoveAccountAsync(leia.Href)).ShouldBeTrue()
		End Function

		<Theory> _
		<MemberData(nameof(TestClients.GetClients), MemberType := GetType(TestClients))> _
		Public Function Adding_account_to_group_by_account_email(clientBuilder As TestClientProvider) As Task
			Dim client = clientBuilder.GetClient()
			Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)
			Dim humans = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)

			Dim han = Await app.GetAccounts().Where(Function(x) x.Email.StartsWith("han.solo")).SingleAsync()
			Await humans.AddAccountAsync(han.Email)

			(Await han.IsMemberOfGroupAsync(Me.fixture.PrimaryGroupHref)).ShouldBeTrue()

			(Await humans.RemoveAccountAsync(han.Email)).ShouldBeTrue()
		End Function

		<Theory> _
		<MemberData(nameof(TestClients.GetClients), MemberType := GetType(TestClients))> _
		Public Function Creating_group_in_application(clientBuilder As TestClientProvider) As Task
			Dim client = clientBuilder.GetClient()
			Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

			Dim instance = client.Instantiate(Of IGroup)()
			instance.SetName(".NET ITs New Test Group {this.fixture.TestRunIdentifier}")
			instance.SetDescription("A nu start")
			instance.SetStatus(GroupStatus.Disabled)

			Dim created = Await app.CreateGroupAsync(instance)
			created.Href.ShouldNotBeNullOrEmpty()
			Me.fixture.CreatedGroupHrefs.Add(created.Href)

			created.Name.ShouldBe(".NET ITs New Test Group {this.fixture.TestRunIdentifier}")
			created.Description.ShouldBe("A nu start")
			created.Status.ShouldBe(GroupStatus.Disabled)

			(Await created.DeleteAsync()).ShouldBeTrue()
			Me.fixture.CreatedGroupHrefs.Remove(created.Href)
		End Function

		<Theory> _
		<MemberData(nameof(TestClients.GetClients), MemberType := GetType(TestClients))> _
		Public Function Creating_group_in_directory(clientBuilder As TestClientProvider) As Task
			Dim client = clientBuilder.GetClient()
			Dim directory = Await client.GetResourceAsync(Of IDirectory)(Me.fixture.PrimaryDirectoryHref)

			Dim instance = client.Instantiate(Of IGroup)()
			Dim directoryName = ".NET ITs New Test Group #2 ({this.fixture.TestRunIdentifier} - {clientBuilder.Name})"
			instance.SetName(directoryName)
			instance.SetDescription("A nu start")
			instance.SetStatus(GroupStatus.Enabled)

			Dim created = Await directory.CreateGroupAsync(instance)
			created.Href.ShouldNotBeNullOrEmpty()
			Me.fixture.CreatedGroupHrefs.Add(created.Href)

			created.Name.ShouldBe(directoryName)
			created.Description.ShouldBe("A nu start")
			created.Status.ShouldBe(GroupStatus.Enabled)

			(Await created.DeleteAsync()).ShouldBeTrue()
			Me.fixture.CreatedGroupHrefs.Remove(created.Href)
		End Function

		<Theory> _
		<MemberData(nameof(TestClients.GetClients), MemberType := GetType(TestClients))> _
		Public Function Creating_group_with_custom_data(clientBuilder As TestClientProvider) As Task
			Dim client = clientBuilder.GetClient()
			Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

			Dim instance = client.Instantiate(Of IGroup)()
			instance.SetName(".NET ITs Custom Data Group ({this.fixture.TestRunIdentifier} - {clientBuilder.Name})")
			instance.CustomData.Put("isNeat", True)
			instance.CustomData.Put("roleBasedSecurity", "pieceOfCake")

			Dim created = Await app.CreateGroupAsync(instance)
			Me.fixture.CreatedGroupHrefs.Add(created.Href)

			Dim customData = Await created.GetCustomDataAsync()
			customData("isNeat").ShouldBe(True)
			customData("roleBasedSecurity").ShouldBe("pieceOfCake")

			(Await created.DeleteAsync()).ShouldBeTrue()
			Me.fixture.CreatedGroupHrefs.Remove(created.Href)
		End Function

		<Theory> _
		<MemberData(nameof(TestClients.GetClients), MemberType := GetType(TestClients))> _
		Public Function Creating_group_with_response_options(clientBuilder As TestClientProvider) As Task
			Dim client = clientBuilder.GetClient()
			Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

			Dim group = client.Instantiate(Of IGroup)().SetName(".NET ITs Custom Data Group #2 ({this.fixture.TestRunIdentifier} - {clientBuilder.Name})")

			Await app.CreateGroupAsync(group, Function(opt) opt.ResponseOptions.Expand(Function(x) x.GetCustomData()))

			group.Href.ShouldNotBeNullOrEmpty()
			Me.fixture.CreatedGroupHrefs.Add(group.Href)

			(Await group.DeleteAsync()).ShouldBeTrue()
			Me.fixture.CreatedGroupHrefs.Remove(group.Href)
		End Function
	End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
