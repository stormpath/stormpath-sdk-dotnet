// <copyright file="ConcurrentDictionaryExtensions.cs" company="Stormpath, Inc.">
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
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Stormpath.SDK.Impl.Extensions
{
    internal static class ConcurrentDictionaryExtensions
    {
        /// <summary>
        /// Atomic conditional removal from a <see cref="ConcurrentDictionary{TKey, TValue}"/>.
        /// Only removes <paramref name="key"/> if its value matches <paramref name="value"/>.
        /// </summary>
        /// <remarks>
        /// Based on code by <see href="http://blogs.msdn.com/b/pfxteam/archive/2011/04/02/10149222.aspx">Stephen Toub</see>.
        /// </remarks>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dictionary">The source dictionary.</param>
        /// <param name="key">The key to attempt to remove.</param>
        /// <param name="value">The value to conditionally match.</param>
        /// <returns><see langword="true"/> if <paramref name="key"/> was removed from the dictionary.</returns>
        public static bool TryRemove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary)
                .Remove(new KeyValuePair<TKey, TValue>(key, value));
        }
    }
}
