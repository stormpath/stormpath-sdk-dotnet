// <copyright file="Sync_Directory_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Directory;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Integration.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Sync
{
    [Collection("Live tenant tests")]
    public class Sync_Directory_tests
    {
        private readonly IntegrationTestFixture fixture;

        public Sync_Directory_tests(IntegrationTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Getting_tenant_directories(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = client.GetCurrentTenant();
            var directories = tenant.GetDirectories().Synchronously().ToList();

            directories.Count.ShouldNotBe(0);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Creating_disabled_directory(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = client.GetCurrentTenant();

            var directoryName = $"My New Disabled Directory (.NET IT {this.fixture.TestRunIdentifier})";
            var created = tenant.CreateDirectory(directoryName, "A great directory for my app", DirectoryStatus.Disabled);
            created.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedDirectoryHrefs.Add(created.Href);

            created.Name.ShouldBe(directoryName);
            created.Description.ShouldBe("A great directory for my app");
            created.Status.ShouldBe(DirectoryStatus.Disabled);

            // Cleanup
            created.Delete();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public void Modifying_directory(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = client.GetCurrentTenant();

            var directoryName = $"My New Directory (.NET IT {this.fixture.TestRunIdentifier})";
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
            updated.Delete();
        }
    }
}