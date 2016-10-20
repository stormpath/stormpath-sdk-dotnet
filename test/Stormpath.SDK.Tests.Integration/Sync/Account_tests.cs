// <copyright file="Account_tests.cs" company="Stormpath, Inc.">
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
using System.Linq;
using FluentAssertions;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Error;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Common.Integration;
using Stormpath.SDK.Tests.Common.RandomData;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Sync
{
    [Collection(nameof(IntegrationTestCollection))]
    public class Account_tests
    {
        private readonly TestFixture fixture;

        public Account_tests(TestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_tenant_accounts(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var accounts = tenant.GetAccounts().Synchronously().ToList();

            accounts.Any().ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_accounts(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_account_provider_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Updating_account(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Saving_with_response_options(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var chewie = application
                .GetAccounts()
                .Synchronously()
                .Where(a => a.Email == "chewie@kashyyyk.rim")
                .Single();

            chewie.SetUsername($"rwaaargh-{this.fixture.TestRunIdentifier}");
            chewie.Save(response => response.Expand(x => x.GetCustomData()));
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_account_applications(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            var luke = client.GetAccount(this.fixture.PrimaryAccountHref);

            var apps = luke.GetApplications().Synchronously().ToList();
            apps.Where(x => x.Href == this.fixture.PrimaryApplicationHref).Any().ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_account_directory(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_account_tenant(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Searching_accounts_by_email(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Searching_accounts_by_firstname(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Searching_accounts_by_lastname(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Searching_accounts_by_middle_name(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Searching_accounts_by_username(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Searching_accounts_by_status(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Searching_accounts_by_creation_date(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var longTimeAgo = application
                .GetAccounts()
                .Synchronously()
                .Where(x => x.CreatedAt < DateTime.Now.Subtract(TimeSpan.FromHours(1)))
                .ToList();
            longTimeAgo.ShouldBeEmpty();

            var createdRecently = application
                .GetAccounts()
                .Synchronously()
                .Where(x => x.CreatedAt >= DateTime.Now.Subtract(TimeSpan.FromHours(1)))
                .ToList();
            createdRecently.ShouldNotBeEmpty();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Searching_accounts_by_creation_date_within_shorthand(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Searching_accounts_using_filter(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Sorting_accounts_by_lastname(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Sorting_accounts_by_username_and_lastname(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Taking_only_two_accounts(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var firstTwo = application
                .GetAccounts()
                .Synchronously()
                .Take(2)
                .ToList();

            firstTwo.Count.ShouldBe(2);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Counting_accounts(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var count = application.GetAccounts().Synchronously().Count();
            count.ShouldBeGreaterThanOrEqualTo(8);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Any_returns_false_for_empty_filtered_set(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var anyDroids = application
                .GetAccounts()
                .Synchronously()
                .Where(x => x.Email.EndsWith("droids.co"))
                .Any();

            anyDroids.ShouldBeFalse();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Any_returns_true_for_nonempty_filtered_set(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var anyWookiees = application
                .GetAccounts()
                .Synchronously()
                .Where(x => x.Email.EndsWith("kashyyyk.rim"))
                .Any();

            anyWookiees.ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Creating_and_deleting_account(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
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
            deleted.ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Creating_account_with_custom_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
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
            account.Delete().ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Creating_account_with_response_options(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var account = client
                .Instantiate<IAccount>()
                .SetGivenName("Galen")
                .SetSurname("Marek")
                .SetEmail("gmarek@kashyyk.rim")
                .SetPassword(new RandomPassword(12));
            application.CreateAccount(account, opt =>
            {
                opt.ResponseOptions.Expand(x => x.GetCustomData());
            });

            account.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedAccountHrefs.Add(account.Href);

            // Clean up
            account.Delete().ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Authenticating_account(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var username = $"sonofthesuns-{this.fixture.TestRunIdentifier}";
            var result = application.AuthenticateAccount(username, "whataPieceofjunk$1138");
            result.ShouldBeAssignableTo<IAuthenticationResult>();
            result.Success.ShouldBeTrue();

            var account = result.GetAccount();
            account.FullName.ShouldBe("Luke Skywalker");
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Authenticating_account_with_response_options(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var request = new UsernamePasswordRequestBuilder();
            request.SetUsernameOrEmail($"sonofthesuns-{this.fixture.TestRunIdentifier}");
            request.SetPassword("whataPieceofjunk$1138");

            var result = application.AuthenticateAccount(request.Build(), response => response.Expand(x => x.GetAccount()));

            result.ShouldBeAssignableTo<IAuthenticationResult>();
            result.Success.ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Authenticating_account_in_specified_account_store(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);
            var accountStore = application.GetDefaultAccountStore();

            var result = application.AuthenticateAccount(
                request => request.SetUsernameOrEmail($"sonofthesuns-{this.fixture.TestRunIdentifier}").SetPassword("whataPieceofjunk$1138").SetAccountStore(accountStore));
            result.ShouldBeAssignableTo<IAuthenticationResult>();
            result.Success.ShouldBeTrue();

            var account = result.GetAccount();
            account.FullName.ShouldBe("Luke Skywalker");
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Authenticating_account_in_specified_account_store_by_href(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);
            var accountStore = application.GetDefaultAccountStore();

            var result = application.AuthenticateAccount(
                request =>
                {
                    request.SetUsernameOrEmail($"sonofthesuns-{this.fixture.TestRunIdentifier}");
                    request.SetPassword("whataPieceofjunk$1138");
                    request.SetAccountStore(this.fixture.PrimaryOrganizationHref);
                });
            result.ShouldBeAssignableTo<IAuthenticationResult>();
            result.Success.ShouldBeTrue();

            var account = result.GetAccount();
            account.FullName.ShouldBe("Luke Skywalker");
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Authenticating_account_in_specified_organization_by_nameKey(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);
            var accountStore = application.GetDefaultAccountStore();

            var result = application.AuthenticateAccount(
                request =>
                {
                    request.SetUsernameOrEmail($"sonofthesuns-{this.fixture.TestRunIdentifier}");
                    request.SetPassword("whataPieceofjunk$1138");
                    request.SetAccountStore(this.fixture.PrimaryOrganizationNameKey);
                });
            result.ShouldBeAssignableTo<IAuthenticationResult>();
            result.Success.ShouldBeTrue();

            var account = result.GetAccount();
            account.FullName.ShouldBe("Luke Skywalker");
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Authenticating_account_in_specified_account_store_with_response_options(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);
            var accountStore = application.GetDefaultAccountStore();

            var result = application.AuthenticateAccount(
                request =>
                {
                    request.SetUsernameOrEmail($"sonofthesuns-{this.fixture.TestRunIdentifier}");
                    request.SetPassword("whataPieceofjunk$1138");
                    request.SetAccountStore(accountStore);
                },
            response => response.Expand(x => x.GetAccount()));

            result.ShouldBeAssignableTo<IAuthenticationResult>();
            result.Success.ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void TryAuthenticating_account(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
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
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Authenticating_throws_for_invalid_credentials(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
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

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Resetting_password_updates_modified_date(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var account = application.GetAccounts()
                .Where(a => a.Email == "chewie@kashyyyk.rim")
                .Synchronously()
                .Single();

            var oldModificationDate = account.PasswordModifiedAt.Value;

            System.Threading.Thread.Sleep(3000); // Wait for a bit because sometimes clocks are slow

            account.SetPassword(new RandomPassword(16));
            account.Save();

            account.PasswordModifiedAt.Value.ShouldBeGreaterThan(oldModificationDate);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Searching_accounts_with_modified_passwords(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var chewie = application.GetAccounts()
                .Where(a => a.Email == "chewie@kashyyyk.rim")
                .Synchronously()
                .Single();
            chewie.SetPassword(new RandomPassword(16));
            chewie.Save();

            // Get all accounts that modified their passwords this year
            var accounts = application.GetAccounts()
                .Where(a => a.PasswordModifiedAt > new DateTimeOffset(DateTimeOffset.Now.Year, 1, 1, 0, 0, 0, TimeSpan.Zero))
                .Synchronously()
                .ToList();

            accounts.ShouldContain(a => a.Email == "chewie@kashyyyk.rim");
        }

        /// <summary>
        /// Regression test for https://github.com/stormpath/stormpath-sdk-dotnet/issues/175
        /// </summary>
        /// <param name="clientBuilder">The client to use.</param>
        /// <returns>A Task that represents the hronous test.</returns>
        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Creating_account_with_special_chars(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var trickyEmail = "admiral.ackbar+itsatrap@dac.rim";
            var password = new RandomPassword(12);
            var account = application.CreateAccount("Gial", "Ackbar", trickyEmail, password);

            account.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedAccountHrefs.Add(account.Href);

            var searchResult = application.GetAccounts().Synchronously().Where(x => x.Username == trickyEmail).Single();
            searchResult.Username.ShouldBe(trickyEmail);
            searchResult.Email.ShouldBe("admiral.ackbar+itsatrap@dac.rim");
            searchResult.Status.ShouldBe(AccountStatus.Enabled);

            var loginResult = application.AuthenticateAccount(trickyEmail, password);
            loginResult.Success.ShouldBeTrue();

            var deleted = account.Delete();
            deleted.ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Creating_account_with_UTF8_properties(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var account = application.CreateAccount("四", "李", "utf8@test.foo", "Supersecret!123");
            this.fixture.CreatedAccountHrefs.Add(account.Href);

            var searched = application.GetAccounts().Where(x => x.Surname == "李").Synchronously().Single();
            searched.GivenName.ShouldBe("四");
            searched.Surname.ShouldBe("李");

            account.Delete().ShouldBeTrue();
            this.fixture.CreatedAccountHrefs.Remove(account.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Create_sms_factor_for_account(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Jack")
                .SetSurname("Bauer");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            var factor = tester.Factors.Add(new SmsFactorCreationOptions
            {
                Number = "+1 818-555-2593" // Jack is back
            });

            (factor.GetAccount()).Href.Should().Be(tester.Href);

            factor.Href.Should().NotBeNullOrEmpty();
            factor.Type.Should().Be(FactorType.Sms);
            factor.Status.Should().Be(FactorStatus.Enabled);
            factor.VerificationStatus.Should().Be(FactorVerificationStatus.Unverified);

            // No challenges yet
            (factor.GetMostRecentChallenge()).Should().BeNull();
            (factor.Challenges.Synchronously().Any()).Should().BeFalse();

            // Get the phone associated with this factor
            var phone = factor.GetPhone();
            phone.Number.Should().Be("+18185552593");

            (factor.Delete()).Should().BeTrue();
            (tester.Delete()).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Create_totp_factor_for_account(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Jack")
                .SetSurname("Bauer");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            var factor = tester.Factors.Add(new GoogleAuthenticatorFactorCreationOptions
            {
                Issuer = "MyApp"
            });

            (factor.GetAccount()).Href.Should().Be(tester.Href);

            factor.Href.Should().NotBeNullOrEmpty();
            factor.Type.Should().Be(FactorType.GoogleAuthenticator);
            factor.Status.Should().Be(FactorStatus.Enabled);
            factor.VerificationStatus.Should().Be(FactorVerificationStatus.Unverified);
            factor.AccountName.Should().NotBeNullOrEmpty();
            factor.Base64QrImage.Should().NotBeNullOrEmpty();
            factor.Issuer.Should().Be("MyApp");
            factor.KeyUri.Should().NotBeNullOrEmpty();
            factor.Secret.Should().NotBeNullOrEmpty();

            // No challenges yet
            (factor.GetMostRecentChallenge()).Should().BeNull();
            (factor.Challenges.Synchronously().Any()).ShouldBeFalse();

            (factor.Delete()).Should().BeTrue();
            (tester.Delete()).Should().BeTrue();
        }

        /// <summary>
        /// Tests that polymorphism is handled correctly when retrieving the Factor resource.
        /// </summary>
        /// <remarks>
        /// An SMS factor should be retrieved as an ISmsFactor, which implements IFactor.
        /// </remarks>
        /// <param name="clientBuilder">The client to use.</param>
        /// <returns>A Task that represents the hronous test.</returns>
        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Retrieving_sms_factor_should_create_sms_class(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Jack")
                .SetSurname("Bauer");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            var factor = tester.Factors.Add(new SmsFactorCreationOptions
            {
                Number = "+1 818-555-2593"
            });

            factor.Should().BeAssignableTo<ISmsFactor>();
            factor.Should().BeAssignableTo<IFactor>();

            var retrievedGenericFactor = client.GetResource<IFactor>(factor.Href);
            retrievedGenericFactor.Should().BeAssignableTo<ISmsFactor>();
            retrievedGenericFactor.Should().BeAssignableTo<IFactor>();
            retrievedGenericFactor.Href.Should().Be(factor.Href);

            var retrievedSpecificFactor = client.GetResource<ISmsFactor>(factor.Href);
            retrievedSpecificFactor.Should().BeAssignableTo<ISmsFactor>();
            retrievedSpecificFactor.Should().BeAssignableTo<IFactor>();
            retrievedSpecificFactor.Href.Should().Be(factor.Href);

            (tester.Delete()).Should().BeTrue();
        }

        /// <summary>
        /// Tests that polymorphism is handled correctly when retrieving a list of Factors.
        /// </summary>
        /// <param name="clientBuilder">The client to use.</param>
        /// <returns>A Task that represents the hronous test.</returns>
        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Retrieving_list_of_factors_should_be_polymorphic(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Jack")
                .SetSurname("Bauer");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            tester.Factors.Add(new SmsFactorCreationOptions
            {
                Number = "+1 818-555-2593"
            });
            tester.Factors.Add(new GoogleAuthenticatorFactorCreationOptions
            {
                Issuer = "MyApp"
            });

            var factors = tester.Factors.Synchronously().ToArray();

            var factor1 = factors.First() as ISmsFactor;
            factor1.Should().NotBeNull();
            (factor1.GetPhone()).Number.Should().Be("+18185552593");

            var factor2 = factors.Last() as IGoogleAuthenticatorFactor;
            factor2.Should().NotBeNull();
            factor2.Issuer.Should().Be("MyApp");

            (tester.Delete()).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Create_sms_factor_for_account_and_challenge_immediately(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Jack")
                .SetSurname("Bauer");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            var factor = tester.Factors.Add(new SmsFactorCreationOptions
            {
                Number = "+1 818-555-2593",
                Challenge = true
            });

            var challenge = factor.GetMostRecentChallenge();
            challenge.Href.Should().NotBeNullOrEmpty();
            challenge.Message.Should().NotBeNullOrEmpty();
            challenge.Status.Should().BeOfType<ChallengeStatus>();

            (challenge.GetAccount()).Href.Should().Be(tester.Href);
            (challenge.GetFactor()).Href.Should().Be(factor.Href);

            // Should get the same object when accessing the Challenges collection
            var challenge2 = factor.Challenges.Synchronously().Single();
            challenge2.Href.Should().Be(challenge.Href);

            (tester.Delete()).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Challenge_existing_factor(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Jack")
                .SetSurname("Bauer");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            var factor = tester.Factors.Add(new SmsFactorCreationOptions
            {
                Number = "+1 818-555-2593"
            });

            var challenge = factor.Challenges.Add();

            var retrievedChallenge = factor.Challenges.Synchronously().Single();
            retrievedChallenge.Href.Should().Be(retrievedChallenge.Href);

            challenge.Status.Should().BeOfType<ChallengeStatus>();

            (challenge.GetAccount()).Href.Should().Be(tester.Href);
            (challenge.GetFactor()).Href.Should().Be(factor.Href);

            (tester.Delete()).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Challenge_existing_factor_with_options(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Jack")
                .SetSurname("Bauer");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            var factor = tester.Factors.Add(new SmsFactorCreationOptions
            {
                Number = "+1 818-555-2593"
            });

            var challenge = factor.Challenges.Add(new ChallengeCreationOptions
            {
                Message = "Dammit Chloe! The code is ${code}"
            });

            var retrievedChallenge = factor.Challenges.Synchronously().Single();
            retrievedChallenge.Href.Should().Be(retrievedChallenge.Href);

            challenge.Message.Should().StartWith("Dammit Chloe!");
            challenge.Status.Should().BeOfType<ChallengeStatus>();

            (challenge.GetAccount()).Href.Should().Be(tester.Href);
            (challenge.GetFactor()).Href.Should().Be(factor.Href);

            (tester.Delete()).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Submit_challenge_with_bad_code(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Jack")
                .SetSurname("Bauer");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            var factor = tester.Factors.Add(new GoogleAuthenticatorFactorCreationOptions());

            var challenge = factor.Challenges.Add();
            challenge.Submit("123456");

            // That's definitely not the right code
            challenge.Status.Should().Be(ChallengeStatus.Failed);

            (tester.Delete()).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Validate_challenge_with_bad_code(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Jack")
                .SetSurname("Bauer");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            var factor = tester.Factors.Add(new GoogleAuthenticatorFactorCreationOptions());

            var challenge = factor.Challenges.Add();
            var result = challenge.Validate("123456");

            // That's definitely not the right code
            result.Should().BeFalse();

            (tester.Delete()).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Update_sms_factor_and_save(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Jack")
                .SetSurname("Bauer");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            var factor = tester.Factors.Add(new SmsFactorCreationOptions
            {
                Number = "+1 818-555-2593",
                Status = FactorStatus.Disabled
            });

            factor.Status.Should().Be(FactorStatus.Disabled);

            factor.Status = FactorStatus.Enabled;
            factor.Save();

            factor.Status.Should().Be(FactorStatus.Enabled);

            (tester.Delete()).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Update_totp_factor_and_save(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Jack")
                .SetSurname("Bauer");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            var factor = tester.Factors.Add(new GoogleAuthenticatorFactorCreationOptions
            {
                Issuer = "MyApp"
            });

            factor.Issuer.Should().Be("MyApp");
            factor.AccountName.Should().Be(tester.Username);

            factor.Issuer = "MyBetterApp";
            factor.AccountName = "luke@hoth.rim";
            factor.Save();

            factor.Issuer.Should().Be("MyBetterApp");
            factor.AccountName.Should().Be("luke@hoth.rim");

            (tester.Delete()).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Searching_factors_by_status(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Jack")
                .SetSurname("Bauer");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            var factor1 = tester.Factors.Add(new SmsFactorCreationOptions
            {
                Number = "+1 818-555-2593",
                Status = FactorStatus.Disabled
            });
            var factor2 = tester.Factors.Add(new GoogleAuthenticatorFactorCreationOptions
            {
                Issuer = "MyApp",
                Status = FactorStatus.Enabled
            });

            var disabledFactor = tester.Factors
                .Where(f => f.Status == FactorStatus.Disabled)
                .Synchronously()
                .Single();
            disabledFactor.Href.Should().Be(factor1.Href);

            var enabledFactor = tester.Factors
                .Where(f => f.Status == FactorStatus.Enabled)
                .Synchronously()
                .Single();
            enabledFactor.Href.Should().Be(factor2.Href);

            (tester.Delete()).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Searching_factors_by_type(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Jack")
                .SetSurname("Bauer");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            var factor1 = tester.Factors.Add(new SmsFactorCreationOptions
            {
                Number = "+1 818-555-2593",
                Status = FactorStatus.Disabled
            });
            var factor2 = tester.Factors.Add(new GoogleAuthenticatorFactorCreationOptions
            {
                Issuer = "MyApp",
                Status = FactorStatus.Enabled
            });

            var disabledFactor = tester.Factors
                .Where(f => f.Type == FactorType.Sms)
                .Synchronously()
                .Single();
            disabledFactor.Href.Should().Be(factor1.Href);

            var enabledFactor = tester.Factors
                .Where(f => f.Type == FactorType.GoogleAuthenticator)
                .Synchronously()
                .Single();
            enabledFactor.Href.Should().Be(factor2.Href);

            (tester.Delete()).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Searching_factors_by_totp_account_name(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Jack")
                .SetSurname("Bauer");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            tester.Factors.Add(new SmsFactorCreationOptions
            {
                Number = "+1 818-555-2593",
                Status = FactorStatus.Disabled
            });
            var totpFactor = tester.Factors.Add(new GoogleAuthenticatorFactorCreationOptions
            {
                Issuer = "MyApp",
                Status = FactorStatus.Enabled
            });

            var foundFactor = tester.Factors
                .Where(f => ((IGoogleAuthenticatorFactor)f).AccountName == tester.Username)
                .Synchronously()
                .Single();

            foundFactor.Href.Should().Be(totpFactor.Href);

            (tester.Delete()).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_factors_with_expansion(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Jack")
                .SetSurname("Bauer");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            var smsFactor = tester.Factors.Add(new SmsFactorCreationOptions
            {
                Number = "+1 818-555-2593",
                Status = FactorStatus.Disabled
            });
            var totpFactor = tester.Factors.Add(new GoogleAuthenticatorFactorCreationOptions
            {
                Issuer = "MyApp",
                Status = FactorStatus.Enabled
            });

            // We can't verify that the HTTP call was correct, but we can verify that it didn't fail
            var allFactors = tester.Factors
                .Expand(e => e.Account)
                .Expand(e => e.GetChallenges())
                .Synchronously()
                .ToArray();
            allFactors.Length.Should().Be(2);

            // Also try getting a factor directly
            var directFactor =
                client.GetResource<ISmsFactor>(smsFactor.Href, opt =>
                {
                    opt.Expand(e => e.Phone);
                    opt.Expand(e => e.Account);
                    opt.Expand(e => e.MostRecentChallenge);
                });

            directFactor.Href.Should().Be(smsFactor.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Adding_phone_to_account(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Tony")
                .SetSurname("Almeida");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            // Add a phone
            var phone = tester.Phones.Add("+1 818-555-7993");
            phone.Number.Should().Be("+18185557993");

            (phone.GetAccount()).Href.Should().Be(tester.Href);

            // Try deleting it
            phone.Delete();

            (tester.Delete()).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Adding_phone_to_account_with_all_options(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Tony")
                .SetSurname("Almeida");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            // Add a phone
            var phone = tester.Phones.Add(new PhoneCreationOptions
            {
                Number = "+1 818-555-7993",
                Name = "Nina's cell",
                Description = "Danger danger",
                Status = PhoneStatus.Disabled,
                VerificationStatus = PhoneVerificationStatus.Verified
            });

            phone.Number.Should().Be("+18185557993");
            phone.Name.Should().Be("Nina's cell");
            phone.Description.Should().Be("Danger danger");
            phone.Status.Should().Be(PhoneStatus.Disabled);
            phone.VerificationStatus.Should().Be(PhoneVerificationStatus.Verified);

            (tester.Delete()).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Updating_existing_phone(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Tony")
                .SetSurname("Almeida");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            // Add a phone
            var phone = tester.Phones.Add("+1 818-555-7993");
            phone.VerificationStatus.Should().Be(PhoneVerificationStatus.Unverified);

            phone.VerificationStatus = PhoneVerificationStatus.Verified;
            var updated = phone.Save();

            updated.VerificationStatus.Should().Be(PhoneVerificationStatus.Verified);

            (tester.Delete()).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Listing_account_phones(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Tony")
                .SetSurname("Almeida");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            // Add some phones
            tester.Phones.Add("+1 818-555-7993");
            tester.Phones.Add("+1 818-555-2593");

            // Grab them via LINQ
            var phones = tester.Phones.Synchronously().ToArray();
            phones.First().Number.Should().Be("+18185557993");
            phones.Last().Number.Should().Be("+18185552593");

            (tester.Delete()).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Searching_account_phones(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Tony")
                .SetSurname("Almeida");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            // Add some phones
            tester.Phones.Add("+1 818-555-7993");
            tester.Phones.Add(new PhoneCreationOptions
            {
                Number = "+1 818-555-2593",
                Name = "Cell"
            });

            var findByNumber = tester.Phones
                .Where(p => p.Number.Contains("5557"))
                .Synchronously()
                .Single();
            findByNumber.Number.Should().Be("+18185557993");

            var findByName = tester.Phones
                .Where(p => p.Name == "Cell")
                .Synchronously()
                .Single();
            findByName.Number.Should().Be("+18185552593");

            (tester.Delete()).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_phones_with_expansion(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();

            // Create an account
            var tester = client.Instantiate<IAccount>()
                .SetEmail(new RandomEmail("testing.foo"))
                .SetPassword(new RandomPassword(12))
                .SetGivenName("Jack")
                .SetSurname("Bauer");
            var directory = client.GetDirectory(fixture.PrimaryDirectoryHref);
            directory.CreateAccount(tester);

            // Add some phones
            tester.Phones.Add("+1 818-555-7993");
            tester.Phones.Add("+1 818-555-2593");

            // We can't verify that the HTTP call was correct, but we can verify that it didn't fail
            var allPhones = tester.Phones
                .Expand(e => e.Account)
                .Synchronously()
                .ToArray();
            allPhones.Length.Should().Be(2);

            // Also try getting a phone directly
            var directPhone = client.GetResource<IPhone>(allPhones[0].Href, opt =>
            {
                opt.Expand(e => e.Account);
            });

            directPhone.Href.Should().Be(allPhones[0].Href);
        }
    }
}
