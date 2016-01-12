// <copyright file="RedisCaches.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Cache.Redis
{
    /// <summary>
    /// Static entry point for working with Redis cache providers.
    /// </summary>
    public static class RedisCaches
    {
        /// <summary>
        /// Instantiates a new <see cref="IRedisCacheProviderBuilder"/>, used to build Redis-backed cache providers.
        /// </summary>
        /// <returns>A new <see cref="IRedisCacheProviderBuilder"/> instance.</returns>
        public static IRedisCacheProviderBuilder NewRedisCacheProvider()
            => new RedisCacheProviderBuilder();
    }
}
