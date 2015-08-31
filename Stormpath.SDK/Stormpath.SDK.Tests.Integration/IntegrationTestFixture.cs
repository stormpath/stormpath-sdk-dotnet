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
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Tenant;
using Stormpath.SDK.Tests.Integration.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Integration
{
    public class IntegrationTestFixture : IDisposable
    {
        private ITenant tenant;
        private IApplication application;

        public IntegrationTestFixture()
        {
            this.AddObjectsToTenantAsync()
                .GetAwaiter().GetResult();
        }

        public ITenant Tenant
        {
            get
            {
                return this.tenant;
            }
        }

        public IApplication Application
        {
            get
            {
                return this.application;
            }
        }

        public void Dispose()
        {
            this.RemoveObjectsFromTenantAsync()
                .GetAwaiter().GetResult();
        }

        private async Task AddObjectsToTenantAsync()
        {
            // Get client and tenant
            var client = IntegrationTestClients.GetSAuthc1Client();

            var tenant = await client.GetCurrentTenantAsync();
            tenant.ShouldNotBe(null);
            tenant.Href.ShouldNotBeNullOrEmpty();
            this.tenant = tenant;

            // Create application
            try
            {
                var application = IntegrationTestData.GetTestApplication(client);
                var createResult = await tenant.CreateApplicationAsync(application);
                createResult.ShouldNotBe(null);
                createResult.Href.ShouldNotBeNullOrEmpty();

                this.application = createResult;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Could not create application", e);
            }

            // Create accounts
            try
            {
                var accountsToCreate = IntegrationTestData.GetTestAccounts(client);

                var accountCreationTasks = accountsToCreate.Select(acct =>
                    this.Application.CreateAccountAsync(acct));

                await Task.WhenAll(accountCreationTasks);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Could not create account", e);
            }
        }

        private async Task RemoveObjectsFromTenantAsync()
        {
            // Delete accounts
            bool accountDeletesSuccessful = false;
            try
            {
                var allAccounts = await this.Tenant.GetAccounts().ToListAsync();
                var accountDeleteTasks = allAccounts.Select(acct => acct.DeleteAsync());

                accountDeletesSuccessful =
                    (await Task.WhenAll(accountDeleteTasks))
                    .All(result => result == true);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Error deleting account", e);
            }

            accountDeletesSuccessful.ShouldBe(true, "At least one account delete result was false");

            // Delete application
            bool deleteApplicationSuccessful = false;
            try
            {
                deleteApplicationSuccessful = await this.Application.DeleteAsync();
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Could not delete application at {this.Application}", e);
            }

            deleteApplicationSuccessful.ShouldBe(true, "Application delete result was false");
        }
    }
}
