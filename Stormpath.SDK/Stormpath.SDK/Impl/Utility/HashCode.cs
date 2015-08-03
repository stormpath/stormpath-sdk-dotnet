// <copyright file="HashCode.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

namespace Stormpath.SDK.Impl.Utility
{
    // Borrowed from Şafak Gür (http://stackoverflow.com/a/18613926/3191599)
    // Helper extension that allows hashcodes to be calcualated fluently
    internal struct HashCode
    {
        private readonly int hashCode;

        public HashCode(int hashCode)
        {
            this.hashCode = hashCode;
        }

        public static HashCode Start
        {
            get { return new HashCode(17); }
        }

        public static implicit operator int(HashCode hashCode)
        {
            return hashCode.GetHashCode();
        }

        public HashCode Include(int existingHash)
        {
            unchecked
            {
                existingHash += hashCode * 31;
            }

            return new HashCode(existingHash);
        }

        public HashCode Hash<T>(T obj)
        {
            var h = obj != null ? obj.GetHashCode() : 0;
            unchecked
            {
                h += hashCode * 31;
            }

            return new HashCode(h);
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}