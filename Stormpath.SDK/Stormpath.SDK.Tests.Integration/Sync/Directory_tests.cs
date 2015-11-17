// <copyright file="Directory_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Directory;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Sync;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Sync
{
    [Collection(nameof(IntegrationTestCollection))]
    public class Directory_tests
    {
        private readonly IntegrationTestFixture fixture;

        public Directory_tests(IntegrationTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Getting_tenant_directories(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();
            var directories = tenant.GetDirectories().Synchronously().ToList();

            directories.Count.ShouldNotBe(0);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Getting_directory_tenant(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var directory = client.GetResource<IDirectory>(this.fixture.PrimaryDirectoryHref);

            // Verify data from IntegrationTestData
            var tenantHref = directory.GetTenant().Href;
            tenantHref.ShouldBe(this.fixture.TenantHref);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Creating_disabled_directory(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var directoryName = $"My New Disabled Directory (.NET IT {this.fixture.TestRunIdentifier})";
            var created = tenant.CreateDirectory(directoryName, "A great directory for my app", DirectoryStatus.Disabled);
            created.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(created.Href);

            created.Name.ShouldBe(directoryName);
            created.Description.ShouldBe("A great directory for my app");
            created.Status.ShouldBe(DirectoryStatus.Disabled);

            // Cleanup
            created.Delete().ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(created.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Creating_directory_with_response_options(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var directory = client
                .Instantiate<IDirectory>()
                .SetName($"My New Directory With Options (.NET IT {this.fixture.TestRunIdentifier}) - Sync")
                .SetDescription("Another great directory for my app")
                .SetStatus(DirectoryStatus.Disabled);

            tenant.CreateDirectory(directory, opt => opt.ResponseOptions.Expand(x => x.GetCustomData));

            directory.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(directory.Href);

            // Cleanup
            directory.Delete().ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(directory.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Modifying_directory(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var directoryName = $"My New Directory (.NET IT {this.fixture.TestRunIdentifier} - {clientBuilder.Name})";
            var newDirectory = client.Instantiate<IDirectory>();
            newDirectory.SetName(directoryName);
            newDirectory.SetDescription("Put some accounts here!");
            newDirectory.SetStatus(DirectoryStatus.Enabled);

            var created = tenant.CreateDirectory(newDirectory);
            created.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(created.Href);

            created.SetDescription("foobar");
            created.CustomData.Put("good", true);
            var updated = created.Save();

            updated.Name.ShouldBe(directoryName);
            updated.Description.ShouldBe("foobar");
            var customData = updated.GetCustomData();
            customData["good"].ShouldBe(true);

            // Cleanup
            updated.Delete().ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(updated.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Saving_with_response_options(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var directoryName = $"My New Directory #2 (.NET IT {this.fixture.TestRunIdentifier} - {clientBuilder.Name})";
            var newDirectory = client.Instantiate<IDirectory>();
            newDirectory.SetName(directoryName);
            newDirectory.SetDescription("Put some accounts here!");
            newDirectory.SetStatus(DirectoryStatus.Enabled);

            var created = tenant.CreateDirectory(newDirectory);
            created.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(created.Href);

            created.SetDescription("foobar");
            created.CustomData.Put("good", true);
            created.Save(response => response.Expand(x => x.GetCustomDataAsync));

            // Cleanup
            created.Delete().ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(created.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Creating_facebook_directory(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var directoryName = $"My New Facebook Directory (.NET IT {this.fixture.TestRunIdentifier} - {clientBuilder.Name} Sync)";

            var instance = client.Instantiate<IDirectory>();
            instance.SetName(directoryName);
            var created = tenant.CreateDirectory(instance, options => options.ForProvider(
                client.Providers().Facebook()
                    .Builder()
                    .SetClientId("foobar")
                    .SetClientSecret("secret123!")
                    .Build()));

            created.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(created.Href);

            created.Name.ShouldBe(directoryName);

            var provider = created.GetProvider() as IFacebookProvider;
            provider.ShouldNotBeNull();
            provider.Href.ShouldNotBeNullOrEmpty();

            provider.ProviderId.ShouldBe("facebook");
            provider.ClientId.ShouldBe("foobar");
            provider.ClientSecret.ShouldBe("secret123!");

            // Cleanup
            created.Delete().ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(created.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Creating_github_directory(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var directoryName = $"My New Github Directory (.NET IT {this.fixture.TestRunIdentifier} - {clientBuilder.Name} Sync)";

            var instance = client.Instantiate<IDirectory>();
            instance.SetName(directoryName);
            var created = tenant.CreateDirectory(instance, options => options.ForProvider(
                client.Providers().Github()
                    .Builder()
                    .SetClientId("foobar")
                    .SetClientSecret("secret123!")
                    .Build()));

            created.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(created.Href);

            created.Name.ShouldBe(directoryName);

            var provider = created.GetProvider() as IGithubProvider;
            provider.ShouldNotBeNull();
            provider.Href.ShouldNotBeNullOrEmpty();

            provider.ProviderId.ShouldBe("github");
            provider.ClientId.ShouldBe("foobar");
            provider.ClientSecret.ShouldBe("secret123!");

            // Cleanup
            created.Delete().ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(created.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Creating_google_directory(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var directoryName = $"My New Google Directory (.NET IT {this.fixture.TestRunIdentifier} - {clientBuilder.Name} Sync)";

            var instance = client.Instantiate<IDirectory>();
            instance.SetName(directoryName);
            var created = tenant.CreateDirectory(instance, options => options.ForProvider(
                client.Providers().Google()
                    .Builder()
                    .SetClientId("foobar")
                    .SetClientSecret("secret123!")
                    .SetRedirectUri("foo://bar")
                    .Build()));

            created.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(created.Href);

            created.Name.ShouldBe(directoryName);

            var provider = created.GetProvider() as IGoogleProvider;
            provider.ShouldNotBeNull();
            provider.Href.ShouldNotBeNullOrEmpty();

            provider.ProviderId.ShouldBe("google");
            provider.ClientId.ShouldBe("foobar");
            provider.ClientSecret.ShouldBe("secret123!");
            provider.RedirectUri.ShouldBe("foo://bar");

            // Cleanup
            created.Delete().ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(created.Href);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Creating_linkedin_directory(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var tenant = client.GetCurrentTenant();

            var directoryName = $"My New LinkedIn Directory (.NET IT {this.fixture.TestRunIdentifier} - {clientBuilder.Name} Sync)";

            var instance = client.Instantiate<IDirectory>();
            instance.SetName(directoryName);
            var created = tenant.CreateDirectory(instance, options => options.ForProvider(
                client.Providers().LinkedIn()
                    .Builder()
                    .SetClientId("foobar")
                    .SetClientSecret("secret123!")
                    .Build()));

            created.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(created.Href);

            created.Name.ShouldBe(directoryName);

            var provider = created.GetProvider() as ILinkedInProvider;
            provider.ShouldNotBeNull();
            provider.Href.ShouldNotBeNullOrEmpty();

            provider.ProviderId.ShouldBe("linkedin");
            provider.ClientId.ShouldBe("foobar");
            provider.ClientSecret.ShouldBe("secret123!");

            // Cleanup
            created.Delete().ShouldBeTrue();
            this.fixture.CreatedDirectoryHrefs.Remove(created.Href);
        }
    }
}