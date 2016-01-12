// <copyright file="IAccessTokenSync.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Impl.Tenant;

namespace Stormpath.SDK.Impl.Oauth
{
    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on <see cref="SDK.Oauth.IAccessToken"/>.
    /// </summary>
    internal interface IAccessTokenSync :
        IHasTenantSync,
        IDeletableSync
    {
        /// <summary>
        /// Synchronous counterpart to <see cref="SDK.Oauth.IAccessToken.GetAccountAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The <see cref="IAccount">Account</see> associated with this <see cref="SDK.Oauth.IAccessToken">AccessToken</see>.</returns>
        IAccount GetAccount();

        /// <summary>
        /// Synchronous counterpart to <see cref="SDK.Oauth.IAccessToken.GetApplicationAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The <see cref="IApplication">Application</see> associated with this <see cref="SDK.Oauth.IAccessToken">AccessToken</see>.</returns>
        IApplication GetApplication();
    }
}
