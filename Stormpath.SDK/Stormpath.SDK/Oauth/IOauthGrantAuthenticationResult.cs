// <copyright file="IOauthGrantAuthenticationResult.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Oauth
{
    /// <summary>
    /// Represents the result of a successful Password or Refresh Grant request.
    /// </summary>
    public interface IOauthGrantAuthenticationResult
    {
        /// <summary>
        /// Gets the string representation of the OAuth 2.0 Access Token created during the Create Grant Authentication operation.
        /// </summary>
        /// <value>The string representation of the OAuth 2.0 Access Token.</value>
        string AccessTokenString { get; }

        /// <summary>
        /// Gets the <c>href</c> of the token created during the Create Grant Authentication operation.
        /// </summary>
        /// <value>The <c>href</c> of the token.</value>
        string AccessTokenHref { get; }

        /// <summary>
        /// Gets the string representation of the OAuth 2.0 Refresh Token created during the Refresh Grant Authentication operation.
        /// </summary>
        /// <value>The string representation of the OAuth 2.0 Refresh Token.</value>
        string RefreshTokenString { get; }

        /// <summary>
        /// Gets the type of the token created during the Create Grant Authentication or Refresh Grant Authentication operations.
        /// </summary>
        /// <value>The type of the token created.</value>
        string TokenType { get; }

        /// <summary>
        /// Gets the lifetime (in seconds) of the access token.
        /// </summary>
        /// <value>
        /// The lifetime (in seconds) of the access token. For example, the value <c>3600</c>denotes that the access token will expire one hour after the response was generated.
        /// </value>
        long ExpiresIn { get; }

        /// <summary>
        /// Retrieves the <see cref="IAccessToken">Access Token</see> created during the Create Grant Authentication operation.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="IAccessToken">Access Token</see>.</returns>
        Task<IAccessToken> GetAccessTokenAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
