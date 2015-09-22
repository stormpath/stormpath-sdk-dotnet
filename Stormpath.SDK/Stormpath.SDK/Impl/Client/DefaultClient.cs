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
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Api;
using Stormpath.SDK.Application;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Tenant;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Serialization;
using Stormpath.SDK.Shared;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Client
{
    internal sealed class DefaultClient : IClient, IClientSync
    {
        private readonly IClientApiKey apiKey;
        private readonly string baseUrl;
        private readonly AuthenticationScheme authenticationScheme;
        private readonly int connectionTimeout;
        private readonly IWebProxy proxy;
        private readonly IInternalDataStore dataStore;
        private readonly IJsonSerializer serializer;
        private readonly ILogger logger;

        private bool alreadyDisposed = false;

        private ITenant tenant;

        public DefaultClient(
            IClientApiKey apiKey,
            string baseUrl,
            AuthenticationScheme authenticationScheme,
            int connectionTimeout,
            IWebProxy proxy,
            IHttpClient httpClient,
            IJsonSerializer serializer,
            ICacheProvider cacheProvider,
            ILogger logger)
        {
            if (apiKey == null || !apiKey.IsValid())
                throw new ArgumentException("API Key is not valid.");
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException("Base URL cannot be empty.");
            if (connectionTimeout < 0)
                throw new ArgumentException("Timeout cannot be negative.");

            this.logger = logger;
            this.apiKey = apiKey;
            this.baseUrl = baseUrl;
            this.connectionTimeout = connectionTimeout;
            this.proxy = proxy;
            this.authenticationScheme = authenticationScheme;
            this.serializer = serializer;

            var requestExecutor = new DefaultRequestExecutor(httpClient, apiKey, authenticationScheme, this.logger);

            this.dataStore = new DefaultDataStore(requestExecutor, baseUrl, this.serializer, this.logger, cacheProvider);
        }

        private IClient AsInterface => this;

        private IClientSync AsSyncInterface => this;

        private string CurrentTenantHref => this.tenant?.Href.Nullable() ?? "tenants/current";

        internal IClientApiKey ApiKey => this.apiKey;

        internal string BaseUrl => this.dataStore.BaseUrl;

        internal AuthenticationScheme AuthenticationScheme => this.authenticationScheme;

        internal int ConnectionTimeout => this.connectionTimeout;

        internal IWebProxy Proxy => this.proxy;

        T IDataStore.Instantiate<T>() => this.dataStore.Instantiate<T>();

        async Task<ITenant> IClient.GetCurrentTenantAsync(CancellationToken cancellationToken)
        {
            this.tenant = await this.dataStore
                .GetResourceAsync<ITenant>(this.CurrentTenantHref, cancellationToken)
                .ConfigureAwait(false);

            return this.tenant;
        }

        ITenant IClientSync.GetCurrentTenant()
        {
            this.tenant = this.dataStore.GetResource<ITenant>(this.CurrentTenantHref);

            return this.tenant;
        }

        Task<T> IDataStore.GetResourceAsync<T>(string href, CancellationToken cancellationToken)
            => this.dataStore.GetResourceAsync<T>(href, cancellationToken);

        T IDataStoreSync.GetResource<T>(string href)
            => this.dataStore.GetResource<T>(href);

        async Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application, Action<ApplicationCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken)
        {
            if (this.tenant == null)
                await this.AsInterface.GetCurrentTenantAsync(cancellationToken).ConfigureAwait(false);

            return await this.tenant.CreateApplicationAsync(application, creationOptionsAction, cancellationToken).ConfigureAwait(false);
        }

        IApplication ITenantActionsSync.CreateApplication(IApplication application, Action<ApplicationCreationOptionsBuilder> creationOptionsAction)
        {
            if (this.tenant == null)
                this.AsSyncInterface.GetCurrentTenant();

            return this.tenant.CreateApplication(application, creationOptionsAction);
        }

        async Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application, IApplicationCreationOptions creationOptions, CancellationToken cancellationToken)
        {
            if (this.tenant == null)
                await this.AsInterface.GetCurrentTenantAsync(cancellationToken).ConfigureAwait(false);

            return await this.tenant.CreateApplicationAsync(application, creationOptions, cancellationToken).ConfigureAwait(false);
        }

        IApplication ITenantActionsSync.CreateApplication(IApplication application, IApplicationCreationOptions creationOptions)
        {
            if (this.tenant == null)
                this.AsSyncInterface.GetCurrentTenant();

            return this.tenant.CreateApplication(application, creationOptions);
        }

        async Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application, CancellationToken cancellationToken)
        {
            if (this.tenant == null)
                await this.AsInterface.GetCurrentTenantAsync(cancellationToken).ConfigureAwait(false);

            return await this.tenant.CreateApplicationAsync(application, cancellationToken).ConfigureAwait(false);
        }

        IApplication ITenantActionsSync.CreateApplication(IApplication application)
        {
            if (this.tenant == null)
                this.AsSyncInterface.GetCurrentTenant();

            return this.tenant.CreateApplication(application);
        }

        async Task<IApplication> ITenantActions.CreateApplicationAsync(string name, bool createDirectory, CancellationToken cancellationToken)
        {
            if (this.tenant == null)
                await this.AsInterface.GetCurrentTenantAsync(cancellationToken).ConfigureAwait(false);

            return await this.tenant.CreateApplicationAsync(name, createDirectory, cancellationToken).ConfigureAwait(false);
        }

        IApplication ITenantActionsSync.CreateApplication(string name, bool createDirectory)
        {
            if (this.tenant == null)
                this.AsSyncInterface.GetCurrentTenant();

            return this.tenant.CreateApplication(name, createDirectory);
        }

        IAsyncQueryable<IApplication> ITenantActions.GetApplications()
        {
            if (this.tenant == null)
                this.AsInterface.GetCurrentTenantAsync().GetAwaiter().GetResult();

            return this.tenant.GetApplications();
        }

        IAsyncQueryable<IDirectory> ITenantActions.GetDirectories()
        {
            if (this.tenant == null)
                this.AsInterface.GetCurrentTenantAsync().GetAwaiter().GetResult();

            return this.tenant.GetDirectories();
        }

        IAsyncQueryable<IAccount> ITenantActions.GetAccounts()
        {
            if (this.tenant == null)
                this.AsInterface.GetCurrentTenantAsync().GetAwaiter().GetResult();

            return this.tenant.GetAccounts();
        }

        private void Dispose(bool disposing)
        {
            if (!this.alreadyDisposed)
            {
                if (disposing)
                {
                    this.dataStore.Dispose();
                }

                this.alreadyDisposed = true;
            }
        }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }
    }
}
