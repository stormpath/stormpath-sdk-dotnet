// <copyright file="ApiKey_tests.cs" company="Stormpath, Inc.">
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
using FluentAssertions;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Api;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Tests.Common.Integration;
using Stormpath.SDK.Tests.Common.RandomData;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Async
{
    [Collection(nameof(IntegrationTestCollection))]
    public class ApiKey_tests
    {
        private readonly TestFixture fixture;

        public ApiKey_tests(TestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Creating_and_deleting_api_key(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = await client.GetApplicationAsync(this.fixture.PrimaryApplicationHref);
            var account = await app.CreateAccountAsync("ApiKey", "Tester", new RandomEmail("testmail.stormpath.com"), new RandomPassword(12));
            this.fixture.CreatedAccountHrefs.Add(account.Href);

            (await account.GetApiKeys().CountAsync()).ShouldBe(0);

            var newKey1 = await account.CreateApiKeyAsync();
            var newKey2 = await account.CreateApiKeyAsync(opt =>
            {
                opt.Expand(e => e.GetAccount());
                opt.Expand(e => e.GetTenant());
            });

            var keysList = await account.GetApiKeys().ToListAsync();
            keysList.Count.ShouldBe(2);
            keysList.ShouldContain(x => x.Href == newKey1.Href);
            keysList.ShouldContain(x => x.Href == newKey2.Href);

            (await newKey1.DeleteAsync()).ShouldBeTrue();
            (await newKey2.DeleteAsync()).ShouldBeTrue();

            // Clean up
            (await account.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Updating_api_key(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = await client.GetApplicationAsync(this.fixture.PrimaryApplicationHref);
            var account = await app.CreateAccountAsync("ApiKey", "Tester", new RandomEmail("testmail.stormpath.com"), new RandomPassword(12));
            this.fixture.CreatedAccountHrefs.Add(account.Href);

            var newKey = await account.CreateApiKeyAsync();
            newKey.Status.ShouldBe(ApiKeyStatus.Enabled);

            newKey.SetStatus(ApiKeyStatus.Disabled);
            await newKey.SaveAsync();
            newKey.Status.ShouldBe(ApiKeyStatus.Disabled);

            var retrieved = await account.GetApiKeys().SingleAsync();
            retrieved.Status.ShouldBe(ApiKeyStatus.Disabled);

            // Clean up
            (await newKey.DeleteAsync()).ShouldBeTrue();

            (await account.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Updating_api_key_name_and_description(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = await client.GetApplicationAsync(this.fixture.PrimaryApplicationHref);
            var account = await app.CreateAccountAsync("ApiKey", "Tester", new RandomEmail("testmail.stormpath.com"), new RandomPassword(12));
            this.fixture.CreatedAccountHrefs.Add(account.Href);

            var newKey = await account.CreateApiKeyAsync();
            newKey.Name.Should().BeNull();
            newKey.Description.Should().BeNull();

            newKey.SetName("Awesome key");
            newKey.SetDescription("This key was awesome before API authentication was cool");

            var updated = await newKey.SaveAsync();

            updated.Name.Should().Be("Awesome key");
            updated.Description.Should().Be("This key was awesome before API authentication was cool");

            // Clean up
            (await newKey.DeleteAsync()).ShouldBeTrue();

            (await account.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Looking_up_api_key_via_application(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = await client.GetApplicationAsync(this.fixture.PrimaryApplicationHref);
            var account = await app.CreateAccountAsync("ApiKey", "Tester", new RandomEmail("testmail.stormpath.com"), new RandomPassword(12));
            this.fixture.CreatedAccountHrefs.Add(account.Href);

            var newKey = await account.CreateApiKeyAsync();

            var foundKey = await app.GetApiKeyAsync(newKey.Id, opt =>
            {
                opt.Expand(e => e.GetAccount());
                opt.Expand(e => e.GetTenant());
            });
            var foundAccount = await foundKey.GetAccountAsync();
            foundAccount.Href.ShouldBe(account.Href);

            // Clean up
            (await newKey.DeleteAsync()).ShouldBeTrue();

            (await account.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Authenticating_api_key(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = await client.GetApplicationAsync(this.fixture.PrimaryApplicationHref);
            var account = await app.CreateAccountAsync("ApiKey", "Tester", new RandomEmail("testmail.stormpath.com"), new RandomPassword(12));
            this.fixture.CreatedAccountHrefs.Add(account.Href);

            var newKey = await account.CreateApiKeyAsync();

            var apiKeyAuthRequest = new ApiKeyRequestBuilder()
                .SetId(newKey.Id)
                .SetSecret(newKey.Secret)
                .Build();

            var result = await app.AuthenticateAccountAsync(apiKeyAuthRequest);
            var resultAccount = await result.GetAccountAsync();

            resultAccount.Href.ShouldBe(account.Href);

            // Clean up
            (await newKey.DeleteAsync()).ShouldBeTrue();

            (await account.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Throws_when_id_is_invalid(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = await client.GetApplicationAsync(this.fixture.PrimaryApplicationHref);
            var account = await app.CreateAccountAsync("ApiKey", "Tester", new RandomEmail("testmail.stormpath.com"), new RandomPassword(12));
            this.fixture.CreatedAccountHrefs.Add(account.Href);

            var newKey = await account.CreateApiKeyAsync();

            var apiKeyAuthRequest = new ApiKeyRequestBuilder()
                .SetId("FOOBAR1")
                .SetSecret(newKey.Secret)
                .Build();

            Func<Task> act = async () => { await app.AuthenticateAccountAsync(apiKeyAuthRequest); };
            act.ShouldThrow<IncorrectCredentialsException>();

            // Clean up
            (await newKey.DeleteAsync()).ShouldBeTrue();

            (await account.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Throws_when_secret_is_invalid(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = await client.GetApplicationAsync(this.fixture.PrimaryApplicationHref);
            var account = await app.CreateAccountAsync("ApiKey", "Tester", new RandomEmail("testmail.stormpath.com"), new RandomPassword(12));
            this.fixture.CreatedAccountHrefs.Add(account.Href);

            var newKey = await account.CreateApiKeyAsync();

            var apiKeyAuthRequest = new ApiKeyRequestBuilder()
                .SetId(newKey.Id)
                .SetSecret("notARealSecret123")
                .Build();

            Func<Task> act = async () => { await app.AuthenticateAccountAsync(apiKeyAuthRequest); };
            act.ShouldThrow<IncorrectCredentialsException>();

            // Clean up
            (await newKey.DeleteAsync()).ShouldBeTrue();

            (await account.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Throws_when_key_is_disabled(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = await client.GetApplicationAsync(this.fixture.PrimaryApplicationHref);
            var account = await app.CreateAccountAsync("ApiKey", "Tester", new RandomEmail("testmail.stormpath.com"), new RandomPassword(12));
            this.fixture.CreatedAccountHrefs.Add(account.Href);

            var newKey = await account.CreateApiKeyAsync();
            newKey.SetStatus(ApiKeyStatus.Disabled);
            await newKey.SaveAsync();

            var apiKeyAuthRequest = new ApiKeyRequestBuilder()
                .SetId(newKey.Id)
                .SetSecret(newKey.Secret)
                .Build();

            Func<Task> act = async () => { await app.AuthenticateAccountAsync(apiKeyAuthRequest); };
            act.ShouldThrow<DisabledApiKeyException>();

            // Clean up
            (await newKey.DeleteAsync()).ShouldBeTrue();

            (await account.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Throws_when_account_is_disabled(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var app = await client.GetApplicationAsync(this.fixture.PrimaryApplicationHref);

            var account = await app.CreateAccountAsync("ApiKey", "Tester", new RandomEmail("testmail.stormpath.com"), new RandomPassword(12));
            this.fixture.CreatedAccountHrefs.Add(account.Href);

            account.SetStatus(AccountStatus.Disabled);
            await account.SaveAsync();

            var newKey = await account.CreateApiKeyAsync();

            var apiKeyAuthRequest = new ApiKeyRequestBuilder()
                .SetId(newKey.Id)
                .SetSecret(newKey.Secret)
                .Build();

            Func<Task> act = async () => { await app.AuthenticateAccountAsync(apiKeyAuthRequest); };
            act.ShouldThrow<DisabledAccountException>();

            // Clean up
            (await newKey.DeleteAsync()).ShouldBeTrue();

            (await account.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }
    }
}
