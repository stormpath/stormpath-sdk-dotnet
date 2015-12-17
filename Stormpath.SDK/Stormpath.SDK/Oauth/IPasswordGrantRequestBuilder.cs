// <copyright file="IPasswordGrantRequestBuilder.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.AccountStore;

namespace Stormpath.SDK.Oauth
{
    /// <summary>
    /// Builder pattern used to construct <see cref="IPasswordGrantRequestBuilder"/> instances.
    /// </summary>
    public interface IPasswordGrantRequestBuilder : IOauthAuthenticationRequestBuilder<IPasswordGrantRequest>
    {
        /// <summary>
        /// Sets the username that will be used to create the authentication token.
        /// </summary>
        /// <param name="login">The username that will be used to create the authentication token.</param>
        /// <returns>This instance for method chaining.</returns>
        IPasswordGrantRequestBuilder SetLogin(string login);

        /// <summary>
        /// Sets the password that will be used to create the authentication token.
        /// </summary>
        /// <param name="password">The account's plaintext password.</param>
        /// <returns>This instance for method chaining.</returns>
        IPasswordGrantRequestBuilder SetPassword(string password);

        /// <summary>
        /// Sets the target <see cref="IAccountStore">Account Store</see> to be used for the request.
        /// </summary>
        /// <param name="accountStore">The specific <see cref="IAccountStore">Account Store</see> that will be queried using the provided credentials.</param>
        /// <returns>This instance for method chaining.</returns>
        IPasswordGrantRequestBuilder SetAccountStore(IAccountStore accountStore);

        /// <summary>
        /// Sets the target <see cref="IAccountStore">Account Store</see> <c>href</c> to be used for the request.
        /// </summary>
        /// <param name="accountStoreHref">The specific <see cref="IAccountStore">Account Store</see> that will be queried using the provided credentials.</param>
        /// <returns>This instance for method chaining.</returns>
        IPasswordGrantRequestBuilder SetAccountStore(string accountStoreHref);
    }
}
