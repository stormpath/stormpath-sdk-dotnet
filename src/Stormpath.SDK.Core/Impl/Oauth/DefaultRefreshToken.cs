// <copyright file="DefaultRefreshToken.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Oauth
{
    internal sealed class DefaultRefreshToken :
        AbstractInstanceResource,
        IRefreshToken,
        IRefreshTokenSync
    {
        private static readonly string AccountPropertyName = "account";
        private static readonly string ApplicationPropertyName = "application";
        private static readonly string JwtPropertyName = "jwt";

        public DefaultRefreshToken(ResourceData data)
            : base(data)
        {
        }

        internal IEmbeddedProperty Account => this.GetLinkProperty(AccountPropertyName);

        internal IEmbeddedProperty Application => this.GetLinkProperty(ApplicationPropertyName);

        string IRefreshToken.Jwt => this.GetStringProperty(JwtPropertyName);

        string IRefreshToken.ApplicationHref => this.Application?.Href;

        Task<IAccount> IRefreshToken.GetAccountAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<IAccount>(this.Account.Href, cancellationToken);

        Task<IAccount> IRefreshToken.GetAccountAsync(Action<IRetrievalOptions<IAccount>> retrievalOptions, CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync(this.Account.Href, retrievalOptions, cancellationToken);

        IAccount IRefreshTokenSync.GetAccount()
            => this.GetInternalSyncDataStore().GetResource<IAccount>(this.Account.Href);

        IAccount IRefreshTokenSync.GetAccount(Action<IRetrievalOptions<IAccount>> retrievalOptions)
            => this.GetInternalSyncDataStore().GetResource(this.Account.Href, retrievalOptions);

        Task<IApplication> IRefreshToken.GetApplicationAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<IApplication>(this.Application.Href, cancellationToken);

        Task<IApplication> IRefreshToken.GetApplicationAsync(Action<IRetrievalOptions<IApplication>> retrievalOptions, CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync(this.Application.Href, retrievalOptions, cancellationToken);

        IApplication IRefreshTokenSync.GetApplication()
            => this.GetInternalSyncDataStore().GetResource<IApplication>(this.Application.Href);

        IApplication IRefreshTokenSync.GetApplication(Action<IRetrievalOptions<IApplication>> retrievalOptions)
            => this.GetInternalSyncDataStore().GetResource(this.Application.Href, retrievalOptions);

        Task<bool> IDeletable.DeleteAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().DeleteAsync(this, cancellationToken);

        bool IDeletableSync.Delete()
            => this.GetInternalSyncDataStore().Delete(this);
    }
}
