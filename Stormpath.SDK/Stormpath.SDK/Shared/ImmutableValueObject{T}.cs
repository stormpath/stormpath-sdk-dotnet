// <copyright file="ImmutableValueObject{T}.cs" company="Stormpath, Inc.">
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
using System.Reflection;
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Shared
{
    // Adapted from an example at http://www.mechonomics.com/generic-value-object-equality-updated/

    /// <summary>
    /// Base class for simple immutable value objects.
    /// </summary>
    /// <typeparam name="T">The value object type.</typeparam>
    public abstract class ImmutableValueObject<T> : IEquatable<T>
        where T : ImmutableValueObject<T>
    {
        private readonly Func<T, T, bool> customEqualityFunction;

        private static Func<ImmutableValueObject<T>, ImmutableValueObject<T>, bool> DefaultEqualityFunction =>
            (a, b) =>
            {
                if (b == null)
                {
                    return false;
                }

                var type = a.GetType();
                var otherType = b.GetType();
                if (type != otherType)
                {
                    return false;
                }

                var fields = GetFields(a);

                foreach (var field in fields)
                {
                    var value1 = field.GetValue(a);
                    var value2 = field.GetValue(b);

                    if (value1 == null && value2 == null)
                    {
                        return true;
                    }

                    if (value1 == null && value2 != null)
                    {
                        return false;
                    }

                    if (!value1.Equals(value2))
                    {
                        return false;
                    }
                }

                return true;
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableValueObject{T}"/> class.
        /// </summary>
        public ImmutableValueObject()
        {
            this.customEqualityFunction = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableValueObject{T}"/> class given an equality function.
        /// </summary>
        /// <param name="equalityFunction">The delegate to use when checking value equality.</param>
        public ImmutableValueObject(Func<T, T, bool> equalityFunction)
        {
            this.customEqualityFunction = equalityFunction;
        }

        /// <summary>
        /// Compares two <see cref="ImmutableValueObject{T}"/> instances for value equality.
        /// </summary>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns><see langword="true"/> if the instances have equal values; <see langword="false"/> otherwise.</returns>
        public static bool operator ==(ImmutableValueObject<T> x, ImmutableValueObject<T> y)
            => EqualsImpl(x as T, y as T);

        /// <summary>
        /// Compares two <see cref="ImmutableValueObject{T}"/> instances for value inequality.
        /// </summary>
        /// <param name="x">The left operand.</param>
        /// <param name="y">The right operand.</param>
        /// <returns><see langword="true"/> if the instances do not have equal values; <see langword="true"/> otherwise.</returns>
        public static bool operator !=(ImmutableValueObject<T> x, ImmutableValueObject<T> y)
            => !(x == y);

        /// <inheritdoc/>
        public override bool Equals(object obj)
            => EqualsImpl(this as T, obj as T);

        /// <inheritdoc/>
        public virtual bool Equals(T other)
            => EqualsImpl(this as T, other);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var fields = GetFields(this);
            var hash = HashCode.Start;

            foreach (var field in fields)
            {
                var value = field.GetValue(this);
                if (value != null)
                {
                    hash = hash.Hash(value);
                }
            }

            return hash;
        }

        private static bool EqualsImpl(T x, T y)
        {
            bool areSameReference = ReferenceEquals(x, y);
            if (areSameReference)
            {
                return true;
            }

            bool eitherIsNull = ReferenceEquals(x, null) || ReferenceEquals(y, null);
            if (eitherIsNull)
            {
                return false;
            }

            if (x.customEqualityFunction != null)
            {
                return x.customEqualityFunction(x, y);
            }

            return DefaultEqualityFunction(x, y);
        }

        private static IEnumerable<FieldInfo> GetFields(object obj)
        {
            var baseType = typeof(ImmutableValueObject<T>);
            var type = obj.GetType();

            var fields = new List<FieldInfo>();

            while (type != baseType)
            {
                fields.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

                type = type.BaseType;
            }

            return fields;
        }
    }
}
