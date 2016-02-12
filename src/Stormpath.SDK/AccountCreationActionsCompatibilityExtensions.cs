// <copyright file="AccountCreationActionsCompatibilityExtensions.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Account;

namespace Stormpath.SDK
{
    public static class AccountCreationActionsCompatibilityExtensions
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
        public static Task<IAccount> CreateAccountAsync(this IAccountCreationActions accountCreationActions, IAccount account, Action<AccountCreationOptionsBuilder> creationOptionsAction, CancellationToken cancellationToken = default(CancellationToken))
        {
            var @this = accountCreationActions as IAccountCreationActionsInternal;

            return AccountCreationActionsShared.CreateAccountAsync(@this.GetInternalAsyncDataStore(), @this.Accounts.Href, account, creationOptionsAction, cancellationToken);
        }
    }
}
