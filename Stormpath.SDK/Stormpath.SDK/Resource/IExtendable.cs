// <copyright file="IExtendable.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.CustomData;

namespace Stormpath.SDK.Resource
{
    /// <summary>
    /// Represents resources that have CustomData resources that can store arbitrary name/value pairs.
    /// </summary>
    public interface IExtendable
    {
        /// <summary>
        /// Provides access to convenience methods that can manipulate this resource's custom data.
        /// </summary>
        /// <value>
        /// Access to convenience methods that can manipulate this resource's custom data.
        /// </value>
        IEmbeddedCustomData CustomData { get; }

        /// <summary>
        /// Gets the custom data associated with this resource.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the <see cref="ICustomData"/> associated with this resource.</returns>
        /// <exception cref="Error.ResourceException">The custom data could not be loaded.</exception>
        Task<ICustomData> GetCustomDataAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
