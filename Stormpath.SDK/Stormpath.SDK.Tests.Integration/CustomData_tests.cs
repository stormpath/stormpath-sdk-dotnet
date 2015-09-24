// <copyright file="CustomData_tests.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Client;
using Stormpath.SDK.Tests.Integration.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Integration
{
    [Collection("Live tenant tests")]
    public class CustomData_tests
    {
        private readonly IntegrationTestFixture fixture;

        public CustomData_tests(IntegrationTestFixture fixture)
        {
            this.fixture = fixture;
        }

        private async Task<IAccount> CreateRandomAccount(IClient client)
        {
            var accountObject = client.Instantiate<IAccount>();
            accountObject.SetEmail(new RandomEmail("testing.foo"));
            accountObject.SetGivenName("Test");
            accountObject.SetSurname("Testerman");
            accountObject.SetPassword(new RandomPassword(12));

            var app = await client.GetApplications().FirstAsync();
            var created = await app.CreateAccountAsync(accountObject, x => x.RegistrationWorkflowEnabled = false);
            this.fixture.CreatedAccountHrefs.Add(created.Href);

            return created;
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Getting_account_custom_data(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = await client.GetResourceAsync<IAccount>(this.fixture.CreatedAccountHrefs.First());
            var customData = await account.GetCustomDataAsync();

            customData.IsEmptyOrDefault().ShouldBeTrue();
            ((string)customData.Get("href")).ShouldNotBeNullOrEmpty();
            ((DateTimeOffset)customData.Get("createdAt")).ShouldNotBeNull();
            ((DateTimeOffset)customData.Get("modifiedAt")).ShouldNotBeNull();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Putting_account_custom_data(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = await this.CreateRandomAccount(client);
            var customData = await account.GetCustomDataAsync();
            customData.IsEmptyOrDefault().ShouldBeTrue();

            // Add some custom data
            customData.Put("appStatsId", 12345);
            var updated = await customData.SaveAsync();
            updated.IsEmptyOrDefault().ShouldBeFalse();
            updated.Count().ShouldBe(4);

            // Cleanup
            await account.DeleteAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Deleting_all_custom_data(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = await this.CreateRandomAccount(client);
            var customData = await account.GetCustomDataAsync();
            customData.IsEmptyOrDefault().ShouldBeTrue();

            // Add some custom data
            customData.Put("admin", true);
            customData.Put("status", 1337);
            customData.Put("text", "foobar");
            var updated = await customData.SaveAsync();
            updated.Count().ShouldBe(6);

            // Try deleting
            var result = await updated.DeleteAsync();
            var newCustomData = await account.GetCustomDataAsync();
            newCustomData.Count().ShouldBe(3);

            // Cleanup
            await account.DeleteAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Deleting_single_item(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = await this.CreateRandomAccount(client);
            var customData = await account.GetCustomDataAsync();
            customData.IsEmptyOrDefault().ShouldBeTrue();

            // Add some custom data...
            customData.Put("claims", "canEdit,canCreate");
            customData.Put("text", "fizzbuzz");
            var updated = await customData.SaveAsync();
            updated.Count().ShouldBe(5);

            // ... and then delete one
            updated.Remove("claims");
            var updated2 = await updated.SaveAsync();
            updated2.Get("claims").ShouldBeNull();
            updated2.Get("text").ShouldBe("fizzbuzz");

            // Cleanup
            await account.DeleteAsync();
        }
    }
}
