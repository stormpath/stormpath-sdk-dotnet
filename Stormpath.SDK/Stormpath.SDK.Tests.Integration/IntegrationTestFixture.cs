// <copyright file="IntegrationTestFixture.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Application;
using Stormpath.SDK.Tests.Integration.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Integration
{
    public class IntegrationTestFixture : IDisposable
    {
        public IntegrationTestFixture()
        {
            Assert.False(true, "Todo");

            AddObjectsToTenantAsync()
                .GetAwaiter().GetResult();
        }

        public string TenantHref { get; private set; }

        public string ApplicationHref { get; private set; }

        public List<string> CreatedHrefs { get; private set; }

        public void Dispose()
        {
            RemoveObjectsFromTenantAsync()
                .GetAwaiter().GetResult();
        }

        private async Task AddObjectsToTenantAsync()
        {
            var client = IntegrationTestClients.GetSAuthc1Client();

            var tenant = await client.GetCurrentTenantAsync();
            TenantHref = tenant.Href;

            //// Create application
            //try
            //{
            //    //var application = await tenant.CreateApplicationAsync($".NET ITs {DateTimeOffset.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture)}");
            //    ApplicationHref = application.Href;
            //    if (string.IsNullOrEmpty(ApplicationHref))
            //        throw new ApplicationException("Returned href is empty");
            //}
            //catch (Exception e)
            //{
            //    throw new InvalidOperationException("Could not create application", e);
            //}
        }

        private async Task RemoveObjectsFromTenantAsync()
        {
            var client = IntegrationTestClients.GetSAuthc1Client();

            // Delete application
            try
            {
                var application = await client.GetResourceAsync<IApplication>(ApplicationHref);
                var deleteSuccessful = await application.DeleteAsync();
                if (!deleteSuccessful)
                    throw new ApplicationException("Delete result was false");
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Could not delete application at {ApplicationHref}", e);
            }
        }
    }
}
