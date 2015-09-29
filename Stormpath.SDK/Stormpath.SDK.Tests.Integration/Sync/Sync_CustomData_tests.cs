// <copyright file="Sync_CustomData_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Application;
using Stormpath.SDK.Client;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Integration.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Sync
{
    [Collection("Live tenant tests")]
    public class Sync_CustomData_tests
    {
        private readonly IntegrationTestFixture fixture;

        public Sync_CustomData_tests(IntegrationTestFixture fixture)
        {
            this.fixture = fixture;
        }

        private IAccount CreateRandomAccount(IClient client)
        {
            var accountObject = client.Instantiate<IAccount>();
            accountObject.SetEmail(new RandomEmail("testing.foo"));
            accountObject.SetGivenName("Test");
            accountObject.SetSurname("Testerman");
            accountObject.SetPassword(new RandomPassword(12));

            var app = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);
            var created = app.CreateAccount(accountObject, x => x.RegistrationWorkflowEnabled = false);
            this.fixture.CreatedAccountHrefs.Add(created.Href);

            return created;
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Getting_account_custom_data(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = client.GetResource<IAccount>(this.fixture.CreatedAccountHrefs.First());
            var customData = account.GetCustomData();

            customData.IsEmptyOrDefault().ShouldBeTrue();
            ((string)customData.Get("href")).ShouldNotBeNullOrEmpty();
            ((DateTimeOffset)customData.Get("createdAt")).ShouldNotBeNull();
            ((DateTimeOffset)customData.Get("modifiedAt")).ShouldNotBeNull();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Putting_account_custom_data(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = this.CreateRandomAccount(client);
            var customData = account.GetCustomData();
            customData.IsEmptyOrDefault().ShouldBeTrue();

            // Add some custom data
            customData.Put("appStatsId", 12345);
            var updated = customData.Save();
            updated.IsEmptyOrDefault().ShouldBeFalse();
            updated.Count().ShouldBe(4);

            // Cleanup
            account.Delete();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Deleting_all_custom_data(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = this.CreateRandomAccount(client);
            var customData = account.GetCustomData();
            customData.IsEmptyOrDefault().ShouldBeTrue();

            // Add some custom data
            customData.Put("admin", true);
            customData.Put("status", 1337);
            customData.Put("text", "foobar");
            var updated = customData.Save();
            updated.Count().ShouldBe(6);

            // Try deleting
            var result = updated.Delete();
            var newCustomData = account.GetCustomData();
            newCustomData.Count().ShouldBe(3);

            // Cleanup
            account.Delete();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Clearing_all_custom_data(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = this.CreateRandomAccount(client);
            var customData = account.GetCustomData();
            customData.IsEmptyOrDefault().ShouldBeTrue();

            // Add some custom data
            customData.Put("admin", true);
            customData.Put("status", 1337);
            customData.Put("text", "foobar");
            var updated = customData.Save();
            updated.Count().ShouldBe(6);

            // Expected behavior: works the same as calling Delete (see Deleting_all_custom_data)
            updated.Clear();
            var result = updated.Save();
            var newCustomData = account.GetCustomData();
            newCustomData.Count().ShouldBe(3);

            // Cleanup
            account.Delete();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Deleting_single_item(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();

            var account = this.CreateRandomAccount(client);
            var customData = account.GetCustomData();
            customData.IsEmptyOrDefault().ShouldBeTrue();

            // Add some custom data...
            customData.Put("claims", "canEdit,canCreate");
            customData.Put("text", "fizzbuzz");
            var updated = customData.Save();
            updated.Count().ShouldBe(5);

            // ... and then delete one
            updated.Remove("claims");
            var updated2 = updated.Save();
            updated2.Get("claims").ShouldBeNull();
            updated2.Get("text").ShouldBe("fizzbuzz");

            // Cleanup
            account.Delete();
        }
    }
}
