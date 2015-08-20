// <copyright file="InternalDataStore_tests.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Tenant;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Tenant;
using Stormpath.SDK.Tests.Fakes;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class InternalDataStore_tests
    {
        private readonly IRequestExecutor fakeRequestExecutor;
        private readonly IDataStore dataStore;

        public InternalDataStore_tests()
        {
            fakeRequestExecutor = Substitute.For<IRequestExecutor>();
            dataStore = new InternalDataStore(fakeRequestExecutor, "http://foobar");
        }

        [Fact]
        public async Task Instantiating_Account_from_JSON()
        {
            var href = "http://foobar/account";
            fakeRequestExecutor.GetAsync(new Uri(href), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(FakeJson.Account));

            var account = await dataStore.GetResourceAsync<IAccount>(href);

            // Verify against data from FakeJson.Account
            account.CreatedAt.ShouldBe(Iso8601.Parse("2015-07-21T23:50:49.078Z"));
            account.Email.ShouldBe("han.solo@corellia.core");
            account.FullName.ShouldBe("Han Solo");
            account.GivenName.ShouldBe("Han");
            account.Href.ShouldBe("https://api.stormpath.com/v1/accounts/foobarAccount");
            account.MiddleName.ShouldBe(null);
            account.ModifiedAt.ShouldBe(Iso8601.Parse("2015-07-21T23:50:49.078Z"));
            account.Status.ShouldBe(AccountStatus.Enabled);
            account.Surname.ShouldBe("Solo");
            account.Username.ShouldBe("han.solo@corellia.core");

            (account as DefaultAccount).AccessTokens.Href.ShouldBe("https://api.stormpath.com/v1/accounts/foobarAccount/accessTokens");
            (account as DefaultAccount).ApiKeys.Href.ShouldBe("https://api.stormpath.com/v1/accounts/foobarAccount/apiKeys");
            (account as DefaultAccount).Applications.Href.ShouldBe("https://api.stormpath.com/v1/accounts/foobarAccount/applications");
            (account as DefaultAccount).CustomData.Href.ShouldBe("https://api.stormpath.com/v1/accounts/foobarAccount/customData");
            (account as DefaultAccount).Directory.Href.ShouldBe("https://api.stormpath.com/v1/directories/foobarDirectory");
            (account as DefaultAccount).EmailVerificationToken.Href.ShouldBe(null);
            (account as DefaultAccount).GroupMemberships.Href.ShouldBe("https://api.stormpath.com/v1/accounts/foobarAccount/groupMemberships");
            (account as DefaultAccount).Groups.Href.ShouldBe("https://api.stormpath.com/v1/accounts/foobarAccount/groups");
            (account as DefaultAccount).ProviderData.Href.ShouldBe("https://api.stormpath.com/v1/accounts/foobarAccount/providerData");
            (account as DefaultAccount).RefreshTokens.Href.ShouldBe("https://api.stormpath.com/v1/accounts/foobarAccount/refreshTokens");
            (account as DefaultAccount).Tenant.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foobarTenant");
        }

        [Fact]
        public async Task Instantiating_Tenant_from_JSON()
        {
            var href = "http://foobar/tenant";
            fakeRequestExecutor.GetAsync(new Uri(href))
                .Returns(Task.FromResult(FakeJson.Tenant));

            var tenant = await dataStore.GetResourceAsync<ITenant>(href);

            // Verify against data from FakeJson.Tenant
            tenant.CreatedAt.ShouldBe(Iso8601.Parse("2015-07-21T23:50:49.058Z"));
            tenant.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foo-bar");
            tenant.Key.ShouldBe("foo-bar");
            tenant.ModifiedAt.ShouldBe(Iso8601.Parse("2015-07-21T23:50:49.579Z"));
            tenant.Name.ShouldBe("foo-bar");

            (tenant as DefaultTenant).Accounts.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foo-bar/accounts");
            (tenant as DefaultTenant).Agents.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foo-bar/agents");
            (tenant as DefaultTenant).Applications.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foo-bar/applications");
            (tenant as DefaultTenant).CustomData.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foo-bar/customData");
            (tenant as DefaultTenant).Directories.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foo-bar/directories");
            (tenant as DefaultTenant).Groups.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foo-bar/groups");
            (tenant as DefaultTenant).IdSites.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foo-bar/idSites");
            (tenant as DefaultTenant).Organizations.Href.ShouldBe("https://api.stormpath.com/v1/tenants/foo-bar/organizations");
        }
    }
}
