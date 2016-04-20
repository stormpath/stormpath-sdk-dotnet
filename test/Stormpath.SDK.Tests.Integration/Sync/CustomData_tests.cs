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
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Client;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Common;
using Stormpath.SDK.Tests.Common.Integration;
using Stormpath.SDK.Tests.Common.RandomData;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Sync
{
    [Collection(nameof(IntegrationTestCollection))]
    public class CustomData_tests
    {
        private readonly TestFixture fixture;

        public CustomData_tests(TestFixture fixture)
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_account_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var account = client.GetResource<IAccount>(this.fixture.CreatedAccountHrefs.First());
            var customData = account.GetCustomData();

            customData.IsEmptyOrDefault().ShouldBeTrue();
            ((string)customData.Get("href")).ShouldNotBeNullOrEmpty();
            ((DateTimeOffset)customData.Get("createdAt")).ShouldNotBeNull();
            ((DateTimeOffset)customData.Get("modifiedAt")).ShouldNotBeNull();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Putting_account_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var account = this.CreateRandomAccount(client);
            var customData = account.GetCustomData();
            customData.IsEmptyOrDefault().ShouldBeTrue();

            // Add some custom data
            customData.Put("appStatsId", 12345);
            var updated = customData.Save();
            updated.IsEmptyOrDefault().ShouldBeFalse();
            updated.Count().ShouldBe(4);

            // Cleanup
            account.Delete().ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Putting_arrays_into_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var account = this.CreateRandomAccount(client);
            var customData = account.GetCustomData();
            customData.IsEmptyOrDefault().ShouldBeTrue();

            // Add some custom data
            customData.Put("someStrings", new string[] { "foo", "bar", "baz" });
            customData.Put("someInts", new int[] { 123, 456, 789 });
            customData.Save();

            customData.Get<IEnumerable<string>>("someStrings").ShouldBe(new string[] { "foo", "bar", "baz" });
            customData.Get<IEnumerable<int>>("someInts").ShouldBe(new int[] { 123, 456, 789 });

            // Cleanup
            account.Delete().ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Putting_interesting_types_into_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var account = this.CreateRandomAccount(client);
            var customData = account.GetCustomData();

            // Add some custom data
            customData.Put("double", 1.2345);
            customData.Put("duration", TimeSpan.FromMinutes(3));

            var guid = Guid.NewGuid();
            customData.Put("guid", guid);

            var now = DateTimeOffset.Now;
            customData.Put("now", now);

            customData.Save();

            var updated = account.GetCustomData();

            updated.Get<double>("double").ShouldBe(1.2345);
            updated.Get<TimeSpan>("duration").ShouldBe(TimeSpan.FromMinutes(3));
            updated.Get<Guid>("guid").ShouldBe(guid);
            updated.Get<DateTimeOffset>("now").ShouldBe(now, tolerance: TimeSpan.FromMilliseconds(50));

            // Cleanup
            account.Delete().ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Creating_account_with_custom_data_inline(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var directory = client.GetDirectory(this.fixture.PrimaryDirectoryHref);

            var account = directory.CreateAccount(
                "Test",
                "Testerman CD Inline",
                new RandomEmail("testing.foo"),
                new RandomPassword(12),
                new { foo = "bar", baz = "qux123!" });

            var customData = account.GetCustomData();

            customData.Get("foo").ShouldBe("bar");
            customData["baz"].ShouldBe("qux123!");

            // Cleanup
            account.Delete().ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Creating_account_with_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var directory = client.GetDirectory(this.fixture.PrimaryDirectoryHref);

            var account = client.Instantiate<IAccount>()
                .SetGivenName("Test")
                .SetSurname("Testerman CD")
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12));

            account.CustomData.Put("foo", "bar");
            account.CustomData["baz"] = "qux123!";

            directory.CreateAccount(account);

            var customData = account.GetCustomData();

            customData.Get("foo").ShouldBe("bar");
            customData["baz"].ShouldBe("qux123!");

            // Cleanup
            account.Delete().ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Deleting_all_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

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
            updated.Delete();
            System.Threading.Thread.Sleep(Delay.UpdatePropogation);

            var newCustomData = account.GetCustomData();
            newCustomData.Count().ShouldBe(3);

            // Cleanup
            account.Delete().ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Clearing_all_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

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

            System.Threading.Thread.Sleep(Delay.UpdatePropogation);

            var newCustomData = account.GetCustomData();
            newCustomData.Count().ShouldBe(3);

            // Cleanup
            account.Delete().ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Deleting_single_item(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

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
            updated.Save();

            System.Threading.Thread.Sleep(Delay.UpdatePropogation);

            var updated2 = account.GetCustomData();
            updated2.Get("claims").ShouldBeNull();
            updated2.Get("text").ShouldBe("fizzbuzz");

            // Cleanup
            account.Delete().ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }
    }
}
