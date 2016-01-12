// <copyright file="IAccountCreationActionsSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;

namespace Stormpath.SDK.Impl.Account
{
    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on <see cref="IAccountCreationActions"/>.
    /// </summary>
    internal interface IAccountCreationActionsSync
    {
        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountCreationActions.CreateAccountAsync(IAccount, Action{AccountCreationOptionsBuilder}, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="account">The account to create/persist.</param>
        /// <param name="creationOptionsAction">An inline builder for an instance of <see cref="IAccountCreationOptions"/>,
        /// which will be used when sending the request.</param>
        /// <returns>The persisted account.</returns>
        IAccount CreateAccount(IAccount account, Action<AccountCreationOptionsBuilder> creationOptionsAction);

        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountCreationActions.CreateAccountAsync(IAccount, IAccountCreationOptions, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="account">The account to create/persist.</param>
        /// <param name="creationOptions">An <see cref="IAccountCreationOptions"/> instance to use when sending the request.</param>
        /// <returns>The persisted account.</returns>
        IAccount CreateAccount(IAccount account, IAccountCreationOptions creationOptions);

        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountCreationActions.CreateAccountAsync(IAccount, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="account">The account to create/persist.</param>
        /// <returns>The persisted account.</returns>
        IAccount CreateAccount(IAccount account);

        /// <summary>
        /// Synchronous counterpart to <see cref="IAccountCreationActions.CreateAccountAsync(string, string, string, string, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="givenName">The given name (aka 'first name' in Western cultures).</param>
        /// <param name="surname">The surname (aka 'last name' in Western cultures).</param>
        /// <param name="email">The account's email address, which must be unique among all other accounts within a <see cref="SDK.Directory.IDirectory"/>.</param>
        /// <param name="password">The account's raw (plaintext) password.</param>
        /// <param name="customData">An anonymous type containing name/value pairs to be stored in this account's <see cref="SDK.CustomData.ICustomData"/>.</param>
        /// <returns>The persisted account.</returns>
        IAccount CreateAccount(string givenName, string surname, string email, string password, object customData = null);
    }
}
