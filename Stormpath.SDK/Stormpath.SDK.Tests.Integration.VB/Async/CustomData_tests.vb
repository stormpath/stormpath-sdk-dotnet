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
Imports Stormpath.SDK.Tests.Common
Imports Stormpath.SDK.Tests.Common.Integration
Imports Stormpath.SDK.Tests.Common.RandomData
Imports Xunit

Namespace Async
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class CustomData_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        Private Async Function CreateRandomAccountAsync(client As IClient) As Task(Of IAccount)
            Dim accountObject = client.Instantiate(Of IAccount)()
            accountObject.SetEmail(New RandomEmail("testing.foo.vb"))
            accountObject.SetGivenName("Test")
            accountObject.SetSurname("Testerman")
            accountObject.SetPassword(New RandomPassword(12))

            Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim created = Await app.CreateAccountAsync(accountObject, Function(x) InlineAssignHelper(x.RegistrationWorkflowEnabled, False))
            Me.fixture.CreatedAccountHrefs.Add(created.Href)

            Return created
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_account_custom_data(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim account = Await client.GetResourceAsync(Of IAccount)(Me.fixture.CreatedAccountHrefs.First())
            Dim customData = Await account.GetCustomDataAsync()

            customData.IsEmptyOrDefault().ShouldBeTrue()
            DirectCast(customData.[Get]("href"), String).ShouldNotBeNullOrEmpty()
            DirectCast(customData.[Get]("createdAt"), DateTimeOffset).ShouldNotBeNull()
            DirectCast(customData.[Get]("modifiedAt"), DateTimeOffset).ShouldNotBeNull()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Putting_account_custom_data(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim account = Await Me.CreateRandomAccountAsync(client)
            Dim customData = Await account.GetCustomDataAsync()
            customData.IsEmptyOrDefault().ShouldBeTrue()

            ' Add some custom data
            customData.Put("appStatsId", 12345)
            Dim updated = Await customData.SaveAsync()
            updated.IsEmptyOrDefault().ShouldBeFalse()
            updated.Count().ShouldBe(4)

            ' Cleanup
            Assert.True(Await account.DeleteAsync())
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Deleting_all_custom_data(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim account = Await Me.CreateRandomAccountAsync(client)
            Dim customData = Await account.GetCustomDataAsync()
            customData.IsEmptyOrDefault().ShouldBeTrue()

            ' Add some custom data
            customData.Put("admin", True)
            customData.Put("status", 1337)
            customData.Put("text", "foobar")
            Dim updated = Await customData.SaveAsync()
            updated.Count().ShouldBe(6)

            ' Try deleting
            Await updated.DeleteAsync()
            Await Task.Delay(Delay.UpdatePropogation)

            Dim newCustomData = Await account.GetCustomDataAsync()
            newCustomData.Count().ShouldBe(3)

            ' Cleanup
            Assert.True(Await account.DeleteAsync())
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Clearing_all_custom_data(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim account = Await Me.CreateRandomAccountAsync(client)
            Dim customData = Await account.GetCustomDataAsync()
            customData.IsEmptyOrDefault().ShouldBeTrue()

            ' Add some custom data
            customData.Put("admin", True)
            customData.Put("status", 1337)
            customData.Put("text", "foobar")
            Dim updated = Await customData.SaveAsync()
            updated.Count().ShouldBe(6)

            ' Expected behavior: works the same as calling DeleteAsync (see Deleting_all_custom_data)
            updated.Clear()
            Dim result = Await updated.SaveAsync()

            Await Task.Delay(Delay.UpdatePropogation)

            Dim newCustomData = Await account.GetCustomDataAsync()
            newCustomData.Count().ShouldBe(3)

            ' Cleanup
            Assert.True(Await account.DeleteAsync())
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Deleting_single_item(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim account = Await Me.CreateRandomAccountAsync(client)
            Dim customData = Await account.GetCustomDataAsync()
            customData.IsEmptyOrDefault().ShouldBeTrue()

            ' Add some custom data...
            customData.Put("claims", "canEdit,canCreate")
            customData.Put("text", "fizzbuzz")
            Dim updated = Await customData.SaveAsync()
            updated.Count().ShouldBe(5)

            ' ... and then delete one
            updated.Remove("claims")
            Await updated.SaveAsync()

            Await Task.Delay(Delay.UpdatePropogation)

            Dim updated2 = Await account.GetCustomDataAsync()
            Assert.Null(updated2.[Get]("claims"))
            CStr(updated2.[Get]("text")).ShouldBe("fizzbuzz")

            ' Cleanup
            Assert.True(Await account.DeleteAsync())
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Function

        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function
    End Class
End Namespace