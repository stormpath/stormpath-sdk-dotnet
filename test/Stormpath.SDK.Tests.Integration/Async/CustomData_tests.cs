// <copyright file="CustomData_tests.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Client;
using Stormpath.SDK.Tests.Common;
using Stormpath.SDK.Tests.Common.Integration;
using Stormpath.SDK.Tests.Common.RandomData;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Async
{
    [Collection(nameof(IntegrationTestCollection))]
    public class CustomData_tests
    {
        private readonly TestFixture fixture;

        public CustomData_tests(TestFixture fixture)
        {
            this.fixture = fixture;
        }

        private async Task<IAccount> CreateRandomAccountAsync(IClient client)
        {
            var accountObject = client.Instantiate<IAccount>();
            accountObject.SetEmail(new RandomEmail("testing.foo"));
            accountObject.SetGivenName("Test");
            accountObject.SetSurname("Testerman");
            accountObject.SetPassword(new RandomPassword(12));

            var app = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);
            var created = await app.CreateAccountAsync(accountObject, x => x.RegistrationWorkflowEnabled = false);
            this.fixture.CreatedAccountHrefs.Add(created.Href);

            return created;
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Getting_account_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var account = await client.GetResourceAsync<IAccount>(this.fixture.CreatedAccountHrefs.First());
            var customData = await account.GetCustomDataAsync();

            customData.IsEmptyOrDefault().ShouldBeTrue();
            ((string)customData.Get("href")).ShouldNotBeNullOrEmpty();
            ((DateTimeOffset)customData.Get("createdAt")).ShouldNotBeNull();
            ((DateTimeOffset)customData.Get("modifiedAt")).ShouldNotBeNull();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Putting_account_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var account = await this.CreateRandomAccountAsync(client);
            var customData = await account.GetCustomDataAsync();
            customData.IsEmptyOrDefault().ShouldBeTrue();

            // Add some custom data
            customData.Put("appStatsId", 12345);
            var updated = await customData.SaveAsync();
            updated.IsEmptyOrDefault().ShouldBeFalse();
            updated.Count().ShouldBe(4);

            // Cleanup
            (await account.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Putting_arrays_into_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var account = await this.CreateRandomAccountAsync(client);
            var customData = await account.GetCustomDataAsync();
            customData.IsEmptyOrDefault().ShouldBeTrue();

            // Add some custom data
            customData.Put("someStrings", new string[] { "foo", "bar", "baz" });
            customData.Put("someInts", new int[] { 123, 456, 789 });
            await customData.SaveAsync();

            customData.Get<IEnumerable<string>>("someStrings").ShouldBe(new string[] { "foo", "bar", "baz" });
            customData.Get<IEnumerable<int>>("someInts").ShouldBe(new int[] { 123, 456, 789 });

            // Cleanup
            (await account.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Putting_pocos_into_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var account = await this.CreateRandomAccountAsync(client);
            var customData = await account.GetCustomDataAsync();
            customData.IsEmptyOrDefault().ShouldBeTrue();

            // Add some POCOs
            customData.Put("data1", new SimplePoco {Foo = "foobar", Bar = 123});
            customData.Put("data2", new SimplePoco {Foo = "barbaz", Bar = 999});
            await customData.SaveAsync();

            var data1 = customData.Get<SimplePoco>("data1");
            data1.Foo.ShouldBe("foobar");
            data1.Bar.ShouldBe(123);

            var data2 = customData.Get<SimplePoco>("data2");
            data2.Foo.ShouldBe("barbaz");
            data2.Bar.ShouldBe(999);

            // Cleanup
            (await account.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Putting_interesting_types_into_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var account = await this.CreateRandomAccountAsync(client);
            var customData = await account.GetCustomDataAsync();

            // Add some custom data
            customData.Put("double", 1.2345);
            customData.Put("duration", TimeSpan.FromMinutes(3));

            var guid = Guid.NewGuid();
            customData.Put("guid", guid);

            var now = DateTimeOffset.Now;
            customData.Put("now", now);

            await customData.SaveAsync();

            var updated = await account.GetCustomDataAsync();

            updated.Get<double>("double").ShouldBe(1.2345);
            updated.Get<TimeSpan>("duration").ShouldBe(TimeSpan.FromMinutes(3));
            updated.Get<Guid>("guid").ShouldBe(guid);
            updated.Get<DateTimeOffset>("now").ShouldBe(now, tolerance: TimeSpan.FromMilliseconds(50));

            // Cleanup
            (await account.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Creating_account_with_custom_data_inline(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var directory = await client.GetDirectoryAsync(this.fixture.PrimaryDirectoryHref);

            var account = await directory.CreateAccountAsync(
                "Test",
                "Testerman CD Inline",
                new RandomEmail("testing.foo"),
                new RandomPassword(12),
                new { foo = "bar", baz = "qux123!" });

            var customData = await account.GetCustomDataAsync();

            customData.Get("foo").ShouldBe("bar");
            customData["baz"].ShouldBe("qux123!");

            // Cleanup
            (await account.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Creating_account_with_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var directory = await client.GetDirectoryAsync(this.fixture.PrimaryDirectoryHref);

            var account = client.Instantiate<IAccount>()
                .SetGivenName("Test")
                .SetSurname("Testerman CD")
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12));

            account.CustomData.Put("foo", "bar");
            account.CustomData["baz"] = "qux123!";

            await directory.CreateAccountAsync(account);

            var customData = await account.GetCustomDataAsync();

            customData.Get("foo").ShouldBe("bar");
            customData["baz"].ShouldBe("qux123!");

            // Cleanup
            (await account.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Deleting_all_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var account = await this.CreateRandomAccountAsync(client);
            var customData = await account.GetCustomDataAsync();
            customData.IsEmptyOrDefault().ShouldBeTrue();

            // Add some custom data
            customData.Put("admin", true);
            customData.Put("status", 1337);
            customData.Put("text", "foobar");
            var updated = await customData.SaveAsync();
            updated.Count().ShouldBe(6);

            // Try deleting
            await updated.DeleteAsync();
            await Task.Delay(Delay.UpdatePropogation);

            var newCustomData = await account.GetCustomDataAsync();
            newCustomData.Count().ShouldBe(3);

            // Cleanup
            (await account.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Clearing_all_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var account = await this.CreateRandomAccountAsync(client);
            var customData = await account.GetCustomDataAsync();
            customData.IsEmptyOrDefault().ShouldBeTrue();

            // Add some custom data
            customData.Put("admin", true);
            customData.Put("status", 1337);
            customData.Put("text", "foobar");
            var updated = await customData.SaveAsync();
            updated.Count().ShouldBe(6);

            // Expected behavior: works the same as calling DeleteAsync (see Deleting_all_custom_data)
            updated.Clear();
            var result = await updated.SaveAsync();

            await Task.Delay(Delay.UpdatePropogation);

            var newCustomData = await account.GetCustomDataAsync();
            newCustomData.Count().ShouldBe(3);

            // Cleanup
            (await account.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public async Task Deleting_single_item(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var account = await this.CreateRandomAccountAsync(client);
            var customData = await account.GetCustomDataAsync();
            customData.IsEmptyOrDefault().ShouldBeTrue();

            // Add some custom data...
            customData.Put("claims", "canEdit,canCreate");
            customData.Put("text", "fizzbuzz");
            var updated = await customData.SaveAsync();
            updated.Count().ShouldBe(5);

            // ... and then delete one
            updated.Remove("claims");
            await updated.SaveAsync();

            await Task.Delay(Delay.UpdatePropogation);

            var updated2 = await account.GetCustomDataAsync();
            updated2.Get("claims").ShouldBeNull();
            updated2.Get("text").ShouldBe("fizzbuzz");

            // Cleanup
            (await account.DeleteAsync()).ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }
    }
}
