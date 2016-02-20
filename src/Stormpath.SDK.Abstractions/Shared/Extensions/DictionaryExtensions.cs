// <copyright file="DictionaryExtensions.cs" company="Stormpath, Inc.">
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

using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Shared.Extensions
{
    /// <summary>
    /// Extension methods for working with <see cref="Map"/> instances.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">
        /// When this method returns, the string value associated with the specified key, if the
        /// key is found; otherwise, <see langword="null"/>. This parameter is passed uninitialized.
        /// </param>
        /// <returns><see langword="true"/> if the dictionary contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetValueAsString(this Map source, string key, out string value)
        {
            value = null;
            object raw = null;

            var keyExists = source.TryGetValue(key, out raw);
            if (keyExists)
            {
                value = raw?.ToString();
            }

            return keyExists;
        }
    }
}
