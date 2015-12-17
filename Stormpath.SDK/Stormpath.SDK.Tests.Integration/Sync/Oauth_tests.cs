// <copyright file="Oauth_tests.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
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
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Common.Integration;
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
                .SetLogin("lskywalker@tattooine.rim")
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

            // Clean up
            accessToken.Delete().ShouldBeTrue();

            createdApplication.Delete().ShouldBeTrue();
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
                .SetLogin("lskywalker@tattooine.rim")
                .SetPassword("notLukesPassword")
                .Build();

            Should.Throw<ResourceException>(() => createdApplication.NewPasswordGrantAuthenticator().AuthenticateAsync(badPasswordGrantRequest));

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
                .SetLogin("lskywalker@tattooine.rim")
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
                .SetLogin("lskywalker@tattooine.rim")
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
                .SetLogin("lskywalker@tattooine.rim")
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

            // Clean up
            refreshTokenForApplication.Delete().ShouldBeTrue();

            createdApplication.Delete().ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }
    }
}
