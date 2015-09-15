using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Stormpath.SDK.Impl.Linq
{
    internal static class LinqHelper
    {
        public static Expression MethodCall(MethodInfo method, params Expression[] expressions)
        {
            return Expression.Call(null, method, expressions);
        }

        public static MethodInfo GetMethodInfo<T1, T2>(Func<T1, T2> func, T1 unused)
        {
            return func.Method;
        }

        public static MethodInfo GetMethodInfo<T1, T2, T3>(Func<T1, T2, T3> func, T1 unused1, T2 unused2)
        {
            return func.Method;
        }
    }
}
