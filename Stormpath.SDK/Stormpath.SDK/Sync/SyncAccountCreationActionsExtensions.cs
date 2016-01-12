// <copyright file="SyncAccountCreationActionsExtensions.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Account;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on <see cref="IAccountCreationActions"/>.
    /// </summary>
    public static class SyncAccountCreationActionsExtensions
    {
        /// <summary>
        /// Synchronously creates a new <see cref="IAccount">Account</see> that may login to this application.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="account">The account to create/persist.</param>
        /// <param name="creationOptionsAction">An inline builder for an instance of <see cref="IAccountCreationOptions"/>,
        /// which will be used when sending the request.</param>
        /// <example>
        /// If you would like to force disabling the backing directory's account registration workflow:
        /// <code>
        /// app.CreateAccount(theAccount, options => options.RegistrationWorkflowEnabled = false);
        /// </code>
        /// </example>
        /// <returns>The persisted account.</returns>
        /// <exception cref="Error.ResourceException">The <see cref="Application.IApplication">Application</see> does not have a dedicated
        ///  <see cref="AccountStore.IAccountStore"/> or if the designated <see cref="AccountStore.IAccountStore"/>
        ///  does not allow new accounts to be created.</exception>
        public static IAccount CreateAccount(this IAccountCreationActions source, IAccount account, Action<AccountCreationOptionsBuilder> creationOptionsAction)
            => (source as IAccountCreationActionsSync).CreateAccount(account, creationOptionsAction);

        /// <summary>
        /// Synchronously creates a new <see cref="IAccount">Account</see> that may login to this application.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="account">The account to create/persist.</param>
        /// <param name="creationOptions">An <see cref="IAccountCreationOptions"/> instance to use when sending the request.</param>
        /// <returns>The persisted account.</returns>
        /// <exception cref="Error.ResourceException">The <see cref="Application.IApplication">Application</see> does not have a dedicated
        ///  <see cref="AccountStore.IAccountStore"/> or if the designated <see cref="AccountStore.IAccountStore"/>
        ///  does not allow new accounts to be created.</exception>
        public static IAccount CreateAccount(this IAccountCreationActions source, IAccount account, IAccountCreationOptions creationOptions)
            => (source as IAccountCreationActionsSync).CreateAccount(account, creationOptions);

        /// <summary>
        /// Synchronously creates a new <see cref="IAccount">Account</see> that may login to this application.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="account">The account to create/persist.</param>
        /// <returns>The persisted account.</returns>
        /// <exception cref="Error.ResourceException">The <see cref="Application.IApplication">Application</see> does not have a dedicated
        ///  <see cref="AccountStore.IAccountStore"/> or if the designated <see cref="AccountStore.IAccountStore"/>
        ///  does not allow new accounts to be created.</exception>
        public static IAccount CreateAccount(this IAccountCreationActions source, IAccount account)
            => (source as IAccountCreationActionsSync).CreateAccount(account);

        /// <summary>
        /// Synchronously creates a new <see cref="IAccount">Account</see> that may login to this application, with the default creation options.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="givenName">The given name (aka 'first name' in Western cultures).</param>
        /// <param name="surname">The surname (aka 'last name' in Western cultures).</param>
        /// <param name="email">The account's email address, which must be unique among all other accounts within a <see cref="Directory.IDirectory">Directory</see>.</param>
        /// <param name="password">The account's raw (plaintext) password.</param>
        /// <returns>The persisted account.</returns>
        /// <exception cref="Error.ResourceException">The <see cref="Application.IApplication">Application</see> does not have a dedicated
        ///  <see cref="AccountStore.IAccountStore"/> or if the designated <see cref="AccountStore.IAccountStore"/>
        ///  does not allow new accounts to be created.</exception>
        public static IAccount CreateAccount(this IAccountCreationActions source, string givenName, string surname, string email, string password)
            => (source as IAccountCreationActionsSync).CreateAccount(givenName, surname, email, password);

        /// <summary>
        /// Synchronously creates a new <see cref="IAccount">Account</see> that may login to this application, with the default creation options.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="givenName">The given name (aka 'first name' in Western cultures).</param>
        /// <param name="surname">The surname (aka 'last name' in Western cultures).</param>
        /// <param name="email">The account's email address, which must be unique among all other accounts within a <see cref="Directory.IDirectory">Directory</see>.</param>
        /// <param name="password">The account's raw (plaintext) password.</param>
        /// <param name="customData">An anonymous type containing name/value pairs to be stored in this account's <see cref="SDK.CustomData.ICustomData"/>.</param>
        /// <returns>The persisted account.</returns>
        /// <exception cref="Error.ResourceException">The <see cref="Application.IApplication">Application</see> does not have a dedicated
        ///  <see cref="AccountStore.IAccountStore"/> or if the designated <see cref="AccountStore.IAccountStore"/>
        ///  does not allow new accounts to be created.</exception>
        public static IAccount CreateAccount(this IAccountCreationActions source, string givenName, string surname, string email, string password, object customData = null)
            => (source as IAccountCreationActionsSync).CreateAccount(givenName, surname, email, password, customData);
    }
}
