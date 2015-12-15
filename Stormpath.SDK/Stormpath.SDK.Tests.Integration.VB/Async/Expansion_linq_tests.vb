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
Imports Stormpath.SDK.Tests.Common.Integration
Imports Xunit

Namespace Async
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class Expansion_linq_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_custom_data(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim account = Await tenant.GetAccounts() _
                .Where(Function(x) x.Email.StartsWith("lskywalker")) _
                .Expand(Function(x) x.GetCustomData()) _
                .FirstOrDefaultAsync()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_directory(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim account = Await tenant.GetAccounts() _
                .Where(Function(x) x.Email.StartsWith("lskywalker")) _
                .Expand(Function(x) x.GetDirectory()) _
                .FirstOrDefaultAsync()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_group_memberships(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim account = Await tenant.GetAccounts() _
                .Where(Function(x) x.Email.StartsWith("lskywalker")) _
                .Expand(Function(x) x.GetGroupMemberships(Nothing, 10)) _
                .FirstOrDefaultAsync()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_groups(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim account = Await tenant.GetAccounts() _
                .Where(Function(x) x.Email.StartsWith("lskywalker")) _
                .Expand(Function(x) x.GetGroups(Nothing, 10)) _
                .FirstOrDefaultAsync()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_provider_data(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim account = Await tenant.GetAccounts() _
                .Where(Function(x) x.Email.StartsWith("lskywalker")) _
                .Expand(Function(x) x.GetProviderData()) _
                .FirstOrDefaultAsync()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_tenant(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim account = Await tenant.GetAccounts() _
                .Where(Function(x) x.Email.StartsWith("lskywalker")) _
                .Expand(Function(x) x.GetTenant()) _
                .FirstOrDefaultAsync()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_accounts(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim account = Await tenant.GetApplications() _
                .Where(Function(x) x.Description = "The Battle of Endor") _
                .Expand(Function(x) x.GetAccounts(Nothing, 10)) _
                .FirstOrDefaultAsync()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_account_store_mappings(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim account = Await tenant.GetApplications() _
                .Where(Function(x) x.Description = "The Battle of Endor") _
                .Expand(Function(x) x.GetAccountStoreMappings(Nothing, 10)) _
                .FirstOrDefaultAsync()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_organization_account_store_mappings(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim account = Await tenant _
                .GetOrganizations() _
                .Where(Function(x) x.Description = "Star Wars") _
                .Expand(Function(x) x.GetAccountStoreMappings(Nothing, 10)) _
                .FirstOrDefaultAsync()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_default_account_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim account = Await tenant.GetApplications() _
                .Where(Function(x) x.Description = "The Battle of Endor") _
                .Expand(Function(x) x.GetDefaultAccountStore()) _
                .FirstOrDefaultAsync()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_default_group_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim account = Await tenant.GetApplications() _
                .Where(Function(x) x.Description = "The Battle of Endor") _
                .Expand(Function(x) x.GetDefaultGroupStore()) _
                .FirstOrDefaultAsync()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_provider(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim account = Await tenant.GetDirectories() _
                .Filter("(primary)") _
                .Expand(Function(x) x.GetProvider()) _
                .FirstOrDefaultAsync()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_account_memberships(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim account = Await tenant.GetGroups() _
                .Where(Function(x) x.Description = "Humans") _
                .Expand(Function(x) x.GetAccountMemberships(Nothing, 10)) _
                .FirstOrDefaultAsync()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_membership_account(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim group = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)

            Dim membership = Await group.GetAccountMemberships() _
                .Expand(Function(x) x.GetAccount()) _
                .FirstOrDefaultAsync()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_membership_group(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim group = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref)

            Dim membership = Await group.GetAccountMemberships() _
                .Expand(Function(x) x.GetGroup()) _
                .FirstOrDefaultAsync()
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_oAuthPolicy(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            Dim app = Await tenant _
                .GetApplications() _
                .Expand(Function(x) x.GetOauthPolicy()) _
                .FirstOrDefaultAsync()
        End Function
    End Class
End Namespace