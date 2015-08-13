// <copyright file="GenericComparer.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;

namespace Stormpath.SDK.Impl.Utility
{
    internal sealed class GenericComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> areEqualFunc;
        private readonly Func<T, int> hashFunc;

        public GenericComparer(Func<T, T, bool> areEqualFunc, Func<T, int> hashFunc)
        {
            this.areEqualFunc = areEqualFunc;
            this.hashFunc = hashFunc;
        }

        public bool Equals(T x, T y)
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

            var result = areEqualFunc(x, y);
            return result;
        }

        public int GetHashCode(T obj)
        {
            // Check whether the object is null
            bool objectIsNull = ReferenceEquals(obj, null);
            if (objectIsNull)
            {
                return 0;
            }

            return hashFunc(obj);
        }
    }
}
