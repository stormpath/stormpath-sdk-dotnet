// <copyright file="GenericHasher.cs" company="Stormpath, Inc.">
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

using System;

namespace Stormpath.SDK.Impl.Utility
{
    internal sealed class GenericHasherDeprecated<T>
    {
        private readonly Func<T, int> hashAction;

        public GenericHasherDeprecated(Func<T, int> hashAction)
        {
            this.hashAction = hashAction;
        }

        public Func<T, int> HashFunction =>
            t =>
            {
                if (!Valid(t))
                    return 0;

                return hashAction(t);
            };

        public static implicit operator Func<T, int>(GenericHasherDeprecated<T> hasher)
        {
            return hasher.HashFunction;
        }

        public int GetHashCode(T obj)
        {
            return HashFunction(obj);
        }

        private bool Valid(T obj)
        {
            bool objectIsNull = ReferenceEquals(obj, null);
            return !objectIsNull;
        }
    }
}
