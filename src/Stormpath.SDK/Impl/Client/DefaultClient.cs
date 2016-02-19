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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.Configuration.Abstractions;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Serialization;
using Stormpath.SDK.Shared.Extensions;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Client
{
    internal sealed partial class DefaultClient : IClient, IClientSync, IHasAsyncDataStoreInternal
    {
        // TODO don't expose this, at least until it's immutable!
        private readonly StormpathConfiguration configuration;

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
            StormpathConfiguration configuration,
            IHttpClient httpClient,
            IJsonSerializer serializer,
            ICacheProvider cacheProvider,
            IUserAgentBuilder userAgentBuilder,
            ILogger logger,
            TimeSpan identityMapExpiration)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
            this.serializer = serializer;
            this.cacheProvider = cacheProvider;
            this.logger = logger;

            var compatibleApiKey = new Api.DefaultClientApiKey(configuration.Client.ApiKey.Id, configuration.Client.ApiKey.Secret);
            var compatibleAuthenticationScheme = AuthenticationScheme.Parse(configuration.Client.AuthenticationScheme.ToString());

            var requestExecutor = new DefaultRequestExecutor(httpClient, compatibleApiKey, compatibleAuthenticationScheme, this.logger);

            this.dataStore = new DefaultDataStore(
                this as IClient,
                requestExecutor,
                configuration.Client.BaseUrl,
                this.serializer,
                this.logger,
                userAgentBuilder,
                cacheProvider,
                 identityMapExpiration);

            this.dataStoreAsync = this.dataStore as IInternalAsyncDataStore;
            this.dataStoreSync = this.dataStore as IInternalSyncDataStore;
        }

        private IClient AsInterface => this;

        private IClientSync AsSyncInterface => this;

        private string CurrentTenantHref => this.tenant?.Href.Nullable() ?? "tenants/current";

        internal IJsonSerializer Serializer => this.serializer;

        internal IHttpClient HttpClient => this.httpClient;

        internal IInternalDataStore DataStore => this.dataStore;

        IInternalAsyncDataStore IHasAsyncDataStoreInternal.GetInternalAsyncDataStore() => this.dataStoreAsync;

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
