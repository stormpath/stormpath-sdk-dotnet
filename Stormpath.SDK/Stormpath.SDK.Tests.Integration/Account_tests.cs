// <copyright file="Account_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Auth;
using Stormpath.SDK.Error;
using Stormpath.SDK.Tests.Integration.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Integration
{
    [Collection("Live tenant tests")]
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

            accounts.Any().ShouldBe(true);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Getting_application_accounts(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.ApplicationHref);

            var accounts = await application.GetAccounts().ToListAsync();

            // Verify data from IntegrationTestData
            accounts.Count.ShouldBe(7);

            var luke = accounts.Where(x => x.GivenName == "Luke").Single();
            luke.FullName.ShouldBe("Luke Skywalker");
            luke.Email.ShouldBe("lskywalker@tattooine.rim");
            luke.Username.ShouldStartWith("sonofthesuns");

            var vader = accounts.Where(x => x.Surname == "Vader").Single();
            vader.FullName.ShouldBe("Darth Vader");
            vader.Email.ShouldStartWith("vader@empire.co");
            vader.Username.ShouldStartWith("lordvader");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Searching_application_accounts_by_email(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.ApplicationHref);

            var coreCitizens = await application
                .GetAccounts()
                .Where(acct => acct.Email.EndsWith(".core"))
                .ToListAsync();

            // Verify data from IntegrationTestData
            coreCitizens.Count.ShouldBe(2);

            var han = coreCitizens.Where(x => x.GivenName == "Han").Single();
            han.FullName.ShouldBe("Han Solo");
            han.Email.ShouldBe("han.solo@corellia.core");
            han.Username.ShouldStartWith("cptsolo");

            var leia = coreCitizens.Where(x => x.GivenName == "Leia").Single();
            leia.FullName.ShouldBe("Leia Organa");
            leia.Email.ShouldStartWith("leia.organa@alderaan.core");
            leia.Username.ShouldStartWith("princessleia");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Creating_and_deleting_account(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.ApplicationHref);

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
            deleted.ShouldBe(true);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Authenticating_good_account(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.ApplicationHref);

            var username = $"sonofthesuns-{this.fixture.TestRunIdentifier}";
            var result = await application.AuthenticateAccountAsync(username, "whataPieceofjunk$1138");
            result.ShouldBeAssignableTo<IAuthenticationResult>();

            var account = await result.GetAccountAsync();
            account.FullName.ShouldBe("Luke Skywalker");
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task TryAuthenticating_accounts(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.ApplicationHref);

            var username = $"sonofthesuns-{this.fixture.TestRunIdentifier}";

            (await application.TryAuthenticateAccountAsync(username, "whataPieceofjunk$1138"))
                .ShouldBe(true);

            (await application.TryAuthenticateAccountAsync(username, "notLukesPassword?"))
                .ShouldBe(false);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Authenticating_throws_for_invalid_credentials(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.ApplicationHref);

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
