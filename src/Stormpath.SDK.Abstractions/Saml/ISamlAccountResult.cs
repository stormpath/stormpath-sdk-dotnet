// <copyright file="ISamlAccountResult.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;

namespace Stormpath.SDK.Saml
{
    /// <summary>
    /// Represents the result of a SAML Identity Provider redirect, obtained after the end-user has
    /// interacted with the external site and has been returned to an application's <c>callbackUri</c>.
    /// </summary>
    public interface ISamlAccountResult
    {
        /// <summary>
        /// Gets any original application-specific state that was applied when the user was redirected to the SAML Identity Provider, or
        /// <see langword="null"/> if no state was specified.
        /// </summary>
        /// <seealso cref="Application.IApplication.NewSamlIdpUrlBuilderAsync(CancellationToken)"/>
        /// <seealso cref="ISamlIdpUrlBuilder.SetState(string)"/>
        /// <value>Application-specific state that was applied to the SAML redirect, or <see langword="null"/> if no state data was set.</value>
        string State { get; }

        /// <summary>
        /// Gets a value indicating whether the account returned by <see cref="GetAccountAsync(CancellationToken)"/> was
        /// newly created (registered) on the SAML Identity Provider, or was an existing account that logged in successfully.
        /// </summary>
        /// <value><see langword="true"/> if the returned <see cref="IAccount">Account</see> was registered on the SAML Identity Provider;
        /// <see langword="false"/> if the account was an existing account that logged in successfully.</value>
        bool IsNewAccount { get; }

        /// <summary>
        /// Gets the status of the SAML invocation (authenticated, logged out).
        /// </summary>
        /// <value>The status of the SAML invocation.</value>
        SamlResultStatus Status { get; }

        /// <summary>
        /// Gets the user account returned by the SAML Identity Provider.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user's <see cref="IAccount">Account</see> resource.</returns>
        /// <exception cref="System.Exception">The account is not present.</exception>
        Task<IAccount> GetAccountAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
