// <copyright file="AuthenticationRequestDispatcher.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Impl.DataStore;

namespace Stormpath.SDK.Impl.Auth
{
    internal sealed class AuthenticationRequestDispatcher
    {
        private static void Validate(IInternalDataStore dataStore, IApplication application, IAuthenticationRequest request)
        {
            if (dataStore == null)
                throw new ArgumentNullException(nameof(dataStore));
            if (application == null)
                throw new ArgumentNullException(nameof(application));
            if (request == null)
                throw new ArgumentNullException(nameof(request));
        }

        public Task<IAuthenticationResult> AuthenticateAsync(IInternalDataStore dataStore, IApplication application, IAuthenticationRequest request, CancellationToken cancellationToken)
        {
            Validate(dataStore, application, request);

            if (request is UsernamePasswordRequest)
                return new BasicAuthenticator(dataStore).AuthenticateAsync(application.Href, request, cancellationToken);

            throw new InvalidOperationException($"The AuthenticationRequest {request.GetType().Name} is not supported by this implementation.");
        }

        public IAuthenticationResult Authenticate(IInternalDataStore dataStore, IApplication application, IAuthenticationRequest request)
        {
            Validate(dataStore, application, request);

            if (request is UsernamePasswordRequest)
                return new BasicAuthenticator(dataStore).Authenticate(application.Href, request);

            throw new InvalidOperationException($"The AuthenticationRequest {request.GetType().Name} is not supported by this implementation.");
        }
    }
}
