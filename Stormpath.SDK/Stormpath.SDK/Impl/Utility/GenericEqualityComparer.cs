// <copyright file="GenericEqualityComparer.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Impl.Utility
{
    internal sealed class GenericEqualityComparerDeprecated<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> equalityAction;
        private readonly Func<T, int> hashAction;

        public GenericEqualityComparerDeprecated(Func<T, T, bool> equalityAction, Func<T, int> hashAction)
        {
            this.equalityAction = equalityAction;
            this.hashAction = hashAction;
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

            var result = this.equalityAction(x, y);
            return result;
        }

        public int GetHashCode(T obj) => this.hashAction(obj);
    }
}
