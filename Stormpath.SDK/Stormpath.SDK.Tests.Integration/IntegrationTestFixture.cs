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
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Error;
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
            this.TestRunIdentifier = this.testData.Nonce;
            this.CreatedAccountHrefs = new List<string>();
            this.CreatedApplicationHrefs = new List<string>();
            this.CreatedDirectoryHrefs = new List<string>();

            this.AddObjectsToTenantAsync()
                .GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            this.RemoveObjectsFromTenantAsync()
                .GetAwaiter().GetResult();
        }

        public string TenantHref { get; private set; }

        public string ApplicationHref { get; private set; }

        public string DirectoryHref { get; private set; }

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

            // Create application
            IApplication createdApplication = null;
            try
            {
                var appData = this.testData.GetTestApplication(client);
                createdApplication = await tenant.CreateApplicationAsync(appData, opt => opt.CreateDirectory = true);
                createdApplication.ShouldNotBe(null);
                createdApplication.Href.ShouldNotBeNullOrEmpty();

                this.ApplicationHref = createdApplication.Href;
                this.DirectoryHref = (await createdApplication.GetDefaultAccountStoreAsync()).Href;

                this.CreatedApplicationHrefs.Add(this.ApplicationHref);
                this.CreatedDirectoryHrefs.Add(this.DirectoryHref);
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
                var createOptions = new AccountCreationOptionsBuilder()
                {
                    RegistrationWorkflowEnabled = false
                }.Build();

                var accountCreationTasks = accountsToCreate.Select(acct =>
                    createdApplication.CreateAccountAsync(acct, createOptions));

                var resultingAccounts = await Task.WhenAll(accountCreationTasks);
                this.CreatedAccountHrefs.AddRange(resultingAccounts.Select(x => x.Href));
            }
            catch (Exception e)
            {
                await this.RemoveObjectsFromTenantAsync();
                throw new ApplicationException("Could not create account", e);
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

            await Task.WhenAll(
                Task.WhenAll(deleteAccountTasks),
                Task.WhenAll(deleteApplicationTasks),
                Task.WhenAll(deleteDirectoryTasks));

            // All done! Throw errors if any occurred
            bool anyErrors = results.Any(kvp => kvp.Value != null);
            if (anyErrors)
            {
                throw new ApplicationException(
                    "Errors occurred during test cleanup. Full log: " + Environment.NewLine

#pragma warning disable SA1119 // Statement must not use unnecessary parenthesis
#pragma warning disable SA1008 // Opening parenthesis must be spaced correctly
                    + string.Join(Environment.NewLine, results.Select(kvp => $"{kvp.Key} : '{(kvp.Value == null ? "Good" : kvp.Value.Message)}'")));
#pragma warning restore SA1008 // Opening parenthesis must be spaced correctly
#pragma warning restore SA1119 // Statement must not use unnecessary parenthesis
            }
        }
    }
}
