// <copyright file="IAuthenticationResult.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Auth
{
    /// <summary>
    /// Represents the return value of an <see cref="Application.IApplication"/> authentication attempt.
    /// The successfully authenticated account may be obtained via <see cref="GetAccountAsync(CancellationToken)"/>.
    /// </summary>
    public interface IAuthenticationResult
    {
        /// <summary>
        /// Gets whether the authentication was successful.
        /// </summary>
        /// <returns><c>true</c> if the authentication was successful; <c>false</c> otherwise.</returns>
        /// <value>Whether the authentication was successful.</value>
        bool Success { get; }

        /// <summary>
        /// Gets the successfully authenticated <see cref="IAccount"/>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the <see cref="IAccount"/> that was successfully authenticated.</returns>
        Task<IAccount> GetAccountAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
