// <copyright file="ICustomData.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.CustomData
{
    /// <summary>
    /// A dictionary resource embedded within another resource
    /// (such as an <see cref="Account.IAccount"/>, <see cref="Group.IGroup"/>, or <see cref="Directory.IDirectory"/>)
    /// that allows you to store arbitrary name/value pairs.
    /// </summary>
    public interface ICustomData : IResource, ISaveable<ICustomData>, IDeletable, IAuditable, IEnumerable<KeyValuePair<string, object>>
    {
        object this[string key] { get; set; }

        /// <summary>
        /// Removes all custom data items when this custom data resource is saved.
        /// <para>To delete immediately, use <see cref="IDeletable.DeleteAsync(System.Threading.CancellationToken)"/>.</para>
        /// </summary>
        void Clear();

        /// <summary>
        /// Determines whether the <see cref="ICustomData"/> contains an
        /// item with the specified name (key).
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns><c>true</c> if an item with the <paramref name="key"/> exists; <c>false</c> otherwise.</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Gets the custom data item with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The name (key) of the custom data item to find.</param>
        /// <returns>The value associated with <paramref name="key"/> if it exists; <c>null</c> otherwise.</returns>
        object Get(string key);

        /// <summary>
        /// Adds a new custom data item.
        /// <para>You must call <see cref="ISaveable{T}.SaveAsync(System.Threading.CancellationToken)"/> to save the changes.</para>
        /// </summary>
        /// <param name="key">The custom data name (key). This must be unique for this resource.</param>
        /// <param name="value">The custom data value.</param>
        void Put(string key, object value);

        /// <summary>
        /// Adds one or more new custom data items.
        /// <para>You must call <see cref="ISaveable{T}.SaveAsync(System.Threading.CancellationToken)"/> to save the changes.</para>
        /// </summary>
        /// <param name="values">The items to add.</param>
        void Put(IEnumerable<KeyValuePair<string, object>> values);

        /// <summary>
        /// Adds one or more new custom data items.
        /// <para>You must call <see cref="ISaveable{T}.SaveAsync(System.Threading.CancellationToken)"/> to save the changes.</para>
        /// </summary>
        /// <param name="customData">An anonymous type containing the items to add.</param>
        void Put(object customData);

        /// <summary>
        /// Adds a new custom data item.
        /// <para>You must call <see cref="ISaveable{T}.SaveAsync(System.Threading.CancellationToken)"/> to save the changes.</para>
        /// </summary>
        /// <param name="item">The custom data item to add.</param>
        void Put(KeyValuePair<string, object> item);

        /// <summary>
        /// Removes a custom data item.
        /// <para>You must call <see cref="ISaveable{T}.SaveAsync(System.Threading.CancellationToken)"/> to save the changes.</para>
        /// </summary>
        /// <param name="key">The name (key) of the custom data item to remove.</param>
        /// <returns>The value of the removed item if it exists; <c>null</c> otherwise.</returns>
        object Remove(string key);

        /// <summary>
        /// Attempts to get the custom data item with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The name (key) of the custom data item to find.</param>
        /// <param name="value">If found, this <c>out</c> parameter will be set to the found value.</param>
        /// <returns><c>true</c> if the custom data item exists; <c>false</c> otherwise.</returns>
        bool TryGetValue(string key, out object value);

        IReadOnlyCollection<string> Keys { get; }

        IReadOnlyCollection<object> Values { get; }

        /// <summary>
        /// Determines whether any user-defined custom data items have been added.
        /// </summary>
        /// <returns>
        /// <c>true</c> if only the default (system-generated) items exist;
        /// <c>false</c> if at least one user-defined custom data item has been added.
        /// </returns>
        bool IsEmptyOrDefault();
    }
}
