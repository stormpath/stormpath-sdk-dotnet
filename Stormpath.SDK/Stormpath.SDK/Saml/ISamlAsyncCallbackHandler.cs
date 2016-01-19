// <copyright file="ISamlAsyncCallbackHandler.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.IdSite;

namespace Stormpath.SDK.Saml
{
    public interface ISamlAsyncCallbackHandler
    {
        ISamlAsyncCallbackHandler SetNonceStore(INonceStore nonceStore);

        ISamlAsyncCallbackHandler SetResultListener(ISamlAsyncResultListener resultListener);

        ISamlAsyncCallbackHandler SetResultListener(
            Func<IAccountResult, CancellationToken, Task> onAuthenticated = null,
            Func<IAccountResult, CancellationToken, Task> onLogout = null);

        Task<IAccountResult> GetAccountResultAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
