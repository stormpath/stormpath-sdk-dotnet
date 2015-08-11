// <copyright file="CollectionResourceQueryableExtensions.cs" company="Stormpath, Inc.">
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
using System.Linq.Expressions;
using System.Reflection;
using Stormpath.SDK.Resource;

// Placed in the base library namespace so that these extension methods are available without any extra usings
namespace Stormpath
{
    public static class CollectionResourceQueryableFilterExtensions
    {
        public static ICollectionResourceQueryable<T> Filter<T>(this ICollectionResourceQueryable<T> source, string caseInsensitiveMatch)
            where T : IResource
        {
            return (ICollectionResourceQueryable<T>)source.Provider.CreateQuery<T>(
                Expression.Call(
                    ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T)),
                    source.Expression,
                    Expression.Constant(caseInsensitiveMatch)));
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single class", Justification = "Extension methods grouped together")]
    public static class CollectionResourceQueryableExpandExtensions
    {
        public static ICollectionResourceQueryable<T> Expand<T, TKey>(this ICollectionResourceQueryable<T> source, Expression<Func<T, TKey>> keySelector)
            where T : IResource
        {
            return (ICollectionResourceQueryable<T>)source.Provider.CreateQuery<T>(
                Expression.Call(
                    ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T), typeof(TKey)),
                    source.Expression,
                    Expression.Quote(keySelector)));
        }

        public static ICollectionResourceQueryable<T> Expand<T, TKey>(this ICollectionResourceQueryable<T> source, Expression<Func<T, TKey>> keySelector, int? offset = null, int? limit = null)
            where T : IResource
        {
            return (ICollectionResourceQueryable<T>)source.Provider.CreateQuery<T>(
                Expression.Call(
                    ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T), typeof(TKey)),
                    source.Expression,
                    Expression.Quote(keySelector),
                    Expression.Constant(offset, typeof(int?)),
                    Expression.Constant(limit, typeof(int?))));
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single class", Justification = "Extension methods grouped together")]
    public static class CollectionResourceQueryableWithinExtensions
    {
        public static bool Within(this DateTimeOffset field, int year)
        {
            throw new NotSupportedException("Direct calls to Within() are not supported. Use from inside a LINQ Where predicate.");
        }

        public static bool Within(this DateTimeOffset field, int year, int month)
        {
            throw new NotSupportedException("Direct calls to Within() are not supported. Use from inside a LINQ Where predicate.");
        }

        public static bool Within(this DateTimeOffset field, int year, int month, int day)
        {
            throw new NotSupportedException("Direct calls to Within() are not supported. Use from inside a LINQ Where predicate.");
        }

        public static bool Within(this DateTimeOffset field, int year, int month, int day, int hour)
        {
            throw new NotSupportedException("Direct calls to Within() are not supported. Use from inside a LINQ Where predicate.");
        }

        public static bool Within(this DateTimeOffset field, int year, int month, int day, int hour, int minute)
        {
            throw new NotSupportedException("Direct calls to Within() are not supported. Use from inside a LINQ Where predicate.");
        }

        public static bool Within(this DateTimeOffset field, int year, int month, int day, int hour, int minute, int second)
        {
            throw new NotSupportedException("Direct calls to Within() are not supported. Use from inside a LINQ Where predicate.");
        }
    }
}
