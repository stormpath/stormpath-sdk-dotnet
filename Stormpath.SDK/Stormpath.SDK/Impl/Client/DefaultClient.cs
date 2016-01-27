// <copyright file="DefaultClient.cs" company="Stormpath, Inc.">
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
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Api;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Serialization;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Client
{
    internal sealed partial class DefaultClient : IClient, IClientSync
    {
        private readonly IClientApiKey apiKey;
        private readonly string baseUrl;
        private readonly AuthenticationScheme authenticationScheme;
        private readonly int connectionTimeout;
        private readonly IWebProxy proxy;
        private readonly ICacheProvider cacheProvider;
        private readonly IJsonSerializer serializer;
        private readonly IHttpClient httpClient;
        private readonly ILogger logger;

        private readonly IInternalDataStore dataStore;
        private readonly IInternalAsyncDataStore dataStoreAsync;
        private readonly IInternalSyncDataStore dataStoreSync;

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
            IUserAgentBuilder userAgentBuilder,
            ILogger logger,
            TimeSpan identityMapExpiration)
        {
            if (apiKey == null || !apiKey.IsValid())
            {
                throw new ArgumentException("API Key is not valid.");
            }

            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentNullException("Base URL cannot be empty.");
            }

            if (connectionTimeout < 0)
            {
                throw new ArgumentException("Timeout cannot be negative.");
            }

            this.logger = logger;
            this.apiKey = apiKey;
            this.baseUrl = baseUrl;
            this.connectionTimeout = connectionTimeout;
            this.proxy = proxy;
            this.cacheProvider = cacheProvider;
            this.authenticationScheme = authenticationScheme;
            this.serializer = serializer;
            this.httpClient = httpClient;

            var requestExecutor = new DefaultRequestExecutor(httpClient, apiKey, authenticationScheme, this.logger);

            this.dataStore = new DefaultDataStore(this as IClient, requestExecutor, baseUrl, this.serializer, this.logger, userAgentBuilder, cacheProvider, identityMapExpiration);
            this.dataStoreAsync = this.dataStore as IInternalAsyncDataStore;
            this.dataStoreSync = this.dataStore as IInternalSyncDataStore;
        }

        private IClient AsInterface => this;

        private IClientSync AsSyncInterface => this;

        private string CurrentTenantHref => this.tenant?.Href.Nullable() ?? "tenants/current";

        internal IClientApiKey ApiKey => this.apiKey;

        internal string BaseUrl => this.dataStoreAsync.BaseUrl;

        internal AuthenticationScheme AuthenticationScheme => this.authenticationScheme;

        internal int ConnectionTimeout => this.connectionTimeout;

        internal IWebProxy Proxy => this.proxy;

        internal IJsonSerializer Serializer => this.serializer;

        internal IHttpClient HttpClient => this.httpClient;

        internal IInternalDataStore DataStore => this.dataStore;

        private async Task EnsureTenantAsync(CancellationToken cancellationToken)
        {
            if (this.tenant == null)
            {
                await this.AsInterface.GetCurrentTenantAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private void EnsureTenant()
        {
            if (this.tenant == null)
            {
                this.GetCurrentTenant();
            }
        }

        ICacheProvider IClient.GetCacheProvider()
            => this.cacheProvider;

        async Task<ITenant> IClient.GetCurrentTenantAsync(CancellationToken cancellationToken)
        {
            this.tenant = await this.dataStoreAsync
                .GetResourceAsync<ITenant>(this.CurrentTenantHref, cancellationToken)
                .ConfigureAwait(false);

            return this.tenant;
        }

        ITenant IClientSync.GetCurrentTenant()
        {
            this.tenant = this.dataStoreSync.GetResource<ITenant>(this.CurrentTenantHref);

            return this.tenant;
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
            this.Dispose(true);
        }
    }
}
