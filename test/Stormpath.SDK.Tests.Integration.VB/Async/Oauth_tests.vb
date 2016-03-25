' <copyright file="Oauth_tests.vb" company="Stormpath, Inc.">
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
Imports Stormpath.SDK.Error
Imports Stormpath.SDK.Jwt
Imports Stormpath.SDK.Oauth
Imports Stormpath.SDK.Tests.Common
Imports Stormpath.SDK.Tests.Common.Integration
Imports Xunit

Namespace Async
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class Oauth_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Creating_token_with_password_grant(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            ' Create a dummy application
            Dim createdApplication = Await tenant _
                .CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Creating Token With Password Grant Flow (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            Await createdApplication.AddAccountStoreAsync(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref) _
                .Build()
            Dim authenticateResult = Await createdApplication.NewPasswordGrantAuthenticator() _
                .AuthenticateAsync(passwordGrantRequest)

            ' Verify authentication response
            authenticateResult.AccessTokenHref.ShouldContain("/accessTokens/")
            authenticateResult.AccessTokenString.ShouldNotBeNullOrEmpty()
            authenticateResult.ExpiresIn.ShouldBeGreaterThanOrEqualTo(3600)
            authenticateResult.TokenType.ShouldBe("Bearer")
            authenticateResult.RefreshTokenString.ShouldNotBeNullOrEmpty()

            ' Verify generated access token
            Dim accessToken = Await authenticateResult.GetAccessTokenAsync()
            accessToken.CreatedAt.ShouldNotBe(Nothing)
            accessToken.Href.ShouldBe(authenticateResult.AccessTokenHref)
            accessToken.Jwt.ShouldBe(authenticateResult.AccessTokenString)
            accessToken.ApplicationHref.ShouldBe(createdApplication.Href)
            accessToken.AccountHref.ShouldNotBeNullOrEmpty()

            // Get account (with some expansions)
            Dim account = Await accessToken.GetAccountAsync(Function(opt) opt.Expand(Function(acct) acct.GetGroups()))
            account.Email.ShouldBe("lskywalker@tattooine.rim")

            ' Clean up
            Call (Await accessToken.DeleteAsync()).ShouldBeTrue()

            Call (Await createdApplication.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Failed_password_grant_throws_ResourceException(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            ' Create a dummy application
            Dim createdApplication = Await tenant _
                .CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Failed Password Grant Throws (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            Await createdApplication.AddAccountStoreAsync(Me.fixture.PrimaryDirectoryHref)

            Dim badPasswordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("notLukesPassword") _
                .Build()

            Await Should.ThrowAsync(Of ResourceException)(Async Function() Await createdApplication.NewPasswordGrantAuthenticator().AuthenticateAsync(badPasswordGrantRequest))

            Call (Await createdApplication.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Listing_account_tokens(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            ' Create a dummy application
            Dim createdApplication = Await tenant _
                .CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Listing Tokens (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            Await createdApplication.AddAccountStoreAsync(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref) _
                .Build()
            Dim authenticateResult = Await createdApplication.NewPasswordGrantAuthenticator() _
                .AuthenticateAsync(passwordGrantRequest)

            Dim account = Await tenant.GetAccountAsync(Me.fixture.PrimaryAccountHref)
            Dim accessTokens = Await account.GetAccessTokens().ToListAsync()
            Dim refreshTokens = Await account.GetRefreshTokens().ToListAsync()

            Dim accessToken = accessTokens.Where(Function(x) x.Jwt = authenticateResult.AccessTokenString).SingleOrDefault()
            Dim refreshToken = refreshTokens.Where(Function(x) x.Jwt = authenticateResult.RefreshTokenString).SingleOrDefault()
            accessToken.ShouldNotBeNull()
            refreshToken.ShouldNotBeNull()

            ' Clean up
            Call (Await accessToken.DeleteAsync()).ShouldBeTrue()
            Call (Await refreshToken.DeleteAsync()).ShouldBeTrue()

            Call (Await createdApplication.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_access_token_for_application(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            ' Create a dummy application
            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Getting Access Token for Application (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            Await createdApplication.AddAccountStoreAsync(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref) _
                .Build()
            Dim authenticateResult = Await createdApplication.NewPasswordGrantAuthenticator() _
                .AuthenticateAsync(passwordGrantRequest)

            Dim account = Await tenant.GetAccountAsync(Me.fixture.PrimaryAccountHref)
            Dim accessTokenForApplication = Await account.GetAccessTokens().Where(Function(x) x.ApplicationHref = createdApplication.Href).SingleOrDefaultAsync()

            accessTokenForApplication.ShouldNotBeNull()

            Call (Await accessTokenForApplication.GetAccountAsync()).Href.ShouldBe(Me.fixture.PrimaryAccountHref)
            Call (Await accessTokenForApplication.GetApplicationAsync()).Href.ShouldBe(createdApplication.Href)
            Call (Await accessTokenForApplication.GetTenantAsync()).Href.ShouldBe(Me.fixture.TenantHref)

            Dim retrievedDirectly = Await client.GetAccessTokenAsync(accessTokenForApplication.Href)
            retrievedDirectly.Href.ShouldBe(accessTokenForApplication.Href)

            ' Clean up
            Call (Await accessTokenForApplication.DeleteAsync()).ShouldBeTrue()

            Call (Await createdApplication.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Getting_refresh_token_for_application(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            ' Create a dummy application
            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Getting Refresh Token for Application (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            Await createdApplication.AddAccountStoreAsync(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref).Build()
            Dim authenticateResult = Await createdApplication.NewPasswordGrantAuthenticator() _
                .AuthenticateAsync(passwordGrantRequest)

            Dim account = Await tenant.GetAccountAsync(Me.fixture.PrimaryAccountHref)
            Dim refreshTokenForApplication = Await account.GetRefreshTokens().Where(Function(x) x.ApplicationHref = createdApplication.Href).SingleOrDefaultAsync()

            refreshTokenForApplication.ShouldNotBeNull()

            Call (Await refreshTokenForApplication.GetAccountAsync()).Href.ShouldBe(Me.fixture.PrimaryAccountHref)
            Call (Await refreshTokenForApplication.GetApplicationAsync()).Href.ShouldBe(createdApplication.Href)
            Call (Await refreshTokenForApplication.GetTenantAsync()).Href.ShouldBe(Me.fixture.TenantHref)

            Dim retrievedDirectly = Await client.GetRefreshTokenAsync(refreshTokenForApplication.Href)
            retrievedDirectly.Href.ShouldBe(refreshTokenForApplication.Href)

            // Get account (with some expansions)
            Dim tokenAccount = Await retrievedDirectly.GetAccountAsync(Function(opt) opt.Expand(Function(acct) acct.GetGroups()))
            account.Email.ShouldBe(account.Email)

            ' Clean up
            Call (Await refreshTokenForApplication.DeleteAsync()).ShouldBeTrue()

            Call (Await createdApplication.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Validating_jwt(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            ' Create a dummy application
            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Validating JWT (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            Await createdApplication.AddAccountStoreAsync(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref) _
                .Build()
            Dim authenticateResult = Await createdApplication.NewPasswordGrantAuthenticator() _
                .AuthenticateAsync(passwordGrantRequest)
            Dim accessTokenJwt = authenticateResult.AccessTokenString

            Dim jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest() _
                .SetJwt(accessTokenJwt) _
                .Build()
            Dim validAccessToken = Await createdApplication.NewJwtAuthenticator() _
                .AuthenticateAsync(jwtAuthenticationRequest)

            validAccessToken.ShouldNotBeNull()
            validAccessToken.ApplicationHref.ShouldBe(createdApplication.Href)
            validAccessToken.AccountHref.ShouldNotBeNullOrEmpty()
            validAccessToken.CreatedAt.ShouldBe(DateTimeOffset.Now, Delay.ReasonableTestRunWindow)
            validAccessToken.Href.ShouldBe(authenticateResult.AccessTokenHref)
            validAccessToken.Jwt.ShouldBe(accessTokenJwt)

            ' Clean up
            Call (Await validAccessToken.DeleteAsync()).ShouldBeTrue()

            Call (Await createdApplication.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Validating_jwt_throws_for_bad_jwt(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            ' Create a dummy application
            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Validating JWT (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            Await createdApplication.AddAccountStoreAsync(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref) _
                .Build()
            Dim authenticateResult = Await createdApplication.NewPasswordGrantAuthenticator() _
                .AuthenticateAsync(passwordGrantRequest)

            Dim badJwt = authenticateResult.AccessTokenString.Substring(0, authenticateResult.AccessTokenString.Length - 3) + "foo"

            Dim jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest() _
                .SetJwt(badJwt) _
                .Build()

            Await Should.ThrowAsync(Of ResourceException)(Async Function() Await createdApplication.NewJwtAuthenticator().AuthenticateAsync(jwtAuthenticationRequest))

            ' Clean up
            Call (Await createdApplication.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Validating_jwt_locally(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            ' Create a dummy application
            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Validating JWT Locally (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            Await createdApplication.AddAccountStoreAsync(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref) _
                .Build()
            Dim authenticateResult = Await createdApplication.NewPasswordGrantAuthenticator() _
                .AuthenticateAsync(passwordGrantRequest)
            Dim accessTokenJwt = authenticateResult.AccessTokenString

            Dim jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest() _
                .SetJwt(accessTokenJwt) _
                .Build()
            Dim validAccessToken = Await createdApplication.NewJwtAuthenticator() _
                .WithLocalValidation() _
                .AuthenticateAsync(jwtAuthenticationRequest)

            validAccessToken.ShouldNotBeNull()
            validAccessToken.ApplicationHref.ShouldBe(createdApplication.Href)
            validAccessToken.AccountHref.ShouldNotBeNullOrEmpty()
            validAccessToken.CreatedAt.ShouldBe(DateTimeOffset.Now, Delay.ReasonableTestRunWindow)
            validAccessToken.Href.ShouldBe(authenticateResult.AccessTokenHref)
            validAccessToken.Jwt.ShouldBe(accessTokenJwt)

            ' Clean up
            Call (Await validAccessToken.DeleteAsync()).ShouldBeTrue()

            Call (Await createdApplication.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Validating_jwt_locally_throws_for_bad_jwt(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            ' Create a dummy application
            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Validating JWT Locally (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            Await createdApplication.AddAccountStoreAsync(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref) _
                .Build()
            Dim authenticateResult = Await createdApplication.NewPasswordGrantAuthenticator() _
                .AuthenticateAsync(passwordGrantRequest)

            Dim badJwt = authenticateResult.AccessTokenString.Substring(0, authenticateResult.AccessTokenString.Length - 3) + "foo"

            Dim jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest() _
                .SetJwt(badJwt) _
                .Build()

            Await Should.ThrowAsync(Of JwtSignatureException)(Async Function() Await createdApplication.NewJwtAuthenticator().WithLocalValidation().AuthenticateAsync(jwtAuthenticationRequest))

            ' Clean up
            Call (Await createdApplication.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Validating_token_after_revocation(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            ' Create a dummy application
            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Validating Token After Revocation (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            Await createdApplication.AddAccountStoreAsync(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref) _
                .Build()
            Dim authenticateResult = Await createdApplication.NewPasswordGrantAuthenticator() _
                .AuthenticateAsync(passwordGrantRequest)

            Dim accessToken = Await authenticateResult.GetAccessTokenAsync()
            Call (Await accessToken.DeleteAsync()).ShouldBeTrue() ' Revoke access token

            Dim jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest() _
                .SetJwt(accessToken.Jwt) _
                .Build()

            Await Should.ThrowAsync(Of ResourceException)(Async Function() Await createdApplication.NewJwtAuthenticator().AuthenticateAsync(jwtAuthenticationRequest))

            ' Clean up
            Call (Await createdApplication.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Refreshing_access_token_with_jwt(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            ' Create a dummy application
            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Refreshing Access Token (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            Await createdApplication.AddAccountStoreAsync(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref) _
                .Build()
            Dim originalGrantResult = Await createdApplication.NewPasswordGrantAuthenticator() _
                .AuthenticateAsync(passwordGrantRequest)

            Dim refreshGrantRequest = OauthRequests.NewRefreshGrantRequest() _
                .SetRefreshToken(originalGrantResult.RefreshTokenString) _
                .Build()

            Dim refreshGrantResult = Await createdApplication.NewRefreshGrantAuthenticator() _
                .AuthenticateAsync(refreshGrantRequest)

            refreshGrantResult.AccessTokenHref.ShouldNotBe(originalGrantResult.AccessTokenHref)
            refreshGrantResult.AccessTokenString.ShouldNotBe(originalGrantResult.AccessTokenString)
            refreshGrantResult.RefreshTokenString.ShouldBe(originalGrantResult.RefreshTokenString)

            ' Clean up
            Call (Await createdApplication.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Refreshing_access_token_with_instance(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim tenant = Await client.GetCurrentTenantAsync()

            ' Create a dummy application
            Dim createdApplication = Await tenant.CreateApplicationAsync($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Getting Refresh Token for Application (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            Await createdApplication.AddAccountStoreAsync(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref) _
                .Build()
            Dim originalGrantResult = Await createdApplication.NewPasswordGrantAuthenticator() _
                .AuthenticateAsync(passwordGrantRequest)

            Dim account = Await tenant.GetAccountAsync(Me.fixture.PrimaryAccountHref)
            Dim refreshToken = Await account _
                .GetRefreshTokens() _
                .Where(Function(x) x.ApplicationHref = createdApplication.Href) _
                .SingleOrDefaultAsync()
            refreshToken.ShouldNotBeNull()

            Dim refreshGrantRequest = OauthRequests.NewRefreshGrantRequest() _
                .SetRefreshToken(refreshToken) _
                .Build()

            Dim refreshGrantResult = Await createdApplication.NewRefreshGrantAuthenticator() _
                .AuthenticateAsync(refreshGrantRequest)

            refreshGrantResult.AccessTokenHref.ShouldNotBe(originalGrantResult.AccessTokenHref)
            refreshGrantResult.AccessTokenString.ShouldNotBe(originalGrantResult.AccessTokenString)
            refreshGrantResult.RefreshTokenString.ShouldBe(originalGrantResult.RefreshTokenString)

            ' Clean up
            Call (Await refreshToken.DeleteAsync()).ShouldBeTrue()

            Call (Await createdApplication.DeleteAsync()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Function
    End Class
End Namespace
