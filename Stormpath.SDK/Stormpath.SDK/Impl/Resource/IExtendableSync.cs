// <copyright file="IExtendableSync.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.CustomData;

namespace Stormpath.SDK.Impl.Resource
{
    /// <summary>
    /// Represents resources that have CustomData resources that can store arbitrary name/value pairs.
    /// </summary>
    internal interface IExtendableSync
    {
        /// <summary>
        /// Gets a proxy that can be used to manipulate the Custom Data for this resource.
        /// </summary>
        /// <seealso cref="GetCustomData"/>
        /// <value>
        /// A proxy that can be used to manipulate the Custom Data for this resource.
        /// </value>
        IEmbeddedCustomData CustomData { get; }

        /// <summary>
        /// Synchronous counterpart to <see cref="SDK.Resource.IExtendable.GetCustomDataAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The <see cref="ICustomData"/> associated with this resource.</returns>
        ICustomData GetCustomData();
    }
}
