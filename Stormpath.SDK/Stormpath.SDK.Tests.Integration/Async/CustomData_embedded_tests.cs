// <copyright file="CustomData_embedded_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Client;
using Stormpath.SDK.Tests.Integration.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Async
{
    [Collection(nameof(IntegrationTestCollection))]
    public class CustomData_embedded_tests
    {
        private readonly IntegrationTestFixture fixture;

        public CustomData_embedded_tests(IntegrationTestFixture fixture)
        {
            this.fixture = fixture;
        }

        private IAccount CreateRandomAccountInstance(IClient client)
        {
            var accountObject = client.Instantiate<IAccount>();
            accountObject.SetEmail(new RandomEmail("testing.foo"));
            accountObject.SetGivenName("Test");
            accountObject.SetSurname("Testerman");
            accountObject.SetPassword(new RandomPassword(12));

            return accountObject;
        }

        private async Task<IAccount> SaveAccountAsync(IClient client, IAccount instance)
        {
            var app = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);
            var created = await app.CreateAccountAsync(instance, x => x.RegistrationWorkflowEnabled = false);
            this.fixture.CreatedAccountHrefs.Add(created.Href);

            return created;
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Creating_new_account_with_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var newAccount = this.CreateRandomAccountInstance(client);
            newAccount.CustomData.Put("status", 1337);
            newAccount.CustomData.Put("isAwesome", true);

            var created = await this.SaveAccountAsync(client, newAccount);
            var customData = await created.GetCustomDataAsync();

            customData["status"].ShouldBe(1337);
            customData["isAwesome"].ShouldBe(true);

            (await created.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(created.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Creating_new_application_with_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var newApp = client.Instantiate<IApplication>();
            newApp.SetName(".NET IT App with CustomData Test " + RandomString.Create());
            newApp.CustomData.Put("isCool", true);
            newApp.CustomData.Put("my-custom-data", 1234);

            var created = await client.CreateApplicationAsync(newApp, options => options.CreateDirectory = false);
            this.fixture.CreatedApplicationHrefs.Add(created.Href);
            var customData = await created.GetCustomDataAsync();

            customData["isCool"].ShouldBe(true);
            customData["my-custom-data"].ShouldBe(1234);

            (await created.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedApplicationHrefs.Remove(created.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Editing_embedded_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var newAccount = this.CreateRandomAccountInstance(client);
            newAccount.CustomData.Put("status", 1337);
            newAccount.CustomData.Put("isAwesome", true);

            var created = await this.SaveAccountAsync(client, newAccount);
            created.CustomData.Remove("isAwesome");
            created.CustomData.Put("phrase", "testing is neet");
            await created.SaveAsync();

            var customData = await created.GetCustomDataAsync();
            customData["status"].ShouldBe(1337);
            customData["isAwesome"].ShouldBe(null);
            customData["phrase"].ShouldBe("testing is neet");

            (await created.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(created.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Clearing_embedded_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var newAccount = this.CreateRandomAccountInstance(client);
            newAccount.CustomData.Put("foo", "bar");
            newAccount.CustomData.Put("admin", true);

            var created = await this.SaveAccountAsync(client, newAccount);
            var customData = await created.GetCustomDataAsync();
            customData.IsEmptyOrDefault().ShouldBeFalse();

            created.CustomData.Clear();
            await created.SaveAsync();

            customData = await created.GetCustomDataAsync();
            customData.IsEmptyOrDefault().ShouldBeTrue();

            (await created.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(created.Href);
        }
    }
}
