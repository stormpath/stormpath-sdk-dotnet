// <copyright file="Sync_Entity_tests.cs" company="Stormpath, Inc.">
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

using System.Linq;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Integration.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Sync
{
    [Collection("Live tenant tests")]
    public class Sync_Entity_tests
    {
        private readonly IntegrationTestFixture fixture;

        public Sync_Entity_tests(IntegrationTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Multiple_instances_reference_same_data(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var account = application.GetAccounts().Synchronously().First();
            var anotherAccount = application.GetAccounts().Synchronously().First();

            var updatedEmail = account.Email + "-foobar";
            account.SetEmail(updatedEmail);
            anotherAccount.Email.ShouldBe(updatedEmail);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Reference_is_updated_after_saving(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var newAccount = client.Instantiate<IAccount>();
            newAccount.SetEmail("identity-maps-are-useful@test.foo");
            newAccount.SetPassword("Changeme123!");
            newAccount.SetGivenName("Testing");
            newAccount.SetSurname("IdentityMaps");

            var created = application.CreateAccount(newAccount, opt => opt.RegistrationWorkflowEnabled = false);
            this.fixture.CreatedAccountHrefs.Add(created.Href);

            created.SetMiddleName("these");
            var updated = created.Save();

            updated.SetEmail("different");
            created.Email.ShouldBe("different");

            updated.Delete();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Original_object_is_updated_after_creating(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var newAccount = client.Instantiate<IAccount>();
            newAccount.SetEmail("super-smart-objects@test.foo");
            newAccount.SetPassword("Changeme123!");
            newAccount.SetGivenName("Testing");
            newAccount.SetSurname("InitialProxy");

            var created = application.CreateAccount(newAccount, opt => opt.RegistrationWorkflowEnabled = false);
            this.fixture.CreatedAccountHrefs.Add(created.Href);

            created.SetMiddleName("these");
            newAccount.MiddleName.ShouldBe("these");

            created.Delete();
        }
    }
}