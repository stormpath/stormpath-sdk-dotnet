// <copyright file="Base64.cs" company="Stormpath, Inc.">
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
using System.Text;

namespace Stormpath.SDK.Impl.Utility
{
    internal static class Base64
    {
        public static string Encode(string plaintext, Encoding encoding)
        {
            if (plaintext == null)
                throw new ArgumentNullException(nameof(plaintext));

            return Encode(encoding.GetBytes(plaintext));
        }

        public static string Encode(byte[] plaintextBytes)
            => Convert.ToBase64String(plaintextBytes);

        public static string EncodeUrlSafe(string plaintext, Encoding encoding)
        {
            if (plaintext == null)
                throw new ArgumentNullException(nameof(plaintext));

            return EncodeUrlSafe(encoding.GetBytes(plaintext));
        }

        public static string EncodeUrlSafe(byte[] plaintextBytes)
        {
            var encoded = Encode(plaintextBytes);

            // Remove trailing '='s
            encoded = encoded.Split('=')[0];

            // Replace illegal characters
            encoded = encoded
                .Replace('+', '-')
                .Replace('/', '_');

            return encoded;
        }

        /// <summary>
        /// Decodes strings encoded with either Encode() EncodeUrlSafe().
        /// </summary>
        /// <param name="encoded">Base64-encoded value.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The decoded string.</returns>
        public static string Decode(string encoded, Encoding encoding)
        {
            if (encoded == null)
                throw new ArgumentNullException(nameof(encoded));

            encoded = encoded
                .Replace('-', '+')
                .Replace('_', '/');

            switch (encoded.Length % 4)
            {
                case 0: break;
                case 2: encoded += "=="; break;
                case 3: encoded += "="; break;
                default:
                    throw new ArgumentException("Illegal base64 string.");
            }

            return encoding.GetString(Convert.FromBase64String(encoded));
        }
    }
}
