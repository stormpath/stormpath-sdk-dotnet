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

using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Tests.Common.Integration;
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

            authenticateResult.AccessTokenHref.ShouldContain("/accessTokens/");
            authenticateResult.AccessTokenString.ShouldNotBeNullOrEmpty();
            authenticateResult.ExpiresIn.ShouldBeGreaterThan(3600);
            authenticateResult.TokenType.ShouldBe("Bearer");
            authenticateResult.RefreshTokenString.ShouldNotBeNullOrEmpty();

            // Clean up
            (await createdApplication.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(createdApplication.Href);
        }
    }
}
