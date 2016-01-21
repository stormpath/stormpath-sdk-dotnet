' <copyright file="ApiKey_tests.vb" company="Stormpath, Inc.">
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

Imports Shouldly
Imports Stormpath.SDK.Api
Imports Stormpath.SDK.Sync
Imports Stormpath.SDK.Tests.Common.Integration
Imports Stormpath.SDK.Tests.Common.RandomData
Imports Xunit

Namespace Sync
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class ApiKey_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_and_deleting_api_key(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetApplication(Me.fixture.PrimaryApplicationHref)
            Dim account = app.CreateAccount("ApiKey", "Tester1", "api-key-tester-1@foo.foo", New RandomPassword(12))
            Me.fixture.CreatedAccountHrefs.Add(account.Href)

            Call (account.GetApiKeys().Synchronously().Count()).ShouldBe(0)

            Dim newKey1 = account.CreateApiKey()
            Dim newKey2 = account.CreateApiKey(Sub(opt)
                                                   opt.Expand(Function(e) e.GetAccount())
                                                   opt.Expand(Function(e) e.GetTenant())
                                               End Sub)

            Dim keysList = account.GetApiKeys().Synchronously().ToList()
            keysList.Count.ShouldBe(2)
            keysList.ShouldContain(Function(x) x.Href = newKey1.Href)
            keysList.ShouldContain(Function(x) x.Href = newKey2.Href)

            Call (newKey1.Delete()).ShouldBeTrue()
            Call (newKey2.Delete()).ShouldBeTrue()

            ' Clean up
            Call (account.Delete()).ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Updating_api_key(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetApplication(Me.fixture.PrimaryApplicationHref)
            Dim account = app.CreateAccount("ApiKey", "Tester2", "api-key-tester-2@foo.foo", New RandomPassword(12))
            Me.fixture.CreatedAccountHrefs.Add(account.Href)

            Dim newKey = account.CreateApiKey()
            newKey.Status.ShouldBe(ApiKeyStatus.Enabled)

            newKey.SetStatus(ApiKeyStatus.Disabled)
            newKey.Save()
            newKey.Status.ShouldBe(ApiKeyStatus.Disabled)

            Dim retrieved = account.GetApiKeys().Synchronously().Single()
            retrieved.Status.ShouldBe(ApiKeyStatus.Disabled)

            ' Clean up
            Call (newKey.Delete()).ShouldBeTrue()

            Call (account.Delete()).ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Looking_up_api_key_via_application(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetApplication(Me.fixture.PrimaryApplicationHref)
            Dim account = app.CreateAccount("ApiKey", "Tester3", "api-key-tester-3@foo.foo", New RandomPassword(12))
            Me.fixture.CreatedAccountHrefs.Add(account.Href)

            Dim newKey = account.CreateApiKey()

            Dim foundKey = app.GetApiKey(newKey.Id, Sub(opt)
                                                        opt.Expand(Function(e) e.GetAccount())
                                                        opt.Expand(Function(e) e.GetTenant())
                                                    End Sub)
            Dim foundAccount = foundKey.GetAccount()
            foundAccount.Href.ShouldBe(account.Href)

            ' Clean up
            Call (newKey.Delete()).ShouldBeTrue()

            Call (account.Delete()).ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Sub
    End Class
End Namespace