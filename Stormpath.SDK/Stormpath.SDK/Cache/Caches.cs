// <copyright file="Caches.cs" company="Stormpath, Inc.">
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

using System;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Cache
{
    /// <summary>
    /// Static entry point for working with <see cref="ICache">Cache</see> objects.
    /// </summary>
    public static class Caches
    {
        /// <summary>
        /// Instantiates a new <see cref="ICacheProviderBuilder"/> suitable for single-instance applications. If your application
        /// is deployed on multiple instances (e.g. for a distributed/clustered web application), you might not want to use this method
        /// and instead implement the <see cref="ICacheProvider">Cache Provider</see> API directly to use your distributed/clustered cache technology of choice.
        /// </summary>
        /// <returns>A new <see cref="ICacheProviderBuilder"/> suitable for single-instance applications.</returns>
        [Obsolete("Use CacheProviders.Create().InMemoryCache()")]
        public static ICacheProviderBuilder NewInMemoryCacheProvider()
            => new InMemoryCacheProviderBuilder();

        /// <summary>
        /// Instantiates a new <see cref="ICacheProvider">Cache Provider</see> that disables caching entirely.  While production applications
        /// will usually enable a working cache provider, you might configure a disabled cache provider for
        /// your Client when testing or debugging to remove 'moving parts' for better clarity into request/response behavior.
        /// </summary>
        /// <returns>A new disabled <see cref="ICacheProvider">Cache Provider</see> instance.</returns>
        [Obsolete("Use CacheProviders.Create().DisabledCache()")]
        public static ICacheProvider NewDisabledCacheProvider()
            => new NullCacheProvider();

        /// <summary>
        /// Returns a new <see cref="ICacheConfigurationBuilder"/> to configure a cache region that will store data
        /// for instances of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The resource type to store in this cache region.</typeparam>
        /// <returns>A new <see cref="ICacheConfigurationBuilder"/> to configure a cache region that will store data
        /// for instances of type <typeparamref name="T"/>.</returns>
        public static ICacheConfigurationBuilder ForResource<T>()
            where T : IResource
            => Named(typeof(T).Name);

        private static ICacheConfigurationBuilder Named(string name)
            => new DefaultCacheConfigurationBuilder(name);
    }
}
