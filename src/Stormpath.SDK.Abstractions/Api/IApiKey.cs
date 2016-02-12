// <copyright file="IApiKey.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Api
{
    /// <summary>
    /// An API Key is a secure random username/password pair attributed to an <see cref="IAccount">Account</see>
    /// that can be used by the account to make secure requests to an API service.
    /// </summary>
    /// <remarks>
    /// An <see cref="IAccount">Account</see> may have zero or more API Keys, and allowing multiple keys is often useful for key
    /// rotation strategies. For example, a new key can be generated while an existing key is in use. Applications can
    /// then reference the new key (e.g., on startup), and once running, the old key can then be deleted. This allows for
    /// key rotation without an interruption in service, which would happen otherwise if an old key was invalidated the
    /// instant a new key was generated.
    /// </remarks>
    public interface IApiKey :
        IResource,
        ISaveable<IApiKey>,
        IDeletable,
        IHasTenant
    {
        /// <summary>
        /// Gets the ApiKey ID that uniquely identifies this ApiKey among all others.
        /// </summary>
        /// <value>The API Key ID that uniquely identifies this API Key among all other.</value>
        string Id { get; }

        /// <summary>
        /// Gets the raw secret used for API authentication.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A very secret, very private value that should never be disclosed to anyone
        /// other than the actual account holder. The secret value is mostly used for computing HMAC digests, but can also
        /// be used as a password for password-based key derivation and encryption.
        /// </para>
        /// </remarks>
        /// <value>The API Key plaintext secret.</value>
        string Secret { get; }

        /// <summary>
        /// Gets the API Key status.
        /// </summary>
        /// <remarks>
        /// API Keys that are not <see cref="ApiKeyStatus.Enabled">Enabled</see> cannot be used to authenticate requests.
        /// API Keys are enabled by default when they are created.
        /// </remarks>
        /// <value>The API Key status.</value>
        ApiKeyStatus Status { get; }

        /// <summary>
        /// Sets the API Key status.
        /// </summary>
        /// <remarks>
        /// API Keys that are not <see cref="ApiKeyStatus.Enabled">Enabled</see> cannot be used to authenticate requests.
        /// API Keys are enabled by default when they are created.
        /// <para>
        /// You <b>must</b> call <c>SaveAsync</c> after updating this property; changes do not take effect until
        /// the resource is saved to the Stormpath API.
        /// </para>
        /// </remarks>
        /// <param name="status">The status.</param>
        void SetStatus(ApiKeyStatus status);

        /// <summary>
        /// Gets the <see cref="IAccount">Account</see> to which the API Key belongs.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="IAccount">Account</see> to which the API Key belongs.</returns>
        Task<IAccount> GetAccountAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
