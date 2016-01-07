// <copyright file="IClient.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Cache;
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Jwt;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Client
{
    /// <summary>
    /// The main entry point into the Stormpath C# SDK. To communicate with the Stormpath REST API from your project,
    /// you must first build a <see cref="IClient"/> instance. After obtaining an instance, the REST API may be
    /// used by making simple calls on objects returned from the <see cref="IClient"/> (or any child objects).
    /// </summary>
    /// <threadsafety instance="true"/>
    public interface IClient : ITenantActions, IDataStore
    {
        /// <summary>
        /// Gets the sole <see cref="ITenant"/> associated to this <see cref="IClient"/>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="ITenant"/> associated to this client.</returns>
        Task<ITenant> GetCurrentTenantAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the <see cref="ICacheProvider"/> associated with this client.
        /// </summary>
        /// <returns>The cache provider associated with this client.</returns>
        ICacheProvider GetCacheProvider();

        /// <summary>
        /// Creates a new <see cref="IJwtBuilder">JWT Builder</see>, which can create <see cref="IJwt">JSON Web Tokens</see>.
        /// </summary>
        /// <returns>A new <see cref="IJwtBuilder"/> instance.</returns>
        IJwtBuilder NewJwtBuilder();

        /// <summary>
        /// Creates a new <see cref="IJwtParser">JWT Parser</see>, which can parse JSON Web Token strings.
        /// </summary>
        /// <returns>A new <see cref="IJwtParser"/> instance.</returns>
        IJwtParser NewJwtParser();
    }
}
