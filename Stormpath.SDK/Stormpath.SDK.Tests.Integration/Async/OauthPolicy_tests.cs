// <copyright file="OauthPolicy_tests.cs" company="Stormpath, Inc.">
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
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Tests.Common.Integration;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Async
{
    [Collection(nameof(IntegrationTestCollection))]
    public class OauthPolicy_tests
    {
        private readonly TestFixture fixture;

        public OauthPolicy_tests(TestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Getting_policy_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = await client.GetApplicationAsync(this.fixture.PrimaryApplicationHref);

            var policy = await app.GetOauthPolicyAsync();
            policy.ShouldNotBeNull();

            var associatedApp = await policy.GetApplicationAsync();
            associatedApp.ShouldNotBeNull();
            associatedApp.Href.ShouldBe(app.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Getting_policy_tenant(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = await client.GetApplicationAsync(this.fixture.PrimaryApplicationHref);

            var policy = await app.GetOauthPolicyAsync();
            policy.ShouldNotBeNull();

            var tenant = await policy.GetTenantAsync();
            tenant.Href.ShouldBe((await app.GetTenantAsync()).Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Getting_policy(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = await client.CreateApplicationAsync(
                $".NET ITs Default OAuth Policy Application {this.fixture.TestRunIdentifier}-{clientBuilder.Name}",
                createDirectory: false);
            app.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(app.Href);

            var policy = await app.GetOauthPolicyAsync();

            // Default OAuth policy values are managed by Stormpath.
            policy.ShouldNotBeNull();
            policy.AccessTokenTimeToLive.ShouldBe(TimeSpan.FromHours(1));
            policy.RefreshTokenTimeToLive.ShouldBe(TimeSpan.FromDays(60));
            policy.TokenEndpointHref.ShouldEndWith("oauth/token");

            // Clean up
            (await app.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(app.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Updating_policy(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = await client.CreateApplicationAsync(
                $".NET ITs Modified OAuth Policy Application {this.fixture.TestRunIdentifier}-{clientBuilder.Name}",
                createDirectory: false);
            app.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedApplicationHrefs.Add(app.Href);

            var policy = await app.GetOauthPolicyAsync();

            policy.SetAccessTokenTimeToLive(TimeSpan.FromDays(8));
            policy.SetRefreshTokenTimeToLive(TimeSpan.FromDays(180));
            await policy.SaveAsync();

            var policyUpdated = await app.GetOauthPolicyAsync();
            policyUpdated.AccessTokenTimeToLive.ShouldBe(TimeSpan.FromDays(8));
            policyUpdated.RefreshTokenTimeToLive.ShouldBe(TimeSpan.FromDays(180));

            // Clean up
            (await app.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(app.Href);
        }
    }
}
