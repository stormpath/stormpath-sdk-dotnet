// <copyright file="AsyncQueryableExtensions.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Linq;

// Placed in the base library namespace so that these extension methods are available without any extra usings
namespace Stormpath.SDK
{
    public static class AsyncQueryableExtensions
    {
        public static IOrderedAsyncQueryable<TSource> OrderBy<TSource, TKey>(this IAsyncQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            return (IOrderedAsyncQueryable<TSource>)source.Provider.CreateQuery(
                LinqHelper.MethodCall(
                    LinqHelper.GetMethodInfo(Queryable.OrderBy, (IQueryable<TSource>)null, keySelector),
                    source.Expression,
                    Expression.Quote(keySelector)));
        }

        public static IOrderedAsyncQueryable<TSource> OrderByDescending<TSource, TKey>(this IAsyncQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            return (IOrderedAsyncQueryable<TSource>)source.Provider.CreateQuery(
                LinqHelper.MethodCall(
                    LinqHelper.GetMethodInfo(Queryable.OrderByDescending, (IQueryable<TSource>)null, keySelector),
                    source.Expression,
                    Expression.Quote(keySelector)));
        }

        public static IOrderedAsyncQueryable<TSource> ThenBy<TSource, TKey>(this IAsyncQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            return (IOrderedAsyncQueryable<TSource>)source.Provider.CreateQuery(
                LinqHelper.MethodCall(
                    LinqHelper.GetMethodInfo(Queryable.ThenBy, (IOrderedQueryable<TSource>)null, keySelector),
                    source.Expression,
                    Expression.Quote(keySelector)));
        }

        public static IOrderedAsyncQueryable<TSource> ThenByDescending<TSource, TKey>(this IAsyncQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            return (IOrderedAsyncQueryable<TSource>)source.Provider.CreateQuery(
                LinqHelper.MethodCall(
                    LinqHelper.GetMethodInfo(Queryable.ThenByDescending, (IOrderedQueryable<TSource>)null, keySelector),
                    source.Expression,
                    Expression.Quote(keySelector)));
        }

        public static IAsyncQueryable<TSource> Skip<TSource>(this IAsyncQueryable<TSource> source, int count)
        {
            return source.Provider.CreateQuery(
                LinqHelper.MethodCall(
                    LinqHelper.GetMethodInfo(Queryable.Skip, (IQueryable<TSource>)null, count),
                    source.Expression,
                    Expression.Constant(count)));
        }

        public static IAsyncQueryable<TSource> Take<TSource>(this IAsyncQueryable<TSource> source, int count)
        {
            return source.Provider.CreateQuery(
                LinqHelper.MethodCall(
                    LinqHelper.GetMethodInfo(Queryable.Take, (IQueryable<TSource>)null, count),
                    source.Expression,
                    Expression.Constant(count)));
        }

        public static IAsyncQueryable<TSource> Where<TSource>(this IAsyncQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return source.Provider.CreateQuery(
                LinqHelper.MethodCall(
                    LinqHelper.GetMethodInfo(Queryable.Where, (IQueryable<TSource>)null, predicate),
                    source.Expression,
                    Expression.Quote(predicate)));
        }

        public static async Task<int> CountAsync<T>(this IAsyncQueryable<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            var collection = source as CollectionResourceQueryable<T>;
            if (collection == null)
                throw new InvalidOperationException("This queryable is not a supported collection resource.");

            if (!await source.MoveNextAsync(cancellationToken).ConfigureAwait(false))
                return 0;

            return collection.Size;
        }

        public static async Task<T> FirstAsync<T>(this IAsyncQueryable<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!await source.MoveNextAsync(cancellationToken).ConfigureAwait(false))
                throw new InvalidOperationException("The sequence has no elements.");

            return source.CurrentPage.First();
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this IAsyncQueryable<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!await source.MoveNextAsync(cancellationToken).ConfigureAwait(false))
                return default(T);

            return source.CurrentPage.FirstOrDefault();
        }

        public static async Task<T> SingleAsync<T>(this IAsyncQueryable<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!await source.MoveNextAsync(cancellationToken).ConfigureAwait(false))
                throw new InvalidOperationException("The sequence has no elements.");

            return source.CurrentPage.Single();
        }

        public static async Task<T> SingleOrDefaultAsync<T>(this IAsyncQueryable<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!await source.MoveNextAsync(cancellationToken).ConfigureAwait(false))
                return default(T);

            return source.CurrentPage.SingleOrDefault();
        }

        public static async Task<List<T>> ToListAsync<T>(this IAsyncQueryable<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            var results = new List<T>();

            while (await source.MoveNextAsync(cancellationToken).ConfigureAwait(false))
            {
                results.AddRange(source.CurrentPage);
            }

            return results;
        }

        public static Task ForEachAsync<T>(this IAsyncQueryable<T> source, Action<T> @delegate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return source.ForEachAsync((item, unused_) => @delegate(item), cancellationToken);
        }

        public static async Task ForEachAsync<T>(this IAsyncQueryable<T> source, Action<T, int> @delegate, CancellationToken cancellationToken = default(CancellationToken))
        {
            var index = 0;

            while (await source.MoveNextAsync(cancellationToken).ConfigureAwait(false))
            {
                foreach (var item in source.CurrentPage)
                {
                    @delegate(item, index++);
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }
    }
}
