using System;
using System.Collections.Generic;
using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.Application;

namespace Stormpath.SDK.Impl.Utility
{
    internal static class PocoTypeLookup
    {
        // TODO: This needs to be removed when refactoring how resources work
        private static readonly IReadOnlyDictionary<Type, Type> ConcreteLookup = new Dictionary<Type, Type>()
        {
            [typeof(IApplicationWebMeExpandConfiguration)] = typeof(DefaultApplicationWebMeExpandConfiguration),
        };

        public static Type GetConcreteType(Type interfaceType)
        {
            Type concrete;
            ConcreteLookup.TryGetValue(interfaceType, out concrete);

            return concrete;
        }
    }
}
