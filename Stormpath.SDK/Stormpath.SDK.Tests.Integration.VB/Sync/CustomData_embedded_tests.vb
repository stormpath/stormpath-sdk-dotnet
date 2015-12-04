' <copyright file="CustomData_embedded_tests.vb" company="Stormpath, Inc.">
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
Imports Stormpath.SDK.Tests.Common
Imports Stormpath.SDK.Tests.Common.Integration
Imports Stormpath.SDK.Tests.Common.RandomData
Imports Xunit

Namespace Stormpath.SDK.Tests.Integration.VB.Sync
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class CustomData_embedded_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        Private Function CreateRandomAccountInstance(client As IClient) As IAccount
            Dim accountObject = client.Instantiate(Of IAccount)()
            accountObject.SetEmail(New RandomEmail("testing.foo"))
            accountObject.SetGivenName("VBTest")
            accountObject.SetSurname("VBTesterman")
            accountObject.SetPassword(New RandomPassword(12))

            Return accountObject
        End Function

        Private Function SaveAccount(client As IClient, instance As IAccount) As IAccount
            Dim app = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim created = app.CreateAccount(instance, Function(x) InlineAssignHelper(x.RegistrationWorkflowEnabled, False))
            Me.fixture.CreatedAccountHrefs.Add(created.Href)

            Return created
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_new_account_with_custom_data(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim newAccount = Me.CreateRandomAccountInstance(client)
            newAccount.CustomData.Put("status", 1337)
            newAccount.CustomData.Put("isAwesome", True)

            Dim created = Me.SaveAccount(client, newAccount)
            Dim customData = created.GetCustomData()

            CInt(customData("status")).ShouldBe(1337)
            CBool(customData("isAwesome")).ShouldBe(True)

            created.Delete().ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(created.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_new_application_with_custom_data(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim newApp = client.Instantiate(Of IApplication)()
            newApp.SetName(".NET IT App with CustomData Test " + RandomString.Create())
            newApp.CustomData.Put("isCool", True)
            newApp.CustomData.Put("my-custom-data", 1234)

            Dim created = client.CreateApplication(newApp, Function(options) InlineAssignHelper(options.CreateDirectory, False))
            Me.fixture.CreatedApplicationHrefs.Add(created.Href)
            Dim customData = created.GetCustomData()

            CBool(customData("isCool")).ShouldBe(True)
            CInt(customData("my-custom-data")).ShouldBe(1234)

            created.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(created.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Editing_embedded_custom_data(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim newAccount = Me.CreateRandomAccountInstance(client)
            newAccount.CustomData.Put("status", 1337)
            newAccount.CustomData.Put("isAwesome", True)

            Dim created = Me.SaveAccount(client, newAccount)
            created.CustomData.Remove("isAwesome")
            created.CustomData.Put("phrase", "testing is neet")
            created.Save()

            Threading.Thread.Sleep(Delay.UpdatePropogation)

            Dim customData = created.GetCustomData()
            CInt(customData("status")).ShouldBe(1337)
            Assert.True(customData("isAwesome") Is Nothing)
            CStr(customData("phrase")).ShouldBe("testing is neet")

            created.Delete().ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(created.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Clearing_embedded_custom_data(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim newAccount = Me.CreateRandomAccountInstance(client)
            newAccount.CustomData.Put("foo", "bar")
            newAccount.CustomData.Put("admin", True)

            Dim created = Me.SaveAccount(client, newAccount)
            Dim customData = created.GetCustomData()
            customData.IsEmptyOrDefault().ShouldBeFalse()

            created.CustomData.Clear()
            created.Save()

            Threading.Thread.Sleep(Delay.UpdatePropogation)

            customData = created.GetCustomData()
            customData.IsEmptyOrDefault().ShouldBeTrue()

            created.Delete().ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(created.Href)
        End Sub

        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function
    End Class
End Namespace