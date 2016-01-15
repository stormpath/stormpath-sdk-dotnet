// <copyright file="DefaultOauthPolicy.Async.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Application;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Oauth
{
    internal sealed partial class DefaultOauthPolicy
    {
        Task<IApplication> IOauthPolicy.GetApplicationAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<IApplication>(this.Application.Href, cancellationToken);

        Task<IOauthPolicy> ISaveable<IOauthPolicy>.SaveAsync(CancellationToken cancellationToken)
            => this.SaveAsync<IOauthPolicy>(cancellationToken);
    }
}
