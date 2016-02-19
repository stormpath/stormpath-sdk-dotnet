// <copyright file="RedisCacheProviderFactoryExtensions.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Cache.Redis;

namespace Stormpath.SDK.Cache
{
    /// <summary>
    /// Provides access to the RedisCache plugin by plugging into <see cref="ICacheProviderFactory"/>.
    /// </summary>
    public static class RedisCacheProviderFactoryExtensions
    {
        /// <summary>
        /// Instantiates a new <see cref="IRedisCacheProviderBuilder"/>, used to build Redis-backed cache providers.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <returns>A new <see cref="IRedisCacheProviderBuilder"/> instance.</returns>
        public static IRedisCacheProviderBuilder RedisCache(this ICacheProviderFactory factory)
            => new RedisCacheProviderBuilder();
    }
}
