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
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Tenant;
using Stormpath.SDK.Tests.Integration.Helpers;

namespace Stormpath.SDK.Tests.Integration
{
    public class IntegrationTestFixture : IDisposable
    {
        private IntegrationTestData testData;

        public IntegrationTestFixture()
        {
            this.testData = new IntegrationTestData();
            this.TestIdentifier = this.testData.Nonce;

            this.AddObjectsToTenantAsync()
                .GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            this.RemoveObjectsFromTenantAsync()
                .GetAwaiter().GetResult();
        }

        public ITenant Tenant { get; private set; }

        public IApplication Application { get; private set; }

        public IDirectory Directory { get; private set; }

        public string TestIdentifier { get; private set; }

        private async Task AddObjectsToTenantAsync()
        {
            // Get client and tenant
            var client = IntegrationTestClients.GetSAuthc1Client();

            var tenant = await client.GetCurrentTenantAsync();
            tenant.ShouldNotBe(null);
            tenant.Href.ShouldNotBeNullOrEmpty();
            this.Tenant = tenant;

            // Create application
            try
            {
                var application = this.testData.GetTestApplication(client);
                var createResult = await tenant.CreateApplicationAsync(application, opt => opt.CreateDirectory = true);
                createResult.ShouldNotBe(null);
                createResult.Href.ShouldNotBeNullOrEmpty();

                this.Application = createResult;
                this.Directory = await client.GetResourceAsync<IDirectory>((await createResult.GetDefaultAccountStoreAsync()).Href);
            }
            catch (Exception e)
            {
                await this.RemoveObjectsFromTenantAsync();
                throw new ApplicationException("Could not create application", e);
            }

            // Create accounts
            try
            {
                var accountsToCreate = this.testData.GetTestAccounts(client);

                var accountCreationTasks = accountsToCreate.Select(acct =>
                    this.Application.CreateAccountAsync(acct));

                await Task.WhenAll(accountCreationTasks);
            }
            catch (Exception e)
            {
                await this.RemoveObjectsFromTenantAsync();
                throw new ApplicationException("Could not create account", e);
            }
        }

        private async Task RemoveObjectsFromTenantAsync()
        {
            var errors = string.Empty;

            // Delete accounts
            bool deleteAccountsSuccessful = false;
            try
            {
                var allAccounts = await this.Directory
                    .GetAccounts()
                    .ToListAsync();
                var accountDeleteTasks = allAccounts.Select(acct => acct.DeleteAsync());

                deleteAccountsSuccessful =
                    (await Task.WhenAll(accountDeleteTasks))
                    .All(result => result == true);
            }
            catch (Exception e)
            {
                errors += "- Error deleting account: "
                    + e.Message + Environment.NewLine;
            }

            if (!deleteAccountsSuccessful)
                errors += "- At least one account delete result was false";

            // Delete application
            bool deleteApplicationSuccessful = false;
            try
            {
                deleteApplicationSuccessful = await this.Application.DeleteAsync();
            }
            catch (Exception e)
            {
                errors += $"- Could not delete application at {this.Application.Href}: "
                    + e.Message + Environment.NewLine;
            }

            if (!deleteApplicationSuccessful)
                errors += "- The application was not removed successfully";

            // Delete directory
            bool deleteDirectorySuccessful = false;
            try
            {
                deleteDirectorySuccessful = await this.Directory.DeleteAsync();
            }
            catch (Exception e)
            {
                errors += $"Could not delete directory at {this.Directory.Href}: "
                    + e.Message + Environment.NewLine;
            }

            if (!deleteDirectorySuccessful)
                errors += "- The directory was not removed successfully";

            // All done! Throw errors if any occurred
            if (!string.IsNullOrEmpty(errors))
                throw new ApplicationException("Errors occurred during object cleanup:" + Environment.NewLine + errors);
        }
    }
}
