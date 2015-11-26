// <copyright file="InlineIdSiteAsyncResultListener.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.IdSite;

namespace Stormpath.SDK.Impl.IdSite
{
    internal sealed class InlineIdSiteAsyncResultListener : IIdSiteAsyncResultListener
    {
        private readonly Func<IAccountResult, CancellationToken, Task> onRegistered;
        private readonly Func<IAccountResult, CancellationToken, Task> onAuthenticated;
        private readonly Func<IAccountResult, CancellationToken, Task> onLogout;

        public InlineIdSiteAsyncResultListener(
            Func<IAccountResult, CancellationToken, Task> onRegistered,
            Func<IAccountResult, CancellationToken, Task> onAuthenticated,
            Func<IAccountResult, CancellationToken, Task> onLogout)
        {
            this.onRegistered = onRegistered;
            this.onAuthenticated = onAuthenticated;
            this.onLogout = onLogout;
        }

        Task IIdSiteAsyncResultListener.OnAuthenticatedAsync(IAccountResult result, CancellationToken cancellationToken)
        {
            return this.onAuthenticated != null
                ? this.onAuthenticated(result, cancellationToken)
                : Task.FromResult(true);
        }

        Task IIdSiteAsyncResultListener.OnLogoutAsync(IAccountResult result, CancellationToken cancellationToken)
        {
            return this.onLogout != null
                ? this.onLogout(result, cancellationToken)
                : Task.FromResult(true);
        }

        Task IIdSiteAsyncResultListener.OnRegisteredAsync(IAccountResult result, CancellationToken cancellationToken)
        {
            return this.onRegistered != null
                ? this.onRegistered(result, cancellationToken)
                : Task.FromResult(true);
        }
    }
}
