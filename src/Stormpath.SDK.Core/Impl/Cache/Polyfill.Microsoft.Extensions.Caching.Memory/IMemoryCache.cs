// <copyright file="IMemoryCache.cs" company="Stormpath, Inc.">
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
//
// Contains code modified from aspnet/Caching. Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

#if NET45 || NET451
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Cache.Polyfill.Microsoft.Extensions.Caching.Memory
{
    internal interface IMemoryCache : IDisposable
    {
        /// <summary>
        /// Create or overwrite an entry in the cache.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        /// <param name="value">The value to be cached.</param>
        /// <param name="options">The <see cref="MemoryCacheEntryOptions"/>.</param>
        /// <returns>The object that was cached.</returns>
        object Set(object key, object value, MemoryCacheEntryOptions options);

        /// <summary>
        /// Gets the item associated with this key if present.
        /// </summary>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <param name="value">The located value or null.</param>
        /// <returns>True if the key was found.</returns>
        bool TryGetValue(object key, out object value);

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        void Remove(object key);
    }
}
#endif