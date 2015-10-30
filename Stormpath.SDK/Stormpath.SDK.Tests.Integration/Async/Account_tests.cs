// <copyright file="Account_tests.cs" company="Stormpath, Inc.">
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
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Error;
using Stormpath.SDK.Tests.Integration.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Async
{
    [Collection(nameof(IntegrationTestCollection))]
    public class Account_tests
    {
        private readonly IntegrationTestFixture fixture;

        public Account_tests(IntegrationTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Getting_tenant_accounts(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            var accounts = await tenant.GetAccounts().ToListAsync();

            accounts.Any().ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Getting_accounts(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var accounts = await application.GetAccounts().ToListAsync();

            // Verify data from IntegrationTestData
            accounts.Count.ShouldBeGreaterThanOrEqualTo(8);

            var luke = accounts.Where(x => x.GivenName == "Luke").Single();
            luke.FullName.ShouldBe("Luke Skywalker");
            luke.Email.ShouldBe("lskywalker@tattooine.rim");
            luke.Username.ShouldStartWith("sonofthesuns");
            luke.Status.ShouldBe(AccountStatus.Enabled);

            var vader = accounts.Where(x => x.Surname == "Vader").Single();
            vader.FullName.ShouldBe("Darth Vader");
            vader.Email.ShouldStartWith("vader@galacticempire.co");
            vader.Username.ShouldStartWith("lordvader");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Getting_account_provider_data(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var luke = await application
                .GetAccounts()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .SingleAsync();

            var providerData = await luke.GetProviderDataAsync();
            providerData.Href.ShouldNotBeNullOrEmpty();
            providerData.ProviderId.ShouldBe("stormpath");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Updating_account(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var leia = await application
                .GetAccounts()
                .Where(a => a.Email == "leia.organa@alderaan.core")
                .SingleAsync();

            leia.SetMiddleName("Organa");
            leia.SetSurname("Solo");
            var saveResult = await leia.SaveAsync();

            // In 8 ABY of course
            saveResult.FullName.ShouldBe("Leia Organa Solo");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Saving_with_response_options(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var chewie = await application
                .GetAccounts()
                .Where(a => a.Email == "chewie@kashyyyk.rim")
                .SingleAsync();

            chewie.SetUsername($"rwaaargh-{this.fixture.TestRunIdentifier}");
            await chewie.SaveAsync(response => response.Expand(x => x.GetCustomDataAsync));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Getting_account_directory(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var luke = await application
                .GetAccounts()
                .Filter("Luke")
                .SingleAsync();

            // Verify data from IntegrationTestData
            var directoryHref = (await luke.GetDirectoryAsync()).Href;
            directoryHref.ShouldBe(this.fixture.PrimaryDirectoryHref);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Getting_account_tenant(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var leia = await application
                .GetAccounts()
                .Filter("Leia")
                .SingleAsync();

            // Verify data from IntegrationTestData
            var tenantHref = (await leia.GetTenantAsync()).Href;
            tenantHref.ShouldBe(this.fixture.TenantHref);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Searching_accounts_by_email(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var coreCitizens = await application
                .GetAccounts()
                .Where(acct => acct.Email.EndsWith(".core"))
                .ToListAsync();

            // Verify data from IntegrationTestData
            coreCitizens.Count.ShouldBe(2);

            var han = coreCitizens.Where(x => x.GivenName == "Han").Single();
            han.Email.ShouldBe("han.solo@corellia.core");
            han.Username.ShouldStartWith("cptsolo");

            var leia = coreCitizens.Where(x => x.GivenName == "Leia").Single();
            leia.Email.ShouldStartWith("leia.organa@alderaan.core");
            leia.Username.ShouldStartWith("princessleia");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Searching_accounts_by_firstname(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var chewie = await application
                .GetAccounts()
                .Where(a => a.GivenName == "Chewbacca")
                .SingleAsync();

            // Verify data from IntegrationTestData
            chewie.FullName.ShouldBe("Chewbacca the Wookiee");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Searching_accounts_by_lastname(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var palpatine = await application
                .GetAccounts()
                .Where(a => a.Surname == "Palpatine")
                .SingleAsync();

            // Verify data from IntegrationTestData
            palpatine.FullName.ShouldBe("Emperor Palpatine");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Searching_accounts_by_middle_name(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var chewie = await application
                .GetAccounts()
                .Where(a => a.MiddleName == "the")
                .SingleAsync();

            // Verify data from IntegrationTestData
            chewie.FullName.ShouldBe("Chewbacca the Wookiee");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Searching_accounts_by_username(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var vader = await application
                .GetAccounts()
                .Where(a => a.Username.Equals($"lordvader-{this.fixture.TestRunIdentifier}"))
                .SingleAsync();

            // Verify data from IntegrationTestData
            vader.Email.ShouldBe("vader@galacticempire.co");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Searching_accounts_by_status(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var tarkin = await application
                .GetAccounts()
                .Where(x => x.Status == AccountStatus.Disabled)
                .SingleAsync();

            // Verify data from IntegrationTestData
            tarkin.FullName.ShouldBe("Wilhuff Tarkin");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Searching_accounts_by_creation_date(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            // Make a new account that's created now
            var createdAfter = DateTime.Now.Subtract(TimeSpan.FromSeconds(10));
            var newAccount = await application.CreateAccountAsync("Wedge", "Antilles", "wedge@gus-treta.corellia.core", new RandomPassword(12));
            this.fixture.CreatedAccountHrefs.Add(newAccount.Href);

            var rightBeforeCreation = newAccount.CreatedAt.Subtract(TimeSpan.FromSeconds(1));
            var createdRecently = await application
                .GetAccounts()
                .Where(x => x.CreatedAt >= rightBeforeCreation)
                .ToListAsync();
            var wedge = createdRecently.Where(x => x.Email == "wedge@gus-treta.corellia.core").Single();
            wedge.FullName.ShouldBe("Wedge Antilles");

            // Clean up
            var didDelete = await newAccount.DeleteAsync();
            if (didDelete)
                this.fixture.CreatedAccountHrefs.Remove(newAccount.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Searching_accounts_by_creation_date_within_shorthand(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var firstAccount = await application.GetAccounts().FirstAsync();
            var created = firstAccount.CreatedAt;

            var createdToday = await application
                .GetAccounts()
                .Where(x => x.CreatedAt.Within(created.Year, created.Month, created.Day))
                .CountAsync();
            createdToday.ShouldNotBe(0);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Searching_accounts_using_filter(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var filtered = await application
                .GetAccounts()
                .Filter("lo")
                .ToListAsync();

            filtered.Count.ShouldBeGreaterThanOrEqualTo(3);
            filtered.ShouldContain(acct => acct.FullName == "Han Solo");
            filtered.ShouldContain(acct => acct.Username.StartsWith("lottanerve"));
            filtered.ShouldContain(acct => acct.Username.StartsWith("lordvader"));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Sorting_accounts_by_lastname(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var accountsSortedByLastName = await application
                .GetAccounts()
                .OrderBy(x => x.Surname)
                .ToListAsync();

            var lando = accountsSortedByLastName.First();
            lando.FullName.ShouldBe("Lando Calrissian");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Sorting_accounts_by_username_and_lastname(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var accountsSortedByMultiple = await application
                .GetAccounts()
                .OrderByDescending(x => x.Username)
                .OrderByDescending(x => x.Surname)
                .ToListAsync();

            var tarkin = accountsSortedByMultiple.First();
            tarkin.FullName.ShouldBe("Wilhuff Tarkin");
            var luke = accountsSortedByMultiple.ElementAt(1);
            luke.FullName.ShouldBe("Luke Skywalker");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Taking_only_two_accounts(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var firstTwo = await application
                .GetAccounts()
                .Take(2)
                .ToListAsync();

            firstTwo.Count.ShouldBe(2);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Counting_accounts(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var count = await application.GetAccounts().CountAsync();
            count.ShouldBeGreaterThanOrEqualTo(8);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Any_returns_false_for_empty_filtered_set(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var anyDroids = await application
                .GetAccounts()
                .Where(x => x.Email.EndsWith("droids.co"))
                .AnyAsync();

            anyDroids.ShouldBeFalse();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Any_returns_true_for_nonempty_filtered_set(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var anyWookiees = await application
                .GetAccounts()
                .Where(x => x.Email.EndsWith("kashyyyk.rim"))
                .AnyAsync();

            anyWookiees.ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Creating_and_deleting_account(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var account = await application.CreateAccountAsync("Gial", "Ackbar", "admiralackbar@dac.rim", new RandomPassword(12));

            account.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedAccountHrefs.Add(account.Href);

            account.FullName.ShouldBe("Gial Ackbar");
            account.Email.ShouldBe("admiralackbar@dac.rim");
            account.Username.ShouldBe("admiralackbar@dac.rim");
            account.Status.ShouldBe(AccountStatus.Enabled);
            account.CreatedAt.ShouldBe(DateTimeOffset.Now, TimeSpan.FromSeconds(10));
            account.ModifiedAt.ShouldBe(DateTimeOffset.Now, TimeSpan.FromSeconds(10));

            var deleted = await account.DeleteAsync(); // It's a trap! :(
            if (deleted)
                this.fixture.CreatedAccountHrefs.Remove(account.Href);
            deleted.ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Creating_account_with_custom_data(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var account = await application.CreateAccountAsync(
                "Mara", "Jade", new RandomEmail("empire.co"), new RandomPassword(12), new { quote = "I'm a fighter. I've always been a fighter.", birth = -17, death = 40 });

            account.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedAccountHrefs.Add(account.Href);
            var customData = await account.GetCustomDataAsync();

            account.FullName.ShouldBe("Mara Jade");
            customData["quote"].ShouldBe("I'm a fighter. I've always been a fighter.");
            customData["birth"].ShouldBe(-17);
            customData["death"].ShouldBe(40);

            // Clean up
            await account.DeleteAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Creating_account_with_response_options(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var account = client
                .Instantiate<IAccount>()
                .SetGivenName("Galen")
                .SetSurname("Marek")
                .SetEmail("gmarek@kashyyk.rim")
                .SetPassword(new RandomPassword(12));
            await application.CreateAccountAsync(account, opt =>
            {
                opt.ResponseOptions.Expand(x => x.GetCustomDataAsync);
            });

            account.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedAccountHrefs.Add(account.Href);

            // Clean up
            await account.DeleteAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Authenticating_account(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var username = $"sonofthesuns-{this.fixture.TestRunIdentifier}";
            var result = await application.AuthenticateAccountAsync(username, "whataPieceofjunk$1138");
            result.ShouldBeAssignableTo<IAuthenticationResult>();
            result.Success.ShouldBeTrue();

            var account = await result.GetAccountAsync();
            account.FullName.ShouldBe("Luke Skywalker");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Authenticating_account_with_response_options(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var username = $"sonofthesuns-{this.fixture.TestRunIdentifier}";
            var result = await application.AuthenticateAccountAsync(new UsernamePasswordRequest(username, "whataPieceofjunk$1138"), response => response.Expand(x => x.GetAccountAsync));

            result.ShouldBeAssignableTo<IAuthenticationResult>();
            result.Success.ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task TryAuthenticating_account(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var username = $"sonofthesuns-{this.fixture.TestRunIdentifier}";

            (await application.TryAuthenticateAccountAsync(username, "whataPieceofjunk$1138"))
                .ShouldBeTrue();

            (await application.TryAuthenticateAccountAsync(username, "notLukesPassword?"))
                .ShouldBeFalse();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Authenticating_throws_for_invalid_credentials(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var username = $"sonofthesuns-{this.fixture.TestRunIdentifier}";
            var password = "notLukesPassword?";

            bool didFailCorrectly = false;
            try
            {
                var result = await application.AuthenticateAccountAsync(username, password);
            }
            catch (ResourceException rex)
            {
                didFailCorrectly = rex.HttpStatus == 400;
            }
            catch
            {
                didFailCorrectly = false;
            }

            Assert.True(didFailCorrectly);
        }
    }
}
