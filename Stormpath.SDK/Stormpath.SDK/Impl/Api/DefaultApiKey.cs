// <copyright file="DefaultApiKey.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Api;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Api
{
    internal sealed class DefaultApiKey : AbstractInstanceResource, IApiKey
    {
        private static readonly string IdPropertyName = "id";
        private static readonly string SecretPropertyName = "secret";
        private static readonly string StatusPropertyName = "status";
        private static readonly string AccountPropertyName = "account";

        public DefaultApiKey(ResourceData data)
            : base(data)
        {
        }

        internal IEmbeddedProperty Account => this.GetLinkProperty(AccountPropertyName);

        string IApiKey.Id
            => this.GetStringProperty(IdPropertyName);

        string IApiKey.Secret
            => this.GetStringProperty(SecretPropertyName);

        ApiKeyStatus IApiKey.Status
            => this.GetProperty<ApiKeyStatus>(StatusPropertyName);

        void IApiKey.SetStatus(ApiKeyStatus status)
            => this.SetProperty(StatusPropertyName, status);

        Task<IAccount> IApiKey.GetAccountAsync()
            => this.GetInternalAsyncDataStore().GetResourceAsync<IAccount>(this.Account.Href);

        Task<IApiKey> ISaveable<IApiKey>.SaveAsync(CancellationToken cancellationToken)
            => this.SaveAsync<IApiKey>(cancellationToken);

        Task<bool> IDeletable.DeleteAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().DeleteAsync(this, cancellationToken);
    }
}
