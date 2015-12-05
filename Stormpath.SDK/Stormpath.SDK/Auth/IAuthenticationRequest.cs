// <copyright file="IAuthenticationRequest.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Auth
{
    /// <summary>
    /// The default implementation of <see cref="IAuthenticationRequest{P, C}"/> that uses <c>string</c>s
    /// for both principals and credentials.
    /// </summary>
    public interface IAuthenticationRequest : IAuthenticationRequest<string, string>
    {
    }

    /// <summary>
    /// An authentication request represents all necessary information to authenticate a specific account.
    /// </summary>
    /// <typeparam name="P">The principals type.</typeparam>
    /// <typeparam name="C">The credentials type.</typeparam>
    public interface IAuthenticationRequest<P, C>
    {
        /// <summary>
        /// Gets the principal(s) (identifying information) that reflects the specific Account to be authenticated. For example, a username or email address.
        /// </summary>
        /// <value>The account's principals.</value>
        P Principals { get; }

        /// <summary>
        /// Gets the credentials (information that proves authenticity) of the specific Account to be authenticated. For example, a password.
        /// </summary>
        /// <value>The account's credentials.</value>
        C Credentials { get; }

        /// <summary>
        /// Gets the account store that should process this request, or <see langword="null"/> if the application's default
        /// account store authentication flow should execute. If non-null, the account store must be assigned
        /// to the application making the request.
        /// </summary>
        /// <value>The account store that should process this request, or <see langword="null"/> if the application's default
        /// account store authentication flow should execute.</value>
        IAccountStore AccountStore { get; }
    }
}
