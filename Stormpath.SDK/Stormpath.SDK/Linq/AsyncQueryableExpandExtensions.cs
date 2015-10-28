// <copyright file="AsyncQueryableExpandExtensions.cs" company="Stormpath, Inc.">
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
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK
{
    public static class AsyncQueryableExpandExtensions
    {
        public static IAsyncQueryable<TSource> Expand<TSource>(this IAsyncQueryable<TSource> source, Expression<Func<TSource, Func<CancellationToken, Task>>> selector)
            where TSource : IResource
        {
            return source.Provider.CreateQuery(
                LinqHelper.MethodCall(
                    ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TSource)),
                    source.Expression,
                    Expression.Quote(selector)));
        }

        public static IAsyncQueryable<TSource> Expand<TSource>(this IAsyncQueryable<TSource> source, Expression<Func<TSource, Func<IAsyncQueryable>>> selector, int? offset = null, int? limit = null)
            where TSource : IResource
        {
            return source.Provider.CreateQuery(
                LinqHelper.MethodCall(
                    ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TSource)),
                    source.Expression,
                    Expression.Quote(selector),
                    Expression.Constant(offset, typeof(int?)),
                    Expression.Constant(limit, typeof(int?))));
        }
    }
}
