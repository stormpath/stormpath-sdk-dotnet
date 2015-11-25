' <copyright file="CustomData_tests.vb" company="Stormpath, Inc.">
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
Imports Stormpath.SDK.Account
Imports Stormpath.SDK.Application
Imports Stormpath.SDK.Client
Imports Stormpath.SDK.Sync
Imports Stormpath.SDK.Tests.Common.Integration
Imports Stormpath.SDK.Tests.Common.RandomData
Imports Xunit

Namespace Stormpath.SDK.Tests.Integration.VB.Sync
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class CustomData_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        Private Function CreateRandomAccount(client As IClient) As IAccount
            Dim accountObject = client.Instantiate(Of IAccount)()
            accountObject.SetEmail(New RandomEmail("testing.foo.vb"))
            accountObject.SetGivenName("Test")
            accountObject.SetSurname("Testerman")
            accountObject.SetPassword(New RandomPassword(12))

            Dim app = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim created = app.CreateAccount(accountObject, Function(x) InlineAssignHelper(x.RegistrationWorkflowEnabled, False))
            Me.fixture.CreatedAccountHrefs.Add(created.Href)

            Return created
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_account_custom_data(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim account = client.GetResource(Of IAccount)(Me.fixture.CreatedAccountHrefs.First())
            Dim customData = account.GetCustomData()

            customData.IsEmptyOrDefault().ShouldBeTrue()
            DirectCast(customData.[Get]("href"), String).ShouldNotBeNullOrEmpty()
            DirectCast(customData.[Get]("createdAt"), DateTimeOffset).ShouldNotBeNull()
            DirectCast(customData.[Get]("modifiedAt"), DateTimeOffset).ShouldNotBeNull()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Putting_account_custom_data(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim account = Me.CreateRandomAccount(client)
            Dim customData = account.GetCustomData()
            customData.IsEmptyOrDefault().ShouldBeTrue()

            ' Add some custom data
            customData.Put("appStatsId", 12345)
            Dim updated = customData.Save()
            updated.IsEmptyOrDefault().ShouldBeFalse()
            updated.Count().ShouldBe(4)

            ' Cleanup
            account.Delete().ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Deleting_all_custom_data(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim account = Me.CreateRandomAccount(client)
            Dim customData = account.GetCustomData()
            customData.IsEmptyOrDefault().ShouldBeTrue()

            ' Add some custom data
            customData.Put("admin", True)
            customData.Put("status", 1337)
            customData.Put("text", "foobar")
            Dim updated = customData.Save()
            updated.Count().ShouldBe(6)

            ' Try deleting
            Dim result = updated.Delete()
            Dim newCustomData = account.GetCustomData()
            newCustomData.Count().ShouldBe(3)

            ' Cleanup
            account.Delete().ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Clearing_all_custom_data(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim account = Me.CreateRandomAccount(client)
            Dim customData = account.GetCustomData()
            customData.IsEmptyOrDefault().ShouldBeTrue()

            ' Add some custom data
            customData.Put("admin", True)
            customData.Put("status", 1337)
            customData.Put("text", "foobar")
            Dim updated = customData.Save()
            updated.Count().ShouldBe(6)

            ' Expected behavior: works the same as calling Delete (see Deleting_all_custom_data)
            updated.Clear()
            Dim result = updated.Save()

            ' Let the cache update
            System.Threading.Thread.Sleep(100)

            Dim newCustomData = account.GetCustomData()
            newCustomData.Count().ShouldBe(3)

            ' Cleanup
            account.Delete().ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Deleting_single_item(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim account = Me.CreateRandomAccount(client)
            Dim customData = account.GetCustomData()
            customData.IsEmptyOrDefault().ShouldBeTrue()

            ' Add some custom data...
            customData.Put("claims", "canEdit,canCreate")
            customData.Put("text", "fizzbuzz")
            Dim updated = customData.Save()
            updated.Count().ShouldBe(5)

            ' ... and then delete one
            updated.Remove("claims")
            Dim updated2 = updated.Save()
            Assert.Null(updated2.[Get]("claims"))
            CStr(updated2.[Get]("text")).ShouldBe("fizzbuzz")

            ' Cleanup
            account.Delete().ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Sub
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function
    End Class
End Namespace