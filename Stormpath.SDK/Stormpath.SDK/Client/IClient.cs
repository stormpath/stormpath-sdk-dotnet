// <copyright file="IClient.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Client
{
    /// <summary>
    /// The main entry point into the Stormpath C# SDK. To communicate with the Stormpath REST API from your project,
    /// you must first build a <see cref="IClient"/> instance. After obtaining an instance, the REST API may be
    /// used by making simple calls on objects returned from the <see cref="IClient"/> (or any child objects).
    /// </summary>
    public interface IClient : ITenantActions, IDataStore
    {
        /// <summary>
        /// Gets the sole <see cref="ITenant"/> associated to this <see cref="IClient"/>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the <see cref="ITenant"/> associated to this client.</returns>
        Task<ITenant> GetCurrentTenantAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
