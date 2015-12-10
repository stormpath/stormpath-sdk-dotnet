' <copyright file="Expansion_linq_tests.vb" company="Stormpath, Inc.">
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
Imports Stormpath.SDK.Group
Imports Stormpath.SDK.Sync
Imports Stormpath.SDK.Tests.Common.Integration
Imports Xunit

Namespace Stormpath.SDK.Tests.Integration.VB.Sync
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class Expansion_linq_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_custom_data(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim account = tenant.GetAccounts().Synchronously().Where(Function(x) x.Email.StartsWith("lskywalker")).Expand(Function(x) x.GetCustomData()).FirstOrDefault()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_directory(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim account = tenant.GetAccounts().Synchronously().Where(Function(x) x.Email.StartsWith("lskywalker")).Expand(Function(x) x.GetDirectory()).FirstOrDefault()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_group_memberships(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim account = tenant.GetAccounts().Synchronously().Where(Function(x) x.Email.StartsWith("lskywalker")).Expand(Function(x) x.GetGroupMemberships()).FirstOrDefault()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_groups(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim account = tenant.GetAccounts().Synchronously().Where(Function(x) x.Email.StartsWith("lskywalker")).Expand(Function(x) x.GetGroups()).FirstOrDefault()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_provider_data(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim account = tenant.GetAccounts().Synchronously().Where(Function(x) x.Email.StartsWith("lskywalker")).Expand(Function(x) x.GetProviderData()).FirstOrDefault()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_tenant(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim account = tenant.GetAccounts().Synchronously().Where(Function(x) x.Email.StartsWith("lskywalker")).Expand(Function(x) x.GetTenant()).FirstOrDefault()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_accounts(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim account = tenant.GetApplications().Synchronously().Where(Function(x) x.Description = "The Battle of Endor").Expand(Function(x) x.GetAccounts()).FirstOrDefault()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_account_store_mappings(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim account = tenant.GetApplications().Synchronously().Where(Function(x) x.Description = "The Battle of Endor").Expand(Function(x) x.GetAccountStoreMappings(Nothing, 10)).FirstOrDefault()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_organization_account_store_mappings(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim account = tenant _
                .GetOrganizations() _
                .Synchronously() _
                .Where(Function(x) x.Description = "The Battle of Endor") _
                .Expand(Function(x) x.GetAccountStoreMappings(Nothing, 10)) _
                .FirstOrDefault()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_default_account_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim account = tenant.GetApplications().Synchronously().Where(Function(x) x.Description = "The Battle of Endor").Expand(Function(x) x.GetDefaultAccountStore()).FirstOrDefault()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_default_group_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim account = tenant.GetApplications().Synchronously().Where(Function(x) x.Description = "The Battle of Endor").Expand(Function(x) x.GetDefaultGroupStore()).FirstOrDefault()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_provider(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim account = tenant.GetDirectories().Synchronously().Filter("(primary)").Expand(Function(x) x.GetProvider()).FirstOrDefault()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_account_memberships(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim account = tenant.GetGroups().Synchronously().Where(Function(x) x.Description = "Humans").Expand(Function(x) x.GetAccountMemberships(Nothing, 10)).FirstOrDefault()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_membership_account(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim group = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref)

            Dim membership = group.GetAccountMemberships().Synchronously().Expand(Function(x) x.GetAccount()).FirstOrDefault()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_membership_group(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim group = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref)

            Dim membership = group.GetAccountMemberships().Synchronously().Expand(Function(x) x.GetGroup()).FirstOrDefault()
        End Sub
    End Class
End Namespace