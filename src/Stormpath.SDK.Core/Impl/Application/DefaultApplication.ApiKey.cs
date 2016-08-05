// <copyright file="DefaultApplication.ApiKey.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Api;
using Stormpath.SDK.Application;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Sync;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed partial class DefaultApplication
    {
        [Obsolete("Use GetApiKeys() collection")]
        Task<IApiKey> IApplication.GetApiKeyAsync(string apiKeyId, CancellationToken cancellationToken)
            => this.AsInterface.GetApiKeyAsync(apiKeyId, opt => { }, cancellationToken);

        [Obsolete("Use GetApiKeys() collection")]
        Task<IApiKey> IApplication.GetApiKeyAsync(string apiKeyId, Action<IRetrievalOptions<IApiKey>> retrievalOptionsAction, CancellationToken cancellationToken)
        {
            return AsInterface.GetApiKeys()
                .Where(key => key.Id == apiKeyId)
                .Expand(key => key.GetAccount())
                .SingleOrDefaultAsync(cancellationToken);
        }

        [Obsolete("Use GetApiKeys() collection")]
        IApiKey IApplicationSync.GetApiKey(string apiKeyId)
            => this.AsSyncInterface.GetApiKey(apiKeyId, opt => { });

        [Obsolete("Use GetApiKeys() collection")]
        IApiKey IApplicationSync.GetApiKey(string apiKeyId, Action<IRetrievalOptions<IApiKey>> retrievalOptionsAction)
        {
            return AsInterface.GetApiKeys()
                .Where(key => key.Id == apiKeyId)
                .Expand(key => key.GetAccount())
                .Synchronously()
                .SingleOrDefault();
        }
    }
}
