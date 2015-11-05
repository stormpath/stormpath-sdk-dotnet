// <copyright file="StringExtensions.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Text;

namespace Stormpath.SDK.Impl.Extensions
{
    internal static class StringExtensions
    {
        public static string Nullable(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return null;
            return source;
        }

        public static string ToBase64(this string source, Encoding encoding)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return Convert.ToBase64String(encoding.GetBytes(source));
        }

        /// <summary>
        /// Decodes strings encoded with either <see cref="ToBase64(string, Encoding)"/>
        /// or <see cref="ToUrlSafeBase64(string, Encoding)"/>.
        /// </summary>
        /// <param name="base64Source">Base64-encoded value.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The decoded string.</returns>
        public static string FromBase64(this string base64Source, Encoding encoding)
        {
            if (base64Source == null)
                throw new ArgumentNullException(nameof(base64Source));

            base64Source = base64Source
                .Replace('-', '+')
                .Replace('_', '/');

            switch (base64Source.Length % 4)
            {
                case 0: break;
                case 2: base64Source += "=="; break;
                case 3: base64Source += "="; break;
                default:
                    throw new ArgumentException("Illegal base64 string.");
            }

            return encoding.GetString(Convert.FromBase64String(base64Source));
        }

        public static string ToUrlSafeBase64(this string source, Encoding encoding)
        {
            var base64 = source.ToBase64(encoding);

            // Remove trailing '='s
            base64 = base64.Split('=')[0];

            // Replace illegal characters
            base64 = base64
                .Replace('+', '-')
                .Replace('/', '_');

            return base64;
        }

        public static KeyValuePair<string, string> SplitToKeyValuePair(this string source, char separator)
        {
            if (string.IsNullOrEmpty(source) || !source.Contains(separator.ToString()))
                throw new FormatException($"Input string is not a '{separator}'-separated string.");

            var pair = source.Split(separator);
            if (pair.Length != 2)
                throw new FormatException($"Input string is not a key-value pair.");

            return new KeyValuePair<string, string>(pair[0], pair[1]);
        }

        public static string Join(this IEnumerable<string> source, string separator)
        {
            return string.Join(separator, source);
        }
    }
}
