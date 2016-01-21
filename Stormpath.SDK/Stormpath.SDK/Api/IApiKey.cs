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

        ApiKeyStatus Status { get; }

        void SetStatus(ApiKeyStatus status);

        Task<IAccount> GetAccountAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
