// <copyright file="Entity_tests.cs" company="Stormpath, Inc.">
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

using System.Linq;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Common.Integration;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Sync
{
    [Collection(nameof(IntegrationTestCollection))]
    public class Entity_tests
    {
        private readonly TestFixture fixture;

        public Entity_tests(TestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Multiple_instances_reference_same_data(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var account = application.GetAccounts().Synchronously().First();
            var anotherAccount = application.GetAccounts().Synchronously().First();

            var updatedEmail = account.Email + "-foobar";
            account.SetEmail(updatedEmail);
            anotherAccount.Email.ShouldBe(updatedEmail);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Reference_is_updated_after_saving(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var newAccount = client.Instantiate<IAccount>();
            newAccount.SetEmail("identity-maps-are-useful-sync@test.foo");
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
            this.fixture.CreatedAccountHrefs.Remove(updated.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Original_object_is_updated_after_creating(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
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
            this.fixture.CreatedAccountHrefs.Remove(created.Href);
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Not_capturing_save_result_works(TestClientProvider clientBuilder)
        {
            // This test is a little redundant, but explicitly tests a style
            // that will be common among consumers of the SDK.
            var client = clientBuilder.GetClient();
            var application = client.GetResource<IApplication>(this.fixture.PrimaryApplicationHref);

            var newAccount = client.Instantiate<IAccount>();
            newAccount.SetEmail("indistinguishable-from-magic@test.foo");
            newAccount.SetPassword("Changeme123!");
            newAccount.SetGivenName("Testing");
            newAccount.SetSurname("InitialProxy-NonCaptureWorkflow");

            newAccount.Href.ShouldBeNullOrEmpty();

            // Instead of capturing result = ...
            // Just execute the method and expect the original object to be updated
            application.CreateAccount(newAccount, opt => opt.RegistrationWorkflowEnabled = false);
            newAccount.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedAccountHrefs.Add(newAccount.Href);

            newAccount.Delete();
            this.fixture.CreatedAccountHrefs.Remove(newAccount.Href);
        }
    }
}
