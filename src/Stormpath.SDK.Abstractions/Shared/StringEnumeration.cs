// <copyright file="StringEnumeration.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Shared
{
    /// <summary>
    /// Represents an enumeration backed by a string value.
    /// </summary>
    public abstract class StringEnumeration : IComparable
    {
        private readonly string value;

        private StringEnumeration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringEnumeration"/> class given a backing value.
        /// </summary>
        /// <param name="value">The enumeration value.</param>
        protected StringEnumeration(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the value of this enumeration member.
        /// </summary>
        /// <value>The enumeration value.</value>
        public string Value => this.value;

        /// <inheritdoc/>
        public override string ToString() => this.Value;

        /// <summary>
        /// Gets the <see cref="string"/> value of a <see cref="StringEnumeration"/> instance.
        /// </summary>
        /// <param name="enum">The instance.</param>
        /// <returns>The string value.</returns>
        public static implicit operator string(StringEnumeration @enum) => @enum.Value;

        /// <summary>
        /// Compares two <see cref="StringEnumeration"/> instances for value equality.
        /// </summary>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns><see langword="true"/> if the instances have equal values; <see langword="false"/> otherwise.</returns>
        public static bool operator ==(StringEnumeration x, StringEnumeration y)
        {
            if ((object)x == null && (object)y == null)
            {
                return true;
            }

            if ((object)x == null || (object)y == null)
            {
                return false;
            }

            return x.Value.Equals(y.value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Compares two <see cref="StringEnumeration"/> instances for value inequality.
        /// </summary>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns><see langword="true"/> if the instances do not have equal values; <see langword="true"/> otherwise.</returns>
        public static bool operator !=(StringEnumeration x, StringEnumeration y) => !(x == y);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var otherValue = obj as StringEnumeration;

            if (otherValue == null)
            {
                return false;
            }

            var typeMatches = this.GetType().Equals(obj.GetType());
            var valueMatches = this.value.Equals(otherValue.Value, StringComparison.OrdinalIgnoreCase);

            return typeMatches && valueMatches;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => this.value.GetHashCode();

        /// <inheritdoc/>
        /// <param name="other">The object to compare to.</param>
        public int CompareTo(object other)
            => string.Compare(Value, ((StringEnumeration) other).Value, StringComparison.OrdinalIgnoreCase);
    }
}
