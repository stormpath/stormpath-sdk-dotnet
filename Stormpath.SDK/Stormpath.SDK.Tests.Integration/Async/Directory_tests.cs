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

using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Tests.Integration.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Async
{
    [Collection("Live tenant tests")]
    public class Directory_tests
    {
        private readonly IntegrationTestFixture fixture;

        public Directory_tests(IntegrationTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Getting_tenant_directories(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();
            var directories = await tenant.GetDirectories().ToListAsync();

            directories.Count.ShouldNotBe(0);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Getting_directory_tenant(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var directory = await client.GetResourceAsync<IDirectory>(this.fixture.PrimaryDirectoryHref);

            // Verify data from IntegrationTestData
            var tenantHref = (await directory.GetTenantAsync()).Href;
            tenantHref.ShouldBe(this.fixture.TenantHref);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Creating_disabled_directory(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            var directoryName = $"My New Disabled Directory (.NET IT {this.fixture.TestRunIdentifier})";
            var created = await tenant.CreateDirectoryAsync(directoryName, "A great directory for my app", DirectoryStatus.Disabled);
            created.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(created.Href);

            created.Name.ShouldBe(directoryName);
            created.Description.ShouldBe("A great directory for my app");
            created.Status.ShouldBe(DirectoryStatus.Disabled);

            var provider = await created.GetProviderAsync();
            provider.ProviderId.ShouldBe("stormpath");

            // Cleanup
            await created.DeleteAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Modifying_directory(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            var directoryName = $"My New Directory (.NET IT {this.fixture.TestRunIdentifier})";
            var newDirectory = client.Instantiate<IDirectory>();
            newDirectory.SetName(directoryName);
            newDirectory.SetDescription("Put some accounts here!");
            newDirectory.SetStatus(DirectoryStatus.Enabled);

            var created = await tenant.CreateDirectoryAsync(newDirectory);
            created.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(created.Href);

            created.SetDescription("foobar");
            created.CustomData.Put("good", true);
            var updated = await created.SaveAsync();

            updated.Name.ShouldBe(directoryName);
            updated.Description.ShouldBe("foobar");
            var customData = await updated.GetCustomDataAsync();
            customData["good"].ShouldBe(true);

            // Cleanup
            await updated.DeleteAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Creating_facebook_directory(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            var directoryName = $"My New Facebook Directory (.NET IT {this.fixture.TestRunIdentifier})";

            var instance = client.Instantiate<IDirectory>();
            instance.SetName(directoryName);
            var created = await tenant.CreateDirectoryAsync(instance, options => options.ForProvider(
                client.Providers().Facebook()
                    .Builder()
                    .SetClientId("foobar")
                    .SetClientSecret("secret123!")
                    .Build()));

            created.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(created.Href);

            created.Name.ShouldBe(directoryName);

            var provider = await created.GetProviderAsync() as IFacebookProvider;
            provider.ShouldNotBeNull();
            provider.Href.ShouldNotBeNullOrEmpty();

            provider.ProviderId.ShouldBe("facebook");
            provider.ClientId.ShouldBe("foobar");
            provider.ClientSecret.ShouldBe("secret123!");

            // Cleanup
            await created.DeleteAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Creating_github_directory(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            var directoryName = $"My New Github Directory (.NET IT {this.fixture.TestRunIdentifier})";

            var instance = client.Instantiate<IDirectory>();
            instance.SetName(directoryName);
            var created = await tenant.CreateDirectoryAsync(instance, options => options.ForProvider(
                client.Providers().Github()
                    .Builder()
                    .SetClientId("foobar")
                    .SetClientSecret("secret123!")
                    .Build()));

            created.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(created.Href);

            created.Name.ShouldBe(directoryName);

            var provider = await created.GetProviderAsync() as IGithubProvider;
            provider.ShouldNotBeNull();
            provider.Href.ShouldNotBeNullOrEmpty();

            provider.ProviderId.ShouldBe("github");
            provider.ClientId.ShouldBe("foobar");
            provider.ClientSecret.ShouldBe("secret123!");

            // Cleanup
            await created.DeleteAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Creating_google_directory(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            var directoryName = $"My New Google Directory (.NET IT {this.fixture.TestRunIdentifier})";

            var instance = client.Instantiate<IDirectory>();
            instance.SetName(directoryName);
            var created = await tenant.CreateDirectoryAsync(instance, options => options.ForProvider(
                client.Providers().Google()
                    .Builder()
                    .SetClientId("foobar")
                    .SetClientSecret("secret123!")
                    .SetRedirectUri("foo://bar")
                    .Build()));

            created.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(created.Href);

            created.Name.ShouldBe(directoryName);

            var provider = await created.GetProviderAsync() as IGoogleProvider;
            provider.ShouldNotBeNull();
            provider.Href.ShouldNotBeNullOrEmpty();

            provider.ProviderId.ShouldBe("google");
            provider.ClientId.ShouldBe("foobar");
            provider.ClientSecret.ShouldBe("secret123!");
            provider.RedirectUri.ShouldBe("foo://bar");

            // Cleanup
            await created.DeleteAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Creating_linkedin_directory(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            var directoryName = $"My New LinkedIn Directory (.NET IT {this.fixture.TestRunIdentifier})";

            var instance = client.Instantiate<IDirectory>();
            instance.SetName(directoryName);
            var created = await tenant.CreateDirectoryAsync(instance, options => options.ForProvider(
                client.Providers().LinkedIn()
                    .Builder()
                    .SetClientId("foobar")
                    .SetClientSecret("secret123!")
                    .Build()));

            created.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(created.Href);

            created.Name.ShouldBe(directoryName);

            var provider = await created.GetProviderAsync() as ILinkedInProvider;
            provider.ShouldNotBeNull();
            provider.Href.ShouldNotBeNullOrEmpty();

            provider.ProviderId.ShouldBe("linkedin");
            provider.ClientId.ShouldBe("foobar");
            provider.ClientSecret.ShouldBe("secret123!");

            // Cleanup
            await created.DeleteAsync();
        }
    }
}