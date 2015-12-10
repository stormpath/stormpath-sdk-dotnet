' <copyright file="Expansion_retrieval_tests.vb" company="Stormpath, Inc.">
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
Imports Stormpath.SDK.Account
Imports Stormpath.SDK.AccountStore
Imports Stormpath.SDK.Application
Imports Stormpath.SDK.Group
Imports Stormpath.SDK.Organization
Imports Stormpath.SDK.Tenant
Imports Stormpath.SDK.Tests.Common.Integration
Imports Xunit

Namespace Async
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class Expansion_retrieval_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_custom_data(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim account = Await client.GetResourceAsync(Of IAccount)(Me.fixture.PrimaryAccountHref, Function(o) o.Expand(Function(x) x.GetCustomData()))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_directory(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim account = Await client.GetResourceAsync(Of IAccount)(Me.fixture.PrimaryAccountHref, Function(o) o.Expand(Function(x) x.GetDirectory()))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_group_memberships(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim account = Await client.GetResourceAsync(Of IAccount)(Me.fixture.PrimaryAccountHref, Function(o) o.Expand(Function(x) x.GetGroupMemberships(0, 10)))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_groups(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim account = Await client.GetResourceAsync(Of IAccount)(Me.fixture.PrimaryAccountHref, Function(o) o.Expand(Function(x) x.GetGroups(0, 10)))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_tenant(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim account = Await client.GetResourceAsync(Of IAccount)(Me.fixture.PrimaryAccountHref, Function(o) o.Expand(Function(x) x.GetTenant()))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_account_store(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim mapping = Await app.GetAccountStoreMappings().FirstAsync()

            Await client.GetResourceAsync(Of IAccountStoreMapping)(mapping.Href, Function(o) o.Expand(Function(x) x.GetAccountStore()))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        <Obsolete("Remove this test after 1.0 breaking change.")>
        Public Async Function Expanding_application_from_generic_mapping(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim mapping = Await app.GetAccountStoreMappings().FirstAsync()

            Await client.GetResourceAsync(Of IAccountStoreMapping)(mapping.Href, Function(o) o.Expand(Function(x) x.GetApplication()))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_application(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim mapping = Await app.GetAccountStoreMappings().FirstAsync()

            Await client.GetResourceAsync(Of IApplicationAccountStoreMapping)(mapping.Href, Function(o) o.Expand(Function(x) x.GetApplication()))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_organization(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim app = Await client.GetResourceAsync(Of IOrganization)(Me.fixture.PrimaryOrganizationHref)

            Dim mapping = Await app.GetAccountStoreMappings().FirstAsync()

            Await client.GetResourceAsync(Of IOrganizationAccountStoreMapping)(mapping.Href, Function(o) o.Expand(Function(x) x.GetOrganization()))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_accounts(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref, Function(o) o.Expand(Function(x) x.GetAccounts(0, 10)))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_account_store_mappings(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim app = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref, Function(o) o.Expand(Function(x) x.GetAccountStoreMappings(0, 10)))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_organization_account_store_mappings(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim app = Await client.GetResourceAsync(Of IOrganization)(Me.fixture.PrimaryOrganizationHref, Function(o) o.Expand(Function(x) x.GetAccountStoreMappings(0, 10)))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_account_memberships(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim group = Await client.GetResourceAsync(Of IGroup)(Me.fixture.PrimaryGroupHref, Function(o) o.Expand(Function(x) x.GetAccountMemberships(0, 10)))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_applications(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim app = Await client.GetResourceAsync(Of ITenant)(Me.fixture.TenantHref, Function(o) o.Expand(Function(x) x.GetApplications(0, 10)))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_directories(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim app = Await client.GetResourceAsync(Of ITenant)(Me.fixture.TenantHref, Function(o) o.Expand(Function(x) x.GetDirectories(0, 10)))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_account(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim account = Await client.GetResourceAsync(Of IAccount)(Me.fixture.PrimaryAccountHref)
            Dim membership = Await account.GetGroupMemberships().FirstAsync()

            Await client.GetResourceAsync(Of IGroupMembership)(membership.Href, Function(o) o.Expand(Function(x) x.GetAccount()))
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Expanding_group(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()

            Dim account = Await client.GetResourceAsync(Of IAccount)(Me.fixture.PrimaryAccountHref)
            Dim membership = Await account.GetGroupMemberships().FirstAsync()

            Await client.GetResourceAsync(Of IGroupMembership)(membership.Href, Function(o) o.Expand(Function(x) x.GetGroup()))
        End Function
    End Class
End Namespace