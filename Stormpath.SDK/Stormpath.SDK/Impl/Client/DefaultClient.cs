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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Api;
using Stormpath.SDK.Application;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Serialization;
using Stormpath.SDK.Shared;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Client
{
    internal sealed class DefaultClient : IClient
    {
        public static readonly int DefaultConnectionTimeout = 20 * 1000;
        public static readonly string DefaultBaseUrl = "https://api.stormpath.com/v1";
        public static readonly AuthenticationScheme DefaultAuthenticationScheme = AuthenticationScheme.SAuthc1;

        private readonly string baseUrl;
        private readonly AuthenticationScheme authenticationScheme;
        private readonly int connectionTimeout;
        private readonly IInternalDataStore dataStore;
        private readonly IJsonSerializer serializer;
        private readonly ILogger logger;

        private string currentTenantHref;

        public DefaultClient(IClientApiKey apiKey, string baseUrl, AuthenticationScheme authenticationScheme, int connectionTimeout, IJsonSerializer serializer, ILogger logger, ICacheProvider cacheProvider)
        {
            if (apiKey == null || !apiKey.IsValid())
                throw new ArgumentException("API Key is not valid.");
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException("Base URL cannot be empty.");
            if (connectionTimeout < 0)
                throw new ArgumentException("Timeout cannot be negative.");

            this.serializer = serializer;
            this.logger = logger == null
                ? new NullLogger()
                : logger;

            var factory = new InternalFactory();

            this.baseUrl = baseUrl;
            this.connectionTimeout = connectionTimeout;
            this.authenticationScheme = authenticationScheme == null
                ? DefaultAuthenticationScheme
                : authenticationScheme;

            var requestExecutor = factory.CreateRequestExecutor(apiKey, authenticationScheme, connectionTimeout, this.logger);
            this.dataStore = factory.CreateDataStore(requestExecutor, baseUrl, this.serializer, this.logger, cacheProvider);
        }

        private IClient AsInterface => this;

        private string CurrentTenantHref => this.currentTenantHref.Nullable() ?? "tenants/current";

        AuthenticationScheme IClient.AuthenticationScheme
        {
            get { return this.dataStore.RequestExecutor.AuthenticationScheme; }
        }

        string IClient.BaseUrl => this.dataStore.BaseUrl;

        int IClient.ConnectionTimeout => this.dataStore.RequestExecutor.ConnectionTimeout;

        T IDataStore.Instantiate<T>() => this.dataStore.Instantiate<T>();

        async Task<ITenant> IClient.GetCurrentTenantAsync(CancellationToken cancellationToken)
        {
            var tenant = await this.dataStore
                .GetResourceAsync<ITenant>(this.CurrentTenantHref, cancellationToken)
                .ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();
            this.currentTenantHref = tenant.Href;

            return tenant;
        }

        Task<T> IDataStore.GetResourceAsync<T>(string href, CancellationToken cancellationToken)
        {
            return this.dataStore.GetResourceAsync<T>(href, cancellationToken);
        }

        async Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application, Action<ApplicationCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken)
        {
            var tenant = await this.AsInterface.GetCurrentTenantAsync().ConfigureAwait(false);

            return await tenant.CreateApplicationAsync(application, creationOptionsAction).ConfigureAwait(false);
        }

        async Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application, IApplicationCreationOptions creationOptions, CancellationToken cancellationToken)
        {
            var tenant = await this.AsInterface.GetCurrentTenantAsync().ConfigureAwait(false);

            return await tenant.CreateApplicationAsync(application, creationOptions).ConfigureAwait(false);
        }

        async Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application, CancellationToken cancellationToken)
        {
            var tenant = await this.AsInterface.GetCurrentTenantAsync().ConfigureAwait(false);

            return await tenant.CreateApplicationAsync(application).ConfigureAwait(false);
        }

        async Task<IApplication> ITenantActions.CreateApplicationAsync(string name, bool createDirectory, CancellationToken cancellationToken)
        {
            var tenant = await this.AsInterface.GetCurrentTenantAsync().ConfigureAwait(false);

            return await tenant.CreateApplicationAsync(name, createDirectory).ConfigureAwait(false);
        }

        ICollectionResourceQueryable<IApplication> ITenantActions.GetApplications()
        {
            var tenant = this.AsInterface.GetCurrentTenantAsync().Result;

            return tenant.GetApplications();
        }

        async Task<IDirectory> ITenantActions.CreateDirectoryAsync(IDirectory directory, CancellationToken cancellationToken)
        {
            var tenant = await this.AsInterface.GetCurrentTenantAsync().ConfigureAwait(false);

            return await tenant.CreateDirectoryAsync(directory).ConfigureAwait(false);
        }

        async Task<IDirectory> ITenantActions.CreateDirectoryAsync(string name, CancellationToken cancellationToken)
        {
            var tenant = await this.AsInterface.GetCurrentTenantAsync().ConfigureAwait(false);

            return await tenant.CreateDirectoryAsync(name, cancellationToken).ConfigureAwait(false);
        }

        ICollectionResourceQueryable<IDirectory> ITenantActions.GetDirectories()
        {
            var tenant = this.AsInterface.GetCurrentTenantAsync().Result;

            return tenant.GetDirectories();
        }

        async Task<IAccount> ITenantActions.VerifyAccountEmailAsync(string token, CancellationToken cancellationToken)
        {
            var tenant = await this.AsInterface.GetCurrentTenantAsync().ConfigureAwait(false);

            return await tenant.VerifyAccountEmailAsync(token).ConfigureAwait(false);
        }

        ICollectionResourceQueryable<IAccount> ITenantActions.GetAccounts()
        {
            var tenant = this.AsInterface.GetCurrentTenantAsync().Result;

            return tenant.GetAccounts();
        }

        ICollectionResourceQueryable<IGroup> ITenantActions.GetGroups()
        {
            var tenant = this.AsInterface.GetCurrentTenantAsync().Result;

            return tenant.GetGroups();
        }
    }
}
