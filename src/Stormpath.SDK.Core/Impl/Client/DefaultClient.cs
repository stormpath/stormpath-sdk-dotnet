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
using Stormpath.Configuration.Abstractions.Immutable;
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
        private readonly StormpathConfiguration configuration;

        private readonly ICacheProvider cacheProvider;
        private readonly IJsonSerializer serializer;
        private readonly IHttpClient httpClient;
        private readonly ILogger logger;
        private readonly IUserAgentBuilder userAgentBuilder;
        private readonly string instanceIdentifier;

        private readonly IInternalDataStore dataStore;
        private readonly IInternalAsyncDataStore dataStoreAsync;
        private readonly IInternalSyncDataStore dataStoreSync;

        // Flags whether this is a scoped instance of another client or not.
        // If it's a scoped instance, we shouldn't dispose the underlying objects.
        private readonly bool isScopedInstance;

        private bool alreadyDisposed = false;

        private ITenant tenant;

        /// <summary>
        /// Create a base (non-scoped) client.
        /// </summary>
        /// <param name="configuration">The configuration model.</param>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="serializer">The JSON serializer.</param>
        /// <param name="cacheProvider">The cache provider.</param>
        /// <param name="userAgentBuilder">The user agent builder.</param>
        /// <param name="instanceIdentifier">A string that uniquely identifies this client instance for logging.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="identityMapExpiration">The internal identity map expiration time.</param>
        public DefaultClient(
            StormpathConfiguration configuration,
            IHttpClient httpClient,
            IJsonSerializer serializer,
            ICacheProvider cacheProvider,
            IUserAgentBuilder userAgentBuilder,
            string instanceIdentifier,
            ILogger logger,
            TimeSpan identityMapExpiration)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
            this.serializer = serializer;
            this.cacheProvider = cacheProvider;
            this.logger = logger;
            this.userAgentBuilder = userAgentBuilder;
            this.instanceIdentifier = instanceIdentifier;

            var compatibleApiKey = new Api.DefaultClientApiKey(configuration.Client.ApiKey.Id, configuration.Client.ApiKey.Secret);
            var compatibleAuthenticationScheme = AuthenticationScheme.Parse(configuration.Client.AuthenticationScheme.ToString());

            var requestExecutor = new DefaultRequestExecutor(httpClient, compatibleApiKey, compatibleAuthenticationScheme, this.logger);

            this.dataStore = new DefaultDataStore(
                this as IClient,
                requestExecutor,
                configuration.Client.BaseUrl,
                this.serializer,
                this.logger,
                this.userAgentBuilder,
                this.instanceIdentifier,
                cacheProvider,
                identityMapExpiration);

            this.dataStoreAsync = this.dataStore as IInternalAsyncDataStore;
            this.dataStoreSync = this.dataStore as IInternalSyncDataStore;

            this.isScopedInstance = false;
        }

        /// <summary>
        /// Create a scoped client by cloning an existing client.
        /// </summary>
        /// <param name="existing">The existing client to clone.</param>
        /// <param name="userAgentBuilder">The scoped user agent builder.</param>
        /// <param name="instanceIdentifier">The scoping identifier.</param>
        public DefaultClient(IClient existing, IUserAgentBuilder userAgentBuilder, string instanceIdentifier)
        {
            var asImpl = existing as DefaultClient;

            if (asImpl == null)
            {
                throw new ArgumentException("Unknown client type.", nameof(existing));
            }

            this.configuration = asImpl.configuration;
            this.httpClient = asImpl.httpClient;
            this.serializer = asImpl.serializer;
            this.cacheProvider = asImpl.cacheProvider;
            this.logger = asImpl.logger;
            this.userAgentBuilder = userAgentBuilder ?? asImpl.userAgentBuilder;
            this.instanceIdentifier = instanceIdentifier ?? asImpl.instanceIdentifier;

            this.dataStore = new DefaultDataStore(asImpl.dataStore, this.userAgentBuilder, this.instanceIdentifier);

            this.dataStoreAsync = this.dataStore as IInternalAsyncDataStore;
            this.dataStoreSync = this.dataStore as IInternalSyncDataStore;

            this.tenant = asImpl.tenant;

            this.isScopedInstance = true;
        }

        private IClient AsInterface => this;

        private IClientSync AsSyncInterface => this;

        private string CurrentTenantHref => this.tenant?.Href.Nullable() ?? "tenants/current";

        internal IJsonSerializer Serializer => this.serializer;

        internal IHttpClient HttpClient => this.httpClient;

        internal IInternalDataStore DataStore => this.dataStore;

        internal IUserAgentBuilder UserAgentBuilder => this.userAgentBuilder;

        IInternalAsyncDataStore IHasAsyncDataStoreInternal.GetInternalAsyncDataStore() => this.dataStoreAsync;

        public StormpathConfiguration Configuration => this.configuration;

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
            if (this.alreadyDisposed)
            {
                return;
            }

            if (!isScopedInstance && disposing)
            {
                this.dataStore.Dispose();
            }

            this.alreadyDisposed = true;
        }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }
    }
}
