' <copyright file="Tenant_tests.vb" company="Stormpath, Inc.">
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
Imports Stormpath.SDK.Tests.Common.Integration
Imports Xunit

Namespace Async
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class Tenant_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_current_tenant(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            tenant.ShouldNotBe(Nothing)
            tenant.Href.ShouldNotBe(Nothing)
            tenant.Name.ShouldNotBe(Nothing)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_account(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim account = Await tenant.GetAccountAsync(Me.fixture.PrimaryAccountHref)
            account.Href.ShouldBe(Me.fixture.PrimaryAccountHref)
            account.FullName.ShouldBe("Luke Skywalker")
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_application(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim app = Await tenant.GetApplicationAsync(Me.fixture.PrimaryApplicationHref)
            app.Href.ShouldBe(Me.fixture.PrimaryApplicationHref)
            app.Description.ShouldBe("The Battle of Endor")
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_directory(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim directory = Await tenant.GetDirectoryAsync(Me.fixture.PrimaryDirectoryHref)
            directory.Href.ShouldBe(Me.fixture.PrimaryDirectoryHref)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_group(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim group = Await tenant.GetGroupAsync(Me.fixture.PrimaryGroupHref)
            group.Href.ShouldBe(Me.fixture.PrimaryGroupHref)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_organization(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim org = Await tenant.GetOrganizationAsync(Me.fixture.PrimaryOrganizationHref)
            org.NameKey.ShouldBe(Me.fixture.PrimaryOrganizationNameKey)
            org.Href.ShouldBe(Me.fixture.PrimaryOrganizationHref)
        End Function
    End Class
End Namespace