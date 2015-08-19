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
using Stormpath.SDK.Api;
using Stormpath.SDK.Application;
using Stormpath.SDK.Client;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Client
{
    internal sealed class DefaultClient : IClient
    {
        private IInternalDataStore dataStore;
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

            var requestExecutor = factory.CreateRequestExecutor(apiKey, authenticationScheme, connectionTimeout);
            this.dataStore = factory.CreateDataStore(requestExecutor, baseUrl);
        }

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

        Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application)
        {
            throw new NotImplementedException();
        }

        IApplicationAsyncList ITenantActions.GetApplications()
        {
            // return new CollectionResourceQueryable()
            throw new NotImplementedException();
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
    }
}
