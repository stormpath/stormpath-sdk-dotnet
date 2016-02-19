// <copyright file="IOauthAuthenticatorSync.cs" company="Stormpath, Inc.">
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


namespace Stormpath.SDK.Impl.Oauth
{
    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on <see cref="IOauthAuthenticator{TRequest, TResult}"/>.
    /// </summary>
    /// <typeparam name="TRequest">The request kind that the authenticator will accept.</typeparam>
    /// <typeparam name="TResult">The response kind that the authenticator will return.</typeparam>
    internal interface IOauthAuthenticatorSync<TRequest, TResult>
    {
        /// <summary>
        /// Synchronous counterpart to <see cref="IOauthAuthenticator{TRequest, TResult}.AuthenticateAsync(TRequest, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="authenticationRequest">The Authentication Request this authenticator will attempt.</param>
        /// <returns>An Authentication Result representing the successful authentication.</returns>
        /// <exception cref="SDK.Error.ResourceException">The authentication failed.</exception>
        TResult Authenticate(TRequest authenticationRequest);
    }
}
