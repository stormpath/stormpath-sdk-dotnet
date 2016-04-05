// <copyright file="TenantActionsShared.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Organization;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Tenant
{
    /// <summary>
    /// Both <see cref="SDK.Client.IClient"/> and <see cref="SDK.Tenant.ITenant"/> implement
    /// <see cref="SDK.Tenant.ITenantActions"/>, so this shared class wraps the methods up in a DRY way.
    /// </summary>
    internal static class TenantActionsShared
    {
        public static Task<IAccount> GetAccountAsync(IInternalAsyncDataStore asyncDataStore, string href, CancellationToken cancellationToken)
            => asyncDataStore.GetResourceAsync<IAccount>(href, cancellationToken);

        public static Task<IAccount> GetAccountAsync(IInternalAsyncDataStore asyncDataStore, string href, Action<IRetrievalOptions<IAccount>> retrievalOptions, CancellationToken cancellationToken)
            => asyncDataStore.GetResourceAsync(href, retrievalOptions, cancellationToken);

        public static Task<IApplication> GetApplicationAsync(IInternalAsyncDataStore asyncDataStore, string href, CancellationToken cancellationToken)
            => asyncDataStore.GetResourceAsync<IApplication>(href, cancellationToken);

        public static Task<IDirectory> GetDirectoryAsync(IInternalAsyncDataStore asyncDataStore, string href, CancellationToken cancellationToken)
            => asyncDataStore.GetResourceAsync<IDirectory>(href, cancellationToken);

        public static Task<IGroup> GetGroupAsync(IInternalAsyncDataStore asyncDataStore, string href, CancellationToken cancellationToken)
            => asyncDataStore.GetResourceAsync<IGroup>(href, cancellationToken);

        public static Task<IAccessToken> GetAccessTokenAsync(IInternalAsyncDataStore asyncDataStore, string href, CancellationToken cancellationToken)
            => asyncDataStore.GetResourceAsync<IAccessToken>(href, cancellationToken);

        public static Task<IRefreshToken> GetRefreshTokenAsync(IInternalAsyncDataStore asyncDataStore, string href, CancellationToken cancellationToken)
            => asyncDataStore.GetResourceAsync<IRefreshToken>(href, cancellationToken);

        public static Task<IOrganization> GetOrganizationAsync(IInternalAsyncDataStore asyncDataStore, string href, CancellationToken cancellationToken)
            => asyncDataStore.GetResourceAsync<IOrganization>(href, cancellationToken);

        public static IAccount GetAccount(IInternalSyncDataStore syncDataStore, string href)
            => syncDataStore.GetResource<IAccount>(href);

        public static IAccount GetAccount(IInternalSyncDataStore syncDataStore, string href, Action<IRetrievalOptions<IAccount>> retrievalOptions)
            => syncDataStore.GetResource(href, retrievalOptions);

        public static IApplication GetApplication(IInternalSyncDataStore syncDataStore, string href)
            => syncDataStore.GetResource<IApplication>(href);

        public static IDirectory GetDirectory(IInternalSyncDataStore syncDataStore, string href)
            => syncDataStore.GetResource<IDirectory>(href);

        public static IGroup GetGroup(IInternalSyncDataStore syncDataStore, string href)
            => syncDataStore.GetResource<IGroup>(href);

        public static IOrganization GetOrganization(IInternalSyncDataStore syncDataStore, string href)
            => syncDataStore.GetResource<IOrganization>(href);
    
        public static IAccessToken GetAccessToken(IInternalSyncDataStore syncDataStore, string href)
            => syncDataStore.GetResource<IAccessToken>(href);

        public static IRefreshToken GetRefreshToken(IInternalSyncDataStore syncDataStore, string href)
            => syncDataStore.GetResource<IRefreshToken>(href);
    }
}
