// <copyright file="IRefreshToken.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Application;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Oauth
{
    /// <summary>
    /// Represents an OAuth 2.0 refresh token created in Stormpath.
    /// </summary>
    public interface IRefreshToken :
        IResource,
        IHasTenant,
        IDeletable
    {
        /// <summary>
        /// Gets the string representation of the JSON Web Token.
        /// </summary>
        /// <value>The string representation of the JSON Web Token.</value>
        string Jwt { get; }

        /// <summary>
        /// Gets the <c>href</c> of the associated <see cref="IApplication">Application</see>.
        /// </summary>
        /// <value>The <c>href</c> of the associated <see cref="IApplication">Application</see>.</value>
        string ApplicationHref { get; }

        /// <summary>
        /// Gets the creation date of the token.
        /// </summary>
        /// <value>The creation <see cref="DateTimeOffset"/> of this resource.</value>
        DateTimeOffset CreatedAt { get; }

        /// <summary>
        /// Retrieves the <see cref="IAccount">Account</see> associated with this <see cref="IRefreshToken">RefreshToken</see>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="IAccount">Account</see> associated with this <see cref="IRefreshToken">RefreshToken</see>.</returns>
        Task<IAccount> GetAccountAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves the <see cref="IApplication">Application</see> associated with this <see cref="IRefreshToken">RefreshToken</see>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="IApplication">Application</see> associated with this <see cref="IRefreshToken">RefreshToken</see>.</returns>
        Task<IApplication> GetApplicationAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
