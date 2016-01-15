' <copyright file="OauthPolicy_tests.vb" company="Stormpath, Inc.">
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
Imports Stormpath.SDK.Tests.Common.Integration
Imports Stormpath.SDK.Sync
Imports Xunit

Namespace Sync
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class OauthPolicy_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_policy_application(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetApplication(Me.fixture.PrimaryApplicationHref)

            Dim policy = app.GetOauthPolicy()
            policy.ShouldNotBeNull()

            Dim associatedApp = policy.GetApplication()
            associatedApp.ShouldNotBeNull()
            associatedApp.Href.ShouldBe(app.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_policy_tenant(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetApplication(Me.fixture.PrimaryApplicationHref)

            Dim policy = app.GetOauthPolicy()
            policy.ShouldNotBeNull()

            Dim tenant = policy.GetTenant()
            tenant.Href.ShouldBe(app.GetTenant().Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_policy(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.CreateApplication($".NET ITs Default OAuth Policy Application {fixture.TestRunIdentifier}-{clientBuilder.Name}", createDirectory:=False)
            app.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(app.Href)

            Dim policy = app.GetOauthPolicy()

            ' Default OAuth policy values are managed by Stormpath.
            policy.ShouldNotBeNull()
            policy.AccessTokenTimeToLive.ShouldBe(TimeSpan.FromHours(1))
            policy.RefreshTokenTimeToLive.ShouldBe(TimeSpan.FromDays(60))
            policy.TokenEndpointHref.ShouldEndWith("oauth/token")

            ' Clean up
            app.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(app.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Updating_policy(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.CreateApplication($".NET ITs Modified OAuth Policy Application {fixture.TestRunIdentifier}-{clientBuilder.Name}", createDirectory:=False)
            app.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(app.Href)

            Dim policy = app.GetOauthPolicy()

            policy.SetAccessTokenTimeToLive(TimeSpan.FromDays(8))
            policy.SetRefreshTokenTimeToLive(TimeSpan.FromDays(180))
            policy.Save()

            Dim policyUpdated = app.GetOauthPolicy()
            policyUpdated.AccessTokenTimeToLive.ShouldBe(TimeSpan.FromDays(8))
            policyUpdated.RefreshTokenTimeToLive.ShouldBe(TimeSpan.FromDays(180))

            ' Clean up
            app.Delete().ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(app.Href)
        End Sub
    End Class
End Namespace