// <copyright file="StringEnumeration.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Shared
{
    public abstract class StringEnumeration : IComparable
    {
        private readonly string value;

        protected StringEnumeration()
        {
        }

        protected StringEnumeration(string value)
        {
            this.value = value;
        }

        public string Value => this.value;

        public override string ToString() => this.Value;

        public static implicit operator string(StringEnumeration @enum) => @enum.Value;

        public static bool operator ==(StringEnumeration a, StringEnumeration b)
        {
            if ((object)a == null && (object)b == null)
            {
                return true;
            }

            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.Value.Equals(b.value, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool operator !=(StringEnumeration a, StringEnumeration b) => !(a == b);

        public override bool Equals(object obj)
        {
            var otherValue = obj as StringEnumeration;

            if (otherValue == null)
            {
                return false;
            }

            var typeMatches = this.GetType().Equals(obj.GetType());
            var valueMatches = this.value.Equals(otherValue.Value, StringComparison.InvariantCultureIgnoreCase);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode() => this.value.GetHashCode();

        public int CompareTo(object other) => this.Value.CompareTo(((StringEnumeration)other).Value);
    }
}
