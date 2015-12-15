' <copyright file="OauthPolicy_tests.vb" company="Stormpath, Inc.">
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

Imports Shouldly
Imports Stormpath.SDK.Tests.Common.Integration
Imports Xunit

Namespace Async
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class OauthPolicy_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_policy_application(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetApplicationAsync(Me.fixture.PrimaryApplicationHref)

            Dim policy = Await app.GetOauthPolicyAsync()
            policy.ShouldNotBeNull()

            Dim associatedApp = Await policy.GetApplicationAsync()
            associatedApp.ShouldNotBeNull()
            associatedApp.Href.ShouldBe(app.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_policy_tenant(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetApplicationAsync(Me.fixture.PrimaryApplicationHref)

            Dim policy = Await app.GetOauthPolicyAsync()
            policy.ShouldNotBeNull()

            Dim tenant = Await policy.GetTenantAsync()
            tenant.Href.ShouldBe((Await app.GetTenantAsync()).Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_policy(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.CreateApplicationAsync($".NET ITs Default OAuth Policy Application {fixture.TestRunIdentifier}-{clientBuilder.Name}", createDirectory:=False)
            app.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(app.Href)

            Dim policy = Await app.GetOauthPolicyAsync()

            ' Default OAuth policy values are managed by Stormpath.
            policy.ShouldNotBeNull()
            policy.AccessTokenTimeToLive.ShouldBe(TimeSpan.FromHours(1))
            policy.RefreshTokenTimeToLive.ShouldBe(TimeSpan.FromDays(60))
            policy.TokenEndpointHref.ShouldEndWith("oauth/token")

            ' Clean up
            Call (Await app.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(app.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Updating_policy(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.CreateApplicationAsync($".NET ITs Modified OAuth Policy Application {fixture.TestRunIdentifier}-{clientBuilder.Name}", createDirectory:=False)
            app.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(app.Href)

            Dim policy = Await app.GetOauthPolicyAsync()

            policy.SetAccessTokenTimeToLive(TimeSpan.FromDays(8))
            policy.SetRefreshTokenTimeToLive(TimeSpan.FromDays(180))
            Await policy.SaveAsync()

            Dim policyUpdated = Await app.GetOauthPolicyAsync()
            policyUpdated.AccessTokenTimeToLive.ShouldBe(TimeSpan.FromDays(8))
            policyUpdated.RefreshTokenTimeToLive.ShouldBe(TimeSpan.FromDays(180))

            ' Clean up
            Call (Await app.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(app.Href)
        End Function
    End Class
End Namespace