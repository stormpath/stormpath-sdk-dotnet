// <copyright file="IAsynchronousCacheProvider.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Cache
{
    /// <summary>
    /// Provides and maintains the lifecycles of <see cref="IAsynchronousCache"/> instances.
    /// </summary>
    public interface IAsynchronousCacheProvider : ICacheProvider
    {
        /// <summary>
        /// Acquires the cache with the specified <code>name</code>. If a cache does not yet exist with that name,
        /// a new one will be created with that name and returned.
        /// </summary>
        /// <param name="name">The name of the cache to acquire.</param>
        /// <returns>The cache with the given name.</returns>
        /// <exception cref="System.ApplicationException">The cache provider has been disposed.</exception>
        /// <exception cref="System.ApplicationException">An asynchronous path is not supported.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="name"/> is null or empty.</exception>
        IAsynchronousCache GetAsyncCache(string name);
    }
}
