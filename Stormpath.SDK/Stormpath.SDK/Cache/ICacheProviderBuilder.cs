// <copyright file="ICacheProviderBuilder.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Cache
{
    /// <summary>
    /// A Builder design pattern used to construct <see cref="ICacheProvider"/> instances.
    /// </summary>
    public interface ICacheProviderBuilder
    {
        /// <summary>
        /// Sets the default Time to Live (TTL) for all cache regions managed by the
        /// <see cref="ICacheProvider"/>. You may override this default for individual cache regions
        /// by using the <see cref="WithCache(ICacheConfigurationBuilder)"/> method for each region
        /// you wish to configure.
        /// <para>
        /// Time to Live is the amount of time a cache entry may exist after first being created before it will expire and no
        /// longer be available. If a cache entry ever becomes older than this amount of time (regardless of how often
        /// it is accessed), it will be removed from the cache as soon as possible.
        /// </para>
        /// <para>
        /// If this value is not configured, it is assumed that cache entries could potentially live indefinitely.
        /// Note however that entries can still be expunged due to other conditions (e.g. memory constraints, Time to
        /// Idle setting, etc).
        /// </para>
        /// </summary>
        /// <param name="ttl">Default Time to Live value</param>
        /// <returns>This instance for method chaining.</returns>
        ICacheProviderBuilder WithDefaultTimeToLive(TimeSpan ttl);

        /// <summary>
        /// Sets the default Time to Idle (TTI) for all cache regions managed by the
        /// <see cref="ICacheProvider"/>. You may override this default for individual cache regions by using the
        /// <see cref="WithCache(ICacheConfigurationBuilder)"/> method for each region you wish to configure.
        /// <para>
        /// Time to Idle is the amount of time a cache entry may be idle (unused/not accessed) before it will expire and
        /// no longer be available. If a cache entry is not accessed at all after this amount of time, it will be removed
        /// from the cache as soon as possible.
        /// </para>
        /// <para>
        /// If this value is not configured, it is assumed that cache entries could potentially live indefinitely.
        /// Note however that entries can still be expunged due to other conditions (e.g. memory constraints, Time to
        /// Live setting, etc).
        /// </para>
        /// </summary>
        /// <param name="tti">Default Time To Idle value.</param>
        /// <returns>This instance for method chaining.</returns>
        ICacheProviderBuilder WithDefaultTimeToIdle(TimeSpan tti);

        /// <summary>
        /// Adds configuration settings for a specific cache region managed by the <see cref="ICacheProvider"/>.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="ICacheConfigurationBuilder"/> instance that will
        /// be used to specify a cache's configuration.
        /// </param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="ApplicationException">The cache configuration is not valid.</exception>
        ICacheProviderBuilder WithCache(ICacheConfigurationBuilder builder);

        /// <summary>
        /// Constructs a new <see cref="ICacheProvider"/> instance based on the builder's current configuration state.
        /// </summary>
        /// <returns>A new <see cref="ICacheProvider"/> instance.</returns>
        ICacheProvider Build();
    }
}
