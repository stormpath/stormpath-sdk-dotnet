// <copyright file="IAccountResult.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;

namespace Stormpath.SDK.IdSite
{
    /// <summary>
    /// Represents the result of an ID Site callback redirect, obtained after the end-user has
    /// used the ID Site and has been returned to an application's <c>callbackUri</c>.
    /// </summary>
    public interface IAccountResult
    {
        /// <summary>
        /// Gets any original application-specific state that was applied when the user was redirected to the ID Site, or
        /// <see langword="null"/> if no state was specified.
        /// </summary>
        /// <seealso cref="Application.IApplication.NewIdSiteUrlBuilder"/>
        /// <seealso cref="IIdSiteUrlBuilder.SetState(string)"/>
        /// <value>Application-specific state that was applied to the ID Site redirect, or <see langword="null"/> if no state data was set.</value>
        string State { get; }

        /// <summary>
        /// Gets a value indicating whether the account returned by <see cref="GetAccountAsync(CancellationToken)"/> was
        /// newly created (registered) on the ID Site, or was an existing account that logged in successfully.
        /// </summary>
        /// <value><see langword="true"/> if the returned <see cref="IAccount"/> was registered on the ID Site;
        /// <see langword="false"/> if the account was an existing account that logged in successfully.</value>
        bool IsNewAccount { get; }

        /// <summary>
        /// Gets the status of the ID Site invocation (registered, authenticated, logged out).
        /// </summary>
        /// <value>The status of the ID Site invocation.</value>
        IdSiteResultStatus Status { get; }

        /// <summary>
        /// Returns the user account that either logged in or was created as a result of registration on the ID Site.
        /// You can determine if the account is newly registered if <see cref="IsNewAccount"/> is <see langword="true"/>.
        /// If <see cref="IsNewAccount"/> is <see langword="false"/>, the account reflects a previously-registered user
        /// that has logged in.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user's <see cref="IAccount"/> resource.</returns>
        /// <exception cref="System.ApplicationException">The account is not present.</exception>
        Task<IAccount> GetAccountAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
