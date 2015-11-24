
' <copyright file="Account_tests.cs" company="Stormpath, Inc.">
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
Imports System.Linq
Imports Shouldly
Imports Stormpath.SDK.Account
Imports Stormpath.SDK.Application
Imports Stormpath.SDK.Auth
Imports Stormpath.SDK.Error
Imports Stormpath.SDK.Linq
Imports Stormpath.SDK.Sync
Imports Stormpath.SDK.Tests.Common.Integration
Imports Stormpath.SDK.Tests.Common.RandomData
Imports Xunit

Namespace Stormpath.SDK.Tests.Integration.VB.Sync
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class Account_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_tenant_accounts(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            Dim accounts = tenant.GetAccounts().Synchronously().ToList()

            accounts.Any().ShouldBeTrue()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_accounts(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim accounts = application.GetAccounts().Synchronously().ToList()

            ' Verify data from IntegrationTestData
            accounts.Count.ShouldBeGreaterThanOrEqualTo(8)

            Dim luke = accounts.Where(Function(x) x.GivenName = "Luke").[Single]()
            luke.FullName.ShouldBe("Luke Skywalker")
            luke.Email.ShouldBe("lskywalker@tattooine.rim")
            luke.Username.ShouldStartWith("sonofthesuns")
            luke.Status.ShouldBe(AccountStatus.Enabled)

            Dim vader = accounts.Where(Function(x) x.Surname = "Vader").[Single]()
            vader.FullName.ShouldBe("Darth Vader")
            vader.Email.ShouldStartWith("vader@galacticempire.co")
            vader.Username.ShouldStartWith("lordvader")
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_account_provider_data(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim luke = application.GetAccounts().Synchronously().Where(Function(x) x.Email.StartsWith("lskywalker")).[Single]()

            Dim providerData = luke.GetProviderData()
            providerData.Href.ShouldNotBeNullOrEmpty()
            providerData.ProviderId.ShouldBe("stormpath")
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Updating_account(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim leia = application.GetAccounts().Synchronously().Where(Function(a) a.Email = "leia.organa@alderaan.core").[Single]()

            leia.SetMiddleName("Organa")
            leia.SetSurname("Solo")
            Dim saveResult = leia.Save()

            ' In 8 ABY of course
            saveResult.FullName.ShouldBe("Leia Organa Solo")
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Saving_with_response_options(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim chewie = application.GetAccounts().Synchronously().Where(Function(a) a.Email = "chewie@kashyyyk.rim").[Single]()

            chewie.SetUsername($"rwaaargh-{fixture.TestRunIdentifier}")
            chewie.Save(Function(response) response.Expand(Function(x) x.GetCustomData()))
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_account_directory(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim luke = application.GetAccounts().Synchronously().Filter("Luke").[Single]()

            ' Verify data from IntegrationTestData
            Dim directoryHref = luke.GetDirectory().Href
            directoryHref.ShouldBe(Me.fixture.PrimaryDirectoryHref)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_account_tenant(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim leia = application.GetAccounts().Synchronously().Filter("Leia").[Single]()

            ' Verify data from IntegrationTestData
            Dim tenantHref = leia.GetTenant().Href
            tenantHref.ShouldBe(Me.fixture.TenantHref)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Searching_accounts_by_email(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim coreCitizens = application.GetAccounts().Synchronously().Where(Function(acct) acct.Email.EndsWith(".core")).ToList()

            ' Verify data from IntegrationTestData
            coreCitizens.Count.ShouldBe(2)

            Dim han = coreCitizens.Where(Function(x) x.GivenName = "Han").[Single]()
            han.Email.ShouldBe("han.solo@corellia.core")
            han.Username.ShouldStartWith("cptsolo")

            Dim leia = coreCitizens.Where(Function(x) x.GivenName = "Leia").[Single]()
            leia.Email.ShouldStartWith("leia.organa@alderaan.core")
            leia.Username.ShouldStartWith("princessleia")
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Searching_accounts_by_firstname(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim chewie = application.GetAccounts().Synchronously().Where(Function(a) a.GivenName = "Chewbacca").[Single]()

            ' Verify data from IntegrationTestData
            chewie.FullName.ShouldBe("Chewbacca the Wookiee")
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Searching_accounts_by_lastname(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim palpatine = application.GetAccounts().Synchronously().Where(Function(a) a.Surname = "Palpatine").[Single]()

            ' Verify data from IntegrationTestData
            palpatine.FullName.ShouldBe("Emperor Palpatine")
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Searching_accounts_by_middle_name(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim chewie = application.GetAccounts().Synchronously().Where(Function(a) a.MiddleName = "the").[Single]()

            ' Verify data from IntegrationTestData
            chewie.FullName.ShouldBe("Chewbacca the Wookiee")
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Searching_accounts_by_username(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim vader = application.GetAccounts().Synchronously().Where(Function(a) a.Username.Equals($"lordvader-{fixture.TestRunIdentifier}")).[Single]()

            ' Verify data from IntegrationTestData
            vader.Email.ShouldBe("vader@galacticempire.co")
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Searching_accounts_by_status(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim tarkin = application.GetAccounts().Synchronously().Where(Function(x) x.Status = AccountStatus.Disabled).[Single]()

            ' Verify data from IntegrationTestData
            tarkin.FullName.ShouldBe("Wilhuff Tarkin")
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Searching_accounts_by_creation_date(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            ' Make a new account that's created now
            Dim createdAfter = DateTime.Now.Subtract(TimeSpan.FromSeconds(10))
            Dim newAccount = application.CreateAccount("Wedge", "Antilles", "wedge@gus-treta.corellia.core", New RandomPassword(12))
            Me.fixture.CreatedAccountHrefs.Add(newAccount.Href)

            Dim rightBeforeCreation = newAccount.CreatedAt.Subtract(TimeSpan.FromSeconds(1))
            Dim createdRecently = application.GetAccounts().Synchronously().Where(Function(x) x.CreatedAt >= rightBeforeCreation).ToList()
            Dim wedge = createdRecently.Where(Function(x) x.Email = "wedge@gus-treta.corellia.core").[Single]()
            wedge.FullName.ShouldBe("Wedge Antilles")

            ' Clean up
            newAccount.Delete().ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(newAccount.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Searching_accounts_by_creation_date_within_shorthand(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim firstAccount = application.GetAccounts().Synchronously().First()
            Dim created = firstAccount.CreatedAt

            Dim createdToday = application.GetAccounts().Synchronously().Where(Function(x) x.CreatedAt.Within(created.Year, created.Month, created.Day)).Count()
            createdToday.ShouldNotBe(0)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Searching_accounts_using_filter(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim filtered = application.GetAccounts().Synchronously().Filter("lo").ToList()

            filtered.Count.ShouldBeGreaterThanOrEqualTo(3)
            filtered.ShouldContain(Function(acct) acct.FullName = "Han Solo")
            filtered.ShouldContain(Function(acct) acct.Username.StartsWith("lottanerve"))
            filtered.ShouldContain(Function(acct) acct.Username.StartsWith("lordvader"))
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Sorting_accounts_by_lastname(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim accountsSortedByLastName = application.GetAccounts().Synchronously().OrderBy(Function(x) x.Surname).ToList()

            Dim lando = accountsSortedByLastName.First()
            lando.FullName.ShouldBe("Lando Calrissian")
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Sorting_accounts_by_username_and_lastname(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim accountsSortedByMultiple = application.GetAccounts().Synchronously().OrderByDescending(Function(x) x.Username).OrderByDescending(Function(x) x.Surname).ToList()

            Dim tarkin = accountsSortedByMultiple.First()
            tarkin.FullName.ShouldBe("Wilhuff Tarkin")
            Dim luke = accountsSortedByMultiple.ElementAt(1)
            luke.FullName.ShouldBe("Luke Skywalker")
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Taking_only_two_accounts(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim firstTwo = application.GetAccounts().Synchronously().Take(2).ToList()

            firstTwo.Count.ShouldBe(2)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Counting_accounts(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim count = application.GetAccounts().Synchronously().Count()
            count.ShouldBeGreaterThanOrEqualTo(8)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Any_returns_false_for_empty_filtered_set(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim anyDroids = application.GetAccounts().Synchronously().Where(Function(x) x.Email.EndsWith("droids.co")).Any()

            anyDroids.ShouldBeFalse()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Any_returns_true_for_nonempty_filtered_set(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim anyWookiees = application.GetAccounts().Synchronously().Where(Function(x) x.Email.EndsWith("kashyyyk.rim")).Any()

            anyWookiees.ShouldBeTrue()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_and_deleting_account(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim account = application.CreateAccount("Gial", "Ackbar", "admiralackbar@dac.rim", New RandomPassword(12))

            account.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedAccountHrefs.Add(account.Href)

            account.FullName.ShouldBe("Gial Ackbar")
            account.Email.ShouldBe("admiralackbar@dac.rim")
            account.Username.ShouldBe("admiralackbar@dac.rim")
            account.Status.ShouldBe(AccountStatus.Enabled)
            account.CreatedAt.ShouldBe(DateTimeOffset.Now, TimeSpan.FromSeconds(10))
            account.ModifiedAt.ShouldBe(DateTimeOffset.Now, TimeSpan.FromSeconds(10))

            Dim deleted = account.Delete()
            ' It's a trap! :(
            deleted.ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_account_with_custom_data(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim account = application.CreateAccount("Mara", "Jade", New RandomEmail("empire.co"), New RandomPassword(12), New With {
                Key .quote = "I'm a fighter. I've always been a fighter.",
                Key .birth = -17,
                Key .death = 40
            })

            account.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedAccountHrefs.Add(account.Href)
            Dim customData = account.GetCustomData()

            account.FullName.ShouldBe("Mara Jade")
            customData("quote").ToString().ShouldBe("I'm a fighter. I've always been a fighter.")
            CInt(customData("birth")).ShouldBe(-17)
            CInt(customData("death")).ShouldBe(40)

            ' Clean up
            account.Delete().ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_account_with_response_options(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim account = client.Instantiate(Of IAccount)().SetGivenName("Galen").SetSurname("Marek").SetEmail("gmarek@kashyyk.rim").SetPassword(New RandomPassword(12))
            application.CreateAccount(account, Sub(opt)
                                                   opt.ResponseOptions.Expand(Function(x) x.GetCustomData())
                                               End Sub)

            account.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedAccountHrefs.Add(account.Href)

            ' Clean up
            account.Delete().ShouldBeTrue()
            Me.fixture.CreatedAccountHrefs.Remove(account.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Authenticating_account(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim username = $"sonofthesuns-{fixture.TestRunIdentifier}"
            Dim result = application.AuthenticateAccount(username, "whataPieceofjunk$1138")
            result.ShouldBeAssignableTo(Of IAuthenticationResult)()
            result.Success.ShouldBeTrue()

            Dim account = result.GetAccount()
            account.FullName.ShouldBe("Luke Skywalker")
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Authenticating_account_with_response_options(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim request = New UsernamePasswordRequestBuilder()
            request.SetUsernameOrEmail($"sonofthesuns-{fixture.TestRunIdentifier}")
            request.SetPassword("whataPieceofjunk$1138")

            Dim result = application.AuthenticateAccount(request.Build(), Function(response) response.Expand(Function(x) x.GetAccount()))

            result.ShouldBeAssignableTo(Of IAuthenticationResult)()
            result.Success.ShouldBeTrue()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Authenticating_account_in_specified_account_store(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim accountStore = application.GetDefaultAccountStore()

            Dim result = application.AuthenticateAccount(Function(request) request.SetUsernameOrEmail($"sonofthesuns-{fixture.TestRunIdentifier}").SetPassword("whataPieceofjunk$1138").SetAccountStore(accountStore))
            result.ShouldBeAssignableTo(Of IAuthenticationResult)()
            result.Success.ShouldBeTrue()

            Dim account = result.GetAccount()
            account.FullName.ShouldBe("Luke Skywalker")
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Authenticating_account_in_specified_account_store_by_href(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim accountStore = application.GetDefaultAccountStore()

            Dim result = application.AuthenticateAccount(Sub(request)
                                                             request.SetUsernameOrEmail($"sonofthesuns-{fixture.TestRunIdentifier}")
                                                             request.SetPassword("whataPieceofjunk$1138")
                                                             request.SetAccountStore(Me.fixture.PrimaryDirectoryHref)
                                                         End Sub)
            result.ShouldBeAssignableTo(Of IAuthenticationResult)()
            result.Success.ShouldBeTrue()

            Dim account = result.GetAccount()
            account.FullName.ShouldBe("Luke Skywalker")
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Authenticating_account_in_specified_account_store_with_response_options(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)
            Dim accountStore = application.GetDefaultAccountStore()

            Dim result = application.AuthenticateAccount(Sub(request)
                                                             request.SetUsernameOrEmail($"sonofthesuns-{fixture.TestRunIdentifier}")
                                                             request.SetPassword("whataPieceofjunk$1138")
                                                             request.SetAccountStore(accountStore)
                                                         End Sub,
                                                         Function(response) response.Expand(Function(x) x.GetAccount()))

            result.ShouldBeAssignableTo(Of IAuthenticationResult)()
            result.Success.ShouldBeTrue()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub TryAuthenticating_account(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim username = $"sonofthesuns-{fixture.TestRunIdentifier}"

            application.TryAuthenticateAccount(username, "whataPieceofjunk$1138").ShouldBeTrue()

            application.TryAuthenticateAccount(username, "notLukesPassword?").ShouldBeFalse()
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Authenticating_throws_for_invalid_credentials(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim application = client.GetResource(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim username = $"sonofthesuns-{fixture.TestRunIdentifier}"
            Dim password = "notLukesPassword?"

            Dim didFailCorrectly As Boolean = False
            Try
                Dim result = application.AuthenticateAccount(username, password)
            Catch rex As ResourceException
                didFailCorrectly = rex.HttpStatus = 400
            Catch
                didFailCorrectly = False
            End Try

            Assert.[True](didFailCorrectly)
        End Sub
    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
