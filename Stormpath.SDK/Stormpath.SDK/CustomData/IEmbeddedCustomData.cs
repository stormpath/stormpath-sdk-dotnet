// <copyright file="IEmbeddedCustomData.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;

namespace Stormpath.SDK.CustomData
{
    /// <summary>
    /// Provides access to convenience methods that manipulate the custom data of the parent resource.
    /// </summary>
    public interface IEmbeddedCustomData
    {
        /// <summary>
        /// Removes all custom data items when the resource is saved.
        /// <para>Note: Does not take effect until the parent resource is saved using <see cref="Resource.ISaveable{T}.SaveAsync(System.Threading.CancellationToken)"/>!</para>
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds a new custom data item when the resource is saved.
        /// </summary>
        /// <param name="key">The custom data name (key). This must be unique for this resource.</param>
        /// <param name="value">The custom data value.</param>
        void Put(string key, object value);

        /// <summary>
        /// Adds one or more new custom data items when the resource is saved.
        /// <para>Note: Does not take effect until the parent resource is saved using <see cref="Resource.ISaveable{T}.SaveAsync(System.Threading.CancellationToken)"/>!</para>
        /// </summary>
        /// <param name="values">The items to add.</param>
        void Put(IEnumerable<KeyValuePair<string, object>> values);

        /// <summary>
        /// Adds a new custom data item when the resource is saved.
        /// <para>Note: Does not take effect until the parent resource is saved using <see cref="Resource.ISaveable{T}.SaveAsync(System.Threading.CancellationToken)"/>!</para>
        /// </summary>
        /// <param name="item">The custom data item to add.</param>
        void Put(KeyValuePair<string, object> item);

        /// <summary>
        /// Adds one or more new custom data items when the resource is saved.
        /// <para>Note: Does not take effect until the parent resource is saved using <see cref="Resource.ISaveable{T}.SaveAsync(System.Threading.CancellationToken)"/>!</para>
        /// </summary>
        /// <param name="customData">An anonymous type containing the items to add.</param>
        void Put(object customData);

        /// <summary>
        /// Removes a custom data item when the resource is saved.
        /// <para>Note: Does not take effect until the parent resource is saved using <see cref="Resource.ISaveable{T}.SaveAsync(System.Threading.CancellationToken)"/>!</para>
        /// </summary>
        /// <param name="key">The name (key) of the custom data item to remove.</param>
        void Remove(string key);
    }
}
