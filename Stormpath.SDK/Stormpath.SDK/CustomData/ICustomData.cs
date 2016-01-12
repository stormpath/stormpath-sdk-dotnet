// <copyright file="ICustomData.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.CustomData
{
    /// <summary>
    /// A dictionary resource embedded within another resource
    /// (such as an <see cref="Account.IAccount">Account</see>, <see cref="Group.IGroup">Group</see>, or <see cref="Directory.IDirectory">Directory</see>)
    /// that allows you to store arbitrary name/value pairs.
    /// </summary>
    public interface ICustomData : IResource, ISaveable<ICustomData>, IDeletable, IAuditable, IEnumerable<KeyValuePair<string, object>>
    {
        /// <summary>
        /// Gets the total number of items in this custom data resource.
        /// </summary>
        /// <value>The total number of items.</value>
        int Count { get; }

        /// <summary>
        /// Looks up a custom data value by key.
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        /// <returns>The value associated with <paramref name="key"/>, or <see langword="null"/> if the key does not exist.</returns>
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
        /// <returns><see langword="true"/> if an item with the <paramref name="key"/> exists; <see langword="false"/> otherwise.</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Gets the custom data item with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The name (key) of the custom data item to find.</param>
        /// <returns>The value associated with <paramref name="key"/> if it exists; <see langword="null"/> otherwise.</returns>
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
        /// <returns>The value of the removed item if it exists; <see langword="null"/> otherwise.</returns>
        object Remove(string key);

        /// <summary>
        /// Attempts to get the custom data item with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The name (key) of the custom data item to find.</param>
        /// <param name="value">If found, this <c>out</c> parameter will be set to the found value.</param>
        /// <returns><see langword="true"/> if the custom data item exists; <see langword="false"/> otherwise.</returns>
        bool TryGetValue(string key, out object value);

        /// <summary>
        /// Determines whether any user-defined custom data items have been added.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if only the default (system-generated) items exist;
        /// <see langword="false"/> if at least one user-defined custom data item has been added.
        /// </returns>
        bool IsEmptyOrDefault();
    }
}
