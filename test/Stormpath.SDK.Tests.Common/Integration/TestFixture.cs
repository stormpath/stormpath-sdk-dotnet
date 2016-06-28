// <copyright file="TestFixture.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlexibleConfiguration.Abstractions;
using FluentAssertions;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Error;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Organization;

namespace Stormpath.SDK.Tests.Common.Integration
{
    public class TestFixture : IDisposable
    {
        private TestDataGenerator testData;

        private bool isDisposed = false;

        public TestFixture()
        {
            this.testData = new TestDataGenerator();
            this.TestRunIdentifier = this.testData.Nonce;
            this.CreatedAccountHrefs = new List<string>();
            this.CreatedApplicationHrefs = new List<string>();
            this.CreatedDirectoryHrefs = new List<string>();
            this.CreatedGroupHrefs = new List<string>();
            this.CreatedOrganizationHrefs = new List<string>();

            StaticLogger.Instance.Info($"IT run {this.testData.Nonce} starting...");
            StaticLogger.Instance.Info($"Running against base URL: {TestClients.CurrentConfiguration.Client.BaseUrl}");

            this.AddObjectsToTenantAsync()
                .GetAwaiter().GetResult();

            StaticLogger.Instance.Info($"Done adding objects. Beginning test...");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    StaticLogger.Instance.Info($"IT run {this.testData.Nonce} finished. Cleaning up...");

                    this.RemoveObjectsFromTenantAsync()
                        .GetAwaiter().GetResult();

                    StaticLogger.Instance.Info($"Done cleaning up objects.");

                    StaticLogger.Instance.Info("Caching statistics:");
                    StaticLogger.Instance.Info(TestClients.GetSAuthc1Client().GetCacheProvider().ToString());

                    var filename = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "its.log");
                    StaticLogger.Instance.Info($"Saving log to file {filename}");
                    System.IO.File.WriteAllText(filename, StaticLogger.GetLog());
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

        public string PrimaryGroupHref { get; private set; }

        public string PrimaryOrganizationHref { get; private set; }

        public string PrimaryOrganizationNameKey { get; private set; }

        public string PrimaryAccountHref { get; private set; }

        public string TestRunIdentifier { get; private set; }

        public List<string> CreatedApplicationHrefs { get; private set; }

        public List<string> CreatedAccountHrefs { get; private set; }

        public List<string> CreatedDirectoryHrefs { get; private set; }

        public List<string> CreatedGroupHrefs { get; private set; }

        public List<string> CreatedOrganizationHrefs { get; private set; }

        public Stormpath.SDK.Logging.ILogger Logger { get; } = StaticLogger.Instance;

