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
using System.Threading.Tasks;
using FluentAssertions;
using Shouldly;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Error;
using Stormpath.SDK.Jwt;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Tests.Common;
using Stormpath.SDK.Tests.Common.Integration;
using Stormpath.SDK.Tests.Common.RandomData;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Async
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
        public async Task Creating_token_with_password_grant(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            // Create a dummy application
            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Creating Token With Password Grant Flow",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            await createdApplication.AddAccountStoreAsync(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@tattooine.rim")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var authenticateResult = await createdApplication.NewPasswordGrantAuthenticator()
                .AuthenticateAsync(passwordGrantRequest);

            // Verify authentication response
            authenticateResult.AccessTokenHref.ShouldContain("/accessTokens/");
            authenticateResult.AccessTokenString.ShouldNotBeNullOrEmpty();
            authenticateResult.ExpiresIn.ShouldBeGreaterThanOrEqualTo(3600);
            authenticateResult.TokenType.ShouldBe("Bearer");
            authenticateResult.RefreshTokenString.ShouldNotBeNullOrEmpty();

            // Verify generated access token
            var accessToken = await authenticateResult.GetAccessTokenAsync();
            accessToken.CreatedAt.ShouldNotBe(default(DateTimeOffset));
            accessToken.Href.ShouldBe(authenticateResult.AccessTokenHref);
            accessToken.Jwt.ShouldBe(authenticateResult.AccessTokenString);
            accessToken.ApplicationHref.ShouldBe(createdApplication.Href);
            accessToken.AccountHref.ShouldNotBeNullOrEmpty();

            // Get account (with some expansions)
            var account = await accessToken.GetAccountAsync(opt => opt.Expand(acct => acct.GetGroups()));
            account.Email.ShouldBe("lskywalker@tattooine.rim");

            // Clean up
            (await accessToken.DeleteAsync()).ShouldBeTrue();

            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Creating_token_with_password_grant_and_nameKey(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            // Create a dummy application
            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Creating Token With Password Grant Flow + ONK",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test organization
            var primaryOrg = await client.GetOrganizationAsync(fixture.PrimaryOrganizationHref);
            await createdApplication.AddAccountStoreAsync(primaryOrg);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@tattooine.rim")
                .SetPassword("whataPieceofjunk$1138")
                .SetOrganizationNameKey(fixture.PrimaryOrganizationNameKey)
                .Build();
            var authenticateResult = await createdApplication.NewPasswordGrantAuthenticator()
                .AuthenticateAsync(passwordGrantRequest);

            // Clean up
            var accessToken = await authenticateResult.GetAccessTokenAsync();
            (await accessToken.DeleteAsync()).ShouldBeTrue();

            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        /// <summary>
        /// Regression test for stormpath/stormpath-sdk-dotnet#161
        /// </summary>
        /// <remarks>The ! character was causing the SAuthc1 signer to break.</remarks>
        /// <param name="clientBuilder">The client builder.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Password_grant_with_special_characters(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            // Create a dummy application
            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Password Grant Flow With Special Characters",
                createDirectory: true);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            var createdDirectory = await createdApplication.GetDefaultAccountStoreAsync();
            this.fixture.CreatedDirectoryHrefs.Add(createdDirectory.Href);

            // Add the test accounts
            var randomEmail = new RandomEmail("foo.bar");
            var password = "P@sword#123$!";
            await createdApplication.CreateAccountAsync("Test", "testerman", randomEmail, password);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin(randomEmail)
                .SetPassword(password)
                .Build();
            var authenticateResult = await createdApplication.NewPasswordGrantAuthenticator()
                .AuthenticateAsync(passwordGrantRequest);

            // Verify authentication response
            authenticateResult.AccessTokenString.ShouldNotBeNullOrEmpty();

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Failed_password_grant_throws_ResourceException(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            // Create a dummy application
            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Failed Password Grant Throws",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            await createdApplication.AddAccountStoreAsync(this.fixture.PrimaryDirectoryHref);

            var badPasswordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@tattooine.rim")
                .SetPassword("notLukesPassword")
                .Build();

            Func<Task> act = async () => await createdApplication.NewPasswordGrantAuthenticator().AuthenticateAsync(badPasswordGrantRequest);
            act.ShouldThrow<ResourceException>();

            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Listing_account_tokens(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            // Create a dummy application
            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Listing Tokens",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            await createdApplication.AddAccountStoreAsync(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@tattooine.rim")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var authenticateResult = await createdApplication.NewPasswordGrantAuthenticator()
                .AuthenticateAsync(passwordGrantRequest);

            var account = await tenant.GetAccountAsync(this.fixture.PrimaryAccountHref);
            var accessTokens = await account.GetAccessTokens().ToListAsync();
            var refreshTokens = await account.GetRefreshTokens().ToListAsync();

            var accessToken = accessTokens.Where(x => x.Jwt == authenticateResult.AccessTokenString).SingleOrDefault();
            var refreshToken = refreshTokens.Where(x => x.Jwt == authenticateResult.RefreshTokenString).SingleOrDefault();
            accessToken.ShouldNotBeNull();
            refreshToken.ShouldNotBeNull();

            // Clean up
            (await accessToken.DeleteAsync()).ShouldBeTrue();
            (await refreshToken.DeleteAsync()).ShouldBeTrue();

            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Getting_access_token_for_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            // Create a dummy application
            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Getting Access Token for Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            await createdApplication.AddAccountStoreAsync(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@tattooine.rim")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var authenticateResult = await createdApplication.NewPasswordGrantAuthenticator()
                .AuthenticateAsync(passwordGrantRequest);

            var account = await tenant.GetAccountAsync(this.fixture.PrimaryAccountHref);
            var accessTokenForApplication = await account
                .GetAccessTokens()
                .Where(x => x.ApplicationHref == createdApplication.Href)
                .SingleOrDefaultAsync();

            accessTokenForApplication.ShouldNotBeNull();

            (await accessTokenForApplication.GetAccountAsync()).Href.ShouldBe(this.fixture.PrimaryAccountHref);
            (await accessTokenForApplication.GetApplicationAsync()).Href.ShouldBe(createdApplication.Href);
            (await accessTokenForApplication.GetTenantAsync()).Href.ShouldBe(this.fixture.TenantHref);

            var retrievedDirectly = await client.GetAccessTokenAsync(accessTokenForApplication.Href);
            retrievedDirectly.Href.ShouldBe(accessTokenForApplication.Href);

            // Clean up
            (await accessTokenForApplication.DeleteAsync()).ShouldBeTrue();

            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Getting_refresh_token_for_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            // Create a dummy application
            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Getting Refresh Token for Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            await createdApplication.AddAccountStoreAsync(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@tattooine.rim")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var authenticateResult = await createdApplication.NewPasswordGrantAuthenticator()
                .AuthenticateAsync(passwordGrantRequest);

            var account = await tenant.GetAccountAsync(this.fixture.PrimaryAccountHref);
            var refreshTokenForApplication = await account
                .GetRefreshTokens()
                .Where(x => x.ApplicationHref == createdApplication.Href)
                .SingleOrDefaultAsync();

            refreshTokenForApplication.ShouldNotBeNull();

            (await refreshTokenForApplication.GetAccountAsync()).Href.ShouldBe(this.fixture.PrimaryAccountHref);
            (await refreshTokenForApplication.GetApplicationAsync()).Href.ShouldBe(createdApplication.Href);
            (await refreshTokenForApplication.GetTenantAsync()).Href.ShouldBe(this.fixture.TenantHref);

            var retrievedDirectly = await client.GetRefreshTokenAsync(refreshTokenForApplication.Href);
            retrievedDirectly.Href.ShouldBe(refreshTokenForApplication.Href);

            // Get account (with some expansions)
            var tokenAccount = await retrievedDirectly.GetAccountAsync(opt => opt.Expand(acct => acct.GetGroups()));
            account.Email.ShouldBe(account.Email);

            // Clean up
            (await refreshTokenForApplication.DeleteAsync()).ShouldBeTrue();

            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Validating_jwt(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            // Create a dummy application
            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Validating JWT",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            await createdApplication.AddAccountStoreAsync(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@tattooine.rim")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var authenticateResult = await createdApplication.NewPasswordGrantAuthenticator()
                .AuthenticateAsync(passwordGrantRequest);
            var accessTokenJwt = authenticateResult.AccessTokenString;

            var jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest()
                .SetJwt(accessTokenJwt)
                .Build();
            var validAccessToken = await createdApplication.NewJwtAuthenticator()
                .AuthenticateAsync(jwtAuthenticationRequest);

            validAccessToken.ShouldNotBeNull();
            validAccessToken.ApplicationHref.ShouldBe(createdApplication.Href);
            validAccessToken.AccountHref.ShouldNotBeNullOrEmpty();
            validAccessToken.CreatedAt.ShouldBe(DateTimeOffset.Now, Delay.ReasonableTestRunWindow);
            validAccessToken.Href.ShouldBe(authenticateResult.AccessTokenHref);
            validAccessToken.Jwt.ShouldBe(accessTokenJwt);

            // Clean up
            (await validAccessToken.DeleteAsync()).ShouldBeTrue();

            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Validating_jwt_throws_for_bad_jwt(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            // Create a dummy application
            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Validating JWT",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            await createdApplication.AddAccountStoreAsync(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@tattooine.rim")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var authenticateResult = await createdApplication.NewPasswordGrantAuthenticator()
                .AuthenticateAsync(passwordGrantRequest);

            var badJwt = authenticateResult.AccessTokenString
                .Substring(0, authenticateResult.AccessTokenString.Length - 3) + "foo";

            var jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest()
                .SetJwt(badJwt)
                .Build();

            Func<Task> act = async () =>
                await createdApplication.NewJwtAuthenticator().AuthenticateAsync(jwtAuthenticationRequest);
            act.ShouldThrow<ResourceException>();

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Validating_jwt_locally(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            // Create a dummy application
            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Validating JWT Locally",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            await createdApplication.AddAccountStoreAsync(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@tattooine.rim")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var authenticateResult = await createdApplication.NewPasswordGrantAuthenticator()
                .AuthenticateAsync(passwordGrantRequest);
            var accessTokenJwt = authenticateResult.AccessTokenString;

            var jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest()
                .SetJwt(accessTokenJwt)
                .Build();
            var validAccessToken = await createdApplication.NewJwtAuthenticator()
                .WithLocalValidation()
                .AuthenticateAsync(jwtAuthenticationRequest);

            validAccessToken.ShouldNotBeNull();
            validAccessToken.ApplicationHref.ShouldBe(createdApplication.Href);
            validAccessToken.AccountHref.ShouldNotBeNullOrEmpty();
            validAccessToken.CreatedAt.ShouldBe(DateTimeOffset.Now, Delay.ReasonableTestRunWindow);
            validAccessToken.Href.ShouldBe(authenticateResult.AccessTokenHref);
            validAccessToken.Jwt.ShouldBe(accessTokenJwt);

            // Clean up
            (await validAccessToken.DeleteAsync()).ShouldBeTrue();

            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Validating_jwt_locally_throws_for_bad_jwt(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            // Create a dummy application
            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Validating JWT Locally",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            await createdApplication.AddAccountStoreAsync(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@tattooine.rim")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var authenticateResult = await createdApplication.NewPasswordGrantAuthenticator()
                .AuthenticateAsync(passwordGrantRequest);

            var badJwt = authenticateResult.AccessTokenString
                .Substring(0, authenticateResult.AccessTokenString.Length - 3) + "foo";

            var jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest()
                .SetJwt(badJwt)
                .Build();

            Func<Task> act = async () =>
                await createdApplication.NewJwtAuthenticator().WithLocalValidation().AuthenticateAsync(jwtAuthenticationRequest);
            act.ShouldThrow<JwtSignatureException>();

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Validating_token_after_revocation(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            // Create a dummy application
            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Validating Token After Revocation",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            await createdApplication.AddAccountStoreAsync(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@tattooine.rim")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var authenticateResult = await createdApplication.NewPasswordGrantAuthenticator()
                .AuthenticateAsync(passwordGrantRequest);

            var accessToken = await authenticateResult.GetAccessTokenAsync();
            (await accessToken.DeleteAsync()).ShouldBeTrue(); // Revoke access token

            var jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest()
                .SetJwt(accessToken.Jwt)
                .Build();

            Func<Task> act = async () => await createdApplication.NewJwtAuthenticator().AuthenticateAsync(jwtAuthenticationRequest);
            act.ShouldThrow<ResourceException>();

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Refreshing_access_token_with_jwt(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            // Create a dummy application
            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Refreshing Access Token",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            await createdApplication.AddAccountStoreAsync(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@tattooine.rim")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var originalGrantResult = await createdApplication.NewPasswordGrantAuthenticator()
                .AuthenticateAsync(passwordGrantRequest);

            var refreshGrantRequest = OauthRequests.NewRefreshGrantRequest()
                .SetRefreshToken(originalGrantResult.RefreshTokenString)
                .Build();

            var refreshGrantResult = await createdApplication.NewRefreshGrantAuthenticator()
                .AuthenticateAsync(refreshGrantRequest);

            refreshGrantResult.AccessTokenHref.ShouldNotBe(originalGrantResult.AccessTokenHref);
            refreshGrantResult.AccessTokenString.ShouldNotBe(originalGrantResult.AccessTokenString);
            refreshGrantResult.RefreshTokenString.ShouldBe(originalGrantResult.RefreshTokenString);

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Refreshing_access_token_with_instance(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = await client.GetCurrentTenantAsync();

            // Create a dummy application
            var createdApplication = await tenant.CreateApplicationAsync(
                $".NET IT {this.fixture.TestRunIdentifier}-{clientBuilder.Name} Getting Refresh Token for Application",
                createDirectory: false);
            createdApplication.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(createdApplication.Href);

            // Add the test accounts
            await createdApplication.AddAccountStoreAsync(this.fixture.PrimaryDirectoryHref);

            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@tattooine.rim")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(this.fixture.PrimaryDirectoryHref)
                .Build();
            var originalGrantResult = await createdApplication.NewPasswordGrantAuthenticator()
                .AuthenticateAsync(passwordGrantRequest);

            var account = await tenant.GetAccountAsync(this.fixture.PrimaryAccountHref);
            var refreshToken = await account
                .GetRefreshTokens()
                .Where(x => x.ApplicationHref == createdApplication.Href)
                .SingleOrDefaultAsync();
            refreshToken.ShouldNotBeNull();

            var refreshGrantRequest = OauthRequests.NewRefreshGrantRequest()
                .SetRefreshToken(refreshToken)
                .Build();

            var refreshGrantResult = await createdApplication.NewRefreshGrantAuthenticator()
                .AuthenticateAsync(refreshGrantRequest);

            refreshGrantResult.AccessTokenHref.ShouldNotBe(originalGrantResult.AccessTokenHref);
            refreshGrantResult.AccessTokenString.ShouldNotBe(originalGrantResult.AccessTokenString);
            refreshGrantResult.RefreshTokenString.ShouldBe(originalGrantResult.RefreshTokenString);

            // Clean up
            (await refreshToken.DeleteAsync()).ShouldBeTrue();

            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }
    }
}
