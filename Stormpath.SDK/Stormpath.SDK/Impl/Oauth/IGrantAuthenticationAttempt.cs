// <copyright file="IGrantAuthenticationAttempt.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Oauth
{
    /// <summary>
    /// Represents the information required to build a Grant Authentication request.
    /// </summary>
    internal interface IGrantAuthenticationAttempt : IResource
    {
        /// <summary>
        /// Gets the <see cref="IAccountStore">Account Store</see> <c>href</c> that will used for the token exchange request.
        /// </summary>
        /// <value>The <see cref="IAccountStore">Account Store</see> <c>href</c> that will used for the token exchange request.</value>
        string AccountStoreHref { get; }

        /// <summary>
        /// Gets the grant type that will used for the token exchange request.
        /// </summary>
        /// <value>The grant type that will used for the token exchange request.</value>
        string GrantType { get; }

        /// <summary>
        /// Gets the username to be used in the token exchange request.
        /// </summary>
        /// <value>The username to be used in the token exchange request.</value>
        string Login { get; }

        /// <summary>
        /// Gets the password to be used in the token exchange request.
        /// </summary>
        /// <value>The password to be used in the token exchange request.</value>
        string Password { get; }

        /// <summary>
        /// Sets the <see cref="IAccountStore">Account Store</see> that will be used for the token exchange request.
        /// </summary>
        /// <param name="accountStore">The Account Store.</param>
        void SetAccountStore(IAccountStore accountStore);

        /// <summary>
        /// Sets the <see cref="IAccountStore">Account Store</see> <c>href</c> that will be used for the token exchange request.
        /// </summary>
        /// <param name="accountStoreHref">The Account Store <c>href</c>.</param>
        void SetAccountStore(string accountStoreHref);

        /// <summary>
        /// Sets the Authentication Grant Type that will be used for the token exchange request.
        /// </summary>
        /// <remarks>Currently only the <c>password</c> grant type is supported for this operation.</remarks>
        /// <param name="grantType">The grant type.</param>
        void SetGrantType(string grantType);

        /// <summary>
        /// Sets the username to be used in the token exchange request.
        /// </summary>
        /// <param name="login">The username.</param>
        void SetLogin(string login);

        /// <summary>
        /// Sets the password to be used in the token exchange request.
        /// </summary>
        /// <param name="password">The password.</param>
        void SetPassword(string password);
    }
}