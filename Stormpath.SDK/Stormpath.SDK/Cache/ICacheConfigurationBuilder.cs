// <copyright file="ICacheConfigurationBuilder.cs" company="Stormpath, Inc.">
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
    /// A Builder design pattern used to construct <see cref="ICacheConfiguration"/> instances.
    /// </summary>
    public interface ICacheConfigurationBuilder
    {
        /// <summary>
        /// Sets the associated <see cref="ICache{K, V}"/> region's entry Time to Live (TTL).
        /// <para>
        /// Time to Live is the amount of time a cache entry may exist after first being created before it will expire and no
        /// longer be available. If a cache entry ever becomes older than this amount of time (regardless of how often
        /// it is accessed), it will be removed from the cache as soon as possible.
        /// </para>
        /// <para>
        /// If this value is not configured, it is assumed that the Cache's entries could potentially live indefinitely.
        /// Note however that entries can still be expunged due to other conditions (e.g. memory constraints, Time to
        /// Idle setting, etc).
        /// </para>
        /// </summary>
        /// <param name="ttl">The entry Time to Live for this cache region.</param>
        /// <returns>This instance for method chaining.</returns>
        ICacheConfigurationBuilder WithTimeToLive(TimeSpan ttl);

        /// <summary>
        /// Sets the associated <see cref="ICache{K, V}"/> region's entry Time to Idle (TTI).
        /// <para>
        /// Time to Idle is the amount of time a cache entry may be idle (unused/not accessed) before it will expire and
        /// no longer be available. If a cache entry is not accessed at all after this amount of time, it will be removed
        /// from the cache as soon as possible.
        /// </para>
        /// <para>
        /// If this value is not configured, it is assumed that the Cache's entries could potentially live indefinitely.
        /// Note however that entries can still be expunged due to other conditions (e.g.memory constraints, Time to
        /// Live setting, etc).
        /// </para>
        /// </summary>
        /// <param name="tti">The entry Time to Idle for this cache region.</param>
        /// <returns>This instance for method chaining.</returns>
        ICacheConfigurationBuilder WithTimeToIdle(TimeSpan tti);

        /// <summary>
        /// Constructs a new <see cref="ICacheConfiguration"/> instance based on the builder's current state.
        /// </summary>
        /// <returns>A new <see cref="ICacheConfiguration"/> instance.</returns>
        ICacheConfiguration Build();
    }
}
