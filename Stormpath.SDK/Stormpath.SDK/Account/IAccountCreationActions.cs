// <copyright file="IAccountCreationActions.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;

namespace Stormpath.SDK.Account
{
    /// <summary>
    /// Represents common Account creation actions.
    /// </summary>
    public interface IAccountCreationActions
    {
        /// <summary>
        /// Creates a new <see cref="IAccount">Account</see> that may login to the <see cref="Application.IApplication">Application</see> or <see cref="Organization.IOrganization">Organization</see>.
        /// </summary>
        /// <param name="account">The account to create/persist.</param>
        /// <param name="creationOptionsAction">An inline builder for an instance of <see cref="IAccountCreationOptions"/>,
        /// which will be used when sending the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <example>
        /// If you would like to force disabling the backing directory's account registration workflow:
        /// <code>
        /// await IApplication.CreateAccountAsync(theAccount, options => options.RegistrationWorkflowEnabled = false, cancellationToken);
        /// </code>
        /// </example>
        /// <returns>The persisted account.</returns>
        /// <exception cref="Error.ResourceException">The <see cref="Application.IApplication">Application</see> or <see cref="Organization.IOrganization">Organization</see> does not have a dedicated
        ///  <see cref="AccountStore.IAccountStore"/> or if the designated <see cref="AccountStore.IAccountStore"/> does not allow new accounts to be created.</exception>
        Task<IAccount> CreateAccountAsync(IAccount account, Action<AccountCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new <see cref="IAccount">Account</see> that may login to the <see cref="Application.IApplication">Application</see> or <see cref="Organization.IOrganization">Organization</see>.
        /// </summary>
        /// <param name="account">The account to create/persist.</param>
        /// <param name="creationOptions">An <see cref="IAccountCreationOptions"/> instance to use when sending the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The persisted account.</returns>
        /// <exception cref="Error.ResourceException">The <see cref="Application.IApplication">Application</see> or <see cref="Organization.IOrganization">Organization</see> does not have a dedicated
        ///  <see cref="AccountStore.IAccountStore"/> or if the designated <see cref="AccountStore.IAccountStore"/> does not allow new accounts to be created.</exception>
        Task<IAccount> CreateAccountAsync(IAccount account, IAccountCreationOptions creationOptions, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new <see cref="IAccount">Account</see> that may login to the <see cref="Application.IApplication">Application</see> or <see cref="Organization.IOrganization">Organization</see>.
        /// </summary>
        /// <param name="account">The account to create/persist.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The persisted account.</returns>
        /// <exception cref="Error.ResourceException">The <see cref="Application.IApplication">Application</see> or <see cref="Organization.IOrganization">Organization</see> does not have a dedicated
        ///  <see cref="AccountStore.IAccountStore"/> or if the designated <see cref="AccountStore.IAccountStore"/> does not allow new accounts to be created.</exception>
        Task<IAccount> CreateAccountAsync(IAccount account, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new enabled <see cref="IAccount">Account</see> that may login to the <see cref="Application.IApplication">Application</see> or <see cref="Organization.IOrganization">Organization</see>,
        /// with the default creation options.
        /// </summary>
        /// <param name="givenName">The given name (aka 'first name' in Western cultures).</param>
        /// <param name="surname">The surname (aka 'last name' in Western cultures).</param>
        /// <param name="email">The account's email address, which must be unique among all other accounts within a <see cref="Directory.IDirectory">Directory</see>.</param>
        /// <param name="password">The account's raw (plaintext) password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The persisted account.</returns>
        /// <exception cref="Error.ResourceException">The <see cref="Application.IApplication">Application</see> or <see cref="Organization.IOrganization">Organization</see> does not have a dedicated
        ///  <see cref="AccountStore.IAccountStore"/> or if the designated <see cref="AccountStore.IAccountStore"/> does not allow new accounts to be created.</exception>
        Task<IAccount> CreateAccountAsync(string givenName, string surname, string email, string password, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new enabled <see cref="IAccount">Account</see> that may login to the <see cref="Application.IApplication">Application</see> or <see cref="Organization.IOrganization">Organization</see>,
        /// with the default creation options and the specified <see cref="CustomData.ICustomData">Custom Data</see>.
        /// </summary>
        /// <param name="givenName">The given name (aka 'first name' in Western cultures).</param>
        /// <param name="surname">The surname (aka 'last name' in Western cultures).</param>
        /// <param name="email">The account's email address, which must be unique among all other accounts within a <see cref="Directory.IDirectory">Directory</see>.</param>
        /// <param name="password">The account's raw (plaintext) password.</param>
        /// <param name="customData">An anonymous type containing name/value pairs to be stored in this account's <see cref="CustomData.ICustomData">Custom Data</see>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The persisted account.</returns>
        /// <exception cref="Error.ResourceException">The <see cref="Application.IApplication">Application</see> or <see cref="Organization.IOrganization">Organization</see> does not have a dedicated
        ///  <see cref="AccountStore.IAccountStore"/> or if the designated <see cref="AccountStore.IAccountStore"/> does not allow new accounts to be created.</exception>
        Task<IAccount> CreateAccountAsync(string givenName, string surname, string email, string password, object customData, CancellationToken cancellationToken = default(CancellationToken));
    }
}
