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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Linq;

// Placed in the base library namespace so that these extension methods are available without any extra usings
namespace Stormpath.SDK
{
    public static class AsyncQueryableExtensions
    {
        public static async Task<T> FirstAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            var asyncSource = source as IAsyncQueryable<T>;
            if (asyncSource == null)
                throw new InvalidOperationException("This queryable does not support asynchronous operations.");

            if (!await asyncSource.MoveNextAsync(cancellationToken).ConfigureAwait(false))
                throw new InvalidOperationException("The sequence has no elements.");

            return asyncSource.CurrentPage.First();
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            var asyncSource = source as IAsyncQueryable<T>;
            if (asyncSource == null)
                throw new InvalidOperationException("This queryable does not support asynchronous operations.");

            if (!await asyncSource.MoveNextAsync(cancellationToken).ConfigureAwait(false))
                return default(T);

            return asyncSource.CurrentPage.FirstOrDefault();
        }

        public static async Task<T> SingleAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            var asyncSource = source as IAsyncQueryable<T>;
            if (asyncSource == null)
                throw new InvalidOperationException("This queryable does not support asynchronous operations.");

            if (!await asyncSource.MoveNextAsync(cancellationToken).ConfigureAwait(false))
                throw new InvalidOperationException("The sequence has no elements.");

            return asyncSource.CurrentPage.Single();
        }

        public static async Task<T> SingleOrDefaultAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            var asyncSource = source as IAsyncQueryable<T>;
            if (asyncSource == null)
                throw new InvalidOperationException("This queryable does not support asynchronous operations.");

            if (!await asyncSource.MoveNextAsync(cancellationToken).ConfigureAwait(false))
                return default(T);

            return asyncSource.CurrentPage.SingleOrDefault();
        }

        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            var asyncSource = source as IAsyncQueryable<T>;
            if (asyncSource == null)
                throw new InvalidOperationException("This queryable does not support asynchronous operations.");

            var results = new List<T>();

            while (await asyncSource.MoveNextAsync(cancellationToken).ConfigureAwait(false))
            {
                results.AddRange(asyncSource.CurrentPage);
            }

            return results;
        }

        public static Task ForEachAsync<T>(this IQueryable<T> source, Action<T> @delegate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return source.ForEachAsync((item, _) => @delegate(item), cancellationToken);
        }

        public static async Task ForEachAsync<T>(this IQueryable<T> source, Action<T, int> @delegate, CancellationToken cancellationToken = default(CancellationToken))
        {
            var asyncSource = source as IAsyncQueryable<T>;
            if (asyncSource == null)
                throw new InvalidOperationException("This queryable does not support asynchronous operations.");

            var index = 0;

            while (await asyncSource.MoveNextAsync(cancellationToken).ConfigureAwait(false))
            {
                foreach (var item in asyncSource.CurrentPage)
                {
                    @delegate(item, index++);
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }
    }
}
