// <copyright file="DefaultApplication.LoginSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Auth;
using Stormpath.SDK.Impl.Auth;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Sync;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed partial class DefaultApplication
    {
        IAuthenticationResult IApplicationSync.AuthenticateAccount(IAuthenticationRequest request)
        {
            var dispatcher = new AuthenticationRequestDispatcher();

            return dispatcher.Authenticate(this.GetInternalSyncDataStore(), this, request, null);
        }

        IAuthenticationResult IApplicationSync.AuthenticateAccount(IAuthenticationRequest request, Action<IRetrievalOptions<IAuthenticationResult>> responseOptions)
        {
            var options = new DefaultRetrievalOptions<IAuthenticationResult>();
            responseOptions(options);

            var dispatcher = new AuthenticationRequestDispatcher();

            return dispatcher.Authenticate(this.GetInternalSyncDataStore(), this, request, options);
        }

        IAuthenticationResult IApplicationSync.AuthenticateAccount(string username, string password)
        {
            var request = new UsernamePasswordRequest(username, password) as IAuthenticationRequest;

            return this.AuthenticateAccount(request);
        }

        bool IApplicationSync.TryAuthenticateAccount(string username, string password)
        {
            try
            {
                var loginResult = this.AuthenticateAccount(username, password);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
