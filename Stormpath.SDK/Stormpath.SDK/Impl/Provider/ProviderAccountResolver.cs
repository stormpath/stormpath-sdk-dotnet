// <copyright file="ProviderAccountResolver.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
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
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Impl.Provider
{
    internal sealed class ProviderAccountResolver
    {
        private readonly IInternalAsyncDataStore dataStoreAsync;
        private readonly IInternalSyncDataStore dataStoreSync;

        public ProviderAccountResolver(IInternalDataStore dataStore)
        {
            this.dataStoreAsync = dataStore as IInternalAsyncDataStore;
            this.dataStoreSync = dataStore as IInternalSyncDataStore;
        }

        public Task<IProviderAccountResult> ResolveProviderAccountAsync(string parentHref, IProviderAccountRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(parentHref))
                throw new ArgumentNullException(nameof(parentHref));
            var providerAccountAccess = this.BuildRequest(request);
            var href = $"{parentHref}/accounts";

            // Do NOT store this result in the identity map, because IProviderAccountResult is really just a wrapper around an IAccount.
            // DefaultProviderAccountResult.OnUpdate does the work of taking the implied IAccount and instantiating it.
            return this.dataStoreAsync.CreateAsync<IProviderAccountAccess, IProviderAccountResult>(
                href,
                providerAccountAccess,
                cancellationToken);
        }

        public IProviderAccountResult ResolveProviderAccount(string parentHref, IProviderAccountRequest request)
        {
            if (string.IsNullOrEmpty(parentHref))
                throw new ArgumentNullException(nameof(parentHref));
            var providerAccountAccess = this.BuildRequest(request);
            var href = $"{parentHref}/accounts";

            // Do NOT store this result in the identity map, because IProviderAccountResult is really just a wrapper around an IAccount.
            // DefaultProviderAccountResult.OnUpdate does the work of taking the implied IAccount and instantiating it.
            return this.dataStoreSync.Create<IProviderAccountAccess, IProviderAccountResult>(
                href,
                providerAccountAccess);
        }

        private IProviderAccountAccess BuildRequest(IProviderAccountRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (request.GetProviderData() == null)
                throw new ArgumentNullException("ProviderData");

            var providerAccountAccess = this.dataStoreAsync.Instantiate<IProviderAccountAccess>();
            providerAccountAccess.SetProviderData(request.GetProviderData());

            return providerAccountAccess;
        }
    }
}
