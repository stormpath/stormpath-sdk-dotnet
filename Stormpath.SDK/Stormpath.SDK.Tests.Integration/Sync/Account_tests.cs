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
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Error;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Integration.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Sync
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
        public void Getting_tenant_accounts(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = client.GetCurrentTenant();

            var accounts = tenant.GetAccounts().Synchronously().ToList();

            accounts.Any().ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Getting_accounts(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var accounts = application.GetAccounts().Synchronously().ToList();

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
        public void Getting_account_provider_data(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var luke = application
                .GetAccounts()
                .Synchronously()
                .Where(x => x.Email.StartsWith("lskywalker"))
                .Single();

            var providerData = luke.GetProviderData();
            providerData.Href.ShouldNotBeNullOrEmpty();
            providerData.ProviderId.ShouldBe("stormpath");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Updating_account(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var leia = application
                .GetAccounts()
                .Synchronously()
                .Where(a => a.Email == "leia.organa@alderaan.core")
                .Single();

            leia.SetMiddleName("Organa");
            leia.SetSurname("Solo");
            var saveResult = leia.Save();

            // In 8 ABY of course
            saveResult.FullName.ShouldBe("Leia Organa Solo");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Saving_with_response_options(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var chewie = application
                .GetAccounts()
                .Synchronously()
                .Where(a => a.Email == "chewie@kashyyyk.rim")
                .Single();

            chewie.SetUsername($"rwaaargh-{this.fixture.TestRunIdentifier}");
            chewie.Save(response => response.Expand(x => x.GetCustomData));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Getting_account_directory(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var luke = application
                .GetAccounts()
                .Synchronously()
                .Filter("Luke")
                .Single();

            // Verify data from IntegrationTestData
            var directoryHref = luke.GetDirectory().Href;
            directoryHref.ShouldBe(this.fixture.PrimaryDirectoryHref);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Getting_account_tenant(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var leia = application
                .GetAccounts()
                .Synchronously()
                .Filter("Leia")
                .Single();

            // Verify data from IntegrationTestData
            var tenantHref = leia.GetTenant().Href;
            tenantHref.ShouldBe(this.fixture.TenantHref);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Searching_accounts_by_email(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var coreCitizens = application
                .GetAccounts()
                .Synchronously()
                .Where(acct => acct.Email.EndsWith(".core"))
                .ToList();

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
        public void Searching_accounts_by_firstname(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var chewie = application
                .GetAccounts()
                .Synchronously()
                .Where(a => a.GivenName == "Chewbacca")
                .Single();

            // Verify data from IntegrationTestData
            chewie.FullName.ShouldBe("Chewbacca the Wookiee");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Searching_accounts_by_lastname(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var palpatine = application
                .GetAccounts()
                .Synchronously()
                .Where(a => a.Surname == "Palpatine")
                .Single();

            // Verify data from IntegrationTestData
            palpatine.FullName.ShouldBe("Emperor Palpatine");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Searching_accounts_by_middle_name(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var chewie = application
                .GetAccounts()
                .Synchronously()
                .Where(a => a.MiddleName == "the")
                .Single();

            // Verify data from IntegrationTestData
            chewie.FullName.ShouldBe("Chewbacca the Wookiee");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Searching_accounts_by_username(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var vader = application
                .GetAccounts()
                .Synchronously()
                .Where(a => a.Username.Equals($"lordvader-{this.fixture.TestRunIdentifier}"))
                .Single();

            // Verify data from IntegrationTestData
            vader.Email.ShouldBe("vader@galacticempire.co");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Searching_accounts_by_status(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var tarkin = application
                .GetAccounts()
                .Synchronously()
                .Where(x => x.Status == AccountStatus.Disabled)
                .Single();

            // Verify data from IntegrationTestData
            tarkin.FullName.ShouldBe("Wilhuff Tarkin");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Searching_accounts_by_creation_date(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            // Make a new account that's created now
            var createdAfter = DateTime.Now.Subtract(TimeSpan.FromSeconds(10));
            var newAccount = application.CreateAccount("Wedge", "Antilles", "wedge@gus-treta.corellia.core", new RandomPassword(12));
            this.fixture.CreatedAccountHrefs.Add(newAccount.Href);

            var rightBeforeCreation = newAccount.CreatedAt.Subtract(TimeSpan.FromSeconds(1));
            var createdRecently = application
                .GetAccounts()
                .Synchronously()
                .Where(x => x.CreatedAt >= rightBeforeCreation)
                .ToList();
            var wedge = createdRecently.Where(x => x.Email == "wedge@gus-treta.corellia.core").Single();
            wedge.FullName.ShouldBe("Wedge Antilles");

            // Clean up
            var didDelete = newAccount.Delete();
            if (didDelete)
                this.fixture.CreatedAccountHrefs.Remove(newAccount.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Searching_accounts_by_creation_date_within_shorthand(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var firstAccount = application.GetAccounts().Synchronously().First();
            var created = firstAccount.CreatedAt;

            var createdToday = application
                .GetAccounts()
                .Synchronously()
                .Where(x => x.CreatedAt.Within(created.Year, created.Month, created.Day))
                .Count();
            createdToday.ShouldNotBe(0);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Searching_accounts_using_filter(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var filtered = application
                .GetAccounts()
                .Synchronously()
                .Filter("lo")
                .ToList();

            filtered.Count.ShouldBeGreaterThanOrEqualTo(3);
            filtered.ShouldContain(acct => acct.FullName == "Han Solo");
            filtered.ShouldContain(acct => acct.Username.StartsWith("lottanerve"));
            filtered.ShouldContain(acct => acct.Username.StartsWith("lordvader"));
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Sorting_accounts_by_lastname(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var accountsSortedByLastName = application
                .GetAccounts()
                .Synchronously()
                .OrderBy(x => x.Surname)
                .ToList();

            var lando = accountsSortedByLastName.First();
            lando.FullName.ShouldBe("Lando Calrissian");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Sorting_accounts_by_username_and_lastname(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var accountsSortedByMultiple = application
                .GetAccounts()
                .Synchronously()
                .OrderByDescending(x => x.Username)
                .OrderByDescending(x => x.Surname)
                .ToList();

            var tarkin = accountsSortedByMultiple.First();
            tarkin.FullName.ShouldBe("Wilhuff Tarkin");
            var luke = accountsSortedByMultiple.ElementAt(1);
            luke.FullName.ShouldBe("Luke Skywalker");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Taking_only_two_accounts(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var firstTwo = application
                .GetAccounts()
                .Synchronously()
                .Take(2)
                .ToList();

            firstTwo.Count.ShouldBe(2);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Counting_accounts(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var count = application.GetAccounts().Synchronously().Count();
            count.ShouldBeGreaterThanOrEqualTo(8);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Any_returns_false_for_empty_filtered_set(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var anyDroids = application
                .GetAccounts()
                .Synchronously()
                .Where(x => x.Email.EndsWith("droids.co"))
                .Any();

            anyDroids.ShouldBeFalse();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Any_returns_true_for_nonempty_filtered_set(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var anyWookiees = application
                .GetAccounts()
                .Synchronously()
                .Where(x => x.Email.EndsWith("kashyyyk.rim"))
                .Any();

            anyWookiees.ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Creating_and_deleting_account(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var account = application.CreateAccount("Gial", "Ackbar", "admiralackbar@dac.rim", new RandomPassword(12));

            account.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedAccountHrefs.Add(account.Href);

            account.FullName.ShouldBe("Gial Ackbar");
            account.Email.ShouldBe("admiralackbar@dac.rim");
            account.Username.ShouldBe("admiralackbar@dac.rim");
            account.Status.ShouldBe(AccountStatus.Enabled);
            account.CreatedAt.ShouldBe(DateTimeOffset.Now, TimeSpan.FromSeconds(10));
            account.ModifiedAt.ShouldBe(DateTimeOffset.Now, TimeSpan.FromSeconds(10));

            var deleted = account.Delete(); // It's a trap! :(
            if (deleted)
                this.fixture.CreatedAccountHrefs.Remove(account.Href);
            deleted.ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Creating_account_with_custom_data(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var account = application.CreateAccount(
                "Mara", "Jade", new RandomEmail("empire.co"), new RandomPassword(12), new { quote = "I'm a fighter. I've always been a fighter.", birth = -17, death = 40 });

            account.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedAccountHrefs.Add(account.Href);
            var customData = account.GetCustomData();

            account.FullName.ShouldBe("Mara Jade");
            customData["quote"].ShouldBe("I'm a fighter. I've always been a fighter.");
            customData["birth"].ShouldBe(-17);
            customData["death"].ShouldBe(40);

            // Clean up
            account.Delete();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Creating_account_with_response_options(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var account = client
                .Instantiate<IAccount>()
                .SetGivenName("Galen")
                .SetSurname("Marek")
                .SetEmail("gmarek@kashyyk.rim")
                .SetPassword(new RandomPassword(12));
            application.CreateAccount(account, opt =>
            {
                opt.ResponseOptions.Expand(x => x.GetCustomData);
            });

            account.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedAccountHrefs.Add(account.Href);

            // Clean up
            account.Delete();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Authenticating_account(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var username = $"sonofthesuns-{this.fixture.TestRunIdentifier}";
            var result = application.AuthenticateAccount(username, "whataPieceofjunk$1138");
            result.ShouldBeAssignableTo<IAuthenticationResult>();
            result.Success.ShouldBeTrue();

            var account = result.GetAccount();
            account.FullName.ShouldBe("Luke Skywalker");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Authenticating_account_with_response_options(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var username = $"sonofthesuns-{this.fixture.TestRunIdentifier}";
            var result = application.AuthenticateAccount(new UsernamePasswordRequest(username, "whataPieceofjunk$1138"), response => response.Expand(x => x.GetAccount));

            result.ShouldBeAssignableTo<IAuthenticationResult>();
            result.Success.ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void TryAuthenticating_account(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var username = $"sonofthesuns-{this.fixture.TestRunIdentifier}";

            application
                .TryAuthenticateAccount(username, "whataPieceofjunk$1138")
                .ShouldBeTrue();

            application
                .TryAuthenticateAccount(username, "notLukesPassword?")
                .ShouldBeFalse();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Authenticating_throws_for_invalid_credentials(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var username = $"sonofthesuns-{this.fixture.TestRunIdentifier}";
            var password = "notLukesPassword?";

            bool didFailCorrectly = false;
            try
            {
                var result = application.AuthenticateAccount(username, password);
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
