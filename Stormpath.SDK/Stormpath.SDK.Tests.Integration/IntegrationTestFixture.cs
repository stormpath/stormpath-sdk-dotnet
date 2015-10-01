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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Error;
using Stormpath.SDK.Tests.Integration.Helpers;

namespace Stormpath.SDK.Tests.Integration
{
    public class IntegrationTestFixture : IDisposable
    {
        private IntegrationTestData testData;

        private bool isDisposed = false;

        public IntegrationTestFixture()
        {
            this.testData = new IntegrationTestData();
            this.TestRunIdentifier = this.testData.Nonce;
            this.CreatedAccountHrefs = new List<string>();
            this.CreatedApplicationHrefs = new List<string>();
            this.CreatedDirectoryHrefs = new List<string>();

            this.AddObjectsToTenantAsync()
                .GetAwaiter().GetResult();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.RemoveObjectsFromTenantAsync()
                        .GetAwaiter().GetResult();
                }

                this.isDisposed = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        public string TenantHref { get; private set; }

        public string PrimaryApplicationHref { get; private set; }

        public string PrimaryDirectoryHref { get; private set; }

        public string TestRunIdentifier { get; private set; }

        public List<string> CreatedApplicationHrefs { get; private set; }

        public List<string> CreatedAccountHrefs { get; private set; }

        public List<string> CreatedDirectoryHrefs { get; private set; }

        private async Task AddObjectsToTenantAsync()
        {
            // Get client and tenant
            var client = IntegrationTestClients.GetSAuthc1Client();

            var tenant = await client.GetCurrentTenantAsync();
            tenant.ShouldNotBe(null);
            tenant.Href.ShouldNotBeNullOrEmpty();
            this.TenantHref = tenant.Href;

            // Create applications
            IApplication primaryApplication = null;
            try
            {
                // Create and verify applications
                var applicationsToCreate = this.testData.GetTestApplications(client);
                var applicationCreationTasks = applicationsToCreate.Select(app =>
                    tenant.CreateApplicationAsync(app, opt => opt.CreateDirectory = true));
                var resultingApplications = await Task.WhenAll(applicationCreationTasks);
                resultingApplications.ShouldNotContain(x => string.IsNullOrEmpty(x.Href));

                // Verify that directories were created, too
                var getDirectoryTasks = resultingApplications.Select(x => x.GetDefaultAccountStoreAsync());
                var resultingDirectories = await Task.WhenAll(getDirectoryTasks);
                resultingDirectories.ShouldNotContain(x => string.IsNullOrEmpty(x.Href));

                // Add them all to the teardown list
                this.CreatedApplicationHrefs.AddRange(resultingApplications.Select(x => x.Href));
                this.CreatedDirectoryHrefs.AddRange(resultingDirectories.Select(x => x.Href));

                // Grab the one marked as primary
                primaryApplication = resultingApplications.Where(x => x.Name.Contains("primary")).Single();
                this.PrimaryApplicationHref = primaryApplication.Href;
                this.PrimaryDirectoryHref = (await primaryApplication.GetDefaultAccountStoreAsync()).Href;
            }
            catch (Exception e)
            {
                await this.RemoveObjectsFromTenantAsync();
                throw new ApplicationException("Could not create applications", e);
            }

            // Create accounts
            try
            {
                var accountsToCreate = this.testData.GetTestAccounts(client);
                var createOptions = new AccountCreationOptionsBuilder()
                {
                    RegistrationWorkflowEnabled = false
                }.Build();

                var accountCreationTasks = accountsToCreate.Select(acct =>
                    primaryApplication.CreateAccountAsync(acct, createOptions));

                var resultingAccounts = await Task.WhenAll(accountCreationTasks);
                this.CreatedAccountHrefs.AddRange(resultingAccounts.Select(x => x.Href));
            }
            catch (Exception e)
            {
                await this.RemoveObjectsFromTenantAsync();
                throw new ApplicationException("Could not create accounts", e);
            }
        }

        private async Task RemoveObjectsFromTenantAsync()
        {
            var client = IntegrationTestClients.GetSAuthc1Client();
            var results = new ConcurrentDictionary<string, Exception>();

            // Delete accounts
            var deleteAccountTasks = this.CreatedAccountHrefs.Select(async href =>
            {
                try
                {
                    var account = await client.GetResourceAsync<IAccount>(href);
                    var deleteResult = await account.DeleteAsync();
                    results.TryAdd(href, null);
                }
                catch (ResourceException rex)
                {
                    if (rex.Code == 404)
                    {
                        // Already deleted
                        results.TryAdd(href, null);
                    }
                }
                catch (Exception e)
                {
                    results.TryAdd(href, e);
                }
            });

            // Delete applications
            var deleteApplicationTasks = this.CreatedApplicationHrefs.Select(async href =>
            {
                try
                {
                    var application = await client.GetResourceAsync<IApplication>(href);
                    var deleteResult = await application.DeleteAsync();
                    results.TryAdd(href, null);
                }
                catch (ResourceException rex)
                {
                    if (rex.Code == 404)
                    {
                        // Already deleted
                        results.TryAdd(href, null);
                    }
                }
                catch (Exception e)
                {
                    results.TryAdd(href, e);
                }
            });

            // Delete directories
            var deleteDirectoryTasks = this.CreatedDirectoryHrefs.Select(async href =>
            {
                try
                {
                    var directory = await client.GetResourceAsync<IDirectory>(href);
                    var deleteResult = await directory.DeleteAsync();
                    results.TryAdd(href, null);
                }
                catch (ResourceException rex)
                {
                    if (rex.Code == 404)
                    {
                        // Already deleted
                        results.TryAdd(href, null);
                    }
                }
                catch (Exception e)
                {
                    results.TryAdd(href, e);
                }
            });

            await Task.WhenAll(deleteAccountTasks);
            await Task.WhenAll(deleteApplicationTasks);
            await Task.WhenAll(deleteDirectoryTasks);

            // All done! Throw errors if any occurred
            bool anyErrors = results.Any(kvp => kvp.Value != null);
            if (anyErrors)
            {
                throw new ApplicationException(
                    "Errors occurred during test cleanup. Full log: " + Environment.NewLine
                    + string.Join(Environment.NewLine, results.Select(kvp => $"{kvp.Key} : '{(kvp.Value == null ? "Good" : kvp.Value.Message)}'")));
            }
        }
    }
}
