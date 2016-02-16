// <copyright file="CacheProviders.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.Cache;

namespace Stormpath.SDK.Cache
{
    /// <summary>
    /// Static entry point for creating <see cref="ICacheProvider">cache provider</see> instances.
    /// </summary>
    public static class CacheProviders
    {
        /// <summary>
        /// Gets a new <see cref="ICacheProviderFactory">factory</see> instance.
        /// </summary>
        /// <returns>A new <see cref="ICacheProviderFactory">factory</see> instance.</returns>
        public static ICacheProviderFactory Create()
            => new DefaultCacheProviderFactory();
    }
}
