// <copyright file="Oauth_tests.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Linq;
using Shouldly;
using Stormpath.SDK.Error;
using Stormpath.SDK.Jwt;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Common;
using Stormpath.SDK.Tests.Common.Integration;
using Stormpath.SDK.Tests.Common.RandomData;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Sync
{
    [Collection(nameof(IntegrationTestCollection))]
    public class Oauth_tests
    {
        private readonly TestFixture fixture;

        public Oauth_tests(TestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Creating_token_with_password_grant(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            // Create a dummy application
            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Creating Token With Password Grant Flow - Sync",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            createdApplication.AddAccountStore(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@testmail.stormpath.com")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var authenticateResult = createdApplication.NewPasswordGrantAuthenticator()
                .Authenticate(passwordGrantRequest);

            // Verify authentication response
            authenticateResult.AccessTokenHref.ShouldContain("/accessTokens/");
            authenticateResult.AccessTokenString.ShouldNotBeNullOrEmpty();
            authenticateResult.ExpiresIn.ShouldBeGreaterThanOrEqualTo(3600);
            authenticateResult.TokenType.ShouldBe("Bearer");
            authenticateResult.RefreshTokenString.ShouldNotBeNullOrEmpty();

            // Verify generated access token
            var accessToken = authenticateResult.GetAccessToken();
            accessToken.CreatedAt.ShouldNotBe(default(DateTimeOffset));
            accessToken.Href.ShouldBe(authenticateResult.AccessTokenHref);
            accessToken.Jwt.ShouldBe(authenticateResult.AccessTokenString);
            accessToken.ApplicationHref.ShouldBe(createdApplication.Href);
            accessToken.AccountHref.ShouldNotBeNullOrEmpty();

            // Get account (with some expansions)
            var account = accessToken.GetAccount(opt => opt.Expand(acct => acct.GetGroups()));
            account.Email.ShouldBe("lskywalker@testmail.stormpath.com");

            // Clean up
            accessToken.Delete().ShouldBeTrue();

            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Creating_token_with_password_grant_and_nameKey(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            // Create a dummy application
            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Creating Token With Password Grant Flow + ONK",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test organization
            var primaryOrg = client.GetOrganization(fixture.PrimaryOrganizationHref);
            createdApplication.AddAccountStore(primaryOrg);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@testmail.stormpath.com")
                .SetPassword("whataPieceofjunk$1138")
                .SetOrganizationNameKey(fixture.PrimaryOrganizationNameKey)
                .Build();
            var authenticateResult = createdApplication.NewPasswordGrantAuthenticator()
                .Authenticate(passwordGrantRequest);

            // Clean up
            var accessToken = authenticateResult.GetAccessToken();
            accessToken.Delete().ShouldBeTrue();

            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        /// <summary>
        /// Regression test for stormpath/stormpath-sdk-dotnet#161
        /// </summary>
        /// <remarks>The ! character was causing the SAuthc1 signer to break.</remarks>
        /// <param name="clientBuilder">The client builder.</param>
        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Password_grant_with_special_characters(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            // Create a dummy application
            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Password Grant Flow With Special Characters",
                createDirectory: true);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var createdDirectory = createdApplication.GetDefaultAccountStore();
            this.fixture.CreatedDirectoryHrefs.Add(createdDirectory.Href);

            // Add the test accounts
            var randomEmail = new RandomEmail("testmail.stormpath.com");
            var password = "P@sword#123$!";
            createdApplication.CreateAccount("Test", "testerman", randomEmail, password);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin(randomEmail)
                .SetPassword(password)
                .Build();
            var authenticateResult = createdApplication.NewPasswordGrantAuthenticator()
                .Authenticate(passwordGrantRequest);

            // Verify authentication response
            authenticateResult.AccessTokenString.ShouldNotBeNullOrEmpty();

            // Clean up
            (createdApplication.Delete()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Failed_password_grant_throws_ResourceException(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            // Create a dummy application
            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Failed Password Grant Throws - Sync",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            createdApplication.AddAccountStore(this.fixture.PrimaryDirectoryHref);

            var badPasswordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@testmail.stormpath.com")
                .SetPassword("notLukesPassword")
                .Build();

            // TODO Mono Shouldly support - should be ResourceException
            Should.Throw<Exception>(() => createdApplication.NewPasswordGrantAuthenticator().Authenticate(badPasswordGrantRequest));

            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Listing_account_tokens(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            // Create a dummy application
            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Listing Tokens - Sync",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            createdApplication.AddAccountStore(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@testmail.stormpath.com")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var authenticateResult = createdApplication.NewPasswordGrantAuthenticator()
                .Authenticate(passwordGrantRequest);

            var account = tenant.GetAccount(this.fixture.PrimaryAccountHref);
            var accessTokens = account.GetAccessTokens().Synchronously().ToList();
            var refreshTokens = account.GetRefreshTokens().Synchronously().ToList();

            var accessToken = accessTokens.Where(x => x.Jwt == authenticateResult.AccessTokenString).SingleOrDefault();
            var refreshToken = refreshTokens.Where(x => x.Jwt == authenticateResult.RefreshTokenString).SingleOrDefault();
            accessToken.ShouldNotBeNull();
            refreshToken.ShouldNotBeNull();

            // Clean up
            accessToken.Delete().ShouldBeTrue();
            refreshToken.Delete().ShouldBeTrue();

            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_access_token_for_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            // Create a dummy application
            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Getting Access Token for Application - Sync",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            createdApplication.AddAccountStore(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@testmail.stormpath.com")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var authenticateResult = createdApplication.NewPasswordGrantAuthenticator()
                .Authenticate(passwordGrantRequest);

            var account = tenant.GetAccount(this.fixture.PrimaryAccountHref);
            var accessTokenForApplication = account
                .GetAccessTokens()
                .Where(x => x.ApplicationHref == createdApplication.Href)
                .Synchronously()
                .SingleOrDefault();

            accessTokenForApplication.ShouldNotBeNull();

            accessTokenForApplication.GetAccount().Href.ShouldBe(this.fixture.PrimaryAccountHref);
            accessTokenForApplication.GetApplication().Href.ShouldBe(createdApplication.Href);
            accessTokenForApplication.GetTenant().Href.ShouldBe(this.fixture.TenantHref);

            var retrievedDirectly = client.GetAccessToken(accessTokenForApplication.Href);
            retrievedDirectly.Href.ShouldBe(accessTokenForApplication.Href);

            // Clean up
            accessTokenForApplication.Delete().ShouldBeTrue();

            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_refresh_token_for_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            // Create a dummy application
            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Getting Refresh Token for Application - Sync",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            createdApplication.AddAccountStore(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@testmail.stormpath.com")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var authenticateResult = createdApplication.NewPasswordGrantAuthenticator()
                .Authenticate(passwordGrantRequest);

            var account = tenant.GetAccount(this.fixture.PrimaryAccountHref);
            var refreshTokenForApplication = account
                .GetRefreshTokens()
                .Where(x => x.ApplicationHref == createdApplication.Href)
                .Synchronously()
                .SingleOrDefault();

            refreshTokenForApplication.ShouldNotBeNull();

            refreshTokenForApplication.GetAccount().Href.ShouldBe(this.fixture.PrimaryAccountHref);
            refreshTokenForApplication.GetApplication().Href.ShouldBe(createdApplication.Href);
            refreshTokenForApplication.GetTenant().Href.ShouldBe(this.fixture.TenantHref);

            var retrievedDirectly = client.GetRefreshToken(refreshTokenForApplication.Href);
            retrievedDirectly.Href.ShouldBe(refreshTokenForApplication.Href);

            // Get account (with some expansions)
            var tokenAccount = retrievedDirectly.GetAccount(opt => opt.Expand(acct => acct.GetGroups()));
            account.Email.ShouldBe(account.Email);

            // Clean up
            refreshTokenForApplication.Delete().ShouldBeTrue();

            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Validating_jwt(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            // Create a dummy application
            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Validating JWT - Sync",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            createdApplication.AddAccountStore(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@testmail.stormpath.com")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var authenticateResult = createdApplication.NewPasswordGrantAuthenticator()
                .Authenticate(passwordGrantRequest);
            var accessTokenJwt = authenticateResult.AccessTokenString;

            var jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest()
                .SetJwt(accessTokenJwt)
                .Build();
            var validAccessToken = createdApplication.NewJwtAuthenticator()
                .Authenticate(jwtAuthenticationRequest);

            validAccessToken.ShouldNotBeNull();
            validAccessToken.ApplicationHref.ShouldBe(createdApplication.Href);
            validAccessToken.AccountHref.ShouldNotBeNullOrEmpty();
            validAccessToken.CreatedAt.ShouldBe(DateTimeOffset.Now, Delay.ReasonableTestRunWindow);
            validAccessToken.Href.ShouldBe(authenticateResult.AccessTokenHref);
            validAccessToken.Jwt.ShouldBe(accessTokenJwt);

            // Clean up
            validAccessToken.Delete().ShouldBeTrue();

            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Validating_jwt_throws_for_bad_jwt(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            // Create a dummy application
            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Validating JWT - Sync",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            createdApplication.AddAccountStore(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@testmail.stormpath.com")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var authenticateResult = createdApplication.NewPasswordGrantAuthenticator()
                .Authenticate(passwordGrantRequest);

            var badJwt = authenticateResult.AccessTokenString
                .Substring(0, authenticateResult.AccessTokenString.Length - 3) + "foo";

            var jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest()
                .SetJwt(badJwt)
                .Build();

            Should.Throw<ResourceException>(() =>
                createdApplication.NewJwtAuthenticator().Authenticate(jwtAuthenticationRequest));

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Validating_jwt_locally(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            // Create a dummy application
            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Validating JWT Locally - Sync",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            createdApplication.AddAccountStore(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@testmail.stormpath.com")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var authenticateResult = createdApplication.NewPasswordGrantAuthenticator()
                .Authenticate(passwordGrantRequest);
            var accessTokenJwt = authenticateResult.AccessTokenString;

            var jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest()
                .SetJwt(accessTokenJwt)
                .Build();
            var validAccessToken = createdApplication.NewJwtAuthenticator()
                .WithLocalValidation()
                .Authenticate(jwtAuthenticationRequest);

            validAccessToken.ShouldNotBeNull();
            validAccessToken.ApplicationHref.ShouldBe(createdApplication.Href);
            validAccessToken.AccountHref.ShouldNotBeNullOrEmpty();
            validAccessToken.CreatedAt.ShouldBe(DateTimeOffset.Now, Delay.ReasonableTestRunWindow);
            validAccessToken.Href.ShouldBe(authenticateResult.AccessTokenHref);
            validAccessToken.Jwt.ShouldBe(accessTokenJwt);

            // Clean up
            validAccessToken.Delete().ShouldBeTrue();

            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Validating_jwt_locally_throws_for_bad_jwt(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            // Create a dummy application
            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Validating JWT Locally - Sync",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            createdApplication.AddAccountStore(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@testmail.stormpath.com")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var authenticateResult = createdApplication.NewPasswordGrantAuthenticator()
                .Authenticate(passwordGrantRequest);

            var badJwt = authenticateResult.AccessTokenString
                .Substring(0, authenticateResult.AccessTokenString.Length - 3) + "foo";

            var jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest()
                .SetJwt(badJwt)
                .Build();

            Should.Throw<JwtSignatureException>(() =>
                createdApplication.NewJwtAuthenticator().WithLocalValidation().Authenticate(jwtAuthenticationRequest));

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Validating_token_after_revocation(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            // Create a dummy application
            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Validating Token After Revocation - Sync",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            createdApplication.AddAccountStore(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@testmail.stormpath.com")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var authenticateResult = createdApplication.NewPasswordGrantAuthenticator()
                .Authenticate(passwordGrantRequest);

            var accessToken = authenticateResult.GetAccessToken();
            accessToken.Delete().ShouldBeTrue(); // Revoke access token

            var jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest()
                .SetJwt(accessToken.Jwt)
                .Build();

            Should.Throw<ResourceException>(() => createdApplication.NewJwtAuthenticator().Authenticate(jwtAuthenticationRequest));

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Refreshing_access_token_with_jwt(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            // Create a dummy application
            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Refreshing Access Token - Sync",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            createdApplication.AddAccountStore(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@testmail.stormpath.com")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var originalGrantResult = createdApplication.NewPasswordGrantAuthenticator()
                .Authenticate(passwordGrantRequest);

            var refreshGrantRequest = OauthRequests.NewRefreshGrantRequest()
                .SetRefreshToken(originalGrantResult.RefreshTokenString)
                .Build();

            var refreshGrantResult = createdApplication.NewRefreshGrantAuthenticator()
                .Authenticate(refreshGrantRequest);

            refreshGrantResult.AccessTokenHref.ShouldNotBe(originalGrantResult.AccessTokenHref);
            refreshGrantResult.AccessTokenString.ShouldNotBe(originalGrantResult.AccessTokenString);
            refreshGrantResult.RefreshTokenString.ShouldBe(originalGrantResult.RefreshTokenString);

            // Clean up
            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Refreshing_access_token_with_instance(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            // Create a dummy application
            var createdApplication = tenant.CreateApplication(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Getting Refresh Token for Application - Sync",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            createdApplication.AddAccountStore(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@testmail.stormpath.com")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var originalGrantResult = createdApplication.NewPasswordGrantAuthenticator()
                .Authenticate(passwordGrantRequest);

            var account = tenant.GetAccount(this.fixture.PrimaryAccountHref);
            var refreshToken = account
                .GetRefreshTokens()
                .Where(x => x.ApplicationHref == createdApplication.Href)
                .Synchronously()
                .SingleOrDefault();
            refreshToken.ShouldNotBeNull();

            var refreshGrantRequest = OauthRequests.NewRefreshGrantRequest()
                .SetRefreshToken(refreshToken)
                .Build();

            var refreshGrantResult = createdApplication.NewRefreshGrantAuthenticator()
                .Authenticate(refreshGrantRequest);

            refreshGrantResult.AccessTokenHref.ShouldNotBe(originalGrantResult.AccessTokenHref);
            refreshGrantResult.AccessTokenString.ShouldNotBe(originalGrantResult.AccessTokenString);
            refreshGrantResult.RefreshTokenString.ShouldBe(originalGrantResult.RefreshTokenString);

            // Clean up
            refreshToken.Delete().ShouldBeTrue();

            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }
    }
}
