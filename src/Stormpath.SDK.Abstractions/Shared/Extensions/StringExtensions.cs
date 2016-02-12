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
    public static class StringExtensions
    {
        public static string Nullable(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            return source;
        }

        public static int? ToInt32(this string source)
        {
            int result;

            if (string.IsNullOrEmpty(source)
                || !int.TryParse(source, out result))
            {
                return null;
            }

            return result;
        }

        public static KeyValuePair<string, string> SplitToKeyValuePair(this string source, char separator)
        {
            if (string.IsNullOrEmpty(source) || !source.Contains(separator.ToString()))
            {
                throw new FormatException($"Input string is not a '{separator}'-separated string.");
            }

            var pair = source.Split(separator);
            if (pair.Length != 2)
            {
                throw new FormatException($"Input string is not a key-value pair.");
            }

            return new KeyValuePair<string, string>(pair[0], pair[1]);
        }

        public static string Join(this IEnumerable<string> source, string separator)
        {
            return string.Join(separator, source);
        }
    }
}
