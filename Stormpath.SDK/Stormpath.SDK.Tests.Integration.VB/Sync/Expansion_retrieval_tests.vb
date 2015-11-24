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
Imports Stormpath.SDK.Linq
Imports Stormpath.SDK.Sync
Imports Stormpath.SDK.Tenant
Imports Stormpath.SDK.Tests.Common.Integration
Imports Xunit

Namespace Stormpath.SDK.Tests.Integration.VB.Sync
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class Expansion_retrieval_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_custom_data(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim account = client.GetResource(Of IAccount)(Me.fixture.PrimaryAccountHref, Function(o) o.Expand(Function(x) x.GetCustomData()))
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_directory(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim account = client.GetResource(Of IAccount)(Me.fixture.PrimaryAccountHref, Function(o) o.Expand(Function(x) x.GetDirectory()))
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_group_memberships(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim account = client.GetResource(Of IAccount)(Me.fixture.PrimaryAccountHref, Function(o) o.Expand(Function(x) x.GetGroupMemberships(0, 10)))
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_groups(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim account = client.GetResource(Of IAccount)(Me.fixture.PrimaryAccountHref, Function(o) o.Expand(Function(x) x.GetGroups(0, 10)))
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_tenant(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim account = client.GetResource(Of IAccount)(Me.fixture.PrimaryAccountHref, Function(o) o.Expand(Function(x) x.GetTenant()))
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_account_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim mapping = app.GetAccountStoreMappings().Synchronously().First()

            client.GetResource(Of IAccountStoreMapping)(mapping.Href, Function(o) o.Expand(Function(x) x.GetAccountStore()))
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_application(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim app = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim mapping = app.GetAccountStoreMappings().Synchronously().First()

            client.GetResource(Of IAccountStoreMapping)(mapping.Href, Function(o) o.Expand(Function(x) x.GetApplication()))
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_accounts(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim app = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref, Function(o) o.Expand(Function(x) x.GetAccounts(0, 10)))
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_account_store_mappings(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim app = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref, Function(o) o.Expand(Function(x) x.GetAccountStoreMappings(0, 10)))
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_account_memberships(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim group = client.GetResource(Of IGroup)(Me.fixture.PrimaryGroupHref, Function(o) o.Expand(Function(x) x.GetAccountMemberships(0, 10)))
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_applications(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim app = client.GetResource(Of ITenant)(Me.fixture.TenantHref, Function(o) o.Expand(Function(x) x.GetApplications(0, 10)))
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_directories(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim app = client.GetResource(Of ITenant)(Me.fixture.TenantHref, Function(o) o.Expand(Function(x) x.GetDirectories(0, 10)))
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_account(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim account = client.GetResource(Of IAccount)(Me.fixture.PrimaryAccountHref)
            Dim membership = account.GetGroupMemberships().Synchronously().First()

            client.GetResource(Of IGroupMembership)(membership.Href, Function(o) o.Expand(Function(x) x.GetAccount()))
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Expanding_group(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()

            Dim account = client.GetResource(Of IAccount)(Me.fixture.PrimaryAccountHref)
            Dim membership = account.GetGroupMemberships().Synchronously().First()

            client.GetResource(Of IGroupMembership)(membership.Href, Function(o) o.Expand(Function(x) x.GetGroup()))
        End Sub
    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
