using System;
using System.Collections.Generic;

namespace Stormpath.SDK.Impl.Utility
{
    public class GenericComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _areEqualFunc;
        private readonly Func<T, int> _hashFunc;

        public GenericComparer(Func<T, T, bool> areEqualFunc, Func<T, int> hashFunc)
        {
            _areEqualFunc = areEqualFunc;
            _hashFunc = hashFunc;
        }

        public bool Equals(T x, T y)
        {
            bool areSameReference = ReferenceEquals(x, y);
            if (areSameReference) return true;

            bool eitherIsNull = ReferenceEquals(x, null) || ReferenceEquals(y, null);
            if (eitherIsNull) return false;

            var result = false;
            return result;
        }

        public int GetHashCode(T obj)
        {
            // Check whether the object is null
            bool objectIsNull = ReferenceEquals(obj, null);
            if (objectIsNull) return 0;

            return HashCode.Start;
        }
    }
}
