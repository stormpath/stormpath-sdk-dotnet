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
Imports Stormpath.SDK.Sync
Imports Stormpath.SDK.Tests.Common
Imports Stormpath.SDK.Tests.Common.Integration
Imports Xunit

Namespace Sync
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class Oauth_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Creating_token_with_password_grant(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            ' Create a dummy application
            Dim createdApplication = tenant _
                .CreateApplication($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Creating Token With Password Grant Flow - Sync (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            createdApplication.AddAccountStore(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref) _
                .Build()
            Dim authenticateResult = createdApplication.NewPasswordGrantAuthenticator() _
                .Authenticate(passwordGrantRequest)

            ' Verify authentication response
            authenticateResult.AccessTokenHref.ShouldContain("/accessTokens/")
            authenticateResult.AccessTokenString.ShouldNotBeNullOrEmpty()
            authenticateResult.ExpiresIn.ShouldBeGreaterThanOrEqualTo(3600)
            authenticateResult.TokenType.ShouldBe("Bearer")
            authenticateResult.RefreshTokenString.ShouldNotBeNullOrEmpty()

            ' Verify generated access token
            Dim accessToken = authenticateResult.GetAccessToken()
            accessToken.CreatedAt.ShouldNotBe(Nothing)
            accessToken.Href.ShouldBe(authenticateResult.AccessTokenHref)
            accessToken.Jwt.ShouldBe(authenticateResult.AccessTokenString)
            accessToken.ApplicationHref.ShouldBe(createdApplication.Href)

            ' Clean up
            Call (accessToken.Delete()).ShouldBeTrue()

            Call (createdApplication.Delete()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Failed_password_grant_throws_ResourceException(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            ' Create a dummy application
            Dim createdApplication = tenant _
                .CreateApplication($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Failed Password Grant Throws - Sync (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            createdApplication.AddAccountStore(Me.fixture.PrimaryDirectoryHref)

            Dim badPasswordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("notLukesPassword") _
                .Build()

            Should.Throw(Of ResourceException)(Sub() createdApplication.NewPasswordGrantAuthenticator().Authenticate(badPasswordGrantRequest))

            Call (createdApplication.Delete()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Listing_account_tokens(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            ' Create a dummy application
            Dim createdApplication = tenant _
                .CreateApplication($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Listing Tokens - Sync (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            createdApplication.AddAccountStore(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref) _
                .Build()
            Dim authenticateResult = createdApplication.NewPasswordGrantAuthenticator() _
                .Authenticate(passwordGrantRequest)

            Dim account = tenant.GetAccount(Me.fixture.PrimaryAccountHref)
            Dim accessTokens = account.GetAccessTokens().Synchronously().ToList()
            Dim refreshTokens = account.GetRefreshTokens().Synchronously().ToList()

            Dim accessToken = accessTokens.Where(Function(x) x.Jwt = authenticateResult.AccessTokenString).SingleOrDefault()
            Dim refreshToken = refreshTokens.Where(Function(x) x.Jwt = authenticateResult.RefreshTokenString).SingleOrDefault()
            accessToken.ShouldNotBeNull()
            refreshToken.ShouldNotBeNull()

            ' Clean up
            Call (accessToken.Delete()).ShouldBeTrue()
            Call (refreshToken.Delete()).ShouldBeTrue()

            Call (createdApplication.Delete()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_access_token_for_application(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            ' Create a dummy application
            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Getting Access Token For Application - Sync (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            createdApplication.AddAccountStore(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref) _
                .Build()
            Dim authenticateResult = createdApplication.NewPasswordGrantAuthenticator() _
                .Authenticate(passwordGrantRequest)

            Dim account = tenant.GetAccount(Me.fixture.PrimaryAccountHref)
            Dim accessTokenForApplication = account.GetAccessTokens() _
                .Where(Function(x) x.ApplicationHref = createdApplication.Href) _
                .Synchronously() _
                .SingleOrDefault()

            accessTokenForApplication.ShouldNotBeNull()

            Call (accessTokenForApplication.GetAccount()).Href.ShouldBe(Me.fixture.PrimaryAccountHref)
            Call (accessTokenForApplication.GetApplication()).Href.ShouldBe(createdApplication.Href)
            Call (accessTokenForApplication.GetTenant()).Href.ShouldBe(Me.fixture.TenantHref)

            Dim retrievedDirectly = client.GetAccessToken(accessTokenForApplication.Href)
            retrievedDirectly.Href.ShouldBe(accessTokenForApplication.Href)

            ' Clean up
            Call (accessTokenForApplication.Delete()).ShouldBeTrue()

            Call (createdApplication.Delete()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_refresh_token_for_application(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            ' Create a dummy application
            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Getting Refresh Token For Application - Sync (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            createdApplication.AddAccountStore(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref).Build()
            Dim authenticateResult = createdApplication.NewPasswordGrantAuthenticator() _
                .Authenticate(passwordGrantRequest)

            Dim account = tenant.GetAccount(Me.fixture.PrimaryAccountHref)
            Dim refreshTokenForApplication = account.GetRefreshTokens() _
                .Where(Function(x) x.ApplicationHref = createdApplication.Href) _
                .Synchronously() _
                .SingleOrDefault()

            refreshTokenForApplication.ShouldNotBeNull()

            Call (refreshTokenForApplication.GetAccount()).Href.ShouldBe(Me.fixture.PrimaryAccountHref)
            Call (refreshTokenForApplication.GetApplication()).Href.ShouldBe(createdApplication.Href)
            Call (refreshTokenForApplication.GetTenant()).Href.ShouldBe(Me.fixture.TenantHref)

            Dim retrievedDirectly = client.GetRefreshToken(refreshTokenForApplication.Href)
            retrievedDirectly.Href.ShouldBe(refreshTokenForApplication.Href)

            ' Clean up
            Call (refreshTokenForApplication.Delete()).ShouldBeTrue()

            Call (createdApplication.Delete()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Validating_jwt(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            ' Create a dummy application
            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Validating JWT - Sync (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            createdApplication.AddAccountStore(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref) _
                .Build()
            Dim authenticateResult = createdApplication.NewPasswordGrantAuthenticator() _
                .Authenticate(passwordGrantRequest)
            Dim accessTokenJwt = authenticateResult.AccessTokenString

            Dim jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest() _
                .SetJwt(accessTokenJwt) _
                .Build()
            Dim validAccessToken = createdApplication.NewJwtAuthenticator() _
                .Authenticate(jwtAuthenticationRequest)

            validAccessToken.ShouldNotBeNull()
            validAccessToken.ApplicationHref.ShouldBe(createdApplication.Href)
            validAccessToken.CreatedAt.ShouldBe(DateTimeOffset.Now, Delay.ReasonableTestRunWindow)
            validAccessToken.Href.ShouldBe(authenticateResult.AccessTokenHref)
            validAccessToken.Jwt.ShouldBe(accessTokenJwt)

            ' Clean up
            Call (validAccessToken.Delete()).ShouldBeTrue()

            Call (createdApplication.Delete()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Validating_jwt_throws_for_bad_jwt(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            ' Create a dummy application
            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Validating JWT - Sync (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            createdApplication.AddAccountStore(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref) _
                .Build()
            Dim authenticateResult = createdApplication.NewPasswordGrantAuthenticator() _
                .Authenticate(passwordGrantRequest)

            Dim badJwt = authenticateResult.AccessTokenString.Substring(0, authenticateResult.AccessTokenString.Length - 3) + "foo"

            Dim jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest() _
                .SetJwt(badJwt) _
                .Build()

            Should.Throw(Of ResourceException)(Function() createdApplication.NewJwtAuthenticator().Authenticate(jwtAuthenticationRequest))

            ' Clean up
            Call (createdApplication.Delete()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Validating_jwt_locally(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            ' Create a dummy application
            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Validating JWT Locally - Sync (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            createdApplication.AddAccountStore(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref) _
                .Build()
            Dim authenticateResult = createdApplication.NewPasswordGrantAuthenticator() _
                .Authenticate(passwordGrantRequest)
            Dim accessTokenJwt = authenticateResult.AccessTokenString

            Dim jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest() _
                .SetJwt(accessTokenJwt) _
                .Build()
            Dim validAccessToken = createdApplication.NewJwtAuthenticator() _
                .WithLocalValidation() _
                .Authenticate(jwtAuthenticationRequest)

            validAccessToken.ShouldNotBeNull()
            validAccessToken.ApplicationHref.ShouldBe(createdApplication.Href)
            validAccessToken.CreatedAt.ShouldBe(DateTimeOffset.Now, Delay.ReasonableTestRunWindow)
            validAccessToken.Href.ShouldBe(authenticateResult.AccessTokenHref)
            validAccessToken.Jwt.ShouldBe(accessTokenJwt)

            ' Clean up
            Call (validAccessToken.Delete()).ShouldBeTrue()

            Call (createdApplication.Delete()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Validating_jwt_locally_throws_for_bad_jwt(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            ' Create a dummy application
            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Validating JWT Locally - Sync (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            createdApplication.AddAccountStore(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref) _
                .Build()
            Dim authenticateResult = createdApplication.NewPasswordGrantAuthenticator() _
                .Authenticate(passwordGrantRequest)

            Dim badJwt = authenticateResult.AccessTokenString.Substring(0, authenticateResult.AccessTokenString.Length - 3) + "foo"

            Dim jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest() _
                .SetJwt(badJwt) _
                .Build()

            Should.Throw(Of JwtSignatureException)(Function() createdApplication.NewJwtAuthenticator().WithLocalValidation().Authenticate(jwtAuthenticationRequest))

            ' Clean up
            Call (createdApplication.Delete()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Validating_token_after_revocation(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            ' Create a dummy application
            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Validating Token After Revocation - Sync (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            createdApplication.AddAccountStore(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref) _
                .Build()
            Dim authenticateResult = createdApplication.NewPasswordGrantAuthenticator() _
                .Authenticate(passwordGrantRequest)

            Dim accessToken = authenticateResult.GetAccessToken()
            Call (accessToken.Delete()).ShouldBeTrue() ' Revoke access token

            Dim jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest() _
                .SetJwt(accessToken.Jwt) _
                .Build()

            Should.Throw(Of ResourceException)(Function() createdApplication.NewJwtAuthenticator().Authenticate(jwtAuthenticationRequest))

            ' Clean up
            Call (createdApplication.Delete()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Refreshing_access_token_with_jwt(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            ' Create a dummy application
            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Refreshing Access Token - Sync (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            createdApplication.AddAccountStore(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref) _
                .Build()
            Dim originalGrantResult = createdApplication.NewPasswordGrantAuthenticator() _
                .Authenticate(passwordGrantRequest)

            Dim refreshGrantRequest = OauthRequests.NewRefreshGrantRequest() _
                .SetRefreshToken(originalGrantResult.RefreshTokenString) _
                .Build()

            Dim refreshGrantResult = createdApplication.NewRefreshGrantAuthenticator() _
                .Authenticate(refreshGrantRequest)

            refreshGrantResult.AccessTokenHref.ShouldNotBe(originalGrantResult.AccessTokenHref)
            refreshGrantResult.AccessTokenString.ShouldNotBe(originalGrantResult.AccessTokenString)
            refreshGrantResult.RefreshTokenString.ShouldBe(originalGrantResult.RefreshTokenString)

            ' Clean up
            Call (createdApplication.Delete()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Refreshing_access_token_with_instance(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim tenant = client.GetCurrentTenant()

            ' Create a dummy application
            Dim createdApplication = tenant.CreateApplication($".NET IT {fixture.TestRunIdentifier}-{clientBuilder.Name} Getting Refresh Token for Application - Sync (VB)", createDirectory:=False)
            createdApplication.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedApplicationHrefs.Add(createdApplication.Href)

            ' Add the test accounts
            createdApplication.AddAccountStore(Me.fixture.PrimaryDirectoryHref)

            Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
                .SetLogin("lskywalker@tattooine.rim") _
                .SetPassword("whataPieceofjunk$1138") _
                .SetAccountStore(Me.fixture.PrimaryDirectoryHref) _
                .Build()
            Dim originalGrantResult = createdApplication.NewPasswordGrantAuthenticator() _
                .Authenticate(passwordGrantRequest)

            Dim account = tenant.GetAccount(Me.fixture.PrimaryAccountHref)
            Dim refreshToken = account _
                .GetRefreshTokens() _
                .Where(Function(x) x.ApplicationHref = createdApplication.Href) _
                .Synchronously() _
                .SingleOrDefault()
            refreshToken.ShouldNotBeNull()

            Dim refreshGrantRequest = OauthRequests.NewRefreshGrantRequest() _
                .SetRefreshToken(refreshToken) _
                .Build()

            Dim refreshGrantResult = createdApplication.NewRefreshGrantAuthenticator() _
                .Authenticate(refreshGrantRequest)

            refreshGrantResult.AccessTokenHref.ShouldNotBe(originalGrantResult.AccessTokenHref)
            refreshGrantResult.AccessTokenString.ShouldNotBe(originalGrantResult.AccessTokenString)
            refreshGrantResult.RefreshTokenString.ShouldBe(originalGrantResult.RefreshTokenString)

            ' Clean up
            Call (refreshToken.Delete()).ShouldBeTrue()

            Call (createdApplication.Delete()).ShouldBeTrue()
            Me.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href)
        End Sub
    End Class
End Namespace