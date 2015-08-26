// <copyright file="DefaultClient.cs" company="Stormpath, Inc.">
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
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Api;
using Stormpath.SDK.Application;
using Stormpath.SDK.Client;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Client
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:Do not use regions", Justification = "Reviewed.")]
    internal sealed class DefaultClient : IClient
    {
        internal const int DefaultConnectionTimeout = 20 * 1000;
        internal const string DefaultBaseUrl = "https://api.stormpath.com/v1";
        private static readonly AuthenticationScheme DefaultAuthenticationScheme = AuthenticationScheme.SAuthc1;

        private readonly string baseUrl;
        private readonly AuthenticationScheme authenticationScheme;
        private readonly int connectionTimeout;
        private readonly IInternalDataStore dataStore;

        private string currentTenantHref;

        public DefaultClient(IClientApiKey apiKey, string baseUrl, AuthenticationScheme authenticationScheme, int connectionTimeout)
        {
            if (apiKey == null || !apiKey.IsValid())
                throw new ArgumentException("API Key is not valid.");
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException("Base URL cannot be empty.");
            if (connectionTimeout < 0)
                throw new ArgumentException("Timeout cannot be negative.");

            var factory = new InternalFactory();

            this.baseUrl = baseUrl;
            this.connectionTimeout = connectionTimeout;
            this.authenticationScheme = authenticationScheme == null
                ? DefaultAuthenticationScheme
                : authenticationScheme;

            var requestExecutor = factory.CreateRequestExecutor(apiKey, authenticationScheme, connectionTimeout);
            this.dataStore = factory.CreateDataStore(requestExecutor, baseUrl);
        }

        private IClient This => this;

        private string CurrentTenantHref => currentTenantHref.Nullable() ?? "tenants/current";

        AuthenticationScheme IClient.AuthenticationScheme
        {
            get { return this.dataStore.RequestExecutor.AuthenticationScheme; }
        }

        string IClient.BaseUrl
        {
            get { return this.dataStore.BaseUrl; }
        }

        int IClient.ConnectionTimeout
        {
            get { return this.dataStore.RequestExecutor.ConnectionTimeout; }
        }

        async Task<ITenant> IClient.GetCurrentTenantAsync(CancellationToken cancellationToken)
        {
            var tenant = await dataStore
                .GetResourceAsync<ITenant>(this.CurrentTenantHref, cancellationToken)
                .ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();
            this.currentTenantHref = tenant.Href;

            return tenant;
        }

        #region ITenantActions (pass-thru to Tenant)

        async Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application)
        {
            var tenant = await This.GetCurrentTenantAsync()
                .ConfigureAwait(false);

            return await tenant.CreateApplicationAsync(application)
                .ConfigureAwait(false);
        }

        async Task<IApplication> ITenantActions.CreateApplicationAsync(string name)
        {
            var tenant = await This.GetCurrentTenantAsync()
                .ConfigureAwait(false);

            return await tenant.CreateApplicationAsync(name)
                .ConfigureAwait(false);
        }

        ICollectionResourceQueryable<IApplication> ITenantActions.GetApplications()
        {
            var tenant = This.GetCurrentTenantAsync().Result;

            return tenant.GetApplications();
        }

        async Task<IDirectory> ITenantActions.CreateDirectory(IDirectory directory)
        {
            var tenant = await This.GetCurrentTenantAsync()
                .ConfigureAwait(false);

            return await tenant.CreateDirectory(directory)
                .ConfigureAwait(false);
        }

        ICollectionResourceQueryable<IDirectory> ITenantActions.GetDirectories()
        {
            var tenant = This.GetCurrentTenantAsync().Result;

            return tenant.GetDirectories();
        }

        async Task<IAccount> ITenantActions.VerifyAccountEmailAsync(string token)
        {
            var tenant = await This.GetCurrentTenantAsync()
                .ConfigureAwait(false);

            return await tenant.VerifyAccountEmailAsync(token)
                .ConfigureAwait(false);
        }

        ICollectionResourceQueryable<IAccount> ITenantActions.GetAccounts()
        {
            var tenant = This.GetCurrentTenantAsync().Result;

            return tenant.GetAccounts();
        }

        ICollectionResourceQueryable<IGroup> ITenantActions.GetGroups()
        {
            var tenant = This.GetCurrentTenantAsync().Result;

            return tenant.GetGroups();
        }

        #endregion
    }
}
