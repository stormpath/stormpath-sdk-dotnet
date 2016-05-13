// <copyright file="CacheExtensions.cs" company="Stormpath, Inc.">
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

#if NET45
namespace Stormpath.SDK.Impl.Cache.Polyfill.Microsoft.Extensions.Caching.Memory
{
    internal static class CacheExtensions
    {
        public static object Get(this IMemoryCache cache, object key)
        {
            object value = null;
            cache.TryGetValue(key, out value);
            return value;
        }

        public static TItem Get<TItem>(this IMemoryCache cache, object key)
        {
            TItem value;
            cache.TryGetValue<TItem>(key, out value);
            return value;
        }

        public static bool TryGetValue<TItem>(this IMemoryCache cache, object key, out TItem value)
        {
            object obj = null;
            if (cache.TryGetValue(key, out obj))
            {
                value = (TItem)obj;
                return true;
            }
            value = default(TItem);
            return false;
        }

        public static object Set(this IMemoryCache cache, object key, object value)
        {
            return cache.Set(key, value, new MemoryCacheEntryOptions());
        }

        public static object Set(this IMemoryCache cache, object key, object value, MemoryCacheEntryOptions options)
        {
            return cache.Set(key, value, options);
        }

        public static TItem Set<TItem>(this IMemoryCache cache, object key, TItem value)
        {
            return (TItem)cache.Set(key, (object)value, new MemoryCacheEntryOptions());
        }

        public static TItem Set<TItem>(this IMemoryCache cache, object key, TItem value, MemoryCacheEntryOptions options)
        {
            return (TItem)cache.Set(key, (object)value, options);
        }
    }
}
#endif