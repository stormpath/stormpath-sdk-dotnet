using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Stormpath.SDK.Directory;

namespace Stormpath.SDK.Tests.Common.Integration
{
    public class CreateSingleDirectoryFixture : IDisposable
    {
        public string DirectoryHref { get; private set; }

        public CreateSingleDirectoryFixture()
        {
            // Create a new directory for the accounts created in this test
            var client = TestClients.GetSAuthc1Client();
            var directory = client.CreateDirectoryAsync($".NET ITs {Guid.NewGuid().ToString()}", "Custom Data Search tests", DirectoryStatus.Enabled).Result;
            directory.Href.Should().NotBeNullOrEmpty();

            this.DirectoryHref = directory.Href;
        }

        public void Dispose()
        {
            // Clean up
            var client = TestClients.GetSAuthc1Client();
            var directory = client.GetDirectoryAsync(this.DirectoryHref).Result;
            directory.DeleteAsync().Result.Should().BeTrue();
        }
    }
}
