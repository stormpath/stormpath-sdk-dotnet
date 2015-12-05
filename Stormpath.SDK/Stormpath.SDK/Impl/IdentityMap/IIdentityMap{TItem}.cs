// <copyright file="IIdentityMap{TItem}.cs" company="Stormpath, Inc.">
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

using System;

namespace Stormpath.SDK.Impl.IdentityMap
{
    /// <summary>
    /// Represents a mapping between keys and object references.
    /// </summary>
    /// <typeparam name="TItem">The type of stored objects.</typeparam>
    internal interface IIdentityMap<TItem> : IDisposable
        where TItem : class
    {
        /// <summary>
        /// Gets the number of items added in the entire lifetime of the identity map.
        /// </summary>
        /// <value>The total number of items added.</value>
        long LifetimeItemsAdded { get; }

        /// <summary>
        /// Gets or adds an object reference to the identity map. If an item with the specified <paramref name="key"/>
        /// already exists, it is returned immediately. If no item is found, a new item is created from <paramref name="itemFactory"/>
        /// and stored.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="itemFactory">A delegate that creates a new item.</param>
        /// <param name="storeInfinitely">Determines whether this item has an infinite expiration.</param>
        /// <returns>The existing item, or the result of <paramref name="itemFactory"/> if no item was found..</returns>
        TItem GetOrAdd(string key, Func<TItem> itemFactory, bool storeInfinitely);
    }
}
