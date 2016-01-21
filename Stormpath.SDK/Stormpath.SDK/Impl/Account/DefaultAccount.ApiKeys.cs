// <copyright file="DefaultAccount.ApiKeys.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Api;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed partial class DefaultAccount
    {
        Task<IApiKey> IAccount.CreateApiKeyAsync(CancellationToken cancellationToken)
            => this.AsInterface.CreateApiKeyAsync(_ => { }, cancellationToken);

        Task<IApiKey> IAccount.CreateApiKeyAsync(Action<IRetrievalOptions<IApiKey>> retrievalOptionsAction, CancellationToken cancellationToken)
        {
            var retrievalOptions = new DefaultRetrievalOptions<IApiKey>();
            retrievalOptionsAction(retrievalOptions);

            return this.GetInternalAsyncDataStore().CreateAsync(
                this.ApiKeys.Href,
                this.GetInternalDataStore().Instantiate<IApiKey>(),
                retrievalOptions,
                cancellationToken);
        }

        IApiKey IAccountSync.CreateApiKey()
            => this.AsSyncInterface.CreateApiKey(_ => { });

        IApiKey IAccountSync.CreateApiKey(Action<IRetrievalOptions<IApiKey>> retrievalOptionsAction)
        {
            var retrievalOptions = new DefaultRetrievalOptions<IApiKey>();
            retrievalOptionsAction(retrievalOptions);

            return this.GetInternalSyncDataStore().Create(
                this.ApiKeys.Href,
                this.GetInternalDataStore().Instantiate<IApiKey>(),
                retrievalOptions);
        }
    }
}
