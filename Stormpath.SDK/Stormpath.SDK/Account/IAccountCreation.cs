// <copyright file="IAccountCreation.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Account
{
    public interface IAccountCreation
    {
        /// <summary>
        /// Creates a new <see cref="IAccount"/> that may login to this application.
        /// </summary>
        /// <param name="account">The account to create/persist.</param>
        /// <param name="creationOptionsAction">An inline builder for an instance of <see cref="IAccountCreationOptions"/>, which will be used when buidling the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <example>
        /// If you would like to force disabling the backing directory's account registration workflow:
        /// <code>await IApplication.CreateAccountAsync(theAccount, options => options.RegistrationWorkflowEnabled = false, cancellationToken);</code>
        /// </example>
        /// <returns>A Task whose result is the persisted account.</returns>
        /// <exception cref="Error.ResourceException">The <see cref="Application.IApplication"/> does not have a dedicated
        ///  <see cref="AccountStore.IAccountStore"/> or if the designated <see cref="AccountStore.IAccountStore"/>
        ///  does not allow new accounts to be created.</exception>
        Task<IAccount> CreateAccountAsync(IAccount account, Action<AccountCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new <see cref="IAccount"/> that may login to this application.
        /// </summary>
        /// <param name="account">The account to create/persist.</param>
        /// <param name="creationOptions">An <see cref="IAccountCreationOptions"/> instance to use when building the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the persisted account.</returns>
        /// <exception cref="Error.ResourceException">The <see cref="Application.IApplication"/> does not have a dedicated
        ///  <see cref="AccountStore.IAccountStore"/> or if the designated <see cref="AccountStore.IAccountStore"/>
        ///  does not allow new accounts to be created.</exception>
        Task<IAccount> CreateAccountAsync(IAccount account, IAccountCreationOptions creationOptions, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new <see cref="IAccount"/> that may login to this application.
        /// </summary>
        /// <param name="account">The account to create/persist.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the persisted account.</returns>
        /// <exception cref="Error.ResourceException">The <see cref="Application.IApplication"/> does not have a dedicated
        ///  <see cref="AccountStore.IAccountStore"/> or if the designated <see cref="AccountStore.IAccountStore"/>
        ///  does not allow new accounts to be created.</exception>
        Task<IAccount> CreateAccountAsync(IAccount account, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new <see cref="IAccount"/> that may login to this application.
        /// This convenience overload creates a request with the default options.
        /// </summary>
        /// <param name="givenName">The given name (aka 'first name' in Western cultures).</param>
        /// <param name="surname">The surname (aka 'last name' in Western cultures).</param>
        /// <param name="email">The account's email address, which must be unique among all other accounts within a <see cref="IDirectory"/>.</param>
        /// <param name="password">The account's raw (plaintext) password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the persisted account.</returns>
        /// <exception cref="Error.ResourceException">The <see cref="Application.IApplication"/> does not have a dedicated
        ///  <see cref="AccountStore.IAccountStore"/> or if the designated <see cref="AccountStore.IAccountStore"/>
        ///  does not allow new accounts to be created.</exception>
        Task<IAccount> CreateAccountAsync(string givenName, string surname, string email, string password, CancellationToken cancellationToken = default(CancellationToken));
    }
}
