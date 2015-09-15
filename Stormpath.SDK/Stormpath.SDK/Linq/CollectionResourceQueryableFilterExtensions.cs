// <copyright file="CollectionResourceQueryableFilterExtensions.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Linq.Expressions;
using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Resource;

// Placed in the base library namespace so that these extension methods are available without any extra usings
namespace Stormpath.SDK
{
    public static class CollectionResourceQueryableFilterExtensions
    {
        public static IAsyncQueryable<TSource> Filter<TSource>(this IAsyncQueryable<TSource> source, string caseInsensitiveMatch)
            where TSource : IResource
        {
            return source.Provider.CreateQuery(
                LinqHelper.MethodCall(
                    LinqHelper.GetMethodInfo(Filter, (IQueryable<TSource>)null, caseInsensitiveMatch),
                    source.Expression,
                    Expression.Constant(caseInsensitiveMatch)));
        }

        private static IQueryable<TSource> Filter<TSource>(IQueryable<TSource> source, string caseInsensitiveMatch)
        {
            // todo remove
            throw new NotImplementedException("This method stub is just a proxy that is inserted into the expression tree.");
        }
    }
}
