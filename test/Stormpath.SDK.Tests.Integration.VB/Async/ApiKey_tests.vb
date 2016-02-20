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
Imports Stormpath.SDK.Account
Imports Stormpath.SDK.Api
Imports Stormpath.SDK.Auth
Imports Stormpath.SDK.Tests.Common.Integration
Imports Stormpath.SDK.Tests.Common.RandomData
Imports Xunit

Namespace Async
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class ApiKey_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Creating_and_deleting_api_key(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetApplicationAsync(Me.fixture.PrimaryApplicationHref)
            Dim account = Await app.CreateAccountAsync("ApiKey", "Tester1", "api-key-tester-1@foo.foo", New RandomPassword(12))
            Me.fixture.CreatedAccountHrefs.Add(account.Href)

            Call (Await account.GetApiKeys().CountAsync()).ShouldBe(0)

            Dim newKey1 = Await account.CreateApiKeyAsync()
            Dim newKey2 = Await account.CreateApiKeyAsync(Sub(opt)
                                                              opt.Expand(Function(e) e.GetAccount())
                                                              opt.Expand(Function(e) e.GetTenant())
                                                          End Sub)

            Dim keysList = Await account.GetApiKeys().ToListAsync()
            keysList.Count.ShouldBe(2)
            keysList.ShouldContain(Function(x) x.Href = newKey1.Href)
            keysList.ShouldContain(Function(x) x.Href = newKey2.Href)

            Call (Await newKey1.DeleteAsync()).ShouldBeTrue()
            Call (Await newKey2.DeleteAsync()).ShouldBeTrue()

            ' Clean up
            Call (Await account.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Updating_api_key(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetApplicationAsync(Me.fixture.PrimaryApplicationHref)
            Dim account = Await app.CreateAccountAsync("ApiKey", "Tester2", "api-key-tester-2@foo.foo", New RandomPassword(12))
            Me.fixture.CreatedAccountHrefs.Add(account.Href)

            Dim newKey = Await account.CreateApiKeyAsync()
            newKey.Status.ShouldBe(ApiKeyStatus.Enabled)

            newKey.SetStatus(ApiKeyStatus.Disabled)
            Await newKey.SaveAsync()
            newKey.Status.ShouldBe(ApiKeyStatus.Disabled)

            Dim retrieved = Await account.GetApiKeys().SingleAsync()
            retrieved.Status.ShouldBe(ApiKeyStatus.Disabled)

            ' Clean up
            Call (Await newKey.DeleteAsync()).ShouldBeTrue()

            Call (Await account.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Looking_up_api_key_via_application(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetApplicationAsync(Me.fixture.PrimaryApplicationHref)
            Dim account = Await app.CreateAccountAsync("ApiKey", "Tester3", "api-key-tester-3@foo.foo", New RandomPassword(12))
            Me.fixture.CreatedAccountHrefs.Add(account.Href)

            Dim newKey = Await account.CreateApiKeyAsync()

            Dim foundKey = Await app.GetApiKeyAsync(newKey.Id, Sub(opt)
                                                                   opt.Expand(Function(e) e.GetAccount())
                                                                   opt.Expand(Function(e) e.GetTenant())
                                                               End Sub)
            Dim foundAccount = Await foundKey.GetAccountAsync()
            foundAccount.Href.ShouldBe(account.Href)

            ' Clean up
            Call (Await newKey.DeleteAsync()).ShouldBeTrue()

            Call (Await account.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Authenticating_api_key(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetApplicationAsync(Me.fixture.PrimaryApplicationHref)
            Dim account = Await app.CreateAccountAsync("ApiKey", "Tester", New RandomEmail("foo.foo"), New RandomPassword(12))
            Me.fixture.CreatedAccountHrefs.Add(account.Href)

            Dim newKey = Await account.CreateApiKeyAsync()

            Dim apiKeyAuthRequest = New ApiKeyRequestBuilder() _
                .SetId(newKey.Id) _
                .SetSecret(newKey.Secret) _
                .Build()

            Dim result = Await app.AuthenticateAccountAsync(apiKeyAuthRequest)
            Dim resultAccount = Await result.GetAccountAsync()

            resultAccount.Href.ShouldBe(account.Href)

            ' Clean up
            Call (Await newKey.DeleteAsync()).ShouldBeTrue()

            Call (Await account.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Throws_when_id_is_invalid(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetApplicationAsync(Me.fixture.PrimaryApplicationHref)
            Dim account = Await app.CreateAccountAsync("ApiKey", "Tester", New RandomEmail("foo.foo"), New RandomPassword(12))
            Me.fixture.CreatedAccountHrefs.Add(account.Href)

            Dim newKey = Await account.CreateApiKeyAsync()

            Dim apiKeyAuthRequest = New ApiKeyRequestBuilder() _
                .SetId("FOOBAR1") _
                .SetSecret(newKey.Secret) _
                .Build()

            Await Should.ThrowAsync(Of IncorrectCredentialsException)(app.AuthenticateAccountAsync(apiKeyAuthRequest))

            ' Clean up
            Call (Await newKey.DeleteAsync()).ShouldBeTrue()

            Call (Await account.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Throws_when_secret_is_invalid(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetApplicationAsync(Me.fixture.PrimaryApplicationHref)
            Dim account = Await app.CreateAccountAsync("ApiKey", "Tester", New RandomEmail("foo.foo"), New RandomPassword(12))
            Me.fixture.CreatedAccountHrefs.Add(account.Href)

            Dim newKey = Await account.CreateApiKeyAsync()

            Dim apiKeyAuthRequest = New ApiKeyRequestBuilder() _
                .SetId(newKey.Id) _
                .SetSecret("notARealSecret123") _
                .Build()

            Await Should.ThrowAsync(Of IncorrectCredentialsException)(app.AuthenticateAccountAsync(apiKeyAuthRequest))

            ' Clean up
            Call (Await newKey.DeleteAsync()).ShouldBeTrue()

            Call (Await account.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Throws_when_key_is_disabled(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetApplicationAsync(Me.fixture.PrimaryApplicationHref)
            Dim account = Await app.CreateAccountAsync("ApiKey", "Tester", New RandomEmail("foo.foo"), New RandomPassword(12))
            Me.fixture.CreatedAccountHrefs.Add(account.Href)

            Dim newKey = Await account.CreateApiKeyAsync()
            newKey.SetStatus(ApiKeyStatus.Disabled)
            Await newKey.SaveAsync()

            Dim apiKeyAuthRequest = New ApiKeyRequestBuilder() _
                .SetId(newKey.Id) _
                .SetSecret(newKey.Secret) _
                .Build()

            Await Should.ThrowAsync(Of DisabledApiKeyException)(app.AuthenticateAccountAsync(apiKeyAuthRequest))

            ' Clean up
            Call (Await newKey.DeleteAsync()).ShouldBeTrue()

            Call (Await account.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Throws_when_account_is_disabled(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetApplicationAsync(Me.fixture.PrimaryApplicationHref)

            Dim account = Await app.CreateAccountAsync("ApiKey", "Tester", New RandomEmail("foo.foo"), New RandomPassword(12))
            Me.fixture.CreatedAccountHrefs.Add(account.Href)

            account.SetStatus(AccountStatus.Disabled)
            Await account.SaveAsync()

            Dim newKey = Await account.CreateApiKeyAsync()

            Dim apiKeyAuthRequest = New ApiKeyRequestBuilder() _
                .SetId(newKey.Id) _
                .SetSecret(newKey.Secret) _
                .Build()

            Await Should.ThrowAsync(Of DisabledAccountException)(app.AuthenticateAccountAsync(apiKeyAuthRequest))

            ' Clean up
            Call (Await newKey.DeleteAsync()).ShouldBeTrue()

            Call (Await account.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Function
    End Class
End Namespace