// <copyright file="StringExtensions.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;

namespace Stormpath.SDK.Shared.Extensions
{
    /// <summary>
    /// Extension methods for working with strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns <c>null</c> if the string is empty.
        /// </summary>
        /// <param name="source">The string.</param>
        /// <returns>The string, or <see langword="null"/> if the string is null or empty.</returns>
        public static string Nullable(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            return source;
        }

        /// <summary>
        /// Joins strings using a separator.
        /// </summary>
        /// <param name="source">A source of strings.</param>
        /// <param name="separator">The separator</param>
        /// <returns>The joined string.</returns>
        public static string Join(this IEnumerable<string> source, string separator)
        {
            return string.Join(separator, source);
        }
    }
}
