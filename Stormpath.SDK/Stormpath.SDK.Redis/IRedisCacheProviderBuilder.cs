// <copyright file="IRedisCacheProviderBuilder.cs" company="Stormpath, Inc.">
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

using StackExchange.Redis;

namespace Stormpath.SDK.Cache.Redis
{
    /// <summary>
    /// Represents a <see cref="ICacheProviderBuilder"/> that can construct
    /// Redis-backed <see cref="ICacheProvider"/> instances.
    /// </summary>
    public interface IRedisCacheProviderBuilder : ICacheProviderBuilder
    {
        /// <summary>
        /// Sets the Redis connection parameters.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>This instance for method chaining.</returns>
        IRedisCacheProviderBuilder WithRedisConnection(string connectionString);

        /// <summary>
        /// Sets the Redis connection parameters.
        /// </summary>
        /// <param name="connection">The existing Redis connection.</param>
        /// <returns>This instance for method chaining.</returns>
        IRedisCacheProviderBuilder WithRedisConnection(IConnectionMultiplexer connection);
    }
}