        private async Task AddObjectsToTenantAsync()
        {
            // Get client and tenant
            var client = TestClients.GetSAuthc1Client();

            var tenant = await client.GetCurrentTenantAsync().ConfigureAwait(false);
            tenant.Should().NotBeNull();
            tenant.Href.Should().NotBeNullOrEmpty();
            this.TenantHref = tenant.Href;

            // Create applications
            try
            {
                // Create and verify applications
                var applicationsToCreate = this.testData.GetTestApplications(client);
                var applicationCreationTasks = applicationsToCreate.Select(app =>
                    tenant.CreateApplicationAsync(app, opt => opt.CreateDirectory = false));
                var resultingApplications = await Task.WhenAll(applicationCreationTasks)
                    .ConfigureAwait(false);
                resultingApplications.Should().NotContain(x => string.IsNullOrEmpty(x.Href));

                // Add them all to the teardown list
                this.CreatedApplicationHrefs.AddRange(resultingApplications.Select(x => x.Href));

                // Grab the one marked as primary
                this.PrimaryApplicationHref = resultingApplications.Where(x => x.Name.Contains("primary")).Single().Href;
            }
            catch (Exception e)
            {
                await this.RemoveObjectsFromTenantAsync()
                    .ConfigureAwait(false);
                throw new Exception("Could not create applications", e);
            }

            // Create organizations
            IOrganization primaryOrganization = null;
            try
            {
                var orgsToCreate = this.testData.GetTestOrganizations(client);
                var orgCreationTasks = orgsToCreate.Select(o =>
                    tenant.CreateOrganizationAsync(o, opt => opt.CreateDirectory = true));
                var resultingOrgs = await Task.WhenAll(orgCreationTasks)
                    .ConfigureAwait(false);
                resultingOrgs.Should().NotContain(x => string.IsNullOrEmpty(x.Href));

                // Verify that directories were created, too
                var getDirectoryTasks = resultingOrgs.Select(x => x.GetDefaultAccountStoreAsync());
                var resultingDirectories = await Task.WhenAll(getDirectoryTasks)
                    .ConfigureAwait(false);
                resultingDirectories.Should().NotContain(x => string.IsNullOrEmpty(x.Href));

                // Add them all to the teardown list
                this.CreatedOrganizationHrefs.AddRange(resultingOrgs.Select(x => x.Href));
                this.CreatedDirectoryHrefs.AddRange(resultingDirectories.Select(x => x.Href));

                // Grab the one marked as primary
                primaryOrganization = resultingOrgs.Where(x => x.Name.Contains("primary")).Single();
                this.PrimaryOrganizationHref = primaryOrganization.Href;
                this.PrimaryOrganizationNameKey = primaryOrganization.NameKey;
                this.PrimaryDirectoryHref = (await primaryOrganization.GetDefaultAccountStoreAsync().ConfigureAwait(false)).Href;
            }
            catch (Exception e)
            {
                await this.RemoveObjectsFromTenantAsync().ConfigureAwait(false);
                throw new Exception("Could not create organizations", e);
            }

            // Add primary organization to primary application as an account store
            var primaryApplication = await client.GetResourceAsync<IApplication>(this.PrimaryApplicationHref)
                .ConfigureAwait(false);
            var mapping = client.Instantiate<IApplicationAccountStoreMapping>()
                .SetAccountStore(primaryOrganization)
                .SetApplication(primaryApplication)
                .SetDefaultAccountStore(true)
                .SetDefaultGroupStore(true);
            await primaryApplication.CreateAccountStoreMappingAsync(mapping)
                .ConfigureAwait(false);

            // Create accounts in primary organization
            try
            {
                var accountsToCreate = this.testData.GetTestAccounts(client);
                var createOptions = new AccountCreationOptionsBuilder()
                {
                    RegistrationWorkflowEnabled = false
                }.Build();

                var accountCreationTasks = accountsToCreate.Select(acct =>
                    primaryOrganization.CreateAccountAsync(acct, createOptions));

                var resultingAccounts = await Task.WhenAll(accountCreationTasks)
                    .ConfigureAwait(false);
                this.CreatedAccountHrefs.AddRange(resultingAccounts.Select(x => x.Href));
            }
            catch (Exception e)
            {
                await this.RemoveObjectsFromTenantAsync()
                    .ConfigureAwait(false);
                throw new Exception("Could not create accounts", e);
            }

            // Create groups
            try
            {
                var groupsToCreate = this.testData.GetTestGroups(client);
                var groupCreationTasks = groupsToCreate.Select(g =>
                    primaryOrganization.CreateGroupAsync(g));

                var resultingGroups = await Task.WhenAll(groupCreationTasks)
                    .ConfigureAwait(false);
                this.CreatedGroupHrefs.AddRange(resultingGroups.Select(x => x.Href));

                this.PrimaryGroupHref = resultingGroups.Where(x => x.Name.Contains("primary")).Single().Href;
            }
            catch (Exception e)
            {
                await this.RemoveObjectsFromTenantAsync()
                    .ConfigureAwait(false);
                throw new Exception("Could not create groups", e);
            }

            // Add some accounts to groups
            try
            {
                var luke = await primaryApplication.GetAccounts()
                    .Where(x => x.Email.StartsWith("lskywalker"))
                    .SingleAsync()
                    .ConfigureAwait(false);
                await luke.AddGroupAsync(this.PrimaryGroupHref).ConfigureAwait(false);

                // Stash an account for easy access
                this.PrimaryAccountHref = luke.Href;
            }
            catch (Exception e)
            {
                await this.RemoveObjectsFromTenantAsync()
                    .ConfigureAwait(false);
                throw new Exception("Could not add accounts to groups", e);
            }
        }

        private async Task RemoveObjectsFromTenantAsync()
        {
            var client = TestClients.GetSAuthc1Client();
            var results = new ConcurrentDictionary<string, Exception>();

            // Delete applications
            var deleteApplicationTasks = this.CreatedApplicationHrefs.Select(async href =>
            {
                try
                {
                    var application = await client.GetResourceAsync<IApplication>(href)
                        .ConfigureAwait(false);
                    var deleteResult = await application.DeleteAsync()
                        .ConfigureAwait(false);
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
                    var directory = await client.GetResourceAsync<IDirectory>(href)
                        .ConfigureAwait(false);
                    var deleteResult = await directory.DeleteAsync()
                        .ConfigureAwait(false);
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

            // Delete organizations
            var deleteOrganizationTasks = this.CreatedOrganizationHrefs.Select(async href =>
            {
                try
                {
                    var org = await client.GetResourceAsync<IOrganization>(href)
                        .ConfigureAwait(false);
                    var deleteResult = await org.DeleteAsync()
                        .ConfigureAwait(false);
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
                Task.WhenAll(deleteApplicationTasks),
                Task.WhenAll(deleteDirectoryTasks),
                Task.WhenAll(deleteOrganizationTasks))
            .ConfigureAwait(false);

            // All done! Throw errors if any occurred
            bool anyErrors = results.Any(kvp => kvp.Value != null);
            if (anyErrors)
            {
                throw new Exception(
                    "Errors occurred during test cleanup. Full log: " + Environment.NewLine
                    + string.Join(Environment.NewLine, results.Select(kvp => $"{kvp.Key} : '{(kvp.Value == null ? "Good" : kvp.Value.Message)}'")));
            }
        }
    }
}
