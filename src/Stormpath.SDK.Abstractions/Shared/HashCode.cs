// <copyright file="HashCode.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Shared
{
    /// <summary>
    /// Represents a hash code computed for an object.
    /// </summary>
    /// <seealso href="http://stackoverflow.com/a/18613926/3191599"/>
    public struct HashCode
    {
        private readonly int hashCode;

        private HashCode(int startingValue)
        {
            this.hashCode = startingValue;
        }

        /// <summary>
        /// Creates a new instance of <see cref="HashCode"/>.
        /// </summary>
        public static HashCode Start => new HashCode(17);

        /// <summary>
        /// Converts a <see cref="HashCode"/> instance to an <c>int</c>.
        /// </summary>
        /// <param name="hashCode">The <see cref="HashCode"/> to convert.</param>
        /// <returns>The <c>int</c> value of this <see cref="HashCode"/>.</returns>
        public static implicit operator int(HashCode hashCode) => hashCode.GetHashCode();

        /// <summary>
        /// Fluently adds an object to the computed hash code.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">The object to hash.</param>
        /// <returns>This instance for method chaining.</returns>
        public HashCode Hash<T>(T obj)
        {
            var h = obj != null ? obj.GetHashCode() : 0;
            unchecked
            {
                h += this.hashCode * 31;
            }

            return new HashCode(h);
        }

        /// <inheritdoc/>
        public override int GetHashCode() => this.hashCode;
    }
}